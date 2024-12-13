using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 15: Science for Hungry People ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/15"/>
    internal class Day15 : Puzzle
    {
        private List<Ingredient> Ingredients { get; set; } = [];
        public Day15()
            : base(Name: "Science for Hungry People", DayNumber: 15) { }

        public override void ParseData()
        {
            Ingredients.Clear();
            var data = DataRaw.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in data)
            {
                var d = line
                    .Replace(":", "")
                    .Replace(",", "")
                    .Replace("capacity ", "")
                    .Replace("durability ", "")
                    .Replace("flavor ", "")
                    .Replace("texture ", "")
                    .Replace("calories ", "")
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries);

                Ingredients.Add(new Ingredient(d[0], int.Parse(d[1]), int.Parse(d[2]), int.Parse(d[3]), int.Parse(d[4]), int.Parse(d[5])));
            }
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";
            (int maxScore, List<int> bestCombination) = GetMaxScore(Ingredients, 100);
            answer += "Ingredients:\n";
            for (int i = 0; i < Ingredients.Count; i++)
            {
                answer += $"{Ingredients[i].Name}: {bestCombination[i]} tsp\n";
            }

            answer += $"Score: {maxScore}\n";

            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";
            (int maxScore, List<int> bestCombination) = GetMaxScore(Ingredients, 100, 500);
            answer += "500 Cal Ingredients:\n";
            for (int i = 0; i < Ingredients.Count; i++)
            {
                answer += $"{Ingredients[i].Name}: {bestCombination[i]} tsp\n";
            }

            answer += $"Score: {maxScore}\n";

            Part2Result = answer;
        }

        private static (int, List<int>) GetMaxScore(List<Ingredient> ingredients, int totalTeaspoons, int? targetCalories = null)
        {
            int maxScore = 0;
            List<int> bestCombination = [];

            // Recursive helper to generate combinations
            void FindBestScore(int index, List<int> quantities, int remaining)
            {
                if (index == ingredients.Count - 1)
                {
                    //Allocate remaining teaspoons to the last ingredient
                    quantities.Add(remaining);

                    // Calculate score
                    CalcScore(ingredients, quantities, out int calories, out int score);

                    if (score > maxScore && (calories == targetCalories || targetCalories == null))
                    {
                        maxScore = score;
                        bestCombination = new List<int>(quantities); 
                    }

                    // Backtrack
                    quantities.RemoveAt(quantities.Count - 1);
                    return;
                }

                // Recurse for current ingredient
                for (int i = 0; i <= remaining; i++)
                {
                    quantities.Add(i);
                    FindBestScore(index + 1, quantities, remaining - i);
                    quantities.RemoveAt(quantities.Count - 1); // Backtrack
                }
            }

            FindBestScore(0, [], totalTeaspoons);

            return (maxScore, bestCombination);
        }
        private static void CalcScore(List<Ingredient> ingredients, List<int> quantities, out int calories, out int score)
        {
            int capacity = 0, durability = 0, flavor = 0, texture = 0;
            calories = 0;

            for (int i = 0; i < ingredients.Count; i++)
            {
                capacity += quantities[i] * ingredients[i].Capacity;
                durability += quantities[i] * ingredients[i].Durability;
                flavor += quantities[i] * ingredients[i].Flavor;
                texture += quantities[i] * ingredients[i].Texture;
                calories += quantities[i] * ingredients[i].Calories;
            }

            capacity = Math.Max(0, capacity);
            durability = Math.Max(0, durability);
            flavor = Math.Max(0, flavor);
            texture = Math.Max(0, texture);

            score = capacity * durability * flavor * texture;
        }
        internal class Ingredient(string name, int capacity, int durability, int flavor, int texture, int calories)
        {
            public string Name { get; } = name;
            public int Capacity { get; } = capacity;
            public int Durability { get; } = durability;
            public int Flavor { get; } = flavor;
            public int Texture { get; } = texture;
            public int Calories { get; } = calories;
        }
    }
}
