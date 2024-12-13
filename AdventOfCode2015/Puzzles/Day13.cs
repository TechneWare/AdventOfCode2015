using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 13: Knights of the Dinner Table ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/13"/>
    internal class Day13 : Puzzle
    {
        private List<Guest> guests = [];
        public Day13()
            : base(Name: "Knights of the Dinner Table", DayNumber: 13) { }

        public override void ParseData()
        {
            guests.Clear();

            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in data)
            {
                var l = line
                    .Replace("would", "")
                    .Replace("gain ", "")
                    .Replace("lose ", "-")
                    .Replace("happiness units by sitting next to", "")
                    .Replace(".", "")
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var g = guests.Where(g => g.Name == l[0]).SingleOrDefault();
                if (g == null)
                {
                    g = new Guest(l[0]);
                    guests.Add(g);
                }

                var g1 = guests.Where(g => g.Name == l[2]).SingleOrDefault();
                if (g1 == null)
                {
                    g1 = new Guest(l[2]);
                    guests.Add(g1);
                }

                g.Relationships.Add(new Relationship(g1, int.Parse(l[1])));

            }
        }
        public override void Part1(bool isTestMode)
        {
            ParseData();

            var guestNames = guests.Select(g => g.Name).ToList();
            List<SeatingArrangment> seatArrangments = GetSeatingArrangments(guestNames);

            var answer = "";
            if (isTestMode)
            {
                answer += "Seating Arrangments:\n";
                foreach (var s in seatArrangments)
                {
                    foreach (var g in s.Guests)
                    {
                        answer += $"{g.Name} ";
                    }
                    answer += "\n";
                    foreach (var score in s.Scores)
                    {
                        answer += $"{(score >= 0 ? $"+" : "")}{score} ";
                    }
                    answer += $"= {s.TotalScore}\n";
                }
            }

            var bestScore = seatArrangments.Max(s => s.TotalScore);
            answer += $"Best Score = {bestScore}\n";

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();
            AddSelfToGuestList();

            var guestNames = guests.Select(g => g.Name).ToList();
            List<SeatingArrangment> seatArrangments = GetSeatingArrangments(guestNames);

            var answer = "";
  
            var bestScore = seatArrangments.Max(s => s.TotalScore);
            answer += $"Best Score = {bestScore}\n";

            Part2Result = answer;
        }

        private List<SeatingArrangment> GetSeatingArrangments(List<string> guestNames)
        {
            List<SeatingArrangment> seatArrangments = [];
            var arrangments = GetCircularPermutations(guestNames);
            foreach (var arrangment in arrangments)
            {
                var a = new SeatingArrangment();
                foreach (var name in arrangment)
                {
                    var g = guests.Where(g => g.Name == name).Single();
                    a.Guests.Add(g);
                }
                seatArrangments.Add(a);
            }

            return seatArrangments;
        }
        private void AddSelfToGuestList()
        {
            var me = new Guest("Self");
            foreach (var g in guests)
            {
                me.Relationships.Add(new Relationship(g, 0));
                g.Relationships.Add(new Relationship(me, 0));
            }

            guests.Add(me);
        }
        private static List<List<string>> GetCircularPermutations(List<string> names)
        {
            var allPermutations = GetPermutations(names, names.Count).ToList();
            var uniqueArrangements = new HashSet<string>();

            // Normalize each permutation to its "canonical" form (smallest rotation).
            foreach (var permutation in allPermutations)
            {
                var rotations = GetRotations(permutation)
                    .Select(rotation => string.Join(",", rotation));    // Convert each rotation to a string
                var canonicalForm = rotations.Min();                    // Find the smallest string lexicographically
                _ = uniqueArrangements.Add(item: canonicalForm!);       // Add to the HashSet
            }

            // Convert back to list of lists
            return uniqueArrangements
                .Select(arrangement => arrangement.Split(',').ToList())
                .ToList();
        }
        private static IEnumerable<List<string>> GetPermutations(List<string> list, int length)
        {
            if (length == 1) return list.Select(t => new List<string> { t });

            return GetPermutations(list, length - 1)
                    .SelectMany(t => list.Where(e => !t.Contains(e)),
                        (t1, t2) => t1.Concat([t2]).ToList());
        }
        private static IEnumerable<List<string>> GetRotations(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                yield return list.Skip(i).Concat(list.Take(i)).ToList();
            }
        }
        internal class SeatingArrangment
        {
            public List<Guest> Guests { get; set; } = [];
            public int TotalScore => Scores.Sum();
            public List<int> Scores
            {
                get
                {
                    var s = new List<int>();

                    for (int i = 0; i < Guests.Count; i++)
                    {
                        Guest other1;
                        if (i == 0)
                            other1 = Guests.Last();
                        else
                            other1 = Guests[i - 1];

                        Guest other2 = null;
                        if (i == Guests.Count - 1)
                            other2 = Guests.First();
                        else
                            other2 = Guests[i + 1];

                        var s1 = Guests[i].Relationships.Where(r => r.Other.Name == other1.Name).Select(o => o.Weight).Single();
                        var s2 = Guests[i].Relationships.Where(r => r.Other.Name == other2.Name).Select(o => o.Weight).Single();

                        s.Add(s1);
                        s.Add(s2);
                    }

                    return s;
                }
            }
        }
        internal class Guest(string name)
        {
            public string Name { get; set; } = name;
            public List<Relationship> Relationships { get; set; } = [];
        }
        internal class Relationship(Day13.Guest other, int weight)
        {
            public Guest Other { get; set; } = other;
            public int Weight { get; set; } = weight;
        }
    }
}
