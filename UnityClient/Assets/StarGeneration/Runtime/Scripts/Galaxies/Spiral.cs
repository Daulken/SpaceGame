﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration.Galaxies
{
    public class Spiral
        : BaseGalaxySpec
    {
        public int Size { get; set; }
        public int Spacing { get; set; }

        public int MinimumArms { get; set; }
        public int MaximumArms { get; set; }

        public float ClusterCountDeviation { get; set; }
        public float ClusterCenterDeviation { get; set; }

        public float MinArmClusterScale { get; set; }
        public float ArmClusterScaleDeviation { get; set; }
        public float MaxArmClusterScale { get; set; }

        public float Swirl { get; set; }

        public float CenterClusterScale { get; set; }
        public float CenterClusterDensityMean { get; set; }
        public float CenterClusterDensityDeviation { get; set; }
        public float CenterClusterSizeDeviation { get; set; }

        public float CenterClusterPositionDeviation { get; set; }
        public float CenterClusterCountDeviation { get; set; }
        public float CenterClusterCountMean { get; set; }

        public float CentralVoidSizeMean { get; set; }
        public float CentralVoidSizeDeviation { get; set; }

        public Spiral()
        {
            Size = 750;
            Spacing = 5;

            MinimumArms = 3;
            MaximumArms = 7;

            ClusterCountDeviation = 0.35f;
            ClusterCenterDeviation = 0.2f;

            MinArmClusterScale = 0.02f;
            ArmClusterScaleDeviation = 0.02f;
            MaxArmClusterScale = 0.1f;

            Swirl = (float)Math.PI * 4;

            CenterClusterScale = 0.19f;
            CenterClusterDensityMean = 0.00005f;
            CenterClusterDensityDeviation = 0.000005f;
            CenterClusterSizeDeviation = 0.00125f;

            CenterClusterCountMean = 20;
            CenterClusterCountDeviation = 3;
            CenterClusterPositionDeviation = 0.075f;

            CentralVoidSizeMean = 25;
            CentralVoidSizeDeviation = 7;
        }

		protected internal override IEnumerable<SpaceLibrary.StarSystem> Generate(System.Random random)
		{
			var centralVoidSize = random.NormallyDistributedSingle(CentralVoidSizeDeviation, CentralVoidSizeMean);
			if (centralVoidSize < 0)
				centralVoidSize = 0;
			var centralVoidSizeSqr = centralVoidSize * centralVoidSize;

			foreach (var starSystem in GenerateArms(random))
			{
				if (starSystem.Position.ToVector3().sqrMagnitude > centralVoidSizeSqr)
					yield return starSystem;
			}

			foreach (var starSystem in GenerateCenter(random))
			{
				if (starSystem.Position.ToVector3().sqrMagnitude > centralVoidSizeSqr)
					yield return starSystem;
			}

			foreach (var starSystem in GenerateBackgroundStars(random))
			{
				if (starSystem.Position.ToVector3().sqrMagnitude > centralVoidSizeSqr)
					yield return starSystem;
			}
		}

        private IEnumerable<SpaceLibrary.StarSystem> GenerateBackgroundStars(System.Random random)
        {
            return new Sphere(Size, 0.000001f, 0.0000001f, 0.35f, 0.125f, 0.35f).Generate(random);
        }

        private IEnumerable<SpaceLibrary.StarSystem> GenerateCenter(System.Random random)
        {
            //Add a single central cluster
            var sphere = new Sphere(
                size: Size * CenterClusterScale,
                densityMean: CenterClusterDensityMean,
                densityDeviation: CenterClusterDensityDeviation,
                deviationX: CenterClusterScale,
                deviationY: CenterClusterScale,
                deviationZ: CenterClusterScale
            );

            var cluster = new Cluster(sphere,
                CenterClusterCountMean, CenterClusterCountDeviation, Size * CenterClusterPositionDeviation, Size * CenterClusterPositionDeviation, Size * CenterClusterPositionDeviation
            );

            foreach (var starSystem in cluster.Generate(random))
                yield return starSystem.Swirl(Vector3.up, Swirl * 5);
        }

        private IEnumerable<SpaceLibrary.StarSystem> GenerateArms(System.Random random)
        {
            int arms = random.Next(MinimumArms, MaximumArms);
            float armAngle = (float) ((Math.PI * 2) / arms);

            int maxClusters = (Size / Spacing) / arms;
            for (int arm = 0; arm < arms; arm++)
            {
                int clusters = (int) Math.Round(random.NormallyDistributedSingle(maxClusters * ClusterCountDeviation, maxClusters));
                for (int i = 0; i < clusters; i++)
                {
                    //Angle from center of this arm
                    float angle = random.NormallyDistributedSingle(0.5f * armAngle * ClusterCenterDeviation, 0) + armAngle * arm;

                    //Distance along this arm
                    float dist = Math.Abs(random.NormallyDistributedSingle(Size * 0.4f, 0));

                    //Center of the cluster
                    var center = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(0, 0, dist);

                    //Size of the cluster
                    var clsScaleDev = ArmClusterScaleDeviation * Size;
                    var clsScaleMin = MinArmClusterScale * Size;
                    var clsScaleMax = MaxArmClusterScale * Size;
                    var cSize = random.NormallyDistributedSingle(clsScaleDev, clsScaleMin * 0.5f + clsScaleMax * 0.5f, clsScaleMin, clsScaleMax);

                    var starSystems = new Sphere(cSize, densityMean: 0.00025f, deviationX: 1, deviationY: 1, deviationZ: 1).Generate(random);
                    foreach (var starSystem in starSystems)
                        yield return starSystem.Offset(center).Swirl(Vector3.up, Swirl);
                }
            }
        }
    }
}
