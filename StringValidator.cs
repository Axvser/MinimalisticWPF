using System.Text.RegularExpressions;

namespace MinimalisticWPF
{
    public class StringValidator
    {
        private string? _start = default;
        private string? _end = default;
        private readonly HashSet<string> _containsSubstrings = [];
        private readonly HashSet<string> _excludeSubstrings = [];
        private Regex? _regexPattern = default;
        private readonly List<Tuple<int, string>> _range = [];
        private int? _minLength = default;
        private int? _maxLength = default;
        private int? _fixLength = default;
        private bool? _allowNull = false;
        private bool? _allowEmpty = false;
        private bool? _allowWhiteSpace = true;
        private HashSet<char>? _allowedChars = default;
        private HashSet<char>? _disallowedChars = default;
        private StringComparison _comparisonType = StringComparison.Ordinal;

        public StringValidator StartWith(string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _start = value;
                _comparisonType = comparisonType;
            }
            return this;
        }

        public StringValidator EndWith(string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _end = value;
                _comparisonType = comparisonType;
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
            if (!string.IsNullOrEmpty(value))
            {
                _range.Add(Tuple.Create(start, value));
            }
            return this;
        }

        public StringValidator Regex(string pattern, RegexOptions options = RegexOptions.None)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                _regexPattern = new Regex(pattern, options);
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

        public StringValidator AllowNull(bool allow = true)
        {
            _allowNull = allow;
            return this;
        }

        public StringValidator AllowEmpty(bool allow = true)
        {
            _allowEmpty = allow;
            return this;
        }

        public StringValidator AllowWhiteSpace(bool allow = true)
        {
            _allowWhiteSpace = allow;
            return this;
        }

        public StringValidator AllowedChars(params char[] chars)
        {
            _allowedChars ??= [];

            foreach (var c in chars)
            {
                _allowedChars.Add(c);
            }
            return this;
        }

        public StringValidator DisallowedChars(params char[] chars)
        {
            _disallowedChars ??= [];

            foreach (var c in chars)
            {
                _disallowedChars.Add(c);
            }
            return this;
        }

        public StringValidator CaseSensitive(bool caseSensitive = true)
        {
            _comparisonType = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return this;
        }

        public bool Validate(string input)
        {
            // Null check
            if (input == null)
                return _allowNull ?? false;

            // Empty check
            if (input.Length == 0)
                return _allowEmpty ?? false;

            // Whitespace check
            if (!(_allowWhiteSpace ?? true) && input.Any(char.IsWhiteSpace))
                return false;

            // Length validations
            if (_fixLength.HasValue && input.Length != _fixLength.Value)
                return false;

            if (_minLength.HasValue && _maxLength.HasValue &&
                (input.Length < _minLength.Value || input.Length > _maxLength.Value))
                return false;

            // Start/end checks
            if (_start != null && !input.StartsWith(_start, _comparisonType))
                return false;

            if (_end != null && !input.EndsWith(_end, _comparisonType))
                return false;

            // Contains/excludes checks
            if (_containsSubstrings.Count > 0 &&
                !_containsSubstrings.All(s => input.IndexOf(s, _comparisonType) >= 0))
                return false;

            if (_excludeSubstrings.Count > 0 &&
                _excludeSubstrings.Any(s => input.IndexOf(s, _comparisonType) >= 0))
                return false;

            // Slice checks
            if (_range.Count > 0)
            {
                foreach (var range in _range)
                {
                    if (range.Item1 < 0 || range.Item1 + range.Item2.Length > input.Length)
                        return false;

                    if (!string.Equals(
                        input.Substring(range.Item1, range.Item2.Length),
                        range.Item2,
                        _comparisonType))
                        return false;
                }
            }

            // Regex check
            if (_regexPattern != null && !_regexPattern.IsMatch(input))
                return false;

            // Allowed/disallowed chars checks
            if (_allowedChars != null && input.Any(c => !_allowedChars.Contains(c)))
                return false;

            if (_disallowedChars != null && input.Any(c => _disallowedChars.Contains(c)))
                return false;

            return true;
        }
    }
}