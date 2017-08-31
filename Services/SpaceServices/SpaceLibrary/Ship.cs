using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLibrary
{
    public class Ship
    {
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
        public string ShipName { get; set; }
        public int CargoModules { get; set; }
        public List<Cargo> CargoContents { get; set; }
    }
}
