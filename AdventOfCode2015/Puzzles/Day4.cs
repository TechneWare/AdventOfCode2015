using System.Text;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 4: The Ideal Stocking Stuffer ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/4"/>
    internal class Day4 : Puzzle
    {
        private List<string> inputs = [];
        public Day4()
            : base(Name: "The Ideal Stocking Stuffer", DayNumber: 4) { }

        public override void ParseData()
        {
            inputs = [.. DataRaw.Split("\r\n")];
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();
            var answer = "";
            foreach (var input in inputs)
            {
                var num = GetLowestNumber(input, 5);
                answer += $"{$"{input} ="} {num}\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();
            var answer = "";
            foreach (var input in inputs)
            {
                var num = GetLowestNumber(input, 6);
                answer += $"{$"{input} ="} {num}\n";
            }

            Part2Result = answer;
        }

        private static int GetLowestNumber(string input, int NumZero)
        {
            var hash = "";
            var num = 0;
            var zeros = "".PadLeft(NumZero, '0');
            while (!hash.StartsWith(zeros))
            {
                num++;
                hash = CreateMD5($"{input}{num}");
            }

            return num;
        }
        private static string CreateMD5(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

            return Convert.ToHexString(hashBytes); 
        }
    }
}
