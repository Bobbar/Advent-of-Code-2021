using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

namespace Dumbo_Octopus
{
    internal class Program
    {
        private static int Columns = 0;
        private static int Rows = 0;
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            Columns = inputText.First().Length;
            Rows = inputText.Count();

            var grid = new int[Rows, Columns];
            var hasFlashed = new bool[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    grid[i, j] = int.Parse(inputText[i][j].ToString());
                }
            }

            Debug.WriteLine($"--- Initial grid ---");
            DrawGrid(Columns, Rows, grid);


            int steps = 100;
            int flashes = 0;
            int step = 0;
            while (true)
            {
                // Increase energy of each octopus. 
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        grid[i, j] += 1;
                    }
                }

                // Check for and record flashes and increase neighbor energies until no more flashes occur.
                while (HasFlashes(grid))
                {
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            if (grid[i, j] > 9 && !hasFlashed[i, j])
                            {
                                flashes++;
                                hasFlashed[i, j] = true;

                                var neighbors = GetNeighbors(grid, i, j);
                                for (int n = 0; n < neighbors.Length; n++)
                                {
                                    var nPnt = neighbors[n];
                                    grid[nPnt.X, nPnt.Y] += 1;
                                }
                            }
                        }
                    }

                    // Reset all flashed octopi.
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            if (hasFlashed[i, j])
                                grid[i, j] = 0;
                        }
                    }
                }

                //Debug.WriteLine($"--- Step: {step + 1} ---");
                //DrawGrid(Columns, Rows, grid);

                step++;

                // Part two break.
                if (AllFlashed(hasFlashed))
                    break;

                //// Part one break.
                //if (step == steps)
                //    break;

                // Reset flashed.
                hasFlashed = new bool[Rows, Columns];
            }

            Debug.WriteLine($"--- After step: {step} ---");
            DrawGrid(Columns, Rows, grid);
            Debug.WriteLine($"Flashes: {flashes}");

            Console.ReadKey();
        }

        private static bool AllFlashed(bool[,] flashed)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!flashed[i, j])
                        return false;
                }
            }

            return true;
        }

        private static bool HasFlashes(int[,] grid)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (grid[i, j] > 9)
                        return true;
                }
            }

            return false;
        }

        private static Point[] GetNeighbors(int[,] grid, int x, int y)
        {
            var neighbors = new List<Point>();
            for (int yOff = -1; yOff <= 1; yOff++)
            {
                for (int xOff = -1; xOff <= 1; xOff++)
                {
                    int nX = x + xOff;
                    int nY = y + yOff;

                    // Don't include ourselves...
                    if (nX == x && nY == y)
                        continue;

                    if (nX >= 0 && nY >= 0 && nX < Columns && nY < Rows)
                        neighbors.Add(new Point(nX, nY));
                }
            }

            return neighbors.ToArray();
        }

        private static void DrawGrid(int cols, int rows, int[,] grid)
        {
            for (int r = 0; r < rows; r++)
            {
                Debug.WriteLine("");

                for (int c = 0; c < cols; c++)
                {
                    Debug.Write($"{grid[r, c]}");
                }
            }

            Debug.WriteLine("");
        }
    }
}
