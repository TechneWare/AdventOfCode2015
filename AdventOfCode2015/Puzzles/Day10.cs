using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 10: Elves Look, Elves Say ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/10"/>
    internal class Day10 : Puzzle
    {
        private string startNumber { get; set; } = "";

        public Day10()
            : base(Name: "Elves Look, Elves Say", DayNumber: 10) { }

        public override void ParseData()
        {
            startNumber = DataRaw.Replace("\r", "").Replace("\n", "");
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            int iterations = isTestMode ? 5 : 40;
            var results = PlayLookAndSay(startNumber, iterations);

            if (isTestMode)
            {
                for (int i = 0; i < iterations; i++)
                {
                    answer += $"{results[i]} -> {results[i + 1]}\n";
                }
            }
            else
            {
                answer += $"Length = {results.Last().Length}\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            int iterations = isTestMode ? 5 : 50;
            var results = PlayLookAndSay(startNumber, iterations);

            if (isTestMode)
            {
                for (int i = 0; i < iterations; i++)
                {
                    answer += $"{results[i]} -> {results[i + 1]}\n";
                }
            }
            else
            {
                answer += $"Length = {results.Last().Length}\n";
            }

            Part2Result = answer;
        }

        private List<string> PlayLookAndSay(string startNumber, int iterations)
        {
            List<string> sequences = [startNumber];

            for (int i = 1; i < iterations + 1; i++)
            {
                sequences.Add(LookAndSay(sequences[i - 1]));
            }

            return sequences;
        }

        static string LookAndSay(string startNumber)
        {
            StringBuilder result = new();

            char repeat = startNumber[0];
            startNumber = string.Concat(startNumber.AsSpan(1, startNumber.Length - 1), " ");
            int times = 1;

            foreach (char actual in startNumber)
            {
                if (actual != repeat)
                {
                    result.Append(Convert.ToString(times) + repeat);
                    times = 1;
                    repeat = actual;
                }
                else
                {
                    times += 1;
                }
            }
            return result.ToString();
        }
    }
}
