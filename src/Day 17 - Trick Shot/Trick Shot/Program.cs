using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace Trick_Shot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();
            var line = inputText.First();
            line = line.Replace("target area: ", "");
            var coords = line.Split(',');
            var xCoords = coords[0].Replace("x=", "").Split("..");
            var yCoords = coords[1].Replace("y=", "").Split("..");

            var topLeft = new Point(int.Parse(xCoords[0]), int.Parse(yCoords[1]));
            var botRight = new Point(int.Parse(xCoords[1]), int.Parse(yCoords[0]));

            int minVelo = botRight.Y;
            int maxVelo = botRight.X;

            var timer = new Stopwatch();

            // Might as well play with some GPGPU stuff while we are brute forcing things... :-}
            using (var gpgpuTest = new GPGPU.TrickShotGPU())
            {
                timer.Restart();

                var gpuMaxY = gpgpuTest.FindMaxY(minVelo, maxVelo, topLeft, botRight);
                var gpuHits = gpgpuTest.FindHits(minVelo, maxVelo, topLeft, botRight);

                timer.Stop();
                Console.WriteLine($"[GPU Results]  Max Y: {gpuMaxY}  Hits: {gpuHits.Count()}   Elap: {timer.Elapsed.TotalMilliseconds} ms  {timer.Elapsed.Ticks} ticks");
            }

            timer.Restart();

            var cpuMaxY = FindMaxY(minVelo, maxVelo, topLeft, botRight);
            var cpuHits = FindHits(minVelo, maxVelo, topLeft, botRight);

            timer.Stop();
            Console.WriteLine($"[CPU Results]  Max Y: {cpuMaxY}  Hits: {cpuHits.Count()}   Elap: {timer.Elapsed.TotalMilliseconds} ms  {timer.Elapsed.Ticks} ticks");

            //var grid = MakeGrid(new Point(), topLeft, botRight, maxTrail);
            //DrawGrid(grid);
            Console.ReadKey();
        }

        private static int FindMaxY(int minVelo, int maxVelo, Point targTopLeft, Point targBotRight)
        {
            int maxYPos = 0;
            var maxYVelo = new Point();
            var maxTrail = new List<Point>();
            for (int x = minVelo; x <= maxVelo; x++)
            {
                for (int y = minVelo; y <= maxVelo; y++)
                {
                    var initPos = new Point();
                    var pos = new Point(initPos.X, initPos.Y);
                    var velo = new Point(x, y);
                    int steps = 0;
                    var trail = new List<Point>() { pos };
                    bool wasHit = false;
                    bool done = false;
                    while (!done)
                    {
                        pos.X += velo.X;
                        pos.Y += velo.Y;

                        if (velo.X > 0)
                            velo.X--;
                        else if (velo.X < 0)
                            velo.X++;

                        velo.Y--;

                        trail.Add(pos);

                        if (pos.X >= targTopLeft.X && pos.X <= targBotRight.X && pos.Y >= targBotRight.Y && pos.Y <= targTopLeft.Y)
                        {
                            wasHit = true;
                            break;
                        }

                        if (pos.X > targBotRight.X || pos.Y < targBotRight.Y)
                            break;

                        steps++;
                    }

                    if (wasHit)
                    {
                        var maxYTrail = trail.Max(p => p.Y);
                        maxYPos = Math.Max(maxYPos, maxYTrail);

                        if (maxYTrail == maxYPos)
                        {
                            maxTrail = trail;
                            maxYVelo = new Point(x, y);
                        }
                    }
                }
            }

            return maxYPos;
        }

        private static List<Point> FindHits(int minVelo, int maxVelo, Point targTopLeft, Point targBotRight)
        {
            int steps = 0;
            var hits = new List<Point>();
            for (int x = minVelo; x <= maxVelo; x++)
            {
                for (int y = minVelo; y <= maxVelo; y++)
                {
                    var initPos = new Point();
                    var pos = new Point(initPos.X, initPos.Y);
                    var velo = new Point(x, y);
                    var trail = new List<Point>() { pos };
                    bool wasHit = false;
                    bool done = false;
                    while (!done)
                    {
                        pos.X += velo.X;
                        pos.Y += velo.Y;

                        if (velo.X > 0)
                            velo.X--;
                        else if (velo.X < 0)
                            velo.X++;

                        velo.Y--;

                        trail.Add(pos);

                        if (pos.X >= targTopLeft.X && pos.X <= targBotRight.X && pos.Y >= targBotRight.Y && pos.Y <= targTopLeft.Y)
                        {
                            wasHit = true;
                            break;
                        }

                        if (pos.X > targBotRight.X || pos.Y < targBotRight.Y)
                            break;

                    }

                    if (wasHit)
                    {
                        hits.Add(new Point(x, y));
                    }
                    steps++;

                }
            }

            return hits;
        }

        private static int[,] MakeGrid(Point start, Point targetTopLeft, Point targetBotRight, List<Point> path)
        {
            var minX = start.X;
            var maxX = Math.Max(path.Max(p => p.X), targetBotRight.X);
            var minY = Math.Min(path.Min(p => p.Y), targetBotRight.Y);
            var maxY = Math.Max(path.Max(p => p.Y), targetTopLeft.Y);
            var minXAbs = Math.Abs(minX);
            var minYAbs = Math.Abs(minY);
            var width = Math.Abs(minX - maxX);
            var height = Math.Abs(minY - maxY);
            var grid = new int[width + 1, height + 1];

            for (int x = targetTopLeft.X; x <= targetBotRight.X; x++)
            {
                for (int y = targetBotRight.Y; y <= targetTopLeft.Y; y++)
                {
                    grid[x + minXAbs, y + minYAbs] = 2;
                }
            }

            foreach (var pnt in path)
            {
                grid[pnt.X + minXAbs, pnt.Y + minYAbs] = 3;
            }

            grid[path.First().X + minXAbs, path.First().Y + minYAbs] = 1;

            grid = FlipY(grid);

            return grid;
        }

        private static int[,] FlipY(int[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            var newGrid = new int[width, height];


            for (int topY = 0, botY = height - 1; topY < height; topY++, botY--)
            {
                for (int x = 0; x < width; x++)
                {
                    newGrid[x, topY] = grid[x, botY];
                }
            }


            return newGrid;
        }

        private static void DrawGrid(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            for (int c = 0; c < cols; c++)
            {
                Debug.WriteLine("");

                for (int r = 0; r < rows; r++)
                {
                    if (grid[r, c] == 1)
                        Debug.Write("S");
                    else if (grid[r, c] == 2)
                        Debug.Write("T");
                    else if (grid[r, c] == 3)
                        Debug.Write("#");
                    else
                        Debug.Write(".");
                }
            }

            Debug.WriteLine("");
        }

    }
}
