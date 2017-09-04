using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Type of bodies found in the belt.
	/// </summary>
	public enum BeltBodyType
	{
		/// <summary>
		/// Rocky, mineral rich bodies, such as asteroids
		/// </summary>
		Rock,

		/// <summary>
		/// Icy bodies such as found in the Oord cloud
		/// </summary>
		Ice,
	}

	/// <summary>
	/// Description of a belt
	/// </summary>
	public class Belt
    {
		/// <summary>
		/// Orbital distance in astronomical units from the system centre
		/// </summary>
		public double OrbitalDistance
		{
			get; set;
		}

		/// <summary>
		/// Width of the belt in astronomical units
		/// </summary>
		public double Width
		{
			get; set;
		}

		/// <summary>
		/// Name of the belt
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// Number of bodies left in the belt
		/// </summary>
		public int BodyCount
		{
			get; set;
		}

		/// <summary>
		/// Type of body found in the belt
		/// </summary>
		public BeltBodyType BodyType
		{
			get; set;
		}
	}

}
