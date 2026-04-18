using System.Text.RegularExpressions;

namespace WikipediaAutomation.Helpers;

public static class TextNormalizer
{
    /// <summary>
    /// Normalizes text and returns a sorted set of unique words.
    /// Steps: lowercase → remove punctuation → split on whitespace → filter empties → deduplicate.
    /// </summary>
    public static HashSet<string> GetUniqueWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new HashSet<string>();

        // Lowercase
        var lower = text.ToLowerInvariant();

        // Remove all characters that are not letters, digits or whitespace
        var clean = Regex.Replace(lower, @"[^a-z0-9\s]", " ");

        // Split and remove empty entries
        var words = clean
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 0)
            .ToHashSet();

        return words;
    }

    /// <summary>
    /// Count unique words in the text.
    /// </summary>
    public static int CountUniqueWords(string text) => GetUniqueWords(text).Count;
}
