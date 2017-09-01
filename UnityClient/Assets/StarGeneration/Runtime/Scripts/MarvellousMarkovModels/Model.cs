using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace MarvellousMarkovModels
{
    public class Model
    {
        private readonly int _order;
        private readonly KeyValuePair<string, float>[] _startingStrings;
		private readonly List<string> _seedNames;

		private readonly Dictionary<string, KeyValuePair<string, float>[]> _productions;  

        public Model(int order, KeyValuePair<string, float>[] startingStrings, Dictionary<string, KeyValuePair<string, float>[]> productions, List<string> seedNames)
        {
            _order = order;
            _startingStrings = startingStrings;
			_seedNames = seedNames;
			_productions = productions;
        }

		public string Generate(Random random)
		{
			string generatedName = GenerateInternal(random).Trim();
			while (_seedNames.Contains(generatedName))
				generatedName = GenerateInternal(random).Trim();
			return generatedName;
		}

		private string GenerateInternal(Random random)
        {
            string builder = "";

            string lastSelected = WeightedRandom(random, _startingStrings);

            do
            {
                //Extend string
                builder += lastSelected;
                if (builder.Length < _order)
                    break;

                //Key to use to find next production
                var key = builder.Substring(builder.Length - _order);

                //Find production rules for this key
                KeyValuePair<string, float>[] prod;
                if (!_productions.TryGetValue(key, out prod))
                    break;

                //Produce next expansion
                lastSelected = WeightedRandom(random, prod);
            }
			while (!string.IsNullOrEmpty(lastSelected));

			// Convert the name to title case
			return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(builder.ToLower()).Trim();
        }

        public static string WeightedRandom(Random random, KeyValuePair<string, float>[] items)
        {
            var num = random.NextDouble();

            for (int i = 0; i < items.Length; i++)
            {
                num -= items[i].Value;
                if (num <= 0)
                    return items[i].Key;
            }

            throw new InvalidOperationException();
        }
    }
}
