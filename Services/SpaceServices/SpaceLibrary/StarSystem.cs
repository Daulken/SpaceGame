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
		/// <summary>
		/// X position
		/// </summary>
		public double X
		{
			get; set;
		}

		/// <summary>
		/// Y position
		/// </summary>
		public double Y
		{
			get; set;
		}

		/// <summary>
		/// Z position
		/// </summary>
		public double Z
		{
			get; set;
		}

		/// <summary>
		/// Construct a new galactic coordinate from an initially known position
		/// </summary>
		public GalacticCoordinate(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	};

	/// <summary>
	/// Description of a star system
	/// </summary>
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
