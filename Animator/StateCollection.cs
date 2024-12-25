using System.Collections;

namespace MinimalisticWPF.Animator
{
    public sealed class StateCollection : ICollection<State>
    {
        private List<State> _nodes = [];

        public State this[int index] { get => _nodes[index < _nodes.Count && index > -1 ? index : throw new ArgumentOutOfRangeException($"Index value [ {index} ] is out of collection range")]; }
        public State this[string stateName]
        {
            get
            {
                State? result = _nodes.FirstOrDefault(x => x.StateName == stateName) ?? throw new ArgumentException($"There is no State named [ {stateName} ] in the collection");
                return result;
            }
        }

        public int Count => _nodes.Count;

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

        public bool IsReadOnly => false;

        public void Add(State item)
        {
            _nodes.RemoveAll(x => x.StateName == item.StateName);
            _nodes.Add(item);
        }
        public void Clear()
        {
            _nodes.Clear();
        }
        public bool Contains(State item)
        {
            return _nodes.Contains(item);
        }
        public void CopyTo(State[] array, int arrayIndex)
        {
            _nodes.CopyTo(array, arrayIndex);
        }
        public bool Remove(State item)
        {
            return _nodes.Remove(item);
        }
        public IEnumerator<State> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
    }
}
