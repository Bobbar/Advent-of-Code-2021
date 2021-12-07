using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Dive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();

            int pos = 0, depth = 0, aim = 0;

            foreach (var line in inputText)
            {
                if (line.Contains("forward"))
                {
                    int forward = int.Parse(line[8].ToString());
                    pos += forward;
                    depth += aim * forward;

                    Debug.WriteLine($"Forward: {forward}");
                }
                else if (line.Contains("down"))
                {
                    int down = int.Parse(line[5].ToString());
                    aim += down;

                    Debug.WriteLine($"Down: {down}");

                }
                else if (line.Contains("up"))
                {
                    int up = int.Parse(line[3].ToString());
                    aim -= up;

                    Debug.WriteLine($"Up: {up}");
                }
            }

            int finalPos = pos * depth;
            Debug.WriteLine($"Final Pos: {finalPos}");

            Console.ReadKey();
        }
    }
}
