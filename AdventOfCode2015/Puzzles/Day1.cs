
namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 1: Not Quite Lisp ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/1"/>
    internal class Day1 : Puzzle
    {
        private int currentFloor = 0;
        private List<string> directions = [];

        public Day1()
            : base(Name: "Not Quite Lisp", DayNumber: 1) { }

        public override void ParseData()
        {
            currentFloor = 0;
            directions.Clear();

            directions = [.. DataRaw.Split("\r\n")];
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var direction in directions)
            {
                var endFloor = FollowDirections(direction);
                answer += $"{(isTestMode ? $"{direction} =" : "floor =")} {endFloor}\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var direction in directions)
            {
                var endFloor = FindBasement(direction);
                answer += $"{(isTestMode ? $"{direction} =" : "floor =")} {endFloor}\n";
            }

            Part2Result = answer;
        }
        private static int FollowDirections(string direction)
        {
            int floor = 0;

            foreach (var c in direction)
            {
                if (c == '(') floor++;
                if (c == ')') floor--;
            }

            return floor;
        }
        private static int FindBasement(string direction)
        {
            int floor = 0;
            int charNum = 0;

            foreach (var c in direction)
            {
                charNum++;
                if (c == '(') floor++;
                if (c == ')') floor--;

                if (floor == -1) break;
            }

            return charNum;
        }
    }
}
