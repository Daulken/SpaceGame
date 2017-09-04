using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
    public abstract class BaseGalaxySpec
    {
        protected internal abstract IEnumerable<SpaceLibrary.System> Generate(System.Random random);
    }
}
