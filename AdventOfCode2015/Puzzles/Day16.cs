using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 16: Aunt Sue ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/16"/>
    internal class Day16 : Puzzle
    {
        private readonly List<AuntSue> sueList = [];
        private readonly List<Property> targetProps =
                            [
                                new("children", 3),
                                new("cats", 7),
                                new("samoyeds", 2),
                                new("pomeranians", 3),
                                new("akitas", 0),
                                new("vizslas", 0),
                                new("goldfish", 5),
                                new("trees", 3),
                                new("cars", 2),
                                new("perfumes", 1)
                            ];

        public Day16()
            : base(Name: "Aunt Sue", DayNumber: 16) { }

        public override void ParseData()
        {
            sueList.Clear();
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in data)
            {
                var temp = line.Replace("Sue ", "");
                var idx = temp.IndexOf(':');
                temp = string.Concat(temp.AsSpan(0, idx), ",", temp.AsSpan(idx + 1, temp.Length - idx - 1));
                var d = temp.Split(',', StringSplitOptions.RemoveEmptyEntries);

                int id = int.Parse(d[0]);
                List<Property> props = [];
                for (int i = 1; i < d.Length; i++)
                {
                    var p = d[i].Split(':', StringSplitOptions.RemoveEmptyEntries);
                    props.Add(new Property(p[0].Trim(), int.Parse(p[1])));
                }

                sueList.Add(new AuntSue(id, props));
            }
        }

        public override void Part1(bool isTestMode)
        {
            var answer = "";
            if (!isTestMode)
            {
                ParseData();

                var matchingAunts = new List<AuntSue>();
                foreach (var a in sueList)
                {
                    if (a.IsMatchOnAllProps(targetProps))
                        matchingAunts.Add(a);
                }

                answer = GetAnswerString(answer, matchingAunts);
            }
            else
                answer += $"No Tests Available\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            var answer = "";
            if (!isTestMode)
            {
                ParseData();

                var matchingAunts = new List<AuntSue>();
                foreach (var a in sueList)
                {
                    if (a.IsMatchOnRangedProps(targetProps: targetProps,
                                       graterThanPropNames: ["cats", "trees"],
                                         lessThanPropNames: ["pomeranians", "goldfish"]))
                    { matchingAunts.Add(a); }
                }

                answer = GetAnswerString(answer, matchingAunts);
            }
            else
                answer += $"No Tests Available\n";

            Part2Result = answer;
        }

        private string GetAnswerString(string answer, List<AuntSue> matchingAunts)
        {
            answer += "Targets:\n";
            foreach (var tp in targetProps)
                answer += $"{tp.Name}: {tp.Value}\n";

            answer += "\nMatches:\n";
            foreach (var ma in matchingAunts)
            {
                answer += $"Sue {ma.Id}:";
                foreach (var p in ma.Properties)
                    answer += $" {p.Name}: {p.Value},";

                answer = string.Concat(answer.AsSpan(0, answer.Length - 1), "\n");
            }

            return answer;
        }
        internal class Property(string name, int value)
        {
            public string Name { get; internal set; } = name;
            public int Value { get; internal set; } = value;
        }
        internal class AuntSue(int Id, List<Day16.Property> properties)
        {
            public int Id { get; internal set; } = Id;
            public List<Property> Properties { get; internal set; } = properties;

            public bool IsMatchOnAllProps(List<Property> targetProps)
            {
                return Properties.All(p => targetProps.Any(tp => tp.Name == p.Name && tp.Value == p.Value));
            }

            public bool IsMatchOnRangedProps(List<Property> targetProps, List<string> graterThanPropNames, List<string> lessThanPropNames)
            {
                //Get targets for exact, greater than and less than matches
                var tp = targetProps.Where(t => !graterThanPropNames.Contains(t.Name) && !lessThanPropNames.Contains(t.Name)).ToList();
                var gp = targetProps.Where(t => graterThanPropNames.Contains(t.Name)).ToList();
                var lp = targetProps.Where(t => lessThanPropNames.Contains(t.Name)).ToList();

                //get props for exact, greater than and less than matches
                var emProps = Properties.Where(p => tp.Any(t => t.Name == p.Name)).ToList();
                var gtProps = Properties.Where(p => gp.Any(t => t.Name == p.Name)).ToList();
                var ltProps = Properties.Where(p => lp.Any(t => t.Name == p.Name)).ToList();

                //either none of the properties are in the target or all are matched by the target
                var allExactMatch = emProps.Count == 0 || emProps.All(p => tp.Any(t => t.Name == p.Name && t.Value == p.Value));
                var allGtMatch = gtProps.Count == 0 || gtProps.All(p => gp.Any(t => t.Name == p.Name && p.Value > t.Value));
                var allLtMatch = ltProps.Count == 0 || ltProps.All(p => lp.Any(t => t.Name == p.Name && p.Value < t.Value));

                return allExactMatch && allGtMatch && allLtMatch;
            }
        }
    }
}