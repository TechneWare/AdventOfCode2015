using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    internal interface IPuzzle
    {
        string Name { get; }
        double DayNumber { get; }
        public void Run();
        public void ShowPuzzleText(int partNum);
        public void Part1(bool isTestMode);
        public void Part2(bool isTestMode);

    }
}
