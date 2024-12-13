using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Tools
{
    public static class PermutationGenerator
    {
        // Method to generate all permutations of a given list
        public static List<List<T>> GetPermutations<T>(List<T> list)
        {
            var result = new List<List<T>>();
            Permute(list, 0, result);
            return result;
        }

        // Recursive helper method to generate permutations
        private static void Permute<T>(List<T> list, int startIndex, List<List<T>> result)
        {
            if (startIndex == list.Count)
            {
                result.Add(new List<T>(list));
            }
            else
            {
                for (int i = startIndex; i < list.Count; i++)
                {
                    Swap(list, startIndex, i);
                    Permute(list, startIndex + 1, result);
                    Swap(list, startIndex, i); // Backtrack
                }
            }
        }

        // Swap two elements in a list
        private static void Swap<T>(List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static List<List<T>> GetAllCombinationsAndPermutationsOfSize<T>(List<T> items,int minSize, int maxSize)
        {
            var result = new List<List<T>>();

            if (items.Count < maxSize)
            {
                var idx = 0;
                var dupes = new List<T>(items);
                while (items.Count < maxSize)
                {
                    items.Add(dupes[idx++]);
                    if (idx >= dupes.Count)
                        idx = 0;
                }
            }

            // Generate all combinations for all sizes from 1 to the full size of the list
            for (int size = minSize; size <= maxSize; size++)
            {
                // Generate all combinations of a given size
                var subsets = GetCombinationsOfSize(items, size);
                foreach (var subset in subsets)
                {
                    // For each subset, generate all permutations and add them to the result
                    var permutations = GetPermutations(subset);
                    result.AddRange(permutations);
                }
            }

            return result;
        }
        // Method to generate all combinations (subsets) of the set of all lengths
        public static List<List<T>> GetAllCombinationsAndPermutations<T>(List<T> items)
        {
            var result = new List<List<T>>();

            // Generate all combinations for all sizes from 1 to the full size of the list
            for (int size = 1; size <= items.Count; size++)
            {
                // Generate all combinations of a given size
                var subsets = GetCombinationsOfSize(items, size);
                foreach (var subset in subsets)
                {
                    // For each subset, generate all permutations and add them to the result
                    var permutations = GetPermutations(subset);
                    result.AddRange(permutations);
                }
            }

            return result;
        }

        // Method to generate combinations of a given size
        private static List<List<T>> GetCombinationsOfSize<T>(List<T> items, int size)
        {
            var result = new List<List<T>>();
            GenerateCombinations(items, size, 0, new List<T>(), result);
            return result;
        }

        // Recursive helper method to generate combinations
        private static void GenerateCombinations<T>(List<T> items, int size, int index, List<T> current, List<List<T>> result)
        {
            if (current.Count == size)
            {
                result.Add(new List<T>(current));
                return;
            }

            for (int i = index; i < items.Count; i++)
            {
                current.Add(items[i]);
                GenerateCombinations(items, size, i + 1, current, result);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
