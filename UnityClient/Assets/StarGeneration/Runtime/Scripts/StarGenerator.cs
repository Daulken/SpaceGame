using System;
using System.Collections.Generic;
using UnityEngine;
using Litipk.ColorSharp.LightSpectrums;

namespace StarGeneration
{
	public class StarGenerator
	{
		public double m_orbitalAngleDegrees;		// Angle in degrees around the orbital ellipse
		public double m_orbitalVelocity;			// Angular velocity (degrees per year)
		public double m_angle;						// Schräglage der Ellipse
		public double m_a;							// kleine halbachse
		public double m_b;							// große halbachse
		public double m_temperatureKelvin;			// Star temperature in Kelvin
		public double m_brightnessMagnitude;		// Magitude of brightness (0..1) to muliply colour by;
		public Vector3 m_center;					// Center of the elliptical orbit
		public Vector3 m_position;                  // Current position in Cartesian coordinates
		public string m_name;                       // Name of the star
		public SpaceLibrary.Star.Class m_class;     // Class of star
		public double m_radius;                     // Radius of the star

		public StarGenerator()
		{
			m_orbitalAngleDegrees = 0;
			m_a = 0;
			m_b = 0;
			m_center = Vector3.zero;
		}

		/// <summary>
		/// The temperature of a star converted to the sRGB colour of light that this star would emit
		/// </summary>
		public Vector3 TemperatureColour()
		{
			var srgb = new BlackBodySpectrum(m_temperatureKelvin).ToSRGB();
			return new Vector3((float)srgb.R, (float)srgb.G, (float)srgb.B);
		}

		public void SetPositionWithPurturbation(int pertN, double pertAmp)
		{
			double beta = -m_angle;
			double alpha = m_orbitalAngleDegrees * (Math.PI / 180);

			// temporaries to save cpu time
			double cosalpha = Math.Cos(alpha);
			double sinalpha = Math.Sin(alpha);
			double cosbeta = Math.Cos(beta);
			double sinbeta = Math.Sin(beta);

			m_position = new Vector3(
							(float)(m_center.x + (m_a * cosalpha * cosbeta - m_b * sinalpha * sinbeta)),
							m_center.y,
							(float)(m_center.z + (m_a * cosalpha * sinbeta + m_b * sinalpha * cosbeta))
							);

			// Add small perturbations to create more spiral arms
			if (pertAmp>0 && pertN>0)
			{
				m_position.x += (float)((m_a / pertAmp) * Math.Sin(alpha * 2 * pertN));
				m_position.z += (float)((m_a / pertAmp) * Math.Cos(alpha * 2 * pertN));
			}
		}

	}
}
