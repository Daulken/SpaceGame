using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
    public abstract class BaseGalaxySpec
    {
        protected internal abstract IEnumerable<SpaceLibrary.StarSystem> Generate(System.Random random);
    }
}
