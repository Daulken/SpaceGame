using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
    public class Galaxy
    {
        public IEnumerable<SpaceLibrary.StarSystem> StarSystems { get; private set; }

        private Galaxy(IEnumerable<SpaceLibrary.StarSystem> starSystems)
        {
			StarSystems = starSystems;
        }

		public static Galaxy Generate(BaseGalaxySpec spec, System.Random random)
		{
			return new Galaxy(spec.Generate(random));
		}
	}
}
