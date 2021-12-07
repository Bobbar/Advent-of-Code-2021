using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Sonar_Sweep
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";

            var inputText = File.ReadAllLines(inputPath).ToList();
            var inputValues = new List<int>();
            inputText.ForEach(line => inputValues.Add(int.Parse(line)));

            int increases = 0, decreases = 0, nochange = 0;

            for(int i = 0; i < inputValues.Count; i++)
            {
                if (i + 4 > inputValues.Count)
                    break;

                int sum = inputValues.GetRange(i, 3).Sum();
                int sumNext = inputValues.GetRange(i + 1, 3).Sum();

                Debug.WriteLine($"[{i}] Cur: {sum}  Nxt: {sumNext}");

                if (sumNext > sum)
                    increases++;
                else if (sumNext < sum)
                    decreases++;
                else
                    nochange++;
            }

            Debug.WriteLine($"Number of increases: {increases}");
            Console.WriteLine($"Number of increases: {increases}");
            Console.ReadKey();
        }
    }
}
