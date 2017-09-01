using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarGeneration
{
    public class Galaxy
    {
        public IEnumerable<SpaceLibrary.Star> Stars { get; private set; }

        private Galaxy(IEnumerable<SpaceLibrary.Star> stars)
        {
            Stars = stars;
        }

		public static Galaxy Generate(BaseGalaxySpec spec, System.Random random)
		{
			return new Galaxy(spec.Generate(random));
		}
	}
}
