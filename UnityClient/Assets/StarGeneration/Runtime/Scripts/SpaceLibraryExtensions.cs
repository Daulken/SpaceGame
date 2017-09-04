using System;
using UnityEngine;
using Litipk.ColorSharp.LightSpectrums;

namespace SpaceLibrary
{
	public static class Constants
	{
		public const double KilometersPerParsec = 3.08567758129e13;
		public const double SecondsPerYear = 365.25 * 86400;
	};

    static class SpaceLibraryExtensions
    {
		/// <summary>
		/// Converts the galactic coordinate to a UnityEngine.Vector3, for ease of use within Unity
		/// </summary>
		public static Vector3 ToVector3(this GalacticCoordinate coord)
		{
			return new Vector3((float)coord.X, (float)coord.Y, (float)coord.Z);
		}

		// -----------------------------------------------------------

		/// <summary>
		/// The temperature of a star converted to the sRGB colour of light that this star would emit
		/// </summary>
		public static Vector3 TemperatureColour(this Star s)
		{
			var srgb = new BlackBodySpectrum(s.Temperature).ToSRGB();
			return new Vector3((float)srgb.R, (float)srgb.G, (float)srgb.B);
		}
	}
}
