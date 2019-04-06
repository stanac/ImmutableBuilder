using System.Collections;
using System.Collections.Generic;

namespace ImmutableBuilder
{
    public class DistinctListOfStrings : IEnumerable<string>
    {
        private List<string> _list = new List<string>();

        public void Add(string s)
        {
            if (!_list.Contains(s))
            {
                _list.Add(s);
            }
        }

        public void AddRange(IEnumerable<string> strings)
        {
            foreach (var s in strings)
                if (!_list.Contains(s))
                    _list.Add(s);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public int Count => _list.Count;

        public IEnumerator GetEnumerator()
        {
            foreach (var s in _list)
            {
                yield return s;
            }
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            foreach (var s in _list)
            {
                yield return s;
            }
        }
    }
}
