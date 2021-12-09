using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

namespace Hydrothermal_Venture
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            var points = new List<Point>();
            var minXY = new Point();
            var maxXY = new Point();

            // Parse the points.
            foreach (var line in inputText)
            {
                var arrowPos = line.IndexOf("->");

                var xy1Str = line.Substring(0, arrowPos - 1).Trim();
                var xy2tr = line.Substring(arrowPos + 2, line.Length - arrowPos - 2).Trim();

                var xy1Coords = xy1Str.Split(',');
                var xy2Coords = xy2tr.Split(',');

                points.Add(new Point(int.Parse(xy1Coords[0]), int.Parse(xy1Coords[1])));
                points.Add(new Point(int.Parse(xy2Coords[0]), int.Parse(xy2Coords[1])));

            }

            // Determine the grid bounds and allocate it.
            foreach (var point in points)
            {
                minXY = new Point(Math.Min(minXY.X, point.X), Math.Min(minXY.Y, point.Y));
                maxXY = new Point(Math.Max(maxXY.X, point.X), Math.Max(maxXY.Y, point.Y));
            }

            var cols = maxXY.X - minXY.X;
            var rows = maxXY.Y - minXY.Y;
            var grid = new int[(cols + 1), (rows + 1)];

            // Plot the lines onto the grid and accumulate overlaps.
            for (int i = 0; i < points.Count; i += 2)
            {
                var xy1 = points[i];
                var xy2 = points[i + 1];

                var diffX = xy2.X - xy1.X;
                var diffY = xy2.Y - xy1.Y;
                var signX = Math.Sign(diffX);
                var signY = Math.Sign(diffY);
                var absMax = Math.Max(Math.Abs(diffX), Math.Abs(diffY));

                //Debug.WriteLine($"XY1: {xy1}  XY2: {xy2}");

                // Handle all directions. (Part 2)
                grid[xy1.X, xy1.Y]++;
                for (int p = 0; p < absMax; p++)
                {
                    grid[xy1.X += signX, xy1.Y += signY]++;
                }


                //// Handle horz/vert only. (Part 1)
                //if (xy1.X == xy2.X || xy1.Y == xy2.Y)
                //{
                //    int startX = Math.Min(xy1.X, xy2.X);
                //    int startY = Math.Min(xy1.Y, xy2.Y);
                //    int endX = Math.Max(xy1.X, xy2.X);
                //    int endY = Math.Max(xy1.Y, xy2.Y);

                //    for (int x = startX; x <= endX; x++)
                //    {
                //        for (int y = startY; y <= endY; y++)
                //        {
                //            grid[x, y]++;
                //        }
                //    }
                //}
            }

            //DrawGrid(cols, rows, grid);

            // Count the overlaps.
            int overlaps = 0;
            foreach (var pnt in grid)
            {
                if (pnt >= 2)
                    overlaps++;
            }

            Debug.WriteLine($"Overlapping points: {overlaps}");

            Console.ReadKey();
        }

        private static void DrawGrid(int cols, int rows, int[,] grid)
        {
            for (int c = 0; c <= cols; c++)
            {
                Debug.WriteLine("");
               
                for (int r = 0; r <= rows; r++)
                {
                    Debug.Write($"{grid[r, c]}");
                }
            }

            Debug.WriteLine("");
        }
    }
}
