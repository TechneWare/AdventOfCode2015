using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015.Tools
{
    public class ThreadSafeHashSet<T>
    {
        private readonly HashSet<T> _hashSet;
        private readonly object _lock = new();

        public ThreadSafeHashSet()
        {
            _hashSet = new HashSet<T>();
        }

        public ThreadSafeHashSet(IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
        }

        public bool Add(T item)
        {
            lock (_lock)
            {
                return _hashSet.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _hashSet.Remove(item);
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _hashSet.Contains(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _hashSet.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _hashSet.Count;
                }
            }
        }

        public List<T> ToList()
        {
            lock (_lock)
            {
                return new List<T>(_hashSet);
            }
        }
    }
}
