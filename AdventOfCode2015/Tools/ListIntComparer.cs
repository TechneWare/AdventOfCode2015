using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Tools
{
    public class ListIntComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int>? x, List<int>? y)
        {
            if (x == null || y == null) return false;
            if (x.Count != y.Count) return false;

            return !x.Except(y).Any();
        }

        public int GetHashCode([DisallowNull] List<int> obj)
        {
            if (obj == null) return 0;

            unchecked
            {
                int hash = 19;
                foreach (var item in obj)
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
        }
    }
}
