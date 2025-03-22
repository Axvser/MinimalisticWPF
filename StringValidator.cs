using System.Text.RegularExpressions;

namespace MinimalisticWPF
{
    public class StringValidator()
    {
        private string? _start = null;
        private string? _end = null;
        private HashSet<string> _containsSubstrings = [];
        private HashSet<string> _excludeSubstrings = [];
        private Regex? _regexPattern = null;
        private List<Tuple<int, string>> _range = [];
        private int? _minLength = null;
        private int? _maxLength = null;
        private int? _fixLength = null;

        public StringValidator StartWith(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _start = value;
            }
            return this;
        }
        public StringValidator EndWith(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _end = value;
            }
            return this;
        }
        public StringValidator VarLength(int min, int max)
        {
            if (max >= min)
            {
                _minLength = min;
                _maxLength = max;
                _fixLength = null;
            }
            return this;
        }
        public StringValidator FixLength(int length)
        {
            _minLength = null;
            _maxLength = null;
            _fixLength = length;
            return this;
        }
        public StringValidator Include(params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (!string.IsNullOrEmpty(substring))
                {
                    _containsSubstrings.Add(substring);
                }
            }
            return this;
        }
        public StringValidator Exclude(params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (!string.IsNullOrEmpty(substring))
                {
                    _excludeSubstrings.Add(substring);
                }
            }
            return this;
        }
        public StringValidator Slice(int start, string value)
        {
            _range.Add(Tuple.Create(start, value));
            return this;
        }
        public StringValidator Regex(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                _regexPattern = new Regex(pattern);
            }
            return this;
        }
        public StringValidator OnlyNumbers()
        {
            _regexPattern = new Regex(@"^\d+$");
            return this;
        }
        public StringValidator OnlyLetters()
        {
            _regexPattern = new Regex(@"^[a-zA-Z]+$");
            return this;
        }

        public bool Validate(string input)
        {
            if (input == null)
                return false;

            if (_start != null && !input.StartsWith(_start))
                return false;
            if (_end != null && !input.EndsWith(_end))
                return false;

            if (_regexPattern != null && !_regexPattern.IsMatch(input))
                return false;
            if (_range.Count != 0 && _range.Any(v => input.Substring(v.Item1, v.Item2.Length) != v.Item2))
                return false;

            if (_containsSubstrings.Count != 0 && _containsSubstrings.Any(s => !input.Contains(s)))
                return false;
            if (_excludeSubstrings.Count != 0 && _excludeSubstrings.Any(s => input.Contains(s)))
                return false;

            if ((_minLength != null && _maxLength != null) && (input.Length < _minLength || input.Length > _maxLength))
                return false;
            if (_fixLength != null && input.Length != _fixLength)
                return false;

            return true;
        }
    }
}