using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Description of a planet
	/// </summary>
	public class Planet
    {
		/// <summary>
		/// Orbital distance in astronomical units from the system centre
		/// </summary>
		public double OrbitalDistance
		{
			get; set;
		}

		/// <summary>
		/// Name of the planet
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// Radius in kilometers
		/// </summary>
		public double Radius
		{
			get; set;
		}

		/// <summary>
		/// Number of available plots to purchase
		/// </summary>
		public int PlotCount
		{
			get; set;
		}

		/// <summary>
		/// Number of plots left to purchase
		/// </summary>
		public int PlotsAvailable
		{
			get; set;
		}
	}

}
