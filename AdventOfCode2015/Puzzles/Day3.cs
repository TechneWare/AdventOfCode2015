using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 3: Perfectly Spherical Houses in a Vacuum ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/3"/>
    internal class Day3 : Puzzle
    {
        private readonly List<House> houses = [];
        private List<string> paths = [];

        public Day3()
            : base(Name: "Perfectly Spherical Houses in a Vacuum", DayNumber: 3) { }

        public override void ParseData()
        {
            paths = DataRaw.Replace("\r", "").Split("\r", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            var totalHouses = 0;

            foreach (var moves in paths)
            {
                houses.Clear();
                FollowMoves(moves);
                totalHouses = houses.Count;

                answer += $"{(isTestMode ? $"{moves} =" : "")} {(isTestMode ? $"{totalHouses}\n" : "")}";
            }

            if (!isTestMode)
                answer += $"Total Houses = {totalHouses}";

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            var totalHouses = 0;

            foreach (var moves in paths)
            {
                houses.Clear();
                var santa1Moves = "";
                var santa2Moves = "";
                for (int i = 0; i < moves.Length; i += 2)
                {
                    santa1Moves += moves[i];
                    santa2Moves += moves[i + 1];
                }

                FollowMoves(santa1Moves);
                FollowMoves(santa2Moves);
                totalHouses = houses.Count;

                answer += $"{(isTestMode ? $"{moves} =" : "")} {(isTestMode ? $"{totalHouses}\n" : "")}";
            }

            if (!isTestMode)
                answer += $"Total Houses = {totalHouses}";

            Part2Result = answer;
        }
        private void FollowMoves(string moves)
        {
            int x = 0;
            int y = 0;

            UpdateHouse(x, y);

            foreach (var m in moves)
            {
                switch (m)
                {
                    case '>':
                        x++;
                        break;
                    case '<':
                        x--;
                        break;
                    case '^':
                        y--;
                        break;
                    case 'v':
                        y++;
                        break;
                    default:
                        break;
                }

                UpdateHouse(x, y);
            }
        }

        private void UpdateHouse(int X, int Y)
        {
            var h = houses.SingleOrDefault(h => h.X == X && h.Y == Y);

            if (h == null)
            {
                h = new House { X = X, Y = Y };
                houses.Add(h);
            }

            h.Visit();
        }
    }

    internal class House
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        public int NumVisits => visits;
        private int visits = 0;

        public void Visit()
        {
            visits++;
        }
    }
}