using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace StarGeneration.Galaxies
{
    public class Sphere
        : BaseGalaxySpec
    {
        private readonly float _size;

        private readonly float _densityMean;
        private readonly float _densityDeviation;

        private readonly float _deviationX;
        private readonly float _deviationY;
        private readonly float _deviationZ;

        public Sphere(
            float size,
            float densityMean = 0.0000025f, float densityDeviation = 0.000001f,
            float deviationX = 0.0000025f,
            float deviationY = 0.0000025f,
            float deviationZ = 0.0000025f
        )
        {
            _size = size;

            _densityMean = densityMean;
            _densityDeviation = densityDeviation;

            _deviationX = deviationX;
            _deviationY = deviationY;
            _deviationZ = deviationZ;
        }

        protected internal override IEnumerable<SpaceLibrary.StarSystem> Generate(System.Random random)
        {
            var density = Math.Max(0, random.NormallyDistributedSingle(_densityDeviation, _densityMean));
            var countMax = Math.Max(0, (int)(_size * _size * _size * density));
            if (countMax <= 0)
                yield break;

            int count = random.Next(countMax);
            for (int i = 0; i < count; i++)
            {
				// First calculate a random position of this star in the sphere
                Vector3 starPosition = new Vector3(random.NormallyDistributedSingle(_deviationX * _size, 0), random.NormallyDistributedSingle(_deviationY * _size, 0), random.NormallyDistributedSingle(_deviationZ * _size, 0));
				float starPositionFractionOfSphereRadius = starPosition.magnitude / _size;

				// Generate a new star system at this position
				SpaceLibrary.StarSystem starSystem = new SpaceLibrary.StarSystem();
				starSystem.Position = new SpaceLibrary.GalacticCoordinate(starPosition.x, starPosition.y, starPosition.z);

				// Generate a random star system name.
				starSystem.Name = StarName.Generate(random);

				// Generate a single star in this system, with a 0 orbital distance (no binary systems)
				SpaceLibrary.Star[] stars = new SpaceLibrary.Star[1];
				starSystem.Stars = stars;

				stars[0] = new SpaceLibrary.Star();
				stars[0].OrbitalDistance = 0.0f;

				// Now calculate the class of the star, based on percentage probability.
				// Bias this toward a higher class of star nearer the galactic centre.
				double classUnit = random.NormallyDistributedSingle(50, Mathf.Lerp(80, 20, starPositionFractionOfSphereRadius), 0, 100);
				if (classUnit < 0.00003)
					stars[0].Classification = SpaceLibrary.Star.Class.O;
				else if (classUnit < (0.00003 + 0.13))
					stars[0].Classification = SpaceLibrary.Star.Class.B;
				else if (classUnit < (0.00003 + 0.13 + 0.6))
					stars[0].Classification = SpaceLibrary.Star.Class.A;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0))
					stars[0].Classification = SpaceLibrary.Star.Class.F;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0 + 7.6))
					stars[0].Classification = SpaceLibrary.Star.Class.G;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0 + 7.6 + 12.1))
					stars[0].Classification = SpaceLibrary.Star.Class.K;
				else
					stars[0].Classification = SpaceLibrary.Star.Class.M;

				// Calculate a size and temperature of the star based on the class
				switch (stars[0].Classification)
				{
				case SpaceLibrary.Star.Class.O:
					stars[0].Radius = Mathf.Lerp(6.6f, 20.0f, (float)random.NextDouble());
					stars[0].Temperature = random.NormallyDistributedSingle(8000, 40000, 30000, 60000);
					break;
				case SpaceLibrary.Star.Class.B:
					stars[0].Radius = Mathf.Lerp(1.8f, 6.6f, (float)random.NextDouble());
					stars[0].Temperature = Mathf.Lerp(10000, 30000, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.A:
					stars[0].Radius = Mathf.Lerp(1.4f, 1.8f, (float)random.NextDouble());
					stars[0].Temperature = Mathf.Lerp(7500, 10000, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.F:
					stars[0].Radius = Mathf.Lerp(1.15f, 1.4f, (float)random.NextDouble());
					stars[0].Temperature = Mathf.Lerp(6000, 7500, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.G:
					stars[0].Radius = Mathf.Lerp(0.96f, 1.15f, (float)random.NextDouble());
					stars[0].Temperature = Mathf.Lerp(5200, 6000, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.K:
					stars[0].Radius = Mathf.Lerp(0.7f, 0.96f, (float)random.NextDouble());
					stars[0].Temperature = Mathf.Lerp(3700, 5200, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.M:
					stars[0].Radius = Mathf.Lerp(0.4f, 0.7f, (float)random.NextDouble());
					stars[0].Temperature = Mathf.Lerp(2400, 3700, (float)random.NextDouble());
					break;
				}

				// Assign the system name to the star too
				// TODO: Use classification and temperature to calculate post fix
				stars[0].Name = starSystem.Name;

				yield return starSystem;
            }
        }
    }
}
