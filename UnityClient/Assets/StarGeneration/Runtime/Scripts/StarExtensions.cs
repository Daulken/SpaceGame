using System;
using UnityEngine;
using SpaceLibrary;
using Litipk.ColorSharp.LightSpectrums;

namespace StarGeneration
{
    static class StarExtensions
    {
		public static Vector3 ToVector3(this Star.GalacticCoordinate coord)
		{
			return new Vector3((float)coord.X, (float)coord.Y, (float)coord.Z);
		}


		public static Star Offset(this Star s, Vector3 offset)
        {
			Star.GalacticCoordinate newPosition = s.Position;
			newPosition.X += offset.x;
			newPosition.Y += offset.y;
			newPosition.Z += offset.z;
			s.Position = newPosition;
            return s;
        }

        public static Star Scale(this Star s, Vector3 scale)
        {
			Star.GalacticCoordinate newPosition = s.Position;
			newPosition.X *= scale.x;
			newPosition.Y *= scale.y;
			newPosition.Z *= scale.z;
			s.Position = newPosition;
			return s;
        }

        public static Star Swirl(this Star s, Vector3 axis, float amount)
        {
			Vector3 newPosition = new Vector3((float)s.Position.X, (float)s.Position.Y, (float)s.Position.Z);
			var d = newPosition.magnitude;
            var a = (float)Math.Pow(d, 0.1f) * amount;
            newPosition = Quaternion.AngleAxis(a, axis) * newPosition;
			s.Position = new Star.GalacticCoordinate(newPosition.x, newPosition.y, newPosition.z);
			return s;
        }

		public static Vector3 TemperatureColour(this Star s)
		{
			var srgb = new BlackBodySpectrum(s.Temperature).ToSRGB();
			return new Vector3((float)srgb.R, (float)srgb.G, (float)srgb.B);
		}
	}
}
