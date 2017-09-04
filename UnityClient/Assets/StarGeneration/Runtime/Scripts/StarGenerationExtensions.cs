﻿using System;
using UnityEngine;
using Litipk.ColorSharp.LightSpectrums;

namespace StarGeneration
{
    static class StarGenerationExtensions
    {
		public static SpaceLibrary.System Offset(this SpaceLibrary.System s, Vector3 offset)
        {
			SpaceLibrary.GalacticCoordinate newPosition = s.Position;
			newPosition.X += offset.x;
			newPosition.Y += offset.y;
			newPosition.Z += offset.z;
            return s;
        }

        public static SpaceLibrary.System Scale(this SpaceLibrary.System s, Vector3 scale)
        {
			SpaceLibrary.GalacticCoordinate newPosition = s.Position;
			newPosition.X *= scale.x;
			newPosition.Y *= scale.y;
			newPosition.Z *= scale.z;
			return s;
        }

        public static SpaceLibrary.System Swirl(this SpaceLibrary.System s, Vector3 axis, float amount)
        {
			Vector3 newPosition = new Vector3((float)s.Position.X, (float)s.Position.Y, (float)s.Position.Z);
			var d = newPosition.magnitude;
            var a = (float)Math.Pow(d, 0.1f) * amount;
            newPosition = Quaternion.AngleAxis(a, axis) * newPosition;
			s.Position = new SpaceLibrary.GalacticCoordinate(newPosition.x, newPosition.y, newPosition.z);
			return s;
        }

		// -----------------------------------------------------------

		/// <summary>
		/// Converts the galactic coordinate to a UnityEngine.Vector3, for ease of use within Unity
		/// </summary>
		public static Vector3 ToVector3(this SpaceLibrary.GalacticCoordinate coord)
		{
			return new Vector3((float)coord.X, (float)coord.Y, (float)coord.Z);
		}

		// -----------------------------------------------------------

		/// <summary>
		/// The temperature of a star converted to the sRGB colour of light that this star would emit
		/// </summary>
		public static Vector3 TemperatureColour(this SpaceLibrary.Star s)
		{
			var srgb = new BlackBodySpectrum(s.Temperature).ToSRGB();
			return new Vector3((float)srgb.R, (float)srgb.G, (float)srgb.B);
		}
	}
}
