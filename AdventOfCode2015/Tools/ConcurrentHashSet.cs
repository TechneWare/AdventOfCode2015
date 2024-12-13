using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Tools
{
    public class ConcurrentHashSet<T>
    {
        private readonly ConcurrentDictionary<T, byte> _dictionary;

        public ConcurrentHashSet() : this(EqualityComparer<T>.Default)
        {
        }

        public ConcurrentHashSet(IEqualityComparer<T> comparer)
        {
            _dictionary = new ConcurrentDictionary<T, byte>(comparer);
        }

        public bool Add(T item)
        {
            // TryAdd returns true if the item was added, false if it already existed
            return _dictionary.TryAdd(item, 0);
        }

        public bool Remove(T item)
        {
            // TryRemove returns true if the item was removed
            return _dictionary.TryRemove(item, out _);
        }

        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public int Count => _dictionary.Count;

        public List<T> ToList()
        {
            return new List<T>(_dictionary.Keys);
        }
    }
}
