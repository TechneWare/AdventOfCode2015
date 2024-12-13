using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 14: Reindeer Olympics ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/14"/>
    internal class Day14 : Puzzle
    {
        private List<Reindeer> reindeerList = [];

        public Day14()
         : base(Name: "Reindeer Olympics", DayNumber: 14) { }

        public override void ParseData()
        {
            reindeerList.Clear();
            var data = DataRaw.Replace("\r", "")
                .Replace("can fly ", "")
                .Replace("km/s for ", "")
                .Replace("seconds, but then must rest for ", "")
                .Replace("seconds.", "")
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in data)
            {
                var d = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                reindeerList.Add(new Reindeer(d[0], int.Parse(d[1]), int.Parse(d[2]), int.Parse(d[3])));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();
            int raceSeconds = isTestMode ? 1000 : 2503;
            var race = new Race(raceSeconds, reindeerList, Race.Rules.BestDistance);
            race.Run();

            var answer = "";

            answer += "Racers:\n";
            foreach (var r in race.Reindeers)
            {
                answer += $"{r.Name} => {r.Distance} km\n";
            }

            foreach (var w in race.Winners)
            {
                answer += $"Winner: {w.Name} => {w.Distance} km\n";
            }

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();
            int raceSeconds = isTestMode ? 1000 : 2503;
            var race = new Race(raceSeconds, reindeerList, Race.Rules.BestPoints);
            race.Run();

            var answer = "";

            answer += "Racers:\n";
            foreach (var r in race.Reindeers)
            {
                answer += $"{r.Name} => {r.Distance} km {r.Points} Points\n";
            }

            foreach (var w in race.Winners)
            {
                answer += $"Winner: {w.Name} => {w.Distance} km {w.Points} Points\n";
            }

            Part2Result = answer;
        }

        internal class Race
        {
            public enum Rules
            {
                BestDistance,
                BestPoints
            }

            public int Duration { get; internal set; }
            public List<Reindeer> Reindeers { get; internal set; }
            public List<Reindeer> Winners { get; internal set; } = [];
            public Rules RaceRules { get; internal set; }
            public Race(int duration, List<Reindeer> reindeers, Rules raceRules)
            {
                Duration = duration;
                this.Reindeers = reindeers;

                foreach (var r in reindeers)
                    r.Reset();

                RaceRules = raceRules;
            }

            public void Run()
            {
                Winners.Clear();
                for (int i = 0; i < Duration; i++)
                {
                    foreach (var r in Reindeers)
                        r.Update();

                    if (RaceRules == Rules.BestPoints)
                    {
                        var maxDistance = Reindeers.Max(r => r.Distance);
                        var leaders = Reindeers.Where(r => r.Distance == maxDistance).ToList();
                        foreach (var leader in leaders)
                            leader.AddPoint();
                    }
                }

                if (RaceRules == Rules.BestDistance)
                {
                    var maxDistance = Reindeers.Max(r => r.Distance);
                    Winners = Reindeers.Where(r => r.Distance == maxDistance).ToList();
                }
                else
                {
                    var maxPoints = Reindeers.Max(r => r.Points);
                    Winners = Reindeers.Where(r => r.Points == maxPoints).ToList();
                }
            }
        }
        internal class Reindeer(string name, int speed, int flyDuration, int restDuration)
        {
            public string Name { get; internal set; } = name;
            public int Speed { get; internal set; } = speed;
            public int FlyDuration { get; internal set; } = flyDuration;
            public int RestDuration { get; internal set; } = restDuration;

            private int _distance = 0;
            private int _FlySeconds = 0;
            private int _RestSeconds = 0;
            private int _TotalSeconds = 0;
            private bool _IsResting = false;
            private int _Points = 0;

            public int Distance => _distance;
            public int TotalSeconds => _TotalSeconds;
            public int RestSeconds => _RestSeconds;
            public int FlySeconds => _FlySeconds;
            public bool IsResting => _IsResting;
            public int Points => _Points;

            public void Reset()
            {
                _distance = 0;
                _FlySeconds = 0;
                _RestSeconds = 0;
                _TotalSeconds = 0;
                _Points = 0;
                _IsResting = false;
            }
            public void Update()
            {
                if (_IsResting)
                {
                    _RestSeconds++;
                    if (_RestSeconds == RestDuration)
                    {
                        _IsResting = false;
                        _RestSeconds = 0;
                    }
                }
                else
                {
                    _FlySeconds++;
                    _distance += Speed;
                    if (_FlySeconds == FlyDuration)
                    {
                        _IsResting = true;
                        _FlySeconds = 0;
                    }
                }

                _TotalSeconds++;
            }
            public void AddPoint() { _Points++; }
        }
    }
}