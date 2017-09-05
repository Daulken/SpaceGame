using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
	public class GalaxyGenerator
	{
		private System.Random		m_random;			// Random number generator for seed control

		private StarGenerator[]		m_pStars;			// Array of star data
		private StarGenerator[]		m_pDust;			// Array of dusty areas
		private StarGenerator[]		m_pH2;				// Array of H2 stellar nurseries

		// Parameters needed for defining the general structure of the galaxy

		private double				m_elEx1;			// Eccentricity of the innermost ellipse
		private double				m_elEx2;			// Eccentricity of the outermost ellipse
		private double				m_angleOffset;		// Angular offset per parsec
		private double				m_coreRadius;		// Radius of the inner core
		private double				m_galaxyRadius;		// Radius of the galaxy
		private double				m_farFieldRadius;	// The radius after which all density waves must have circular shape
		private int					m_numStars;			// Total number of stars
		private int					m_numDust;			// Number of Dust Particles
		private int					m_numH2;			// Number of H2 Regions
		private int					m_pertN;			// Number of spiral perturbations
		private double				m_pertAmpDamp;		// Amplitude damping factor of perturbation (width of arms: smaller = tight, larger = spread)

		// Realistically looking velocity curves for galaxy rotation, taking into account dark matter.
		// https://en.wikipedia.org/wiki/Galaxy_rotation_curve
		private static class GalaxyVelocityCurve
		{
			private static double MS(double radius)
			{
				double d = 2000;        // Thickness of the disc
				double rho_so = 1;      // Density in the center
				double rH = 2000;       // Radius at which the density has fallen by half
				return rho_so * Math.Exp(-radius / rH) * (radius * radius) * Math.PI * d;
			}

			private static double MH(double radius)
			{
				double rho_h0 = 0.15;   // Density of the halos in the center
				double rC = 2500;       // Typical scale length in the halo
				return rho_h0 * 1 / (1 + Math.Pow(radius / rC, 2)) * (4 * Math.PI * Math.Pow(radius, 3) / 3);
			}

			// Velocity curve with dark matter
			public static double velocity(double radius)
			{
				double MZ = 100;
				double G = 6.672e-11;
				return 20000 * Math.Sqrt(G * (MH(radius) + MS(radius) + MZ) / radius);
			}

		};


		public GalaxyGenerator(System.Random random, double galaxyRadius, double coreRadius, double deltaAng, double ex1, double ex2, int numStars, int pertN, double pertAmp)
		{
			m_random = random;
			m_elEx1 = ex1;
			m_elEx2 = ex2;
			m_angleOffset = deltaAng;
			m_coreRadius = coreRadius;
			m_galaxyRadius = galaxyRadius;
			m_farFieldRadius = m_galaxyRadius * 2;  // there is no science behind this threshold it just looks nice
			m_numStars = numStars;
			m_numDust = numStars / 2;
			m_numH2 = numStars / 150;
			m_pertN = pertN;
			m_pertAmpDamp = pertAmp;

			// Re-allocate data arrays
			m_pDust = new StarGenerator[m_numDust];
			for (int i = 0; i < m_pDust.Length; ++i)
				m_pDust[i] = new StarGenerator();
			m_pH2 = new StarGenerator[m_numH2];
			for (int i = 0; i < m_pH2.Length; ++i)
				m_pH2[i] = new StarGenerator();
			m_pStars = new StarGenerator[m_numStars];
			for (int i = 0; i < m_pStars.Length; ++i)
				m_pStars[i] = new StarGenerator();

			// Construct the distribution function using Wave Density Theory
			CumulativeDistributionFunction cdf = new CumulativeDistributionFunction();
			cdf.SetupRealistic(1.0,                     // Maximal intensity
							   0.02,                    // k (bulge)
							   m_galaxyRadius / 3.0,    // disc size
							   m_coreRadius,            // bulge radius
							   0,                       // start of intensity curve
							   m_farFieldRadius,        // end of intensity curve
							   1000);                   // Stars in field

			// Initialise Dust
			for (int i = 0; i < m_numDust; ++i)
			{
				// Every fourth dust is on the spiral arm, otherwise pick a grid point at random
				// and get the distance from that (so we can then apply a random angle)
				double distanceFromGalacticCentre;
				if ((i % 4) == 0)
				{
					distanceFromGalacticCentre = cdf.ValueFromProbability(m_random.NextDouble());
				}
				else
				{
					double x = (2 * m_galaxyRadius * m_random.NextDouble()) - m_galaxyRadius;
					double y = (2 * m_galaxyRadius * m_random.NextDouble()) - m_galaxyRadius;
					distanceFromGalacticCentre = Math.Sqrt(x*x+y*y);
				}

				// Set the elliptical distances
				m_pDust[i].m_smallEllipticalHalfAxis = distanceFromGalacticCentre;
				m_pDust[i].m_largeEllipticalHalfAxis = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);

				// Set the initial 0..360 angle around orbit, and velocity in orbit
				m_pDust[i].m_obliqueEllipticalAngle = GetAngularOffset(distanceFromGalacticCentre);
				m_pDust[i].m_orbitalAngleDegrees = 360.0 * m_random.NextDouble();
				m_pDust[i].m_orbitalVelocity = GetOrbitalVelocity(m_pDust[i]);

				// The outer parts should appear blue, the inner parts yellow.
				// No science here it just looks right!
				m_pDust[i].m_temperatureKelvin = (((distanceFromGalacticCentre / m_farFieldRadius)) * 10000) + 5000;

				// Brightness magnitude of dust should be low, as it overlays and builds up
				m_pDust[i].m_brightnessMagnitude = 0.015 + (0.01 * m_random.NextDouble());
			}

			// Initialise H-II stellar nurseries
			for (int i = 0; i < (m_numH2 / 2); ++i)
			{
				// Pick a grid point at random and get the distance from that (so we can then apply a random angle)
				double x = (2 * m_galaxyRadius * m_random.NextDouble()) - m_galaxyRadius;
				double y = (2 * m_galaxyRadius * m_random.NextDouble()) - m_galaxyRadius;
				double distanceFromGalacticCentre = Math.Sqrt(x*x+y*y);

				// Get the index of the first nursery point
				int k1 = (2 * i);

				// Set the elliptical distances
				m_pH2[k1].m_smallEllipticalHalfAxis = distanceFromGalacticCentre;
				m_pH2[k1].m_largeEllipticalHalfAxis = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);

				// Set the initial 0..360 angle around orbit, and velocity in orbit
				m_pH2[k1].m_obliqueEllipticalAngle = GetAngularOffset(distanceFromGalacticCentre);
				m_pH2[k1].m_orbitalAngleDegrees = 360.0 * m_random.NextDouble();
				m_pH2[k1].m_orbitalVelocity = GetOrbitalVelocity(m_pH2[k1]);

				// Roughly dark red to white-blue in colour
				m_pH2[k1].m_temperatureKelvin = 3000 + (6000 * m_random.NextDouble());

				// Brightness magnitude of H2 should be fairly high
				m_pH2[k1].m_brightnessMagnitude = 0.4 + (0.1 * m_random.NextDouble());

				// Get the index of the second nursery point, 1000 parsecs away from the first one
				int dist = 1000;
				int k2 = (2 * i) + 1;

				// Set the elliptical distances
				m_pH2[k2].m_smallEllipticalHalfAxis = distanceFromGalacticCentre + dist;
				m_pH2[k2].m_largeEllipticalHalfAxis = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);

				// Set the initial 0..360 angle around orbit, and inherit velocity in orbit from the first point
				m_pH2[k2].m_obliqueEllipticalAngle = GetAngularOffset(distanceFromGalacticCentre);
				m_pH2[k2].m_orbitalAngleDegrees = m_pH2[k1].m_orbitalAngleDegrees;
				m_pH2[k2].m_orbitalVelocity = m_pH2[k1].m_orbitalVelocity;

				// Copy the temperature and brightness of the first point
				m_pH2[k2].m_temperatureKelvin = m_pH2[k1].m_temperatureKelvin;
				m_pH2[k2].m_brightnessMagnitude = m_pH2[k1].m_brightnessMagnitude;
			}

			// Initialize the stars
			for (int i = 0; i < m_numStars; ++i)
			{
				// Stars are always positioned according to the wave density distribution
				double distanceFromGalacticCentre = cdf.ValueFromProbability(m_random.NextDouble());

				// Set the elliptical distances
				m_pStars[i].m_smallEllipticalHalfAxis = distanceFromGalacticCentre;
				m_pStars[i].m_largeEllipticalHalfAxis = distanceFromGalacticCentre * GetEccentricity(distanceFromGalacticCentre);

				// Set the initial 0..360 angle around orbit, and velocity in orbit
				m_pStars[i].m_obliqueEllipticalAngle = GetAngularOffset(distanceFromGalacticCentre);
				m_pStars[i].m_orbitalAngleDegrees = 360.0 * m_random.NextDouble();
				m_pStars[i].m_orbitalVelocity = GetOrbitalVelocity(distanceFromGalacticCentre);

				// Calculate the class of the star, based on percentage probability.
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
					m_pStars[i].m_temperatureKelvin = m_random.NormallyDistributedSingle(8000, 40000, 30000, 60000);
					m_pStars[i].m_brightnessMagnitude = 0.9 + (0.1 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.B:
					m_pStars[i].m_radius = Mathf.Lerp(1.8f, 6.6f, (float)m_random.NextDouble());
					m_pStars[i].m_temperatureKelvin = Mathf.Lerp(10000, 30000, (float)m_random.NextDouble());
					m_pStars[i].m_brightnessMagnitude = 0.5 + (0.4 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.A:
					m_pStars[i].m_radius = Mathf.Lerp(1.4f, 1.8f, (float)m_random.NextDouble());
					m_pStars[i].m_temperatureKelvin = Mathf.Lerp(7500, 10000, (float)m_random.NextDouble());
					m_pStars[i].m_brightnessMagnitude = 0.3 + (0.2 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.F:
					m_pStars[i].m_radius = Mathf.Lerp(1.15f, 1.4f, (float)m_random.NextDouble());
					m_pStars[i].m_temperatureKelvin = Mathf.Lerp(6000, 7500, (float)m_random.NextDouble());
					m_pStars[i].m_brightnessMagnitude = 0.2 + (0.1 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.G:
					m_pStars[i].m_radius = Mathf.Lerp(0.96f, 1.15f, (float)m_random.NextDouble());
					m_pStars[i].m_temperatureKelvin = Mathf.Lerp(5200, 6000, (float)m_random.NextDouble());
					m_pStars[i].m_brightnessMagnitude = 0.1 + (0.1 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.K:
					m_pStars[i].m_radius = Mathf.Lerp(0.7f, 0.96f, (float)m_random.NextDouble());
					m_pStars[i].m_temperatureKelvin = Mathf.Lerp(3700, 5200, (float)m_random.NextDouble());
					m_pStars[i].m_brightnessMagnitude = 0.05 + (0.05 * m_random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.M:
					m_pStars[i].m_radius = Mathf.Lerp(0.4f, 0.7f, (float)m_random.NextDouble());
					m_pStars[i].m_temperatureKelvin = Mathf.Lerp(2400, 3700, (float)m_random.NextDouble());
					m_pStars[i].m_brightnessMagnitude = 0.01 + (0.04 * m_random.NextDouble());
					break;
				}

				// Generate a random star name
				m_pStars[i].m_name = StarName.Generate(m_random);
			}

			// Set up initial positions
			SingleTimeStep(0);
		}

		public void SingleTimeStep(double time)
		{
			for (int i = 0; i < m_numStars; ++i)
			{
				m_pStars[i].m_orbitalAngleDegrees += (m_pStars[i].m_orbitalVelocity * time);
				m_pStars[i].SetPositionWithPurturbation(m_pertN, m_pertAmpDamp);
			}

			for (int i = 0; i < m_numDust; ++i)
			{
				m_pDust[i].m_orbitalAngleDegrees += (m_pDust[i].m_orbitalVelocity * time);
				m_pDust[i].SetPositionWithPurturbation(m_pertN, m_pertAmpDamp);
			}

			for (int i = 0; i < m_numH2; ++i)
			{
				m_pH2[i].m_orbitalAngleDegrees += (m_pH2[i].m_orbitalVelocity * time);
				m_pH2[i].SetPositionWithPurturbation(m_pertN, m_pertAmpDamp);
			}
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

		public double GetFarFieldRadius()
		{
			return m_farFieldRadius;
		}


		// ----------------------------------------------------------------------
		// Properties depending on the orbital distance from galactic centre
		// ----------------------------------------------------------------------

		private double GetAngularOffset(double distanceFromGalacticCentre)
		{
			return distanceFromGalacticCentre * m_angleOffset;
		}

		private double GetOrbitalVelocity(StarGenerator star)
		{
			return GetOrbitalVelocity((star.m_smallEllipticalHalfAxis + star.m_largeEllipticalHalfAxis) / 2.0);
		}

		/// <summary>
		/// Returns the orbital velocity in degrees per year
		/// <param name="distanceFromGalacticCentre">Distance from galactic centre in parsecs</param>
		/// </summary>
		private double GetOrbitalVelocity(double distanceFromGalacticCentre)
		{
			// Velocity in kilometers per second
			double vel_kms = GalaxyVelocityCurve.velocity(distanceFromGalacticCentre);

			// Calculate velocity in degrees per year
			double u = 2 * Math.PI * distanceFromGalacticCentre * SpaceLibrary.Constants.KilometersPerParsec;
			double time = u / (vel_kms * SpaceLibrary.Constants.SecondsPerYear);
			return 360.0 / time;
		}

		/// <summary>
		/// Returns the eccentricity of an orbit based on distance from galactic centre.
		/// Eccentricity is minimal (circular) in the centre, changing to full eccentricity (chosen elliptical maximum) at edges
		/// <param name="distanceFromGalacticCentre">Distance from galactic centre in parsecs</param>
		/// </summary>
		private double GetEccentricity(double distanceFromGalacticCentre)
		{
			if (distanceFromGalacticCentre < m_coreRadius)
			{
				// Core region of the galaxy. Innermost part is round
				// eccentricity increasing linear to the border of the core.
				return 1 + (distanceFromGalacticCentre / m_coreRadius) * (m_elEx1 - 1);
			}
			else if ((distanceFromGalacticCentre > m_coreRadius) && (distanceFromGalacticCentre <= m_galaxyRadius))
			{
				return m_elEx1 + (distanceFromGalacticCentre - m_coreRadius) / (m_galaxyRadius - m_coreRadius) * (m_elEx2 - m_elEx1);
			}
			else if ((distanceFromGalacticCentre > m_galaxyRadius) && (distanceFromGalacticCentre < m_farFieldRadius))
			{
				// eccentricity is slowly reduced to 1.
				return m_elEx2 + (distanceFromGalacticCentre - m_galaxyRadius) / (m_farFieldRadius - m_galaxyRadius) * (1 - m_elEx2);
			}
			else
			{
				return 1;
			}
		}

	}
}
