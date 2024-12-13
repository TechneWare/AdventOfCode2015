using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Tools
{
    public static class PermutationGeneratorLazy<T>
    {
        public static IEnumerable<List<T>> GetPermutations(List<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return Generate(items, 0);
        }

        private static IEnumerable<List<T>> Generate(List<T> items, int index)
        {
            if (index == items.Count - 1)
            {
                // Return a copy to prevent external modification
                yield return new List<T>(items);
            }
            else
            {
                for (int i = index; i < items.Count; i++)
                {
                    Swap(items, index, i);                     // Swap current index with i
                    foreach (var perm in Generate(items, index + 1)) // Recurse to generate further permutations
                    {
                        yield return perm;
                    }
                    Swap(items, index, i);                     // Backtrack
                }
            }
        }

        private static void Swap(List<T> items, int i, int j)
        {
            if (i != j)
            {
                (items[i], items[j]) = (items[j], items[i]);
            }
        }
    }
}
