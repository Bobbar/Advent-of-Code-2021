using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

namespace Syntax_Scoring
{
    internal class Program
    {

        private static readonly Dictionary<char, char> ChunkClosers = new Dictionary<char, char>()
        {
            {'(',')'},
            {'[',']'},
            {'{','}'},
            {'<','>'},
        };

        private static Dictionary<char, int> SyntaxScores = new Dictionary<char, int>()
        {
            {')', 0},
            {']', 0},
            {'}', 0},
            {'>', 0}
        };

        private static readonly Dictionary<char, int> SyntaxPointsPart1 = new Dictionary<char, int>()
        {
            {')', 3},
            {']', 57},
            {'}', 1197},
            {'>', 25137}
        };

        private static readonly Dictionary<char, int> SyntaxPointsPart2 = new Dictionary<char, int>()
        {
            {')', 1},
            {']', 2},
            {'}', 3},
            {'>', 4}
        };

        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            // Remove all complete blocks.
            var trimmedBlocks = new List<string>();
            foreach (var line in inputText)
            {
                var trimBlocks = line;

                while (trimBlocks.Contains("()") || trimBlocks.Contains("[]") || trimBlocks.Contains("{}") || trimBlocks.Contains("<>"))
                {
                    trimBlocks = trimBlocks.Replace("()", "");
                    trimBlocks = trimBlocks.Replace("[]", "");
                    trimBlocks = trimBlocks.Replace("{}", "");
                    trimBlocks = trimBlocks.Replace("<>", "");
                }

                trimmedBlocks.Add(trimBlocks);
            }

            var incompleteBlocks = new List<string>();

            // Compute syntax counts and record incomplete blocks.
            for (int i = 0; i < trimmedBlocks.Count; i++)
            {
                var block = trimmedBlocks[i];
                int pos = 0;
                bool errorFound = false;

                while (pos < block.Length - 1)
                {
                    var cur = block[pos];
                    var next = block[pos + 1];

                    if (ChunkClosers.Values.Contains(next))
                    {
                        SyntaxScores[next]++;
                        errorFound = true;
                        break;
                    }

                    pos++;
                }

                if (!errorFound)
                    incompleteBlocks.Add(block);
            }

            // Part 1 score.
            int finalScore = 0;
            foreach (var val in SyntaxScores.Keys)
            {
                finalScore += SyntaxScores[val] * SyntaxPointsPart1[val];
            }

            Console.WriteLine($"Part 1 Final Score: {finalScore}");


            // Part 2 score.
            var lineScores = new List<long>();
            foreach (var block in incompleteBlocks)
            {
                var revBlock = new string(block.Reverse().ToArray());
                long lineScore = 0;

                for (int i = 0; i < revBlock.Length; i++)
                {
                    var cur = revBlock[i];

                    lineScore *= 5;
                    lineScore += SyntaxPointsPart2[ChunkClosers[cur]];
                }

                lineScores.Add(lineScore);
            }

            lineScores = lineScores.OrderBy(x => x).ToList();
            var middleScore = lineScores[(lineScores.Count - 1) / 2];

            Console.WriteLine($"Part 2 Final Score: {middleScore}");

            Console.ReadKey();
        }
    }
}
