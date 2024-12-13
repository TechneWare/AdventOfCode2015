using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 19: Medicine for Rudolph ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/19"/>
    internal class Day19 : Puzzle
    {
        private RedNosedFusionPlant? FusionPlant { get; set; } = null;
        public Day19()
            : base(Name: "Medicine for Rudolph", DayNumber: 19) { }

        private List<string> CalMolecules { get; set; } = [];
        public override void ParseData()
        {
            CalMolecules.Clear();
            FusionPlant = new RedNosedFusionPlant();

            var data = DataRaw.Replace("\r", "").Split("\n");
            bool importReplacments = true;

            foreach (var line in data)
            {
                if (line == string.Empty)
                {
                    importReplacments = false;
                    continue;
                }

                if (importReplacments)
                {
                    var r = line.Split(" => ");
                    FusionPlant.AddReplacement(new Replacement(r[0], r[1]));
                }
                else
                {
                    CalMolecules.Add(line);
                }
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            foreach (var cal in CalMolecules)
            {
                FusionPlant!.SetCalibrationMolecule(cal);
                FusionPlant.Calculate();

                if (isTestMode)
                {
                    answer += GetTestOutput();
                }

                answer += $"Molecules Generated: {FusionPlant.GeneratedMolecules.Count}\n" +
                          $"Distinct Molecules : {FusionPlant.GeneratedMolecules.Distinct().Count()}\n" +
                          $"------------------------------\n";
            }

            Part1Result = answer;
        }


        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";

            foreach (var cal in CalMolecules)
            {
                int minSteps = 0;
                if (isTestMode)
                    minSteps = FusionPlant.Fabricate(cal);
                else
                    minSteps = RedNosedFusionPlant.GetFabricationStepCount(DataRaw.Replace("\r", "").Split('\n'));

                answer += $"{cal} => {minSteps} steps\n";
            }

            Part2Result = answer;
        }
        private string GetTestOutput()
        {
            var result = "";

            result += $"Replacments:\n";
            foreach (var rep in FusionPlant.Replacements)
                result += $"{rep.From} => {rep.To}\n";
            result += $"Cal Molecule: {FusionPlant.CalibrationMolecule}\n";

            result += "Generated Molecules:\n";
            foreach (var m in FusionPlant.GeneratedMolecules)
                result += $"- {m}\n";

            return result;
        }

        internal class RedNosedFusionPlant
        {
            public List<Replacement> Replacements { get; internal set; }
            public string CalibrationMolecule { get; internal set; }
            public List<string> GeneratedMolecules { get; internal set; }
            public RedNosedFusionPlant()
            {
                Replacements = [];
                GeneratedMolecules = [];
                CalibrationMolecule = string.Empty;
            }

            public void AddReplacement(Replacement replacement)
            {
                Replacements.Add(replacement);
            }
            public void SetCalibrationMolecule(string calibrationMolecule)
            {
                CalibrationMolecule = calibrationMolecule;
            }
            public void Calculate()
            {
                GeneratedMolecules.Clear();

                foreach (var rep in Replacements)
                {
                    var mol = CalibrationMolecule;
                    var idxs = new List<int>();

                    //find all indexes to replace at
                    for (int i = 0; i < mol.Length; i++)
                    {
                        var l = mol.Length - i;
                        var sub = mol.Substring(i, l);
                        if (sub.StartsWith(rep.From))
                        {
                            idxs.Add(i);
                        }
                    }

                    //replace and store at each index
                    foreach (var idx in idxs)
                    {
                        var c = CalibrationMolecule;
                        var f = rep.From.Length;
                        var left = c[..idx];
                        var right = c[(idx + f)..];

                        c = left + rep.To + right;
                        GeneratedMolecules.Add(c);
                    }
                }
            }
            public int Fabricate(string targetMolecule)
            {
                List<(string output, string input)> replacements = [];
                foreach (var rep in Replacements)
                    replacements.Add(new(rep.From, rep.To));

                return FewestStepsToCreateMoleculeForward(replacements, targetMolecule);
            }
            public static int GetFabricationStepCount(string[] input)
            {
                var molecule = string.Concat(input[^1].Reverse());
                var reverseRules = new Dictionary<string, string>();

                foreach (var line in input[..^2])
                {
                    var match = Regex.Match(line, @"(?<M>\w+)\s=>\s(?<Y>\w+)");
                    var a = string.Concat(match.Groups["M"].Value.Reverse());
                    var b = string.Concat(match.Groups["Y"].Value.Reverse());
                    reverseRules[b] = a;
                }

                var alternation = $"(?<E>{string.Join('|', reverseRules.Keys)})";
                var count = 0;

                while (molecule != "e")
                {
                    var match = Regex.Match(molecule, alternation);
                    var element = match.Groups["E"].Value;
                    var replace = new Regex(element);

                    molecule = replace.Replace(molecule, reverseRules[element], count: 1);
                    count++;
                }

                return count;
            }
            static int FewestStepsToCreateMoleculeForward(List<(string input, string output)> replacements, string targetMolecule)
            {
                var queue = new Queue<(string molecule, int steps)>();
                var visited = new HashSet<string>();

                queue.Enqueue(("e", 0));
                visited.Add("e");

                const int maxQueueSize = 1000000; // Maximum size of the queue
                const int maxVisitedSize = 2000000; // Maximum size of the visited set

                while (queue.Count > 0)
                {
                    // Prune the queue if it grows too large
                    if (queue.Count > maxQueueSize)
                    {
                        queue = new Queue<(string molecule, int steps)>(
                            queue.OrderBy(item => Math.Abs(item.molecule.Length - targetMolecule.Length))
                                 .Take(maxQueueSize / 2)
                        );
                    }

                    // Prune the visited set if it grows too large
                    if (visited.Count > maxVisitedSize)
                    {
                        visited = new HashSet<string>(
                            visited.OrderBy(item => Math.Abs(item.Length - targetMolecule.Length))
                                   .Take(maxVisitedSize / 2)
                        );
                    }

                    var (currentMolecule, steps) = queue.Dequeue();

                    // Check if we've reached the target molecule
                    if (currentMolecule == targetMolecule)
                        return steps;

                    foreach (var (input, output) in replacements)
                    {
                        int index = currentMolecule.IndexOf(input);
                        while (index != -1)
                        {
                            string newMolecule = ReplaceFirst(currentMolecule, input, output, index);

                            // Add to the queue if not visited and is reasonable in length
                            if (!visited.Contains(newMolecule) && newMolecule.Length <= targetMolecule.Length)
                            {
                                visited.Add(newMolecule);
                                queue.Enqueue((newMolecule, steps + 1));
                            }

                            // Find the next occurrence of the input
                            index = currentMolecule.IndexOf(input, index + 1);
                        }
                    }
                }

                return -1; // Return -1 if the target molecule is unreachable
            }
            static string ReplaceFirst(string str, string oldValue, string newValue, int index)
            {
                return string.Concat(str.AsSpan(0, index), newValue, str.AsSpan(index + oldValue.Length));
            }
        }
        internal class Replacement(string from, string to)
        {
            public string From { get; internal set; } = from;
            public string To { get; internal set; } = to;
        }
    }
}
