using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2015.Puzzles.Day9;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 9: All in a Single Night ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/9"/>
    internal class Day9 : Puzzle
    {
        private List<Route> availableRoutes = [];

        public Day9()
            : base(Name: "All in a Single Night", DayNumber: 9) { }

        public override void ParseData()
        {
            availableRoutes.Clear();

            var data = DataRaw.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var rt in data)
            {
                var d = rt.Replace(" to ", ",")
                          .Replace(" = ", ",")
                          .Split(',', StringSplitOptions.RemoveEmptyEntries);

                availableRoutes.Add(new Route(d[0], d[1], int.Parse(d[2])));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            List<Trip> trips = GetTrips();

            if (isTestMode)
            {
                foreach (var trip in trips)
                {
                    answer += $"{trip}\n";
                }
            }

            var minDistance = trips.Min(t => t.TotalDistance);
            var shortestTrip = trips.Where(t => t.TotalDistance == minDistance).FirstOrDefault();
            answer += $"Shortest Trip:\n{shortestTrip}\n";

            Part1Result = answer;
        }
        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            List<Trip> trips = GetTrips();

            if (isTestMode)
            {
                foreach (var trip in trips)
                {
                    answer += $"{trip}\n";
                }
            }

            var maxDistance = trips.Max(t => t.TotalDistance);
            var longestTrip = trips.Where(t => t.TotalDistance == maxDistance).FirstOrDefault();
            answer += $"Longest Trip:\n{longestTrip}\n";

            Part2Result = answer;
        }

        private List<Trip> GetTrips()
        {
            var allCities = availableRoutes.SelectMany(r => r.Endpoints).Distinct().OrderBy(r => r).ToList();
            var trips = new List<Trip>();

            for (int i = 0; i < allCities.Count; i++)
            {
                var newPaths = GetPathsFrom(allCities[i]);

                foreach (var path in newPaths)
                {
                    var trip = new Trip();
                    for (int j = 0; j < path.Count - 1; j++)
                    {
                        var points = new List<string> { path[j], path[j + 1] };
                        var rt = availableRoutes.Where(r => r.Endpoints.All(ep => points.Contains(ep))).Single();
                        var tripRt = new Route(path[j], path[j + 1], rt.Distance);
                        trip.Routes.Add(tripRt);
                    }
                    trips.Add(trip);
                }
            }

            return trips;
        }

        private List<List<string>> GetPathsFrom(string startCity, List<string> VisitedCities = null)
        {
            var allPaths = new List<List<string>>();

            VisitedCities ??= [startCity];

            var allCities = availableRoutes.SelectMany(r => r.Endpoints).Distinct().OrderBy(r => r).ToList();
            var notVisitedCities = allCities.Except(VisitedCities).ToList();

            var possibleRoutes = availableRoutes
                .Where(r => r.Endpoints.Contains(startCity)
                         && r.Endpoints.Any(ep => notVisitedCities.Contains(ep))
                ).ToList();

            if (possibleRoutes.Any())
            {
                foreach (var possibleRoute in possibleRoutes)
                {
                    var nextCity = possibleRoute.Endpoints.Where(ep => ep != startCity).Single();
                    var t = new List<string>();
                    t.AddRange(VisitedCities);
                    t.Add(nextCity);

                    var nextPaths = GetPathsFrom(nextCity, t);

                    allPaths.AddRange(nextPaths);
                }
            }
            else
            {
                allPaths.Add(VisitedCities);
            }

            return allPaths;
        }


        internal class Route
        {
            public List<string> Endpoints { get; internal set; }
            public string From => Endpoints[0];
            public string To => Endpoints[1];
            public int Distance { get; internal set; }
            public Route(string from, string to, int distance)
            {
                Endpoints = [from, to];
                Distance = distance;
            }
        }
        internal class Trip
        {
            public List<Route> Routes { get; set; } = [];
            public int TotalDistance => Routes.Sum(r => r.Distance);
            public override string ToString()
            {
                var result = "";
                foreach (var route in Routes)
                {
                    var name = route != Routes.Last() ? $"{route.From} -> " : $"{route.From} -> {route.To} = {TotalDistance}";
                    result += $"{name}";
                }

                return result;
            }
        }
    }
}
