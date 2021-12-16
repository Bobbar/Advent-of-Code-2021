using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text;

namespace Transparent_Origami
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            var dots = new List<Point>();
            var folds = new List<Point>();
            bool readCmds = false;
            while (inputText.Count > 0)
            {
                var line = inputText.First();

                if (string.IsNullOrEmpty(line))
                {
                    inputText.Remove(line);
                    line = inputText.First();
                    readCmds = true;
                }

                if (!readCmds)
                {
                    var xy = line.Split(',');
                    dots.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
                }
                else
                {
                    var val = line.Split('=').Last();
                    if (line.Contains("x"))
                        folds.Add(new Point(int.Parse(val), 0));
                    else if (line.Contains("y"))
                        folds.Add(new Point(0, int.Parse(val)));
                }

                inputText.Remove(line);
            }

            var min = new Point();
            var max = new Point();

            foreach (var dot in dots)
            {
                min.X = Math.Min(min.X, dot.X);
                min.Y = Math.Min(min.Y, dot.Y);

                max.X = Math.Max(max.X, dot.X);
                max.Y = Math.Max(max.Y, dot.Y);
            }

            var dotsGrid = new bool[max.X + 1, max.Y + 1];
            foreach (var dot in dots)
            {
                dotsGrid[dot.X, dot.Y] = true;
            }

            var folded = dotsGrid;
            foreach (var fold in folds)
            {
                folded = Fold(folded, fold);
            }

            //var gridString = DrawGridToString(folded);
            //File.AppendAllText($@"{Environment.CurrentDirectory}\output.txt", gridString);

            DrawGrid(folded);

            var dotCount = 0;
            foreach (var dot in folded)
                if (dot)
                    dotCount++;

            Console.ReadKey();
        }

        private static bool[,] Fold(bool[,] grid, Point coord)
        {
            if (coord.X > 0)
                return FoldAtX(grid, coord.X);
            else
                return FoldAtY(grid, coord.Y);
        }

        private static bool[,] FoldAtX(bool[,] grid, int x)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            int newWidth = x;

            var folded = new bool[newWidth, height];

            for (int y = 0; y < height; y++)
            {
                for (int leftX = 0, rightX = width - 1; leftX < newWidth; leftX++, rightX--)
                {
                    folded[leftX, y] = grid[leftX, y] | grid[rightX, y];
                }
            }

            return folded;
        }


        private static bool[,] FoldAtY(bool[,] grid, int y)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            int newHeight = y;

            if (newHeight % 2 != 0)
                newHeight--;

            var folded = new bool[width, newHeight];

            for (int topY = 0, botY = height - 1; topY < newHeight; topY++, botY--)
            {
                for (int x = 0; x < width; x++)
                {
                    folded[x, topY] = grid[x, topY] | grid[x, botY];
                }
            }

            return folded;
        }

        private static void DrawGrid(bool[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int c = 0; c < cols; c++)
            {
                Debug.WriteLine("");

                for (int r = 0; r < rows; r++)
                {
                    if (grid[r, c])
                        Debug.Write("#");
                    else
                        Debug.Write(".");
                }
            }
            Debug.WriteLine("");
        }

        private static string DrawGridToString(bool[,] grid)
        {
            var sb = new StringBuilder();
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int c = 0; c < cols; c++)
            {
                sb.AppendLine();

                for (int r = 0; r < rows; r++)
                {
                    if (grid[r, c])
                        sb.Append("#");
                    else
                        sb.Append(".");
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
