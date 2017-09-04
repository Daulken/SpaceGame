using System;
using System.Collections.Generic;
using UnityEngine;
using Litipk.ColorSharp.LightSpectrums;

namespace StarGeneration
{
	public class StarGenerator
	{
		public double m_theta;						// position auf der ellipse
		public double m_velTheta;					// angular velocity
		public double m_angle;						// Schräglage der Ellipse
		public double m_a;							// kleine halbachse
		public double m_b;							// große halbachse
		public double m_temp;						// star temperature
		public double m_mag;						// brightness;
		public Vector3 m_center;					// center of the elliptical orbit
		public Vector3 m_vel;						// Current velocity (calculated)
		public Vector3 m_pos;                       // current position in cartesian coordinates
		public string m_name;                       // Name of the star
		public SpaceLibrary.Star.Class m_class;     // Class of star
		public double m_radius;                     // Radius of the star

		public StarGenerator()
		{
			m_theta = 0;
			m_a = 0;
			m_b = 0;
			m_center = Vector3.zero;
		}

		/// <summary>
		/// The temperature of a star converted to the sRGB colour of light that this star would emit
		/// </summary>
		public Vector3 TemperatureColour()
		{
			var srgb = new BlackBodySpectrum(m_temp).ToSRGB();
			return new Vector3((float)srgb.R, (float)srgb.G, (float)srgb.B);
		}

		public Vector3 CalcXY(int pertN, double pertAmp)
		{
			double beta = -m_angle;
			double alpha = m_theta * Math.PI / 180;

			// temporaries to save cpu time
			double cosalpha = Math.Cos(alpha);
			double sinalpha = Math.Sin(alpha);
			double cosbeta = Math.Cos(beta);
			double sinbeta = Math.Sin(beta);

			m_pos = new Vector3(
							(float)(m_center.x + (m_a * cosalpha * cosbeta - m_b * sinalpha * sinbeta)),
							m_center.y,
							(float)(m_center.z + (m_a * cosalpha * sinbeta + m_b * sinalpha * cosbeta))
							);

			// Add small perturbations to create more spiral arms
			if (pertAmp>0 && pertN>0)
			{
				m_pos.x += (float)((m_a / pertAmp) * Math.Sin(alpha * 2 * pertN));
				m_pos.z += (float)((m_a / pertAmp) * Math.Cos(alpha * 2 * pertN));
			}

			return m_pos;
		}

	}
}
