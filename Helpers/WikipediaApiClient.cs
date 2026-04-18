using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace WikipediaAutomation.Helpers;

public static class WikipediaApiClient
{
    private static readonly HttpClient HttpClient = new();

    static WikipediaApiClient()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "WikipediaAutomationBot/1.0 (contact: admin@example.com)");
    }

    private const string ApiUrl =
        "https://en.wikipedia.org/w/api.php" +
        "?action=parse&page=Playwright_(software)&prop=wikitext&section=0&format=json";

    /// <summary>
    /// Fetches the "Debugging features" section text via the MediaWiki Parse API.
    /// </summary>
    public static async Task<string> GetDebuggingFeaturesSectionAsync()
    {
        // Use sections API to find section index
        var sectionsUrl =
            "https://en.wikipedia.org/w/api.php" +
            "?action=parse&page=Playwright_(software)&prop=sections&format=json";

        var sectionsResponse = await HttpClient.GetStringAsync(sectionsUrl);
        var sectionsJson = JObject.Parse(sectionsResponse);

        var sections = sectionsJson["parse"]?["sections"] as JArray
                       ?? throw new InvalidOperationException("Could not retrieve sections.");

        // Find "Debugging features" section
        var debugSection = sections
            .FirstOrDefault(s => s["line"]?.ToString()
                .Contains("Debugging features", StringComparison.OrdinalIgnoreCase) == true);

        if (debugSection == null)
            throw new InvalidOperationException("Section 'Debugging features' not found via API.");

        var sectionIndex = debugSection["index"]?.ToString()
                           ?? throw new InvalidOperationException("Section index not found.");

        // Fetch section wikitext
        var sectionUrl =
            $"https://en.wikipedia.org/w/api.php" +
            $"?action=parse&page=Playwright_(software)&prop=wikitext&section={sectionIndex}&format=json";

        var sectionResponse = await HttpClient.GetStringAsync(sectionUrl);
        var sectionJson = JObject.Parse(sectionResponse);

        var wikitext = sectionJson["parse"]?["wikitext"]?["*"]?.ToString()
                       ?? throw new InvalidOperationException("Wikitext not found.");

        return StripWikiMarkup(wikitext);
    }

    /// <summary>
    /// Strips wiki markup from raw wikitext to leave plain text.
    /// </summary>
    private static string StripWikiMarkup(string wikitext)
    {
        // Remove templates {{...}}
        var text = Regex.Replace(wikitext, @"\{\{[^}]*\}\}", " ");

        // Remove [[File:...]] and [[Image:...]]
        text = Regex.Replace(text, @"\[\[(File|Image):[^\]]*\]\]", " ",
            RegexOptions.IgnoreCase);

        // Replace [[link|display]] with display text
        text = Regex.Replace(text, @"\[\[(?:[^|\]]*\|)?([^\]]+)\]\]", "$1");

        // Remove external links [url text] → text
        text = Regex.Replace(text, @"\[https?://[^\s\]]+\s+([^\]]+)\]", "$1");
        text = Regex.Replace(text, @"\[https?://[^\]]+\]", " ");

        // Remove bold/italic markup
        text = text.Replace("'''", "").Replace("''", "");

        // Remove headings markers
        text = Regex.Replace(text, @"={2,}([^=]+)={2,}", "$1");

        // Remove HTML tags
        text = Regex.Replace(text, @"<[^>]+>", " ");

        // Remove ref tags and their content
        text = Regex.Replace(text, @"<ref[^/]*/?>.*?</ref>", " ",
            RegexOptions.Singleline);
        text = Regex.Replace(text, @"<ref\s*/?>", " ");

        // Collapse whitespace
        text = Regex.Replace(text, @"\s+", " ").Trim();

        return text;
    }
}
