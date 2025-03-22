using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringCatcher
{
    public static IEnumerable<string> Between(string input, string start, string end)
    {
        var matches = Regex.Matches(input, $@"{start}(.*?){end}");
        foreach (Match match in matches)
        {
            yield return match.Groups[1].Value;
        }
    }

    public static IEnumerable<string> Like(string input, params string[] features)
    {
        string pattern = string.Join(".*?", Array.ConvertAll(features, Regex.Escape));
        var matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            yield return match.Value;
        }
    }

    public static IEnumerable<IEnumerable<string>> Hierarchical(string input)
    {
        var outermost = GetTopLevelBraces(input).FirstOrDefault();
        if (outermost == null)
            yield break;

        var currentLevel = new List<string> { outermost };
        while (true)
        {
            var levelElements = new List<string>();
            foreach (var s in currentLevel)
            {
                levelElements.AddRange(GetTopLevelBraces(s));
            }

            if (levelElements.Count == 0)
                break;

            yield return levelElements.Select(e => e.Trim());
            currentLevel = levelElements;
        }
    }

    private static IEnumerable<string> GetTopLevelBraces(string input)
    {
        var pattern = @"\{((?>[^{}]+|\{(?<Depth>)|}(?<-Depth>))*(?(Depth)(?!)))\}";
        var matches = Regex.Matches(input, pattern);
        foreach (Match match in matches)
        {
            if (match.Success)
            {
                yield return match.Groups[1].Value.Trim();
            }
        }
    }

    public static IEnumerable<string> Numbers(string input, int minLength = 1, int maxLength = int.MaxValue)
    {
        ValidateLengths(minLength, maxLength);
        string pattern = $@"\d{{{minLength},{maxLength}}}";
        return Regex.Matches(input, pattern).Cast<Match>().Select(m => m.Value);
    }

    public static IEnumerable<string> Words(string input, int minLength = 1, int maxLength = int.MaxValue)
    {
        ValidateLengths(minLength, maxLength);
        string pattern = $@"\b[A-Za-z]{{{minLength},{maxLength}}}\b";
        return Regex.Matches(input, pattern).Cast<Match>().Select(m => m.Value);
    }

    public static IEnumerable<string> Chinese(string input, int minLength = 1, int maxLength = int.MaxValue)
    {
        ValidateLengths(minLength, maxLength);
        string pattern = $@"[\u4e00-\u9fff]{{{minLength},{maxLength}}}";
        return Regex.Matches(input, pattern).Cast<Match>().Select(m => m.Value);
    }

    private static void ValidateLengths(int minLength, int maxLength)
    {
        if (minLength < 1)
            throw new ArgumentOutOfRangeException(nameof(minLength), "minLength must be at least 1.");
        if (maxLength < minLength)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength must be ≥ minLength.");
    }
}