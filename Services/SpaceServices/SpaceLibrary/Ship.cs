using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Description of a ship
	/// </summary>
	public class Ship
    {
		/// <summary>
		/// Default constructor for a new ship
		/// </summary>
		public Ship()
        {
#if DEBUG
            ShipName = "Lollypop";
            CargoModules = 2;
            CargoContents = new List<Cargo>();
            CargoContents.Add(new SpaceLibrary.Cargo() { CargoMaterial = Material.Metal1, CargoValue = 100 });
            CargoContents.Add(new SpaceLibrary.Cargo() { CargoMaterial = Material.Liquid2, CargoValue = 10 });
#endif
        }

		/// <summary>
		/// The name of the ship
		/// </summary>
		public string ShipName
		{
			get; set;
		}

		/// <summary>
		/// The number of cargo modules available to carry things in
		/// </summary>
		public int CargoModules
		{
			get; set;
		}

		/// <summary>
		/// The currently carried cargo
		/// </summary>
		public List<Cargo> CargoContents
		{
			get; set;
		}
    }
}
