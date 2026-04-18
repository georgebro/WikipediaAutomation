using NUnit.Framework;
using WikipediaAutomation.Pages;

namespace WikipediaAutomation.Tests;

/// <summary>
/// Task 3: Navigate to the "Color (beta)" section from the right panel,
/// change the theme to "Dark", and validate the change took effect.
/// </summary>
[TestFixture]
[Category("Task3")]
public class Task3_DarkModeTest : BaseTest
{
    private WikipediaPlaywrightPage _wikiPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _wikiPage = new WikipediaPlaywrightPage(Page);
        await _wikiPage.GoToAsync();
    }

    [Test]
    [Description("Selecting 'Dark' in the Color (beta) panel should switch the page to dark mode")]
    public async Task SelectingDarkColor_ShouldActivateDarkMode()
    {
        // ── Verify starting state is NOT dark ───────────────────────────
        TestReport?.Info("Checking initial color theme...");
        var initiallyDark = await _wikiPage.IsDarkModeActiveAsync();
        TestReport?.Info($"Initially dark: {initiallyDark}");

        // ── Change to Dark ───────────────────────────────────────────────
        TestReport?.Info("Opening Color (beta) panel and selecting 'Dark'...");
        await _wikiPage.SetColorThemeToDarkAsync();

        // ── Assert dark mode is active ───────────────────────────────────
        TestReport?.Info("Validating dark mode is now active...");
        var isDarkAfterChange = await _wikiPage.IsDarkModeActiveAsync();

        if (isDarkAfterChange)
            TestReport?.Pass("Dark mode successfully activated ✓");
        else
            TestReport?.Fail("Dark mode did NOT activate after selecting 'Dark'");

        Assert.That(isDarkAfterChange, Is.True,
            "Expected the page to be in dark mode after selecting 'Dark' in the Color (beta) panel, " +
            "but the page appears to still be in light mode.");
    }
}
