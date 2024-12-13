using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 20: Infinite Elves and Infinite Houses ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/20"/>
    internal class Day20 : Puzzle
    {
        public Day20()
            : base(Name: "Infinite Elves and Infinite Houses", DayNumber: 20) { }

        public override void ParseData()
        {
            //no data available for this puzzle
        }

        public override void Part1(bool isTestMode)
        {
            int target = isTestMode ? 250 : 36000000;
            int presentsToDeliver = 10;

            int houseLimit = target / presentsToDeliver;
            var houses = new int[houseLimit + 1];
            bool targetFound = false;

            for (int elf = 1; elf <= houseLimit && !targetFound; elf++)
            {
                for (int house = elf; house <= houseLimit && !targetFound; house += elf)
                {
                    houses[house] += elf * presentsToDeliver;

                    // Early exit if we find a house meeting the target
                    if (houses[house] >= target && house == elf)
                    {
                        targetFound = true;
                    }
                }
            }

            int minHouseNumber = Array.FindIndex(houses, h => h >= target);
            Part1Result = $"Smallest House Number [{minHouseNumber}] to reach the target of [{target}] got [{houses[minHouseNumber]}] presents";
        }

        public override void Part2(bool isTestMode)
        {
            int target = isTestMode ? 250 : 36000000;
            int presentsToDeliver = 11;

            // Approximate upper limit for houses; can be adjusted if needed
            int houseLimit = target / presentsToDeliver;
            var houses = new int[houseLimit + 1];

            for (int elf = 1; elf <= houseLimit; elf++)
            {
                // Each elf delivers to up to 50 houses
                for (int houseVisit = 0; houseVisit < 50; houseVisit++)
                {
                    int house = elf * (houseVisit + 1);
                    if (house > houseLimit) break; // Stop if house index exceeds the limit

                    houses[house] += elf * presentsToDeliver;
                }
            }

            // Find the smallest house number meeting the target
            int minHouseNumber = Array.FindIndex(houses, h => h >= target);
            Part2Result = $"Smallest House Number [{minHouseNumber}] to reach the target of [{target}] got [{houses[minHouseNumber]}] presents";
        }
        private static string Part1BruteForce(bool isTestMode)
        {
            int target = isTestMode ? 250 : 36000000;

            var houses = new Dictionary<int, int>();
            var elves = new Dictionary<int, int>();

            int elfCount = 0;
            int presentsToDeliver = 10;
            bool targetFound = false;

            while (!targetFound)
            {
                elfCount++;

                for (int e = 1; e <= elfCount; e++)
                {
                    if (!elves.ContainsKey(e))
                        elves[e] = e;

                    if (!houses.ContainsKey(elves[e]))
                        houses[elves[e]] = 0;

                    houses[elves[e]] += e * presentsToDeliver;

                    elves[e] += e;

                    if (houses.Any(h => h.Value >= target))
                    {
                        targetFound = true;
                        break;
                    }
                }
            }

            var targetHouses = houses.Where(h => h.Value >= target).ToList();
            var minHouseNumber = targetHouses.Min(h => h.Key);

            var answer = "";

            if (isTestMode)
            {
                foreach (var house in houses.OrderBy(h => h.Key))
                {
                    answer += $"House {house.Key} got {house.Value} presents.\n";
                }
            }

            answer += $"Smallest House Number [{minHouseNumber}] to reach the target of [{target}] got [{houses[minHouseNumber]}] presents\n";
            return answer;
        }
    }
}
