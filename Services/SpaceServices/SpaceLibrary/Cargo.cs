using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLibrary
{
    public class Cargo
    {
        public Material CargoMaterial { get; set; }
        public double CargoValue { get; set; }
    }

    public enum Material
    {
        Metal1,
        Metal2, 
        Metal3,
        Metal4,
        Metal5,
        Liquid1,
        Liquid2,
        Liquid3,
        Liquid4,
        Liquid5,
        Gas1,
        Gas2,
        Gas3,
        Gas4,
        Gas5
    }
}
