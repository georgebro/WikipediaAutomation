using NUnit.Framework;
using WikipediaAutomation.Helpers;
using WikipediaAutomation.Pages;

namespace WikipediaAutomation.Tests;

/// <summary>
/// Task 1: Extract "Debugging features" section via UI and API,
/// normalize both texts, count unique words, and assert counts are equal.
/// </summary>
[TestFixture]
[Category("Task1")]
public class Task1_WordCountComparisonTest : BaseTest
{
    private WikipediaPlaywrightPage _wikiPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _wikiPage = new WikipediaPlaywrightPage(Page);
        await _wikiPage.GoToAsync();
    }

    [Test]
    [Description("Unique word count from UI and API extractions of 'Debugging features' must match")]
    public async Task DebuggingSection_UniqueWordCount_ShouldBeEqualFromUIAndAPI()
    {
        // ── UI extraction ────────────────────────────────────────────────
        TestReport?.Info("Extracting 'Debugging features' section text via UI...");
        var uiText = await _wikiPage.GetDebuggingFeaturesSectionTextAsync();

        Assert.That(uiText, Is.Not.Empty,
            "UI text extraction returned empty content.");

        TestReport?.Info($"UI text length: {uiText.Length} characters");

        // ── API extraction ───────────────────────────────────────────────
        TestReport?.Info("Fetching 'Debugging features' section text via MediaWiki API...");
        var apiText = await WikipediaApiClient.GetDebuggingFeaturesSectionAsync();

        Assert.That(apiText, Is.Not.Empty,
            "API text extraction returned empty content.");

        TestReport?.Info($"API text length: {apiText.Length} characters");

        // ── Normalize & count ────────────────────────────────────────────
        var uiWordCount  = TextNormalizer.CountUniqueWords(uiText);
        var apiWordCount = TextNormalizer.CountUniqueWords(apiText);

        TestReport?.Info($"UI  unique word count  : {uiWordCount}");
        TestReport?.Info($"API unique word count  : {apiWordCount}");

        // Log the sets for diagnostics
        var uiWords  = TextNormalizer.GetUniqueWords(uiText);
        var apiWords = TextNormalizer.GetUniqueWords(apiText);

        var onlyInUI  = uiWords.Except(apiWords).OrderBy(w => w).ToList();
        var onlyInAPI = apiWords.Except(uiWords).OrderBy(w => w).ToList();

        if (onlyInUI.Any())
            TestReport?.Info($"Words only in UI  ({onlyInUI.Count}): {string.Join(", ", onlyInUI.Take(20))}");
        if (onlyInAPI.Any())
            TestReport?.Info($"Words only in API ({onlyInAPI.Count}): {string.Join(", ", onlyInAPI.Take(20))}");

        // ── Assert ───────────────────────────────────────────────────────
        Assert.That(uiWordCount, Is.EqualTo(apiWordCount),
            $"Unique word count mismatch: UI={uiWordCount}, API={apiWordCount}.\n" +
            $"Only in UI: {string.Join(", ", onlyInUI)}\n" +
            $"Only in API: {string.Join(", ", onlyInAPI)}");
    }
}
