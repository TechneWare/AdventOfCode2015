using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 2: I Was Told There Would Be No Math ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/2"/>
    internal class Day2 : Puzzle
    {
        private readonly List<Box> boxes = [];

        public Day2()
            : base(Name: "I Was Told There Would Be No Math", DayNumber: 2) { }

        public override void ParseData()
        {
            boxes.Clear();

            foreach (var line in DataRaw.Replace("\r", "").Split("\n"))
            {
                var dims = line.Split('x', StringSplitOptions.RemoveEmptyEntries);
                boxes.Add(new Box(int.Parse(dims[0]), int.Parse(dims[1]), int.Parse(dims[2])));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var b in boxes)
            {
                answer += $"{(isTestMode ? $"{b.Length}x{b.Width}x{b.Height} =" : "")}{(isTestMode ? $"{b.TotalArea} SqrFeet\n" : "")}";
            }

            answer += $"Total Area = {boxes.Sum(b => b.TotalArea)} SqrFeet";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var b in boxes)
            {
                answer += $"{(isTestMode ? $"{b.Length}x{b.Width}x{b.Height} =" : "")}{(isTestMode ? $"{b.TotalRibbon} Feet\n" : "")}";
            }

            answer += $"Total Ribbon = {boxes.Sum(b => b.TotalRibbon)} Feet";

            Part2Result = answer;
        }

        internal class Box
        {
            public int Length { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int TotalArea => sideArea[0] * 2 + sideArea[1] * 2 + sideArea[2] * 2 + sideArea.Min();

            public int TotalRibbon => GetWrapLength() + GetBowLength();

            private readonly List<int> sideArea = [];

            public Box(int length, int width, int height)
            {
                Length = length;
                Width = width;
                Height = height;

                sideArea.Add(Length * Width);
                sideArea.Add(Width * Height);
                sideArea.Add(Height * Length);
            }
            private int GetWrapLength()
            {
                var dims = new List<int>() { Length, Width, Height };
                var maxDim = dims.Max();
                dims.Remove(maxDim);

                var l = dims[0] * 2 + dims[1] * 2;

                return l;
            }
            private int GetBowLength()
            {
                return Length * Width * Height;
            }
        }
    }
}
