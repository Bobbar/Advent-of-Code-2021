using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Binary_Diagnostic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();
            int bitsPerLine = inputText.First().Length;
            var bitLines = new List<int[]>();
            foreach (var line in inputText)
            {
                var bitLine = new int[bitsPerLine];
                for (int i = 0; i < bitsPerLine; i++)
                    bitLine[i] = int.Parse(line[i].ToString());

                bitLines.Add(bitLine);
            }

            var bitLinesOxy = new List<int[]>(bitLines);
            var bitLinesCO2 = new List<int[]>(bitLines);

            for (int i = 0; i < bitsPerLine; i++)
            {
                int ones = 0;
                int zeros = 0;
                for (int j = 0; j < bitLinesOxy.Count; j++)
                {
                    if (bitLinesOxy[j][i] == 1)
                        ones++;
                    else
                        zeros++;
                }

                int mostCommon = ones > zeros ? 1 : ones == zeros ? 1 : 0;
                if (bitLinesOxy.Count > 1)
                    bitLinesOxy.RemoveAll(bit => bit[i] != mostCommon);

                ones = 0;
                zeros = 0;
                for (int j = 0; j < bitLinesCO2.Count; j++)
                {
                    if (bitLinesCO2[j][i] == 1)
                        ones++;
                    else
                        zeros++;
                }

                int leastCommon = ones < zeros ? 1 : 0;
                if (bitLinesCO2.Count > 1)
                    bitLinesCO2.RemoveAll(bit => bit[i] != leastCommon);
            }

            var oxyRating = ConvertToInteger(bitLinesOxy.First());
            var co2Rating = ConvertToInteger(bitLinesCO2.First());
            var lifeSupportRating = oxyRating * co2Rating;
            Debug.WriteLine($"Life support rating: {lifeSupportRating}");

            //var gamma = new int[bitsPerLine];
            //var epsilon = new int[bitsPerLine];

            //for (int i = 0; i < bitsPerLine; i++)
            //{
            //    int ones = 0, zeros = 0;
            //    for (int j = 0; j < bitLines.Count; j++)
            //    {
            //        if (bitLines[j][i] == 1)
            //            ones++;
            //        else
            //            zeros++;
            //    }

            //    gamma[i] = ones > zeros ? 1 : 0;
            //    epsilon[i] = ones < zeros ? 1 : 0;
            //}

            //var gammaVal = ConvertToInteger(gamma);
            //var epsilonVal = ConvertToInteger(epsilon);
            //var power = gammaVal * epsilonVal;

            //Debug.WriteLine($"Power consumption: {power}");
            Console.ReadKey();
        }

        // Eh?
        private static int ConvertToInteger(int[] input)
        {
            int value = 0;
            int max = 1 << input.Length;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == 1)
                    value += i == input.Length - 1 ? 1 : max >> (i + 1);
            }

            return value;
        }
    }
}
