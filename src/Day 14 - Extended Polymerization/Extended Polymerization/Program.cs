using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text;

namespace Extended_Polymerization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            var template = inputText.First();
            inputText.Remove(template);

            var rules = new Dictionary<string, string>();
            foreach (var line in inputText)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var parts = line.Replace("-> ", "").Split(' ');
                rules.Add(parts[0], parts[1]);
            }

            var currentPairs = new Dictionary<string, long>();
            bool done = false;
            int pos = 0;
            while (!done)
            {
                var pair = template.Substring(pos, 2);

                currentPairs.Add(pair, 1);

                pos++;

                if (pos + 1 >= template.Length)
                    done = true;
            }

			int steps = 40;

			for (int step = 0; step < steps; step++)
			{
				var newPairs = new Dictionary<string, long>();
				foreach (var pair in currentPairs)
				{
					var pairValue = pair.Key;
					var nPairs = pair.Value;

					if (rules.TryGetValue(pairValue, out string rule))
					{
						var firstPair = $"{pairValue[0]}{rule}";
						newPairs.TryAdd(firstPair, 0);
						newPairs[firstPair] += nPairs;

						var secondPair = $"{rule}{pairValue[1]}";
						newPairs.TryAdd(secondPair, 0);
						newPairs[secondPair] += nPairs;
					}
				}

				currentPairs = newPairs;
			}

			var counts = new Dictionary<char, long>();
			foreach (var pair in currentPairs)
			{
				if (pair.Value > 0)
				{
					var first = pair.Key[0];
					counts.TryAdd(first, 0);
					counts[first] += pair.Value;
				}
			}

			// Add the last char from the original template...
			counts.TryAdd(template.Last(), 0);
			counts[template.Last()]++;

			var mostCommon = counts.Max(c => c.Value);
			var leastCommon = counts.Min(c => c.Value);
			var result = mostCommon - leastCommon;
			Console.WriteLine($"After {steps} steps: {result}");

			Console.ReadKey();
        }
    }
}
