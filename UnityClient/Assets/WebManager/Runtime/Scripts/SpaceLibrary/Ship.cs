using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
    public class Ship
    {
        public string ShipName { get; set; }
        public int CargoModules { get; set; }
        public List<Cargo> CargoContents { get; set; }
    }
}
