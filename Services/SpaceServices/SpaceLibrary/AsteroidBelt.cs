using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Description of an asteroid belt
	/// </summary>
	public class AsteroidBelt
    {
		/// <summary>
		/// Orbital distance in astronomical units from the system centre
		/// </summary>
		public double OrbitalDistance
		{
			get; set;
		}

		/// <summary>
		/// Name of the asteroid belt
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// Number of asteroids left in the belt
		/// </summary>
		public int AsteroidCount
		{
			get; set;
		}
	}

}
