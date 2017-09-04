using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Description of a cargo
	/// </summary>
	public class Cargo
    {
		/// <summary>
		/// The material that the cargo is made of
		/// </summary>
		public Material CargoMaterial
		{
			get; set;
		}

		/// <summary>
		/// The value of the cargo
		/// </summary>
		public double CargoValue
		{
			get; set;
		}
    }

	/// <summary>
	/// Description of a cargo material
	/// </summary>
	public enum Material
    {
		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Metal1,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Metal2,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Metal3,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Metal4,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Metal5,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Liquid1,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Liquid2,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Liquid3,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Liquid4,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Liquid5,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Gas1,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Gas2,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Gas3,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Gas4,

		/// <summary>
		/// TO BE NAMED
		/// </summary>
		Gas5
	}
}
