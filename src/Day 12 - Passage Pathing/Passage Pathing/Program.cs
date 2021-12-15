using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Passage_Pathing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            var caves = new List<Cave>();

            // Get all unique caves.
            foreach (var line in inputText)
            {
                var cs = line.Split('-');
                var first = cs[0];
                var second = cs[1];

				if (!caves.ContainsCave(first))
					caves.Add(new Cave(first));

				if (!caves.ContainsCave(second))
					caves.Add(new Cave(second));
            }

            // Link caves.
            foreach (var line in inputText)
            {
                var cs = line.Split('-');
                var first = cs[0];
                var second = cs[1];

                var firstCave = caves.GetCave(first);
                var secondCave = caves.GetCave(second);

                firstCave.Caves.Add(secondCave);
                secondCave.Caves.Add(firstCave);
            }

            var start = caves.GetCave("start");
            var end = caves.GetCave("end");

            var countPart1 = CountPart1(caves, start, null, end);
            var countPart2 = CountPart2(caves, start, null, start, end);

            Console.WriteLine($"Part 1: {countPart1}");
            Console.WriteLine($"Part 2: {countPart2}");

            Console.ReadKey();
        }

        private static int CountPart2(List<Cave> caves, Cave node, List<Cave> visited, Cave start, Cave end, bool doubleTest = false)
        {
            if (visited == null)
                visited = new List<Cave>();

            if (node.Equals(end))
                return 1;

            int total = 0;
            foreach (var next in node.Caves)
            {
                if (next.Equals(start))
                    continue;

                if (visited.Contains(next) && doubleTest)
                    continue;

                var unionVisited = visited;

                if (node.IsSmall)
                    unionVisited = unionVisited.Union(new List<Cave> { node }).ToList();
                else
                    unionVisited = unionVisited.Union(visited).ToList();

                if (visited.Contains(next))
                {
                    total += CountPart2(caves, next, unionVisited, start, end, true);
                }
                else
                {
                    total += CountPart2(caves, next, unionVisited, start, end, doubleTest);
                }
            }

            return total;
        }

        private static int CountPart1(List<Cave> caves, Cave node, List<Cave> visited, Cave end)
        {
            if (visited == null)
                visited = new List<Cave>();

            if (node.Equals(end))
                return 1;

            int total = 0;
            foreach (var next in node.Caves)
            {
                if (visited.Contains(next))
                    continue;

                var unionVisited = visited;

                if (node.IsSmall)
                    unionVisited = unionVisited.Union(new List<Cave> { node }).ToList();
                else
                    unionVisited = unionVisited.Union(visited).ToList();

                total += CountPart1(caves, next, unionVisited, end);
            }

            return total;
        }
    }


    public class Cave : IEquatable<Cave>
    {
        public string Name { get; set; }
        public List<Cave> Caves { get; set; } = new List<Cave>();

        public bool IsSmall
        {
            get
            {
                return Name.ToLower() == Name;
            }
        }

        public Cave(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public bool Equals(Cave other)
        {
            return Name == other.Name;
        }
    }
}
