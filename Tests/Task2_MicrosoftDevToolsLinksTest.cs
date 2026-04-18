using NUnit.Framework;
using WikipediaAutomation.Pages;

namespace WikipediaAutomation.Tests;

/// <summary>
/// Task 2: Validate that every technology name under "Microsoft development tools"
/// (inside the "Debugging features" section) is a clickable text link.
/// </summary>
[TestFixture]
[Category("Task2")]
public class Task2_MicrosoftDevToolsLinksTest : BaseTest
{
    private WikipediaPlaywrightPage _wikiPage = null!;
    private List<(string Text, bool HasLink)> _techItems = new();

    [SetUp]
    public async Task SetUp()
    {
        _wikiPage = new WikipediaPlaywrightPage(Page);
        await _wikiPage.GoToAsync();

        _techItems = await _wikiPage.GetMicrosoftDevToolsLinkStatusAsync();

        Assert.That(_techItems, Is.Not.Empty,
            "No items found under 'Microsoft development tools' section. " +
            "Please verify the section exists on the page.");

        TestReport?.Info($"Found {_techItems.Count} technology item(s) under 'Microsoft development tools'");

        foreach (var (text, hasLink) in _techItems)
            TestReport?.Info($"  • [{(hasLink ? "LINK ✓" : "NO LINK ✗")}] {text}");
    }

    /// <summary>
    /// One consolidated test: all technology names must be links.
    /// Fails with a detailed list of offending items.
    /// </summary>
    [Test]
    [Description("All technology names under 'Microsoft development tools' must be text links")]
    public void AllTechnologyNames_ShouldBeTextLinks()
    {
        var nonLinks = _techItems
            .Where(item => !item.HasLink)
            .Select(item => item.Text)
            .ToList();

        if (nonLinks.Any())
            TestReport?.Fail($"The following technologies are NOT links: {string.Join(", ", nonLinks)}");
        else
            TestReport?.Pass("All technology names are text links ✓");

        Assert.That(nonLinks, Is.Empty,
            $"The following technology names are NOT text links:\n" +
            string.Join("\n", nonLinks.Select(t => $"  - {t}")));
    }

    /// <summary>
    /// Parameterized variant: one test case per technology name.
    /// (Bonus approach – gives granular pass/fail per item in the report.)
    /// </summary>
    [Test]
    [Description("Each individual technology name should be a text link (parameterized)")]
    public void EachTechnologyName_ShouldBeATextLink()
    {
        var failures = new List<string>();

        foreach (var (text, hasLink) in _techItems)
        {
            if (!hasLink)
            {
                failures.Add(text);
                TestReport?.Warning($"'{text}' is not a link");
            }
        }

        Assert.Multiple(() =>
        {
            foreach (var item in _techItems)
            {
                Assert.That(item.HasLink, Is.True,
                    $"Technology '{item.Text}' is expected to be a text link, but it is not.");
            }
        });
    }
}
