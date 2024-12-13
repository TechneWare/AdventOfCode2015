using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 25: Let It Snow ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/25"/>
    internal class Day25 : Puzzle
    {
        private (int Row, int Col) CodeLocation;

        public Day25()
            : base(Name: "Let It Snow", DayNumber: 25) { }

        public override void ParseData()
        {
            if (!string.IsNullOrEmpty(DataRaw))
            {
                var data = DataRaw.Replace("To continue, please consult the code grid in the manual.  Enter the code at row ", "")
                                  .Replace(" column ", "")
                                  .Replace(".", "")
                                  .Split(',', StringSplitOptions.RemoveEmptyEntries);
                CodeLocation = (int.Parse(data[0]), int.Parse(data[1]));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            if (isTestMode)
                CodeLocation = (2, 1);
            
            var answer = $"Code Location: Row {CodeLocation.Row}, Col {CodeLocation.Col}\n";
            long curCode = GetCodeAtLocation(CodeLocation.Row, CodeLocation.Col);
            answer += $"Code = {curCode}\n";

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {

        }
        private static long GetCodeAtLocation(int row, int col)
        {
            long firstCode = 20151125;
            var codeCount = GetCodeCount(row, col);

            var curCode = firstCode;
            for (int i = 0; i < codeCount - 1; i++)
                curCode = GetNextCode(curCode);
            return curCode;
        }
        private static long GetCodeCount(int row, int col)
        {
            return (from r in Enumerable.Range(0, row + col - 1)
                    where 1 == 1
                    select r)
                    .Sum() + col;
        }
        private static long GetNextCode(long curCode)
        {
            return (curCode * 252533) % 33554393;
        }
    }
}
