using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 5: Doesn't He Have Intern-Elves For This? ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/5"/>
    internal class Day5 : Puzzle
    {
        private List<string> strings = [];
        public Day5()
            : base(Name: "Doesn't He Have Intern-Elves For This?", DayNumber: 5) { }

        public override void ParseData()
        {
            strings = [.. DataRaw.Replace("\r", "").Split("\n")];
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();
            var answer = "";
            var niceCount = 0;

            foreach (var s in strings)
            {
                bool isNice = IsNiceV1(s);
                if (isNice) niceCount++;

                if (isTestMode)
                    answer += $"{s} = {(isNice ? "Nice" : "Naughty")}\n";
            }

            answer += $"Total Nice = {niceCount}\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();
            var answer = "";
            var niceCount = 0;

            foreach (var s in strings)
            {
                bool isNice = IsNiceV2(s);
                if (isNice) niceCount++;

                if (isTestMode)
                    answer += $"{s} = {(isNice ? "Nice" : "Naughty")}\n";
            }

            answer += $"Total Nice = {niceCount}\n";

            Part2Result = answer;
        }

        private static bool IsNiceV1(string input)
        {
            bool hasProhibited = HasProhibitedChar(input);
            bool hasThreeVowels = HasThreeVowels(input);
            bool hasDoubleChar = HasDoubleChar(input);

            return !hasProhibited && hasThreeVowels && hasDoubleChar;
        }
        private static bool HasDoubleChar(string input)
        {
            var result = false;

            foreach (var c in input)
            {
                if (input.Contains($"{c}{c}"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        private static bool HasProhibitedChar(string input)
        {
            List<string> prohibited = ["ab", "cd", "pq", "xy"];
            return prohibited.Any(p => input.Contains(p));
        }
        private static bool HasThreeVowels(string input)
        {
            var vowels = "aeiou";
            var found = vowels.Intersect(input).ToList();
            var count = input.Count(i => found.Contains(i));

            return count >= 3;
        }
        private static bool IsNiceV2(string input)
        {
            bool hasDoublePair = HasDoublePair(input);
            bool hasSplitRepeat = HasSplitRepeat(input);

            return hasDoublePair && hasSplitRepeat;
        }
        private static bool HasDoublePair(string input)
        {
            var result = false;
            List<string> pairs = [];

            for (int i = 0; i < input.Length - 1; i++)
                pairs.Add($"{input[i]}{input[i + 1]}");

            foreach (var p in pairs.Where(p => pairs.Count(p1 => p1 == p) > 1).Distinct())
            {
                var i = input;

                var idx1 = i.IndexOf(p);
                i = i.Substring(idx1 + 2);

                var idx2 = i.IndexOf(p);

                result = idx1 >= 0 && idx2 >= 0;
                if (result)
                    break;
            }

            return result;
        }
        private static bool HasSplitRepeat(string input)
        {
            var result = false;

            for (int i = 0; i < input.Length - 2; i++)
            {
                if (input[i] == input[i + 2])
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
