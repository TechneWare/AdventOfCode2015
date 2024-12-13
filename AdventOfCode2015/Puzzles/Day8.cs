using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 8: Matchsticks ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/8"/>
    internal class Day8 : Puzzle
    {
        private List<StringInfo> StringInfos { get; set; } = [];
        public Day8()
            : base(Name: "Matchsticks", DayNumber: 8) { }

        public override void ParseData()
        {
            StringInfos.Clear();
            StringInfos = DataRaw.Split("\r\n")
                .Select(s => new StringInfo(s))
                .ToList();
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            string answer = "";
            var litLengths = StringInfos.Select(s => s.LitLength).ToList();
            var valLengths = StringInfos.Select(s => s.ValLength).ToList();
            var totalVal = valLengths.Sum();
            var totalLit = litLengths.Sum();

            if (isTestMode)
            {
                answer += $"Literal : {String.Join(" + ", litLengths)} = {totalLit}\n";
                answer += $"-Values : {String.Join(" + ", valLengths)} = {totalVal}\n";
            }

            answer += $"Diff    : {totalLit} - {totalVal} = {totalLit - totalVal}\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            string answer = "";
            var litLengths = StringInfos.Select(s => s.LitLength).ToList();
            var codeLengths = StringInfos.Select(s => s.EncodeLength).ToList();
            var totalCoded = codeLengths.Sum();
            var totalLit = litLengths.Sum();

            if (isTestMode)
            {
                answer += $"Encoded : {String.Join(" + ", codeLengths)} = {totalCoded}\n";
                answer += $"-Literal: {String.Join(" + ", litLengths)} = {totalLit}\n";
            }

            answer += $"Diff    : {totalCoded} - {totalLit} = {totalCoded - totalLit}\n";

            Part2Result = answer;
        }

        internal class StringInfo
        {
            public string Literal { get; internal set; }
            public int LitLength => Literal.Length;
            public string Value { get; internal set; }
            public int ValLength => Value.Length;
            public int EncodeLength => GetLiteralLength(Literal) + 2;

            public StringInfo(string literal)
            {
                Literal = literal;
                ParseLiteral();
            }

            private void ParseLiteral()
            {
                var val = Encoding.ASCII.GetBytes(Literal);
                var val1 = Encoding.UTF8.GetString(val, 1, val.Length - 2);
                Value = Regex.Unescape(val1);
            }

            private int GetLiteralLength(string input)
            {
                // Convert each character to its literal representation and sum their lengths
                int length = 0;
                foreach (char c in input)
                {
                    switch (c)
                    {
                        case '\n': length += 2; break; // \n
                        case '\r': length += 2; break; // \r
                        case '\t': length += 2; break; // \t
                        case '\\': length += 2; break; // \\
                        case '\"': length += 2; break; // \"
                        case '\'': length += 2; break; // \'
                        default:
                            length += 1; // Regular characters count as 1
                            break;
                    }
                }

                return length;
            }
        }
    }
}
