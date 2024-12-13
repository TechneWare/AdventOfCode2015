using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 6: Probably a Fire Hazard ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/6"/>
    internal class Day6 : Puzzle
    {
        private List<Instruction> instructions = [];
        private bool[][] binaryLights = new bool[1000][];
        private int[][] analogLights = new int[1000][];

        private int litCount => binaryLights.SelectMany(l => l).Count(l => l == true);
        private int brightness => analogLights.SelectMany(l => l).Sum();
        public Day6()
            : base(Name: "Probably a Fire Hazard", DayNumber: 6) { }

        public override void ParseData()
        {
            instructions.Clear();
            for (int x = 0; x < 1000; x++)
            {
                binaryLights[x] = new bool[1000];
                analogLights[x] = new int[1000];
            }

            foreach (var ins in DataRaw.Replace("\r", "").Split("\n"))
            {
                var i = ins
                     .Replace("turn on", "Turn_On")
                     .Replace("turn off", "Turn_Off")
                     .Replace("toggle", "Toggle")
                     .Replace(" through ", " ");

                var parts = i.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var instruction = new Instruction(
                    action: parts[0].ToEnum(Instruction.Actions.None),
                    start: new Coordinate(parts[1]),
                    end: new Coordinate(parts[2]));

                instructions.Add(instruction);
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var i in instructions)
            {
                ExecuteBinary(i);
                if (isTestMode)
                    answer += $"{i.Action} {i.Start.X},{i.Start.Y}->{i.End.X},{i.End.Y} = " +
                              $"{litCount}\n";
            }

            var total = litCount;

            Part1Result = $"{answer}Total Lit = {total}\n";
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var i in instructions)
            {
                ExecuteAnalog(i);
                if (isTestMode)
                    answer += $"{i.Action} {i.Start.X},{i.Start.Y}->{i.End.X},{i.End.Y} = " +
                              $"{brightness}\n";
            }

            Part2Result = $"{answer}Brightness = {brightness}\n";
        }


        public void ExecuteBinary(Instruction instruction)
        {
            for (int x = instruction.Start.X; x <= instruction.End.X; x++)
            {
                for (int y = instruction.Start.Y; y <= instruction.End.Y; y++)
                {
                    switch (instruction.Action)
                    {
                        case Instruction.Actions.Turn_On:
                            binaryLights[x][y] = true;
                            break;
                        case Instruction.Actions.Turn_Off:
                            binaryLights[x][y] = false;
                            break;
                        case Instruction.Actions.Toggle:
                            binaryLights[x][y] = binaryLights[x][y] ^ true;
                            break;
                        case Instruction.Actions.None:
                            throw new InvalidOperationException("Action cannot be None");
                        default:
                            break;
                    }
                }
            }
        }
        public void ExecuteAnalog(Instruction instruction)
        {
            for (int x = instruction.Start.X; x <= instruction.End.X; x++)
            {
                for (int y = instruction.Start.Y; y <= instruction.End.Y; y++)
                {
                    switch (instruction.Action)
                    {
                        case Instruction.Actions.Turn_On:
                            analogLights[x][y]++;
                            break;
                        case Instruction.Actions.Turn_Off:
                            analogLights[x][y]--;
                            if (analogLights[x][y] < 0) 
                                analogLights[x][y] = 0;
                            break;
                        case Instruction.Actions.Toggle:
                            analogLights[x][y] += 2;
                            break;
                        case Instruction.Actions.None:
                            throw new InvalidOperationException("Action cannot be None");
                        default:
                            break;
                    }
                }
            }
        }
        internal class Instruction
        {
            public enum Actions
            {
                None,
                Turn_On,
                Turn_Off,
                Toggle
            }

            public Actions Action;
            public Coordinate Start { get; set; }
            public Coordinate End { get; set; }

            public Instruction(Actions action, Coordinate start, Coordinate end)
            {
                Action = action;
                Start = start;
                End = end;
            }
        }
        internal class Coordinate
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Coordinate(string coord)
            {
                var parts = coord.Split(",");
                X = int.Parse(parts[0]);
                Y = int.Parse(parts[1]);
            }
        }
    }
}
