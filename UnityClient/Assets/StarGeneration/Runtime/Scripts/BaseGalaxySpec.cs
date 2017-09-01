using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
    public abstract class BaseGalaxySpec
    {
        protected internal abstract IEnumerable<SpaceLibrary.Star> Generate(System.Random random);
    }
}
