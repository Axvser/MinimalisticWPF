using System.Text.RegularExpressions;

namespace MinimalisticWPF.Fluent
{
    public class FluentStringValidator()
    {
        private HashSet<string> _validationStart = [];
        private HashSet<string> _validationEnd = [];
        private HashSet<string> _containsSubstrings = [];
        private HashSet<string> _excludeSubstrings = [];
        private HashSet<Regex> _regexPattern = [];
        private List<Tuple<int, string>> _range = [];
        private int? _minLength;
        private int? _maxLength;
        private int? _fixLength;

        public FluentStringValidator StartWith(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _validationStart.Add(value);
                }
            }
            return this;
        }
        public FluentStringValidator EndWith(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _validationEnd.Add(value);
                }
            }
            return this;
        }
        public FluentStringValidator MinLength(int length)
        {
            if (length > -1)
            {
                _minLength = length;
                _fixLength = null;
            }
            return this;
        }
        public FluentStringValidator MaxLength(int length)
        {
            if (length > -1)
            {
                _maxLength = length;
                _fixLength = null;
            }
            return this;
        }
        public FluentStringValidator FixLength(int length)
        {
            _minLength = null;
            _maxLength = null;
            _fixLength = length;
            return this;
        }
        public FluentStringValidator Include(params string[] substrings)
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
        public FluentStringValidator Exclude(params string[] substrings)
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
        public FluentStringValidator Slice(int start, string value)
        {
            _range.Add(Tuple.Create(start, value));
            return this;
        }
        public FluentStringValidator Regex(params string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                if (!string.IsNullOrEmpty(pattern))
                {
                    _regexPattern.Add(new Regex(pattern));
                }
            }
            return this;
        }

        public bool Validate(string input)
        {
            if (input == null)
                return false;

            if (_validationStart.Count != 0 && !_validationStart.Any(v => input.StartsWith(v)))
                return false;
            if (_validationEnd.Count != 0 && !_validationEnd.Any(v => input.EndsWith(v)))
                return false;
            if (_regexPattern.Count != 0 && !_regexPattern.Any(v => v.IsMatch(input)))
                return false;
            if (_range.Count != 0 && _range.Any(v => input.Substring(v.Item1, v.Item2.Length) != v.Item2))
                return false;
            if (_containsSubstrings.Count != 0 && _containsSubstrings.Any(s => !input.Contains(s)))
                return false;
            if (_excludeSubstrings.Count != 0 && _excludeSubstrings.Any(s => input.Contains(s)))
                return false;

            if (_minLength.HasValue && input.Length < _minLength.Value)
                return false;
            if (_maxLength.HasValue && input.Length > _maxLength.Value)
                return false;
            if (_fixLength.HasValue && input.Length != _fixLength.Value)
                return false;

            return true;
        }
    }
}