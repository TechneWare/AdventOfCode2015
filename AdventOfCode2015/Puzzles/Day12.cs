using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Puzzles
{
    /// <summary>
    /// --- Day 12: JSAbacusFramework.io ---
    /// </summary>
    /// <see cref="https://adventofcode.com/2015/day/12"/>
    internal class Day12 : Puzzle
    {
        private List<string> jsonStrings { get; set; } = [];
        public Day12()
            : base(Name: "JSAbacusFramework.io", DayNumber: 12) { }

        public override void ParseData()
        {
            jsonStrings.Clear();
            var docs = DataRaw.Replace("\r", "").Replace("\n", "").Split("</>", StringSplitOptions.RemoveEmptyEntries);
            foreach (var doc in docs)
                jsonStrings.Add(doc);
        }

        public override void Part1(bool isTestMode)
        {
            ParseData();

            var answer = "";

            foreach (var jstr in jsonStrings)
            {
                var d = GetNumbersFromJson(jstr);
                var sum = d.Sum();

                if (isTestMode)
                    answer += $"{jstr} = {sum}\n";
                else
                    answer += $"Sum = {sum}";
            }


            Part1Result = answer;
        }

        public override void Part2(bool isTestMode)
        {
            ParseData();

            var answer = "";

            foreach (var jstr in jsonStrings)
            {
                var sum = SumIgnoringRedObjects(jstr);

                if (isTestMode)
                    answer += $"{jstr} = {sum}\n";
                else
                    answer += $"Sum = {sum}";
            }


            Part2Result = answer;
        }

        private static List<double> GetNumbersFromJson(string json)
        {
            var numbers = new List<double>();

            try
            {
                var token = JToken.Parse(json);
                FindNumbers(token, numbers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            return numbers;
        }
        private static void FindNumbers(JToken token, List<double> numbers)
        {
            if (token == null)
                return;

            switch (token.Type)
            {
                case JTokenType.Array:
                    foreach (var child in token.Children())
                    {
                        FindNumbers(child, numbers);
                    }
                    break;
                case JTokenType.Object:
                    foreach (var property in token.Children<JProperty>())
                    {
                        FindNumbers(property.Value, numbers);
                    }
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                    numbers.Add(token.Value<double>());
                    break;
                    //default:
                    //    throw new Exception($"Unhandled token type {token.Type}");
            }
        }
        private static int SumIgnoringRedObjects(string json)
        {
            try
            {
                var token = JToken.Parse(json);
                return SumNumbers(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return 0;
            }
        }

        private static int SumNumbers(JToken token)
        {
            if (token == null)
                return 0;

            switch (token.Type)
            {
                case JTokenType.Array:
                    int arraySum = 0;
                    foreach (var child in token.Children())
                    {
                        arraySum += SumNumbers(child);
                    }
                    return arraySum;
                case JTokenType.Object:
                    foreach (var property in token.Children<JProperty>())
                    {
                        if (property.Value.Type == JTokenType.String && property.Value.Value<string>() == "red")
                        {
                            return 0;
                        }
                    }

                    int objectSum = 0;
                    foreach (var property in token.Children<JProperty>())
                    {
                        objectSum += SumNumbers(property.Value);
                    }
                    return objectSum;

                case JTokenType.Integer:
                case JTokenType.Float:
                    return token.Value<int>();

                default:
                    return 0;
            }
        }
    }
}
