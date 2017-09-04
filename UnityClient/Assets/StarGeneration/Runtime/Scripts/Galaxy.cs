using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
    public class Galaxy
    {
        public IEnumerable<SpaceLibrary.System> Systems
		{
			get; private set;
		}

        private Galaxy(IEnumerable<SpaceLibrary.System> systems)
        {
			Systems = systems;
        }

		public static Galaxy Generate(BaseGalaxySpec spec, System.Random random)
		{
			return new Galaxy(spec.Generate(random));
		}
	}
}
