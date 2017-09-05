using System;
using System.Collections.Generic;
using UnityEngine;
using Litipk.ColorSharp.LightSpectrums;

namespace StarGeneration
{
	public class StarGenerator
	{
		public string m_name;							// Name of the star
		public Vector3 m_position = Vector3.zero;       // Current position in Cartesian coordinates
		public SpaceLibrary.Star.Class m_class;			// Class of star
		public double m_radius = 0;                     // Radius of the star
		public double m_orbitalAngleDegrees = 0;		// Angle in degrees around the orbital ellipse
		public double m_orbitalVelocity = 0;			// Angular velocity (degrees per year)
		public double m_obliqueEllipticalAngle = 0;		// Oblique position of the ellipse
		public double m_largeEllipticalHalfAxis = 0;    // Half the length of the largest diameter of an ellipse
		public double m_smallEllipticalHalfAxis = 0;	// Half the length of the diameter at a right-angle to the large half-axis
		public double m_temperatureKelvin = 0;			// Star temperature in Kelvin
		public double m_brightnessMagnitude = 0;		// Magitude of brightness (0..1) to muliply colour by;

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
			double orbitalAngleRadians = m_orbitalAngleDegrees * (Math.PI / 180);
			double inverseEllipticalAngle = -m_obliqueEllipticalAngle;

			// Temporaries to save CPU time
			double cosOrbitalAngleRadians = Math.Cos(orbitalAngleRadians);
			double sinOrbitalAngleRadians = Math.Sin(orbitalAngleRadians);
			double cosInverseEllipticalAngle = Math.Cos(inverseEllipticalAngle);
			double sinInverseEllipticalAngle = Math.Sin(inverseEllipticalAngle);

			m_position = new Vector3(
							(float)(((m_smallEllipticalHalfAxis * cosOrbitalAngleRadians * cosInverseEllipticalAngle) - (m_largeEllipticalHalfAxis * sinOrbitalAngleRadians * sinInverseEllipticalAngle))),
							0.0f,
							(float)(((m_smallEllipticalHalfAxis * cosOrbitalAngleRadians * sinInverseEllipticalAngle) + (m_largeEllipticalHalfAxis * sinOrbitalAngleRadians * cosInverseEllipticalAngle)))
							);

			// Add small perturbations to create more spiral arms
			if ((pertAmp > 0) && (pertN > 0))
			{
				m_position.x += (float)((m_smallEllipticalHalfAxis / pertAmp) * Math.Sin(orbitalAngleRadians * 2 * pertN));
				m_position.z += (float)((m_smallEllipticalHalfAxis / pertAmp) * Math.Cos(orbitalAngleRadians * 2 * pertN));
			}
		}

	}
}
