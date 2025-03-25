using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MinimalisticWPF
{
    public class StringValidator
    {
        private string? _start = null;
        private bool _startIgnoreCase = false;
        private string? _end = null;
        private bool _endIgnoreCase = false;
        private readonly HashSet<SubstringCondition> _containsSubstrings = [];
        private readonly HashSet<SubstringCondition> _excludeSubstrings = [];
        private Regex? _regexPattern = null;
        private readonly List<Tuple<int, string, bool>> _range = [];
        private int? _minLength = null;
        private int? _maxLength = null;
        private int? _fixLength = null;

        public StringValidator StartWith(string value, bool ignoreCase = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _start = value;
                _startIgnoreCase = ignoreCase;
            }
            return this;
        }

        public StringValidator EndWith(string value, bool ignoreCase = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _end = value;
                _endIgnoreCase = ignoreCase;
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
            return Include(false, substrings);
        }

        public StringValidator Include(bool ignoreCase, params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (!string.IsNullOrEmpty(substring))
                {
                    _containsSubstrings.Add(new SubstringCondition(substring, ignoreCase));
                }
            }
            return this;
        }

        public StringValidator Exclude(params string[] substrings)
        {
            return Exclude(false, substrings);
        }

        public StringValidator Exclude(bool ignoreCase, params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (!string.IsNullOrEmpty(substring))
                {
                    _excludeSubstrings.Add(new SubstringCondition(substring, ignoreCase));
                }
            }
            return this;
        }

        public StringValidator Slice(int start, string value, bool ignoreCase = false)
        {
            _range.Add(Tuple.Create(start, value, ignoreCase));
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

        public StringValidator OnlyWords()
        {
            _regexPattern = new Regex(@"^[a-zA-Z]+$");
            return this;
        }

        public bool Validate(string input)
        {
            if (_start != null)
            {
                var comparison = _startIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!input.StartsWith(_start, comparison))
                    return false;
            }

            if (_end != null)
            {
                var comparison = _endIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!input.EndsWith(_end, comparison))
                    return false;
            }

            if (_regexPattern != null && !_regexPattern.IsMatch(input))
                return false;

            foreach (var sliceCondition in _range)
            {
                int start = sliceCondition.Item1;
                string expected = sliceCondition.Item2;
                bool ignoreCase = sliceCondition.Item3;

                if (start < 0 || start + expected.Length > input.Length)
                    return false;

                string actual = input.Substring(start, expected.Length);
                var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!actual.Equals(expected, comparison))
                    return false;
            }

            foreach (var condition in _containsSubstrings)
            {
                bool found = input.IndexOf(condition.Value, condition.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
                if (!found)
                    return false;
            }

            foreach (var condition in _excludeSubstrings)
            {
                bool found = input.IndexOf(condition.Value, condition.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
                if (found)
                    return false;
            }

            if (_minLength != null && input.Length < _minLength)
                return false;
            if (_maxLength != null && input.Length > _maxLength)
                return false;
            if (_fixLength != null && input.Length != _fixLength)
                return false;

            return true;
        }

        private readonly struct SubstringCondition(string value, bool ignoreCase)
        {
            public string Value { get; } = value;
            public bool IgnoreCase { get; } = ignoreCase;
        }
    }
}