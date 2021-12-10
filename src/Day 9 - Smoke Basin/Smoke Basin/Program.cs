using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

namespace Smoke_Basin
{
    internal class Program
    {
        private static int Height = 0;
        private static int Width = 0;
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();
          
            Height = inputText.Count;
            Width = inputText.First().Length;

            var map = new int[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pChar = inputText[y][x];
                    map[x, y] = int.Parse(pChar.ToString());
                }
            }

            // Find lowest points.
            var lowestPoints = new List<Point>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var point = map[x, y];
                    var neighbors = GetNeighbors(map, new Point(x, y));

                    bool isLowest = true;
                    neighbors.ForEach(n =>
                    {
                        if (point >= map[n.X, n.Y])
                            isLowest = false;
                    });

                    if (isLowest)
                        lowestPoints.Add(new Point(x, y));

                }
            }

            // Compute risk level.
            int riskLevel = 0;
            lowestPoints.ForEach(p => riskLevel += (map[p.X, p.Y] + 1));

            // Find basins.
            var basins = new List<int>();
            foreach (var p in lowestPoints)
            {
                var sum = FindBasin(map, p, null);
                basins.Add(sum + 1);
            }

            // Sum largest 3 basins.
            int basinSum = 1;
            basins = basins.OrderByDescending(s => s).Take(3).ToList();
            basins.ForEach(s => basinSum *= s);

            Debug.WriteLine($"Risk Level: {riskLevel}  Basin Sum: {basinSum}");

            Console.ReadKey();

        }

        private static int FindBasin(int[,] grid, Point pnt, List<Point> visited)
        {
            if (visited == null)
                visited = new List<Point>();

            visited.Add(pnt);

            int sum = 0;
            var ns = GetNeighbors(grid, pnt);
            foreach (var n in ns)
            {
                if (visited.Contains(n))
                    continue;
                else
                    visited.Add(n);

                var val = grid[n.X, n.Y];
                if (val < 9)
                {
                    sum += FindBasin(grid, n, visited) + 1;
                }
            }

            return sum;
        }

        private static List<Point> GetNeighbors(int[,] grid, Point pnt)
        {
            var neighbors = new List<Point>();
            for (int xOff = -1; xOff <= 1; xOff++)
            {
                for (int yOff = -1; yOff <= 1; yOff++)
                {
                    if (xOff == 0 || yOff == 0)
                    {
                        if (xOff == 0 && yOff == 0)
                            continue;

                        int nX = pnt.X + xOff;
                        int nY = pnt.Y + yOff;

                        if (nX >= 0 && nY >= 0 && nX < Width && nY < Height)
                            neighbors.Add(new Point(nX, nY));

                    }
                }
            }

            return neighbors;
        }
    }
}
