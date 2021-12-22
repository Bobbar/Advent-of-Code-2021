using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;
using Priority_Queue;
namespace Chiton
{
    internal class Program
    {
        private static int Width;
        private static int Height;
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            Width = inputText.First().Length;
            Height = inputText.Count;

            var grid = new int[Width, Height];

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    grid[x, y] = int.Parse(inputText[y][x].ToString());


            var start = new Point(0, 0);
            var end = new Point(Width - 1, Height - 1);
            var visited = new bool[Width, Height];

            var queue = new SimplePriorityQueue<Point>();
            queue.Enqueue(start, 0);

            float lowstRiskPath = 0;
            while (true)
            {
                var cRisk = queue.GetPriority(queue.First());
                var cPnt = queue.Dequeue();
                

                if (visited[cPnt.X, cPnt.Y])
                    continue;

                if (cPnt == end)
                {
                    lowstRiskPath = cRisk;
                    break;
                }

                visited[cPnt.X, cPnt.Y] = true;
                var ns = GetNeighbors(grid, cPnt);
                foreach (var n in ns)
                {
                    if (visited[n.X, n.Y])
                        continue;
                    queue.Enqueue(n, cRisk + grid[n.X, n.Y]);
                }

            }

            Console.ReadKey();
        }

        private static List<Point>[,] FindNs(int[,] grid)
        {
            var ns = new List<Point>[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var n = GetNeighbors(grid, x, y);
                    ns[x, y] = new List<Point>(n);
                }
            }

