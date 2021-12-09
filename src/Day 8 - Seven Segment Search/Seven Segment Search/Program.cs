using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Seven_Segment_Search
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();
            var signals = new List<string>();
            var outputs = new List<string>();
            foreach (var line in inputText)
            {
                var inputSplit = line.Split('|');
                signals.Add(inputSplit[0].Trim());
                outputs.Add(inputSplit[1].Trim());
            }

            // Map for 0-9 on a standard 7-segment display.
            var digitMap = new Dictionary<string, string>()
            {
                {"abcdef","0"},
                {"bc", "1"},
                {"abdeg", "2"},
                {"abcdg", "3"},
                {"bcfg", "4"},
                {"acdfg", "5"},
                {"acdefg", "6"},
                {"abc", "7"},
                {"abcdefg", "8"},
                {"abcdfg", "9"},
            };

            // I'm sure there must be a better way to do this, but I surrender my clock cycles to just testing all permutations of the segments map...
            var displaySegments = new string[] { "a", "b", "c", "d", "e", "f", "g" };
            var permuteKeys = Permute(displaySegments, 0, displaySegments.Length - 1, new List<List<string>>());
            var segMap = new Dictionary<string, string>();

            int digitSum = 0;
            for (int i = 0; i < signals.Count; i++)
            {
                // Parse the signal segments then test against each permutation of map until we find a match.
                var signal = signals[i];
                var sigSegs = signal.Split(' ');

                bool keyFound = false;
                int pIdx = 0;
                while (!keyFound)
                {
                    // Build the map from the current permutation.
                    var key = permuteKeys[pIdx++];
                    var map = new Dictionary<string, string>();
                    for (int k = 0; k < displaySegments.Length; k++)
                    {
                        map.Add(displaySegments[k], key[k]);
                    }

                    // Test the current map to see if it works with all the signal segments.
                    int matches = 0;
                    foreach (var seg in sigSegs)
                    {
                        var decode = DecodeSeg(seg, map);
                        var sortSeg = string.Concat(decode.OrderBy(x => x));

                        if (digitMap.ContainsKey(sortSeg))
                            matches++;
                        else
                            break;
                    }

                    // Found a working map.
                    if (matches == sigSegs.Length)
                    {
                        segMap = map;
                        keyFound = true;
                    }
                }

                // Decode the output segments using the found map and sum the values.
                var op = outputs[i];
                var opSegs = op.Split(' ');
                var segDigits = new List<string>();
                foreach (var seg in opSegs)
                {
                    var decoded = DecodeSeg(seg, segMap);
                    var sortSeg = string.Concat(decoded.OrderBy(x => x));
                    var segDigit = digitMap[sortSeg];
                    segDigits.Add(segDigit);
                }

                var segInteger = int.Parse(string.Concat(segDigits));
                digitSum += segInteger;
            }

            Debug.WriteLine($"Output sum: {digitSum}");

            // Woof...
            Console.ReadKey();
        }

        private static List<List<string>> Permute(string[] segs, int start, int end, List<List<string>> list)
        {
            if (start == end)
            {
                list.Add(new List<string>(segs));
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    Swap(ref segs[start], ref segs[i]);
                    Permute(segs, start + 1, end, list);
                    Swap(ref segs[start], ref segs[i]);
                }
            }

            return list; ;
        }

        private static void Swap(ref string a, ref string b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        private static string DecodeSeg(string seg, Dictionary<string, string> map)
        {
            var decoded = new string[seg.Length];
            for (int i = 0; i < seg.Length; i++)
                decoded[i] = map[seg[i].ToString()];

            return string.Concat(decoded);
        }
    }
}
