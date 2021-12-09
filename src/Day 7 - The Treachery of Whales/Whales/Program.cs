using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Whales
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            var positionsRaw = inputText.First().Split(',');
            var positions = new List<int>();
            foreach (var position in positionsRaw)
            {
                positions.Add(int.Parse(position));
            }

            var fuelCosts = new List<int>();
            int minCost = int.MaxValue;
            int minPosition = -1;
            for (int i = 0; i < positions.Count; i++)
            {
                int fuelCost = 0;
                for (int j = 0; j < positions.Count; j++)
                {
                    if (i == j)
                        continue;

                    //fuelCost += Math.Abs(positions[j] - positions[i]);

                    var absDiff = Math.Abs(positions[j] - positions[i]);
                    int curCost = 1;
                    for (int k = 0; k < absDiff; k++)
                    {
                        fuelCost += curCost++;
                    }
                }

                if (fuelCost < minCost)
                {
                    minCost = fuelCost;
                    minPosition = positions[i];
                }

                fuelCosts.Add(fuelCost);
            }

            Debug.WriteLine($"Cheapest Position: {minPosition}  Cost: {minCost}");

            Console.ReadKey();
        }
    }
}
