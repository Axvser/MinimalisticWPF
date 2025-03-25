using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MinimalisticWPF
{
    public static class StringCatcher
    {
        #region 预编译正则表达式
        private static readonly Regex HierarchicalRegex = new(
            @"\{((?>[^{}]+|\{(?<Depth>)|}(?<-Depth>))*(?(Depth)(?!)))}",
            RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(500)
        );

        private static readonly Regex ChineseRegex = new(
            @"[\u4e00-\u9fff]+",
            RegexOptions.Compiled
        );

        private static readonly Regex WordsRegex = new(
            @"\b[A-Za-z]+\b",
            RegexOptions.Compiled
        );

        private static readonly Regex NumbersRegex = new(
            @"\d+",
            RegexOptions.Compiled
        );

        private static readonly Dictionary<string, Regex> DynamicRegexCache = [];
        private static readonly object CacheLock = new();
        #endregion

        #region 同步方法
        public static IEnumerable<string> Between(string input, string start, string end)
        {
            if (string.IsNullOrEmpty(input)) yield break;

            var pattern = $"{EscapeForRegex(start)}(.*?){EscapeForRegex(end)}";
            var regex = GetCachedRegex(pattern);

            foreach (Match match in regex.Matches(input))
            {
                if (match.Success)
                    yield return match.Groups[1].Value;
            }
        }

        public static IEnumerable<string> Like(string input, params string[] features)
        {
            if (features == null || features.Length == 0)
                throw new ArgumentException("At least one feature must be specified");

            string pattern = string.Join(".*?", features.Select(EscapeForRegex));
            var regex = GetCachedRegex(pattern);

            foreach (Match match in regex.Matches(input))
            {
                yield return match.Value;
            }
        }

        public static IEnumerable<IEnumerable<string>> Hierarchical(string input)
        {
            var currentLevel = GetTopLevelBraces(input).ToList();
            if (currentLevel.Count == 0) yield break;

            while (currentLevel.Count > 0)
            {
                yield return currentLevel.Select(s => s.Trim());
                currentLevel = [.. currentLevel.SelectMany(GetTopLevelBraces)];
            }
        }

        private static IEnumerable<string> GetTopLevelBraces(string input)
        {
            foreach (Match match in HierarchicalRegex.Matches(input))
            {
                if (match.Success)
                    yield return match.Groups[1].Value;
            }
        }

        public static IEnumerable<string> Numbers(string input, int minLength = 1, int maxLength = int.MaxValue)
        {
            ValidateLengths(minLength, maxLength);

            foreach (Match match in NumbersRegex.Matches(input))
            {
                if (match.Length >= minLength && match.Length <= maxLength)
                    yield return match.Value;
            }
        }

        public static IEnumerable<string> Words(string input, int minLength = 1, int maxLength = int.MaxValue)
        {
            ValidateLengths(minLength, maxLength);

            foreach (Match match in WordsRegex.Matches(input))
            {
                if (match.Length >= minLength && match.Length <= maxLength)
                    yield return match.Value;
            }
        }

        public static IEnumerable<string> Chinese(string input, int minLength = 1, int maxLength = int.MaxValue)
        {
            ValidateLengths(minLength, maxLength);

            foreach (Match match in ChineseRegex.Matches(input))
            {
                if (match.Length >= minLength && match.Length <= maxLength)
                    yield return match.Value;
            }
        }
        #endregion

        #region 异步方法
        public static async Task<IEnumerable<string>> BetweenAsync(
            TextReader reader,
            string start,
            string end,
            int bufferSize = 4096,
            CancellationToken token = default)
        {
            var result = new List<string>();
            var buffer = new char[bufferSize];
            var leftover = new StringBuilder();
            var pattern = $"{EscapeForRegex(start)}(.*?){EscapeForRegex(end)}";
            var regex = GetCachedRegex(pattern);

            while (!token.IsCancellationRequested)
            {
                var bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                var chunk = leftover.ToString() + new string(buffer, 0, bytesRead);
                leftover.Clear();

                foreach (Match match in regex.Matches(chunk))
                {
                    if (match.Success)
                        result.Add(match.Groups[1].Value);
                }

                // 处理跨块匹配
                var lastEnd = chunk.LastIndexOf(end, StringComparison.Ordinal);
                if (lastEnd != -1 && lastEnd < chunk.Length - end.Length)
                {
                    leftover.Append(chunk.Substring(lastEnd + end.Length));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<IEnumerable<string>>> HierarchicalAsync(
            string input,
            CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                var result = new List<List<string>>();
                var currentLevel = new ConcurrentBag<string>(GetTopLevelBraces(input));

                while (!currentLevel.IsEmpty && !token.IsCancellationRequested)
                {
                    var nextLevel = new ConcurrentBag<string>();
                    Parallel.ForEach(currentLevel, new ParallelOptions { CancellationToken = token }, str =>
                    {
                        foreach (var inner in GetTopLevelBraces(str))
                        {
                            if (!token.IsCancellationRequested)
                                nextLevel.Add(inner);
                        }
                    });

                    result.Add([.. nextLevel]);
                    currentLevel = nextLevel;
                }

                return result;
            }, token);
        }
        #endregion

        #region 辅助方法
        private static void ValidateLengths(int minLength, int maxLength)
        {
            if (minLength < 1)
                throw new ArgumentOutOfRangeException(nameof(minLength), "minLength must be at least 1");
            if (maxLength < minLength)
                throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength must be ≥ minLength");
        }

        private static Regex GetCachedRegex(string pattern)
        {
            lock (CacheLock)
            {
                if (!DynamicRegexCache.TryGetValue(pattern, out var regex))
                {
                    regex = new Regex(pattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));
                    DynamicRegexCache[pattern] = regex;
                }
                return regex;
            }
        }

        private static string EscapeForRegex(string input)
        {
            // .NET Framework兼容的Regex.Escape替代方案
            return Regex.Escape(input);
        }

        public static IEnumerable<string> NumbersSpan(string input, int minLength = 1, int maxLength = int.MaxValue)
        {
            // .NET Framework兼容的Span替代方案
            ValidateLengths(minLength, maxLength);

            for (var i = 0; i < input.Length; i++)
            {
                if (!char.IsDigit(input[i])) continue;

                var start = i;
                while (i < input.Length && char.IsDigit(input[i])) i++;

                var length = i - start;
                if (length >= minLength && length <= maxLength)
                    yield return input.Substring(start, length);
            }
        }
        #endregion
    }
}