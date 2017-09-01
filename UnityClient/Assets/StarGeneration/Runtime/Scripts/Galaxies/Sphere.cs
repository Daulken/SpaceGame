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

        protected internal override IEnumerable<SpaceLibrary.Star> Generate(System.Random random)
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

				// Generate a new star at this position
				SpaceLibrary.Star star = new SpaceLibrary.Star();
				star.Position = new SpaceLibrary.Star.GalacticCoordinate(starPosition.x, starPosition.y, starPosition.z);

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
				// Class,	Effective Temp.,	Vega-relative colour label,	Chromaticity,		Main-sequence mass,	Main-sequence radius,	Main-sequence luminosity,	Hydrogen lines,	Fraction of stars
				//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
				// O,		≥ 30,000 K,			blue,						blue,				≥ 16 M☉,			≥ 6.6 R☉,				≥ 30,000 L☉,				Weak,			~0.00003%
				// B,		10,000–30,000 K,	blue white,					deep blue white,	2.1–16 M☉,			1.8–6.6 R☉,				25–30,000 L☉,				Medium,			0.13%
				// A,		7,500–10,000 K,		white,						blue white,			1.4–2.1 M☉,			1.4–1.8 R☉,				5–25 L☉,					Strong,			0.6%
				// F,		6,000–7,500 K,		yellow white,				white,				1.04–1.4 M☉,		1.15–1.4 R☉,			1.5–5 L☉,					Medium,			3%
				// G,		5,200–6,000 K,		yellow,						yellowish white,	0.8–1.04 M☉,		0.96–1.15 R☉,			0.6–1.5 L☉,					Weak,			7.6%
				// K,		3,700–5,200 K,		orange,						pale yellow orange,	0.45–0.8 M☉,		0.7–0.96 R☉,			0.08–0.6 L☉,				Very weak,		12.1%
				// M,		2,400–3,700 K,		red,						light orange red,	0.08–0.45 M☉,		≤ 0.7 R☉,				≤ 0.08 L☉,					Very weak,		76.45%

				// Now calculate the class of the star, based on percentage probability.
				// Bias this toward a higher class of star nearer the galactic centre.
				double classUnit = random.NormallyDistributedSingle(50, Mathf.Lerp(80, 20, starPositionFractionOfSphereRadius), 0, 100);
				if (classUnit < 0.00003)
					star.Classification = SpaceLibrary.Star.Class.O;
				else if (classUnit < (0.00003 + 0.13))
					star.Classification = SpaceLibrary.Star.Class.B;
				else if (classUnit < (0.00003 + 0.13 + 0.6))
					star.Classification = SpaceLibrary.Star.Class.A;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0))
					star.Classification = SpaceLibrary.Star.Class.F;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0 + 7.6))
					star.Classification = SpaceLibrary.Star.Class.G;
				else if (classUnit < (0.00003 + 0.13 + 0.6 + 3.0 + 7.6 + 12.1))
					star.Classification = SpaceLibrary.Star.Class.K;
				else
					star.Classification = SpaceLibrary.Star.Class.M;

				// Calculate a size and temperature of the star based on the class
				switch (star.Classification)
				{
				case SpaceLibrary.Star.Class.O:
					star.Size = Mathf.Lerp(6.6f, 20.0f, (float)random.NextDouble());
					star.Temperature = random.NormallyDistributedSingle(8000, 40000, 30000, 60000);
					break;
				case SpaceLibrary.Star.Class.B:
					star.Size = Mathf.Lerp(1.8f, 6.6f, (float)random.NextDouble());
					star.Temperature = Mathf.Lerp(10000, 30000, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.A:
					star.Size = Mathf.Lerp(1.4f, 1.8f, (float)random.NextDouble());
					star.Temperature = Mathf.Lerp(7500, 10000, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.F:
					star.Size = Mathf.Lerp(1.15f, 1.4f, (float)random.NextDouble());
					star.Temperature = Mathf.Lerp(6000, 7500, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.G:
					star.Size = Mathf.Lerp(0.96f, 1.15f, (float)random.NextDouble());
					star.Temperature = Mathf.Lerp(5200, 6000, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.K:
					star.Size = Mathf.Lerp(0.7f, 0.96f, (float)random.NextDouble());
					star.Temperature = Mathf.Lerp(3700, 5200, (float)random.NextDouble());
					break;
				case SpaceLibrary.Star.Class.M:
					star.Size = Mathf.Lerp(0.4f, 0.7f, (float)random.NextDouble());
					star.Temperature = Mathf.Lerp(2400, 3700, (float)random.NextDouble());
					break;
				}

				// Generate a random star name.
				// TODO: Use classification and temperature to calculate post fix
				star.Name = StarName.Generate(random);

				yield return star;
            }
        }
    }
}
