using AdventOfCode2015.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pgen = AdventOfCode2015.Tools.ThreadSafePermutationGeneratorLazy<int>;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 24: It Hangs in the Balance ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/24"/>
    internal class Day24 : Puzzle
    {
        private List<int> Weights = [];
        public Day24()
            : base(Name: "It Hangs in the Balance", DayNumber: 24) { }

        public override void ParseData()
        {
            Weights.Clear();
            Weights = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => int.Parse(n)).ToList();
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();

            string answer = "";

            var balancers = new HashSet<Balancer>(new BalancerComparer());
            var sortedWeights = Weights.OrderByDescending(n => n).ToList();
            var exactSums = GetSplitSets(sortedWeights, numSets: 3);

            foreach (var exact in exactSums)
            {
                if (isTestMode)
                {
                    answer = GetTestResults(answer, sortedWeights, balancers, exact);
                }
                else
                {
                    var b = new Balancer(exact);
                    if (b.IsValid)
                        balancers.Add(b);
                    else
                        Debugger.Break();
                }
            }

            answer = GetRunResult(answer, balancers);

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            string answer = "";

            var balancers = new HashSet<Balancer>(new BalancerComparer());
            var sortedWeights = Weights.OrderByDescending(n => n).ToList();
            var exactSums = GetSplitSets(sortedWeights, numSets: 4);

            foreach (var exact in exactSums)
            {
                if (isTestMode)
                {
                    answer = GetTestResults(answer, sortedWeights, balancers, exact);
                }
                else
                {
                    var b = new Balancer(exact);
                    if (b.IsValid)
                        balancers.Add(b);
                    else
                        Debugger.Break();
                }
            }

            answer = GetRunResult(answer, balancers);

            Part2Result = answer;
        }
        private static string GetRunResult(string answer, HashSet<Balancer> balancers)
        {
            var minLen = balancers.ToList().Min(b => b.SmallSize);
            var shortest = balancers.ToList().Where(g => g.SmallSize == minLen).ToList();
            var ovarllMinQE = balancers.Min(b => b.QE);

            var bestMinQE = shortest.Min(g => g.QE);
            var best = shortest.Count > 1
                ? shortest.Where(g => g.QE == bestMinQE).Single()
                : shortest.First();

            answer += $"\nTotal Configurations  = {balancers.Count}\n";
            answer += $"Short 1st Group Length = {minLen}\n";
            answer += $"Short 1st Group Count  = {shortest.Count}\n";
            answer += $"Minimum QE             = {ovarllMinQE}\n";
            answer += $"Best QE                = {best.QE}\n";
            return answer;
        }
        private static string GetTestResults(string answer, List<int> sortedWeights, HashSet<Balancer> balancers, List<int> exact)
        {
            var otherGroups = GetSetsForTarget(sortedWeights.Except(exact).ToList(), exact.Sum());
            var pairs = PairListsWithoutOverlap(otherGroups);

            foreach (var (l1, l2) in pairs)
            {
                var others = new List<List<int>>() { l1, l2 };
                var l3 = sortedWeights.Except(exact).Except(l1).Except(l2).ToList();
                if (l3.Any())
                    others.Add(l3);

                var b = new Balancer(exact, others);
                if (b.IsValid)
                    balancers.Add(b);
                else
                    Debugger.Break();

                answer += $"{string.Join(' ', b.FirstGroup).PadRight(15)} (QE={b.QE,3}); ";
                for (int g = 1; g < b.Groups.Count; g++)
                {
                    answer += $"{string.Join(' ', b.Groups[g]).PadRight(15)}";
                }
                answer += "\n";
            }

            return answer;
        }
        
        static List<(List<int> l1, List<int> l2)> PairListsWithoutOverlap(List<List<int>> listOfLists)
        {
            var results = new List<(List<int> l1, List<int> l2)>();

            for (int i = 0; i < listOfLists.Count; i++)
            {
                for (int j = i + 1; j < listOfLists.Count; j++)
                {
                    if (!HasOverlap(listOfLists[i], listOfLists[j]))
                    {
                        results.Add((listOfLists[i], listOfLists[j]));
                    }
                }
            }

            return results;
        }
        static bool HasOverlap(List<int> l1, List<int> l2)
        {
            var set = new HashSet<int>(l1);
            return l2.Any(set.Contains);
        }
        private static List<List<int>> GetSplitSets(List<int> allNumbers, int numSets)
        {
            var target = allNumbers.Sum() / numSets;
            var result = GetSetsForTarget(allNumbers, target);

            return [.. result];
        }
        private static List<List<int>> GetSetsForTarget(List<int> numbers, int target)
        {
            var results = new List<List<int>>();
            FindSubsets(numbers, target, 0, [], results);
            return results;
        }
        private static void FindSubsets(List<int> numbers, int target, int index, List<int> currentSubset, List<List<int>> results)
        {
            if (target == 0)
            {
                results.Add(new List<int>(currentSubset));
                return;
            }

            if (target < 0 || index >= numbers.Count)
            {
                return;
            }

            // Include the current number
            currentSubset.Add(numbers[index]);
            FindSubsets(numbers, target - numbers[index], index + 1, currentSubset, results);

            // Exclude the current number
            currentSubset.RemoveAt(currentSubset.Count - 1);
            FindSubsets(numbers, target, index + 1, currentSubset, results);
        }

        internal class Balancer
        {
            private readonly bool requireAllGroups;

            public List<int> FirstGroup => Groups.First();
            public int SmallSize => FirstGroup.Count;
            public List<List<int>> Groups { get; set; } = [];
            public ulong QE { get; private set; }
            public bool IsValid => !requireAllGroups || (Groups.Count >= 3 && QE > 0);

            public Balancer(List<int> firstGroup)
            {
                requireAllGroups = false;
                Init(firstGroup);
            }
            public Balancer(List<int> firstGroup, List<List<int>> otherGroups)
            {
                requireAllGroups = true;
                Init(firstGroup);

                foreach (var group in otherGroups)
                    Groups.Add(group);
            }

            private void Init(List<int> firstGroup)
            {
                Groups.Add(firstGroup);

                ulong qe = 1;
                foreach (var n in firstGroup)
                    qe = qe * (ulong)n;
                QE = qe;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Groups);
            }
        }
        internal class BalancerComparer : IEqualityComparer<Balancer>
        {
            public bool Equals(Balancer? x, Balancer? y)
            {
                if (x == null || y == null) return false;

                if (x!.Groups.Count == y!.Groups.Count)
                {
                    return !x.FirstGroup.Except(y.FirstGroup).Any();
                }

                return false;
            }

            public int GetHashCode([DisallowNull] Balancer obj)
            {
                if (obj == null || obj.Groups == null) return 0;

                unchecked
                {
                    int hash = 19;
                    foreach (var item in obj.FirstGroup)
                    {
                        hash = hash * 31 + item.GetHashCode();
                    }
                    return hash;
                }
            }
        }
    }
}