            return ns;
        }


        private static IEnumerable<Point> GridPoints()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }



        private static List<Point> FindPath(int[,] grid, List<Point>[,] neighbors, Point start, Point end)
        {
            var path = new List<Point>() { start };

            var next = start;

            //if (next == end)
            //    return path;


            bool done = false;
            while (!done)
            {
                //if (next == end && path.Count > 10)
                //    Debugger.Break();

                if (next == end)
                    break;

                //var ns = GetNeighbors(grid, next);
                var ns = neighbors[next.X, next.Y];
                if (ns.Count > 0)
                {
                    var min = int.MaxValue;
                    var minN = new Point();
                    //var minPath = new List<Point>();
                    foreach (var n in ns)
                    {
                        //if (n == end)
                        //   return path;

                        var testPath = FindPath(grid, neighbors, n, end);
                        var testSum = SumPath(grid, testPath);

                        if (testSum < min)
                        {
                            min = testSum;
                            minN = n;
                            //minPath = testPath;
                        }

                        //var val = grid[n.X, n.Y];
                        //if (val < min)
                        //{
                        //    min = val;
                        //    minN = n;
                        //}

                    }

                    //MinPaths.TryAdd(next, minPath);
                    //MinPaths[next] = minPath;

                    //if (SumPath(grid, MinPaths[next]) > min)
                    //    MinPaths[next] = minPath;

                    //    Debugger.Break();

                    //path.AddRange(minPath);
                    path.Add(minN);
                    next = minN;
                }
                else
                {
                    break;
                }

            }

            return path;
        }




        private static int SumPath(int[,] grid, List<Point> path)
        {
            int sum = 0;
            for (int i = 0; i < path.Count; i++)
                sum += grid[path[i].X, path[i].Y];
            return sum;
        }

        private static int SumPath(int[,] grid, Point[] path)
        {
            int sum = 0;
            for (int i = 0; i < path.Length; i++)
                sum += grid[path[i].X, path[i].Y];
            return sum;
        }


        //// Works. But VERY slow...
        //private static List<Point> FindPath(int[,] grid, Point start, List<Point> prev = null)
        //{
        //    var end = new Point(Width - 1, Height - 1);
        //    //var visited = new bool[Width, Height];
        //    var path = new List<Point>() { start };
        //    var next = start;

        //    //if (prev != null && prev.Count > 10)
        //    //    Debugger.Break();

        //    if (start == end)
        //        return path;

        //    bool done = false;
        //    while (!done)
        //    {
        //        if (next == end)
        //            break;

        //        var ns = GetNeighbors(grid, next.X, next.Y);
        //        //ns = ns.Where(n => !visited[n.X, n.Y]).ToArray();

        //        //if (ns.Contains(end))
        //        //    return path;

        //        if (ns.Length > 0)
        //        {
        //            var minPath = int.MaxValue;
        //            var minN = new Point(0, 0);

        //            foreach (var n in ns)
        //            {
        //                var nPath = FindPath(grid, n, path);
        //                var nPathSum = SumPath(grid, nPath);
        //                if (nPathSum < minPath)
        //                {
        //                    minPath = nPathSum;
        //                    minN = n;
        //                }
        //            }

        //            path.Add(minN);
        //            next = minN;
        //        }
        //        else
        //        {
        //            break;
        //        }


        //        //DrawGrid(grid, path);
        //    }

        //    return path;

        //}

        private static int SumGrid(int[,] grid, Point start, Point end)
        {
            int sum = 0;

            for (int y = start.Y; y <= end.Y; y++)
            {
                for (int x = start.X; x <= end.X; x++)
                {
                    sum += grid[x, y];
                }
            }

            return sum;
        }

        private static Point[] GetNeighbors(int[,] grid, Point pnt)
        {
            return GetNeighbors(grid, pnt.X, pnt.Y);
        }

        //private static Point[] GetNeighbors(int[,] grid, int x, int y)
        //{
        //    var neighbors = new List<Point>();

        //    if (x + 1 < Width)
        //        neighbors.Add(new Point(x + 1, y));

        //    if (y + 1 < Height)
        //        neighbors.Add(new Point(x, y + 1));

        //    return neighbors.ToArray();
        //}


        private static Point[] GetNeighbors(int[,] grid, int x, int y)
        {
            var neighbors = new List<Point>();
            for (int yOff = -1; yOff <= 1; yOff++)
            {
                for (int xOff = -1; xOff <= 1; xOff++)
                {
                    if (xOff == 0 || yOff == 0)
                    {
                        int nX = x + xOff;
                        int nY = y + yOff;

                        // Don't include ourselves...
                        if (nX == x && nY == y)
                            continue;

                        if (nX >= 0 && nY >= 0 && nX < Width && nY < Height)
                            neighbors.Add(new Point(nX, nY));
                    }

                }
            }

            return neighbors.ToArray();
        }

        private static void DrawGrid(int[,] grid, List<Point> path, Point current)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            for (int r = 0; r < rows; r++)
            {
                Debug.WriteLine("");
                //for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    var pnt = new Point(c, r);
                    if (path.Contains(pnt))
                    {
                        if (path.Last() == pnt)
                            Debug.Write("@");
                        else if (pnt == current)
                            Debug.Write("^");
                        else
                            Debug.Write("#");
                    }
                    else
                        Debug.Write(grid[c, r]);
                }
            }
            Debug.WriteLine("");
        }


        private static void DrawGrid(int[,] grid, List<Point> path)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            for (int r = 0; r < rows; r++)
            {
                Debug.WriteLine("");
                //for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    var pnt = new Point(c, r);
                    if (path.Contains(pnt))
                    {
                        if (path.Last() == pnt)
                            Debug.Write("@");
                        else
                            Debug.Write("#");
                    }
                    else
                        Debug.Write(grid[c, r]);
                }
            }
            Debug.WriteLine("");
        }


        private static Color GetVariableColor(Color startColor, Color midColor, Color endColor, float maxValue, float currentValue, int alpha, bool translucent = false)
        {
            float intensity = 0;
            byte r1, g1, b1, r2, g2, b2;
            float maxHalf = maxValue * 0.5f;

            if (currentValue <= maxHalf)
            {
                r1 = startColor.R;
                g1 = startColor.G;
                b1 = startColor.B;

                r2 = midColor.R;
                g2 = midColor.G;
                b2 = midColor.B;

                maxValue = maxHalf;
            }
            else
            {
                r1 = midColor.R;
                g1 = midColor.G;
                b1 = midColor.B;

                r2 = endColor.R;
                g2 = endColor.G;
                b2 = endColor.B;

                maxValue = maxHalf;
                currentValue = currentValue - maxValue;
            }

            if (currentValue > 0)
            {
                // Compute the intensity of the end color.
                intensity = (currentValue / maxValue);
            }

            intensity = Math.Min(intensity, 1.0f);

            byte newR, newG, newB;
            newR = (byte)(r1 + (r2 - r1) * intensity);
            newG = (byte)(g1 + (g2 - g1) * intensity);
            newB = (byte)(b1 + (b2 - b1) * intensity);

            if (translucent)
            {
                return Color.FromArgb(alpha, newR, newG, newB);
            }
            else
            {
                return Color.FromArgb(newR, newG, newB);
            }
        }
    }
}
