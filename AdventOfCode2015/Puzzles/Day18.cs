using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 18: Like a GIF For Your Yard ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/18"/>
    internal class Day18 : Puzzle
    {
        private bool[,] grid = new bool[1, 1];
        private int width = 0;
        private int height = 0;
        private bool useStuckLights = false;

        public Day18()
            : base(Name: "Like a GIF For Your Yard", DayNumber: 18) { }

        public override void ParseData()
        {
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            width = data[0].Length;
            height = data.Length;

            grid = new bool[width, height];
            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    bool value = data[row][col] == '#' ? true : false;
                    grid[row, col] = value;
                }
            }

            if (useStuckLights)
            {
                grid[0, 0] = true;
                grid[0, width - 1] = true;
                grid[height - 1, 0] = true;
                grid[height - 1, width - 1] = true;
            }
        }

        public override void Part1(bool isTestMode)
        {
            useStuckLights = false;
            ParseData();

            var answer = "";
            int updateSteps = isTestMode ? 4 : 100;

            for (int i = 0; i < updateSteps; i++)
                UpdateGrid();

            if (isTestMode)
                answer = GetGridDisplay() + "\n";

            answer += $"Total On: {GetTotalOn()}\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            useStuckLights = true;
            ParseData();

            var answer = "";
            int updateSteps = isTestMode ? 5 : 100;

            for (int i = 0; i < updateSteps; i++)
                UpdateGrid();

            if (isTestMode)
                answer = GetGridDisplay() + "\n";

            answer += $"Total On: {GetTotalOn()}\n";

            Part2Result = answer;
        }
        private void UpdateGrid()
        {
            //clone the grid and update the clone
            bool[,] g = (bool[,])grid.Clone();

            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    //skip the stuck corner lights
                    if (useStuckLights && (col == 0 || col == width - 1) && (row == 0 || row == height - 1))
                        continue;

                    var adj = GetAdjacent(row, col);
                    var onNeighbors = adj.Count(a => a == true);

                    if (grid[row, col] == true)
                        //Rule: A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
                        g[row, col] = onNeighbors == 2 || onNeighbors == 3;
                    else
                        //Rule: A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.
                        g[row, col] = onNeighbors == 3;
                }
            }

            //transfer the clone back to the grid
            grid = g;
        }
        private List<bool> GetAdjacent(int row, int col)
        {
            //create a list of possible neighbor indexes
            List<(int r, int c)> adjIndexes = [
                    new(row-1,col),
                    new(row-1, col+1),
                    new(row, col+1),
                    new(row+1,col+1),
                    new(row+1, col),
                    new(row+1, col-1),
                    new(row,col-1),
                    new(row-1,col-1)
                ];

            //trim out any invalid indexes
            adjIndexes = adjIndexes.Where(i => i.r >= 0 && i.c >= 0 && i.r < height & i.c < width).ToList();

            //retrieve the grid values at those indexes
            List<bool> result = [];
            foreach (var (r, c) in adjIndexes)
                result.Add(grid[r, c]);

            return result;
        }
        private string GetGridDisplay()
        {
            string result = "";

            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    result += $"{(grid[row, col] ? "#" : ".")}";
                }
                result += "\n";
            }

            return result;
        }
        private int GetTotalOn()
        {
            int result = 0;

            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    if (grid[row, col]) result++;
                }
            }

            return result;
        }
    }
}
