using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Lanternfish
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to produce fish exponentially...");
            Console.ReadKey();

            // Read the input from a text file instead of hard coding that huge input.
            var inputPath = $@"{Environment.CurrentDirectory}\input_256.csv";
            //var inputPath = $@"{Environment.CurrentDirectory}\input_5.csv";

            var inputText = File.ReadAllText(inputPath);

            // Parse dah inpoot.
            var initialFish = new List<int>();
            inputText.Split(',').ToList().ForEach(f => { initialFish.Add(int.Parse(f)); });

            // Buckets for the fish states.
            var fishAgeBuckets = new long[9];

            // Fill the buckets...
            foreach (var f in initialFish)
            {
                fishAgeBuckets[f]++;
            }

            // How many days to iterate?
            const int numDays = 256;
            //const int numDays = 18;


            int currentDay = 1;
            PrintFishBuckets(0, fishAgeBuckets);

            // Iterate the days.
            while (currentDay <= numDays)
            {
                // Number of expired fish this day.
                long expiredFish = 0;

                // Record and reset any expired fish when the zero bucket has states.
                if (fishAgeBuckets[0] > 0)
                {
                    expiredFish = fishAgeBuckets[0];
                    fishAgeBuckets[0] = 0;
                }

                // Shift all the states/buckets to the left by one.
                // This effectively subtracts one day from all the fish ages.
                for (int i = 0; i < fishAgeBuckets.Length - 1; i++)
                {
                    Swap(fishAgeBuckets, i + 1, i);
                }

                // Add the new and reset old fish ages as needed.
                if (expiredFish > 0)
                {
                    fishAgeBuckets[6] += expiredFish;
                    fishAgeBuckets[8] += expiredFish;
                }

                PrintFishBuckets(currentDay, fishAgeBuckets);
                
                // Move to the next day.
                currentDay++;
            }

            Console.WriteLine($"Number of fish after {numDays} days: {fishAgeBuckets.Sum()}");
            Console.ReadKey();
        }

        private static void PrintFishBuckets(int day, long[] buckets)
        {
            int maxLen = buckets.Max().ToString().Length;
            var mask = new string('0', maxLen);

            if (day == 0)
            {
                Console.WriteLine("States: [0] [1] [2] [3] [4] [5] [6] [7] [8]");
                Console.Write("Init:");
            }
            else
                Console.Write($"{day.ToString("0000")}:");

            foreach (var bucket in buckets)
                Console.Write($"[{bucket.ToString(mask)}]");

            Console.Write($" Tot: {buckets.Sum()}");

            Console.WriteLine();
        }

        private static void Swap(long[] arr, long a, long b)
        {
            var tmp = arr[a];
            arr[a] = arr[b];
            arr[b] = tmp;
        }
    }
}
