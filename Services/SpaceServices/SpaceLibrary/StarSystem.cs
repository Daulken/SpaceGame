using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Coordinates in light years from galactic centre
	/// </summary>
	public class GalacticCoordinate
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

		public GalacticCoordinate(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	};

	public class StarSystem
    {
		/// <summary>
		/// Position of the star system relative to galactic centre
		/// </summary>
		public GalacticCoordinate Position
		{
			get; set;
		}

		/// <summary>
		/// Name of the star system
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// The stars present in this system
		/// </summary>
		public Star[] Stars
		{
			get; set;
		}

		/// <summary>
		/// The planets present in this system
		/// </summary>
		public Planet[] Planets
		{
			get; set;
		}

		/// <summary>
		/// The asteroid belts present in this system
		/// </summary>
		public AsteroidBelt[] AsteroidBelts
		{
			get; set;
		}

	}
}
