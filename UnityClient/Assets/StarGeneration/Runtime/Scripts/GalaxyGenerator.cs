using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
	public class GalaxyGenerator
	{
		// Realistically looking velocity curves for the Wikipedia models.
		private static class VelocityCurve
		{
			private static double MS(double r)
			{
				double d = 2000;  // Dicke der Scheibe
				double rho_so = 1;  // Dichte im Mittelpunkt
				double rH = 2000; // Radius auf dem die Dichte um die Hälfte gefallen ist
				return rho_so * Math.Exp(-r / rH) * (r * r) * Math.PI * d;
			}

			private static double MH(double r)
			{
				double rho_h0 = 0.15; // Dichte des Halos im Zentrum
				double rC = 2500;     // typische skalenlänge im Halo
				return rho_h0 * 1 / (1 + Math.Pow(r / rC, 2)) * (4 * Math.PI * Math.Pow(r, 3) / 3);
			}

			// Velocity curve with dark matter
			public static double v(double r)
			{
				double MZ = 100;
				double G = 6.672e-11;
				return 20000 * Math.Sqrt(G * (MH(r) + MS(r) + MZ) / r);
			}

			// velocity curve without dark matter
			public static double vd(double r)
			{
				double MZ = 100;
				double G = 6.672e-11;
				return 20000 * Math.Sqrt(G * (MS(r) + MZ) / r);
			}
		};

		private System.Random m_random;     /// Random number generator for seed control

		private StarGenerator[] m_pStars;	/// Array of star data
		private StarGenerator[] m_pDust;	/// Array of dusty areas
		private StarGenerator[] m_pH2;

		// Parameters needed for defining the general structure of the galaxy

		private double m_elEx1;             /// Eccentricity of the innermost ellipse
		private double m_elEx2;             /// Eccentricity of the outermost ellipse

		private double m_velInner;          /// Velocity at the core edge in km/s
		private double m_velOuter;          /// Velocity at the edge of the disk in km/s

		private double m_angleOffset;       /// Angular offset per parsec

		private double m_coreRadius;        /// Radius of the inner core
		private double m_galaxyRadius;      /// Radius of the galaxy
		private double m_radFarField;       /// The radius after which all density waves must have circular shape
		private double m_sigma;             /// Distribution of stars

		private double m_dustRenderSize;

		private int m_numStars;             /// Total number of stars
		private int m_numDust;              /// Number of Dust Particles
		private int m_numH2;                /// Number of H2 Regions

		private int m_pertN;
		private double m_pertAmp;

		private double m_time;
		private double m_timeStep;

		private bool m_bHasDarkMatter;

		public int[] m_numberByRad = new int[100];  /// Histogram showing distribution of stars

		public GalaxyGenerator()
		{
		}

		public void Reset(System.Random random, double galaxyRadius, double coreRadius, double deltaAng, double ex1, double ex2, double sigma, double velInner, double velOuter, int numStars, bool hasDarkMatter, int pertN, double pertAmp, double dustRenderSize)
		{
			m_sigma = 0.45;
			m_numH2 = 300;
			m_time = 0;
			m_timeStep = 0;

			m_random = random;
			m_elEx1 = ex1;
			m_elEx2 = ex2;
			m_velInner = velInner;
			m_velOuter = velOuter;
			m_elEx2 = ex2;
			m_angleOffset = deltaAng;
			m_coreRadius = coreRadius;
			m_galaxyRadius = galaxyRadius;
			m_radFarField = m_galaxyRadius * 2;  // there is no science behind this threshold it just looks nice
			m_sigma = sigma;
			m_numStars = numStars;
			m_numDust = numStars / 2;
			m_time = 0;
			m_dustRenderSize = dustRenderSize;
			m_bHasDarkMatter = hasDarkMatter;
			m_pertN = pertN;
			m_pertAmp = pertAmp;

			for (int i = 0; i < 100; ++i)
				m_numberByRad[i] = 0;

			InitStars(m_sigma);
		}

		public void Reset()
		{
			Reset(m_random, m_galaxyRadius, m_coreRadius, m_angleOffset, m_elEx1, m_elEx2, m_sigma, m_velInner, m_velOuter, m_numStars, m_bHasDarkMatter, m_pertN, m_pertAmp, m_dustRenderSize);
		}

		public void ToggleDarkMatter()
		{
			m_bHasDarkMatter = !m_bHasDarkMatter;
			Reset();
		}

		private void InitStars(double sigma)
		{
			// Re-allocate star arrays
			m_pDust = new StarGenerator[m_numDust];
			for (int i = 0; i < m_pDust.Length; ++i)
				m_pDust[i] = new StarGenerator();
			m_pStars = new StarGenerator[m_numStars];
			for (int i = 0; i < m_pStars.Length; ++i)
				m_pStars[i] = new StarGenerator();
			m_pH2 = new StarGenerator[m_numH2 * 2];
			for (int i = 0; i < m_pH2.Length; ++i)
				m_pH2[i] = new StarGenerator();

			// The first three stars can be used for aligning the camera with the galaxy rotation.

			// First star is the black hole at the centre
			m_pStars[0].m_a = 0;
			m_pStars[0].m_b = 0;
			m_pStars[0].m_angle = 0;
			m_pStars[0].m_theta = 0;
			m_pStars[0].m_velTheta = 0;
			m_pStars[0].m_center = Vector3.zero;
			m_pStars[0].m_velTheta = GetOrbitalVelocity((m_pStars[0].m_a + m_pStars[0].m_b)/2.0);
			m_pStars[0].m_temp = 6000;

			// second star is at the edge of the core area
			m_pStars[1].m_a = m_coreRadius;
			m_pStars[1].m_b = m_coreRadius * GetEccentricity(m_coreRadius);
			m_pStars[1].m_angle = GetAngularOffset(m_coreRadius);
			m_pStars[1].m_theta = 0;
			m_pStars[1].m_center = Vector3.zero;
			m_pStars[1].m_velTheta = GetOrbitalVelocity((m_pStars[1].m_a + m_pStars[1].m_b)/2.0);
			m_pStars[1].m_temp = 6000;

			// third star is at the edge of the disk
			m_pStars[2].m_a = m_galaxyRadius;
			m_pStars[2].m_b = m_galaxyRadius * GetEccentricity(m_galaxyRadius);
			m_pStars[2].m_angle = GetAngularOffset(m_galaxyRadius);
			m_pStars[2].m_theta = 0;
			m_pStars[2].m_center = Vector3.zero;
			m_pStars[2].m_velTheta = GetOrbitalVelocity((m_pStars[2].m_a + m_pStars[2].m_b)/2.0);
			m_pStars[2].m_temp = 6000;

			// cell width of the histogram
			double dh = (double)m_radFarField/100.0;

			// Initialize the stars
			CumulativeDistributionFunction cdf = new CumulativeDistributionFunction();
			cdf.SetupRealistic(1.0,                     // Maximal intensity
							   0.02,                    // k (bulge)
							   m_galaxyRadius / 3.0,    // disc size
							   m_coreRadius,            // bulge radius
							   0,                       // start of intensity curve
							   m_radFarField,           // end of intensity curve
							   1000);                   // Stars in field
			for (int i = 3; i<m_numStars; ++i)
			{
				double distanceFromGalacticCentre = cdf.ValueFromProbability(m_random.NextDouble());

				m_pStars[i].m_a = distanceFromGalacticCentre;
				m_pStars[i].m_b = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);
				m_pStars[i].m_angle = GetAngularOffset(distanceFromGalacticCentre);
				m_pStars[i].m_theta = 360.0 * m_random.NextDouble();
				m_pStars[i].m_velTheta = GetOrbitalVelocity(distanceFromGalacticCentre);
				m_pStars[i].m_center = Vector3.zero;

				m_pStars[i].m_name = StarName.Generate(m_random);

				// Now calculate the class of the star, based on percentage probability.
				double classUnit = m_random.NextDouble() * 100;
				if (classUnit < 0.00003)
					m_pStars[i].m_class = SpaceLibrary.Star.Class.O;
				else if (classUnit < (0.00003 + 0.13))
					m_pStars[i].m_class = SpaceLibrary.Star.Class.B;
				else if (classUnit < (0.00003 + 0.13 + 0.6))
					m_pStars[i].m_class = SpaceLibrary.Star.Class.A;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0))
					m_pStars[i].m_class = SpaceLibrary.Star.Class.F;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0 + 7.6))
					m_pStars[i].m_class = SpaceLibrary.Star.Class.G;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0 + 7.6 + 12.1))
					m_pStars[i].m_class = SpaceLibrary.Star.Class.K;
				else
					m_pStars[i].m_class = SpaceLibrary.Star.Class.M;

				// Calculate a size, temperature and magnitude of the star based on the class
				switch (m_pStars[i].m_class)
				{
				case SpaceLibrary.Star.Class.O:
					m_pStars[i].m_radius = Mathf.Lerp(6.6f, 80.0f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = m_random.NormallyDistributedSingle(8000, 40000, 30000, 60000);
					m_pStars[i].m_mag = 0.9 + (0.1 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.B:
					m_pStars[i].m_radius = Mathf.Lerp(1.8f, 6.6f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = Mathf.Lerp(10000, 30000, (float)m_random.NextDouble());
					m_pStars[i].m_mag = 0.5 + (0.4 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.A:
					m_pStars[i].m_radius = Mathf.Lerp(1.4f, 1.8f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = Mathf.Lerp(7500, 10000, (float)m_random.NextDouble());
					m_pStars[i].m_mag = 0.3 + (0.2 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.F:
					m_pStars[i].m_radius = Mathf.Lerp(1.15f, 1.4f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = Mathf.Lerp(6000, 7500, (float)m_random.NextDouble());
					m_pStars[i].m_mag = 0.2 + (0.1 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.G:
					m_pStars[i].m_radius = Mathf.Lerp(0.96f, 1.15f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = Mathf.Lerp(5200, 6000, (float)m_random.NextDouble());
					m_pStars[i].m_mag = 0.1 + (0.1 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.K:
					m_pStars[i].m_radius = Mathf.Lerp(0.7f, 0.96f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = Mathf.Lerp(3700, 5200, (float)m_random.NextDouble());
					m_pStars[i].m_mag = 0.05 + (0.05 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.M:
					m_pStars[i].m_radius = Mathf.Lerp(0.4f, 0.7f, (float)m_random.NextDouble());
					m_pStars[i].m_temp = Mathf.Lerp(2400, 3700, (float)m_random.NextDouble());
					m_pStars[i].m_mag = 0.01 + (0.04 * m_random.NextDouble());
					break;
				}

				int idx = (int)Math.Min(1.0/dh * (m_pStars[i].m_a + m_pStars[i].m_b)/2.0, 99.0);
				m_numberByRad[idx]++;
			}

			// Initialise Dust
			for (int i = 0; i<m_numDust; ++i)
			{
				double distanceFromGalacticCentre;
				if (i%4==0)
				{
					distanceFromGalacticCentre = cdf.ValueFromProbability(m_random.NextDouble());
				}
				else
				{
					double x = 2*m_galaxyRadius * m_random.NextDouble() - m_galaxyRadius;
					double y = 2*m_galaxyRadius * m_random.NextDouble() - m_galaxyRadius;
					distanceFromGalacticCentre = Math.Sqrt(x*x+y*y);
				}

				m_pDust[i].m_a = distanceFromGalacticCentre;
				m_pDust[i].m_b = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);
				m_pDust[i].m_angle = GetAngularOffset(distanceFromGalacticCentre);
				m_pDust[i].m_theta = 360.0 * m_random.NextDouble();
				m_pDust[i].m_velTheta = GetOrbitalVelocity((m_pDust[i].m_a + m_pDust[i].m_b)/2.0);
				m_pDust[i].m_center = Vector3.zero;

				// I want the outer parts to appear blue, the inner parts yellow. I'm imposing
				// the following temperature distribution (no science here it just looks right)
				m_pDust[i].m_temp = 5000 + distanceFromGalacticCentre/4.5;

				m_pDust[i].m_mag = 0.015 + 0.01 * m_random.NextDouble();
				int idx = (int)Math.Min(1.0/dh * (m_pDust[i].m_a + m_pDust[i].m_b)/2.0, 99.0);
				m_numberByRad[idx]++;
			}

			// Initialise Dust
			for (int i = 0; i<m_numH2; ++i)
			{
				double x = 2*(m_galaxyRadius) * m_random.NextDouble() - (m_galaxyRadius);
				double y = 2*(m_galaxyRadius) * m_random.NextDouble() - (m_galaxyRadius);
				double distanceFromGalacticCentre = Math.Sqrt(x*x+y*y);

				int k1 = 2*i;
				m_pH2[k1].m_a = distanceFromGalacticCentre;
				m_pH2[k1].m_b = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);
				m_pH2[k1].m_angle = GetAngularOffset(distanceFromGalacticCentre);
				m_pH2[k1].m_theta = 360.0 * m_random.NextDouble();
				m_pH2[k1].m_velTheta = GetOrbitalVelocity((m_pH2[k1].m_a + m_pH2[k1].m_b)/2.0);
				m_pH2[k1].m_center = Vector3.zero;
				m_pH2[k1].m_temp = 6000 + (6000 * m_random.NextDouble()) - 3000;
				m_pH2[k1].m_mag = 0.1 + 0.05 * m_random.NextDouble();
				int idx = (int)Math.Min(1.0/dh * (m_pH2[k1].m_a + m_pH2[k1].m_b)/2.0, 99.0);
				m_numberByRad[idx]++;

				// Create second point 100 pc away from the first one
				int dist = 1000;
				int k2 = 2*i + 1;
				m_pH2[k2].m_a = distanceFromGalacticCentre + dist;
				m_pH2[k2].m_b = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);
				m_pH2[k2].m_angle = GetAngularOffset(distanceFromGalacticCentre);
				m_pH2[k2].m_theta = m_pH2[k1].m_theta;
				m_pH2[k2].m_velTheta = m_pH2[k1].m_velTheta;
				m_pH2[k2].m_center = m_pH2[k1].m_center;
				m_pH2[k2].m_temp = m_pH2[k1].m_temp;
				m_pH2[k2].m_mag = m_pH2[k1].m_mag;
				idx = (int)Math.Min(1.0/dh * (m_pH2[k2].m_a + m_pH2[k2].m_b)/2.0, 99.0);
				m_numberByRad[idx]++;
			}
		}

		public void SetSigma(double sigma)
		{
			m_sigma = sigma;
			Reset();
		}

		public double GetSigma()
		{
			return m_sigma;
		}

		public void SetDustRenderSize(double sz)
		{
			m_dustRenderSize = Math.Max(sz, 1.0);
		}

		public double GetDustRenderSize()
		{
			return m_dustRenderSize;
		}

		public void SetAngularOffset(double offset)
		{
			m_angleOffset = offset;
			Reset();
		}

		public double GetAngularOffset(double distanceFromGalacticCentre)
		{
			return distanceFromGalacticCentre * m_angleOffset;
		}

		public double GetAngularOffset()
		{
			return m_angleOffset;
		}

		public void SetPertN(int n)
		{
			m_pertN = Math.Max(0, n);
		}
		public int GetPertN()
		{
			return m_pertN;
		}

		public void SetPertAmp(double amp)
		{
			m_pertAmp = Math.Max(0.0, amp);
		}

		public double GetPertAmp()
		{
			return m_pertAmp;
		}


		public StarGenerator[] GetStars()
		{
			return m_pStars;
		}

		public StarGenerator[] GetDust()
		{
			return m_pDust;
		}

		public StarGenerator[] GetH2()
		{
			return m_pH2;
		}

		public void SetRad(double distanceFromGalacticCentre)
		{
			m_galaxyRadius = distanceFromGalacticCentre;
			Reset();
		}

		public double GetRad()
		{
			return m_galaxyRadius;
		}

		public void SetCoreRad(double distanceFromGalacticCentre)
		{
			m_coreRadius = distanceFromGalacticCentre;
			Reset();
		}

		public double GetCoreRad()
		{
			return m_coreRadius;
		}

		public double GetFarFieldRad()
		{
			return m_radFarField;
		}

		public void SetExInner(double ex)
		{
			m_elEx1 = ex;
			Reset();
		}

		public double GetExInner()
		{
			return m_elEx1;
		}

		public void SetExOuter(double ex)
		{
			m_elEx2 = ex;
			Reset();
		}

		public double GetExOuter()
		{
			return m_elEx2;
		}

		// Properties depending on the orbital distance from galactic centre

		/// <summary>
		/// Returns the orbital velocity in degrees per year
		/// <param name="distanceFromGalacticCentre">Distance from galactic centre in parsecs</param>
		/// </summary>
		public double GetOrbitalVelocity(double distanceFromGalacticCentre)
		{
			// Velocity in kilometers per second
			double vel_kms = m_bHasDarkMatter ? VelocityCurve.v(distanceFromGalacticCentre) : VelocityCurve.vd(distanceFromGalacticCentre);

			// Calculate velocity in degree per year
			double u = 2 * Math.PI * distanceFromGalacticCentre * SpaceLibrary.Constants.KilometersPerParsec;
			double time = u / (vel_kms * SpaceLibrary.Constants.SecondsPerYear);
			return 360.0 / time;
		}

		public double GetEccentricity(double distanceFromGalacticCentre)
		{
			if (distanceFromGalacticCentre < m_coreRadius)
			{
				// Core region of the galaxy. Innermost part is round
				// eccentricity increasing linear to the border of the core.
				return 1 + (distanceFromGalacticCentre / m_coreRadius) * (m_elEx1-1);
			}
			else if ((distanceFromGalacticCentre > m_coreRadius) && (distanceFromGalacticCentre <= m_galaxyRadius))
			{
				return m_elEx1 + (distanceFromGalacticCentre - m_coreRadius) / (m_galaxyRadius - m_coreRadius) * (m_elEx2-m_elEx1);
			}
			else if ((distanceFromGalacticCentre > m_galaxyRadius) && (distanceFromGalacticCentre < m_radFarField))
			{
				// eccentricity is slowly reduced to 1.
				return m_elEx2 + (distanceFromGalacticCentre - m_galaxyRadius) / (m_radFarField - m_galaxyRadius) * (1-m_elEx2);
			}
			else
			{
				return 1;
			}
		}

		public double GetTimeStep()
		{
			return m_timeStep;
		}

		public double GetTime()
		{
			return m_time;
		}

		public int GetNumStars()
		{
			return m_numStars;
		}

		public int GetNumDust()
		{
			return m_numDust;
		}

		public int GetNumH2()
		{
			return m_numH2;
		}


		public void SingleTimeStep(double time)
		{
			m_timeStep = time;
			m_time += time;

			Vector2 posOld;
			for (int i = 0; i<m_numStars; ++i)
			{
				m_pStars[i].m_theta += (m_pStars[i].m_velTheta * time);
				posOld = m_pStars[i].m_pos;
				m_pStars[i].CalcXY(m_pertN, m_pertAmp);
				m_pStars[i].m_vel = new Vector2(m_pStars[i].m_pos.x - posOld.x, m_pStars[i].m_pos.y - posOld.y);
			}

			for (int i = 0; i<m_numDust; ++i)
			{
				m_pDust[i].m_theta += (m_pDust[i].m_velTheta * time);
				posOld = m_pDust[i].m_pos;
				m_pDust[i].CalcXY(m_pertN, m_pertAmp);
			}

			for (int i = 0; i<m_numH2*2; ++i)
			{
				m_pH2[i].m_theta += (m_pH2[i].m_velTheta * time);
				posOld = m_pDust[i].m_pos;
				m_pH2[i].CalcXY(m_pertN, m_pertAmp);
			}
		}

		public Vector3 GetStarPos(int idx)
		{
			return m_pStars[idx].m_pos;
		}

	}
}
