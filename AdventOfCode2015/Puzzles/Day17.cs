using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 17: No Such Thing as Too Much ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/17"/>
    internal class Day17 : Puzzle
    {
        private List<int> containers = [];
        public Day17()
            : base(Name: "", DayNumber: 17) { }

        public override void ParseData()
        {
            containers.Clear();
            containers = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => int.Parse(c))
                .ToList();
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            var targetSize = isTestMode ? 25 : 150;
            var combos = GetAllCombinations(containers, targetSize);

            if (isTestMode)
            {
                answer += "Combinations:\n";
                foreach (var c in combos)
                {
                    answer += string.Join(", ", c) + "\n";
                }
                
                answer += "\n";
            }

            answer += $"Total Combos: {combos.Count}\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            var targetSize = isTestMode ? 25 : 150;
            var combos = GetAllCombinations(containers, targetSize);

            if (isTestMode)
            {
                answer += "Combinations:\n";
                foreach (var c in combos)
                {
                    answer += string.Join(", ", c) + "\n";
                }
                answer += "\n";
            }

            var minNumContainers = combos.Min(c => c.Count);
            var combWithMin = combos.Where(c => c.Count == minNumContainers).ToList();

            answer += $"Min Containers: {minNumContainers}\n";
            answer += $"Combos of Min : {combWithMin.Count}\n";

            Part2Result = answer;
        }

        private static List<List<int>> GetAllCombinations(List<int> containers, int targetVolume)
        {
            var results = new List<List<int>>();
            FindCombinationsRecursive(containers, targetVolume, 0, [], results);
            return results;
        }

        private static void FindCombinationsRecursive(List<int> containers, int targetVolume, int index, List<int> current, List<List<int>> results)
        {
            //Target reached
            if (targetVolume == 0)
            {
                results.Add(new List<int>(current));
                return;
            }

            //Target exceeded or no more containers
            if (targetVolume < 0 || index >= containers.Count)
            {
                return;
            }

            // Include the current container
            current.Add(containers[index]);
            FindCombinationsRecursive(containers, targetVolume - containers[index], index + 1, current, results);

            // Exclude the current container
            current.RemoveAt(current.Count - 1);
            FindCombinationsRecursive(containers, targetVolume, index + 1, current, results);
        }
    }
}
