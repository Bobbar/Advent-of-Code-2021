using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Giant_Squid
{
    internal class Program
    {

        private const int BOARD_SZ = 25;
        private const int ROWS_COLS = 5;

        static void Main(string[] args)
        {
            var inputPath = $@"{Environment.CurrentDirectory}\input.txt";
            var inputText = File.ReadAllLines(inputPath).ToList();
            inputText.RemoveAll(l => string.IsNullOrEmpty(l.Trim()));

            // Parse the randoms numbers to be drawn.
            var randomNumbersText = inputText.First().Split(',').ToList();
            var randomNumbers = new List<int>();
            foreach (var num in randomNumbersText)
                randomNumbers.Add(int.Parse(num));

            // Remove the random numbers line.
            inputText.RemoveAt(0);

            // Parse the boards into 1D arrays.
            var boards = new List<int>();
            for (int i = 0; i < inputText.Count; i++)
            {
                var row = inputText[i].Split(' ').ToList();
                row.RemoveAll(c => string.IsNullOrEmpty(c.Trim()));
                var rowNums = new List<int>();
                row.ForEach(v => rowNums.Add(int.Parse(v)));
                boards.AddRange(rowNums);
            }

            var boardArr = boards.ToArray();
            var marks = new bool[boards.Count];
            int numBoards = boardArr.Length / BOARD_SZ;
            int lastDrawnNum = 0;
            int numWins = 0;
            int[] winningBoards;
            var prevBoardWins = new List<int>();

            // Draw the random numbers and check for wins.
            foreach (var num in randomNumbers)
            {
                //Debug.WriteLine($"Draw: {num}");

                winningBoards = AddMarkAndCheckForWins(boardArr, marks, num);
                if (winningBoards.Length > 0)
                {
                    lastDrawnNum = num;

                    foreach (var board in winningBoards)
                    {
                        //Debug.WriteLine($"-----  Board # {board + 1} wins!!! -----");
                        if (!prevBoardWins.Contains(board))
                        {
                            prevBoardWins.Add(board);
                            numWins++;
                        }
                    }
                }

                //DrawBoards(boardArr, marks, num);

                if (numWins == numBoards)
                    break;
            }

            // Sum the last winning board.
            int sum = 0;
            int startOffset = prevBoardWins.Last() * BOARD_SZ;
            for (int i = startOffset; i < startOffset + BOARD_SZ; i++)
            {
                if (!marks[i])
                    sum += boardArr[i];
            }

            int finalScore = sum * lastDrawnNum;
            Debug.WriteLine($"Final Score: {finalScore}");

            Console.ReadKey();
        }

        private static int[] AddMarkAndCheckForWins(int[] boards, bool[] marks, int num)
        {
            var winningBoards = new List<int>();
            for (int i = 0; i < boards.Length; i++)
            {
                var cell = boards[i];
                if (cell == num)
                    marks[i] = true;
            }

            for (int b = 0; b < marks.Length / BOARD_SZ; b++)
            {
                int boardOffset = b * BOARD_SZ;

                // Check for row wins.
                for (int row = 0; row < ROWS_COLS; row++)
                {
                    bool columnIsComplete = true;
                    for (int col = 0; col < ROWS_COLS; col++)
                    {
                        var cell = boards[row * ROWS_COLS + col + boardOffset];
                        var mark = marks[row * ROWS_COLS + col + boardOffset];

                        if (!mark)
                            columnIsComplete = false;
                    }

                    if (columnIsComplete)
                        winningBoards.Add(b);
                }

                // Check for column wins.
                for (int col = 0; col < ROWS_COLS; col++)
                {
                    bool rowIsComplete = true;
                    for (int row = 0; row < ROWS_COLS; row++)
                    {
                        var cell = boards[row * ROWS_COLS + col + boardOffset];
                        var mark = marks[row * ROWS_COLS + col + boardOffset];

                        if (!mark)
                            rowIsComplete = false;
                    }

                    if (rowIsComplete)
                        winningBoards.Add(b);
                }

            }

            return winningBoards.ToArray();
        }

        private static void DrawBoards(int[] boards, bool[] marks, int lastNum)
        {
            for (int b = 0; b < marks.Length / BOARD_SZ; b++)
            {
                Debug.WriteLine($"[Board #: {b + 1}]");

                int boardOffset = b * BOARD_SZ;
                for (int col = 0; col < ROWS_COLS; col++)
                {
                    for (int row = 0; row < ROWS_COLS; row++)
                    {
                        var cell = boards[col * ROWS_COLS + row + boardOffset];
                        var mark = marks[col * ROWS_COLS + row + boardOffset];

                        if (mark)
                        {
                            if (cell == lastNum)
                                Debug.Write($"({cell.ToString("00")})");
                            else
                                Debug.Write($"[{cell.ToString("00")}]");
                        }
                        else
                            Debug.Write($"{cell.ToString(" 00 ")}");

                    }
                    Debug.WriteLine("");
                }
                Debug.WriteLine("");
            }
        }
    }
}
