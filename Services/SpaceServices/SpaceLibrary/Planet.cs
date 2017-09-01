using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
    public class Planet
    {
		// Coordinates in astronomical units from the star
		public class SystemCoordinate
		{
			public double X
			{
				get; set;
			}
			public double Y
			{
				get; set;
			}
			public double Z
			{
				get; set;
			}

			public SystemCoordinate(double x, double y, double z)
			{
				X = x;
				Y = y;
				Z = z;
			}
		};

		public SystemCoordinate Position
		{
			get; set;
		}

		public string Name
		{
			get; set;
		}

		// Size in kilometers
		public double Size
		{
			get; set;
		}

	}

}
