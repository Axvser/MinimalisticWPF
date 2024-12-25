using System.Collections;
using System.Collections.Concurrent;

namespace MinimalisticWPF.Animator
{
    public sealed class StateCollection : ICollection<State>
    {
        private ConcurrentDictionary<string, State> _nodes = new();
        private int _suffix = 0;
        public int BoardSuffix
        {
            get
            {
                if (_suffix < 50)
                {
                    _suffix++;
                    return _suffix;
                }
                if (_suffix == 50)
                {
                    _suffix = 0;
                    return _suffix;
                }
                return -1;
            }
        }
        public State this[int index]
        {
            get => _nodes.Values.ElementAtOrDefault(index) ?? throw new ArgumentOutOfRangeException($"Index value [ {index} ] is out of collection range");
        }
        public State this[string stateName]
        {
            get
            {
                if (!_nodes.TryGetValue(stateName, out var result))
                    throw new ArgumentException($"There is no State named [ {stateName} ] in the collection");
                return result;
            }
        }

        public int Count => _nodes.Count;
        public bool IsReadOnly => false;
        public void Add(State item)
        {
            _nodes.AddOrUpdate(item.StateName, item, (key, oldValue) => item);
        }
        public void Clear()
        {
            _nodes.Clear();
        }
        public bool Contains(State item)
        {
            return _nodes.ContainsKey(item.StateName);
        }
        public void CopyTo(State[] array, int arrayIndex)
        {
            _nodes.Values.CopyTo(array, arrayIndex);
        }
        public bool Remove(State item)
        {
            return _nodes.TryRemove(item.StateName, out _);
        }
        public IEnumerator<State> GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }
    }
}
