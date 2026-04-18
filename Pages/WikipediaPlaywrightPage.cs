using Microsoft.Playwright;

namespace WikipediaAutomation.Pages;

public class WikipediaPlaywrightPage : BasePage
{
    private const string Url = "https://en.wikipedia.org/wiki/Playwright_(software)";

    // Selectors
    private readonly ILocator _debuggingSection;
    private readonly ILocator _colorThemeButton;

    public WikipediaPlaywrightPage(IPage page) : base(page)
    {
        _debuggingSection = page.Locator("#Debugging_features").Locator("xpath=ancestor::h2");
        _colorThemeButton = page.Locator("#skin-client-pref-night-mode-value-2");
    }

    public async Task GoToAsync() => await NavigateToAsync(Url);

    /// <summary>
    /// Extracts the full text content of the "Debugging features" section via UI.
    /// </summary>
    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        // Find the h3 heading for "Debugging features"
        var headingLocator = Page.GetByRole(AriaRole.Heading, new() { Name = "Debugging features" });
        await headingLocator.WaitForAsync();

        // Collect all text following this heading until the next heading
        var sectionText = await Page.EvaluateAsync<string>(@"() => {
            const heading = Array.from(document.querySelectorAll('h1, h2, h3, h4, h5, h6'))
                                 .find(h => h.innerText.includes('Debugging features'));
            if (!heading) return '';

            let text = heading.innerText + '\n';
            // If heading is wrapped in a div (Vector 2022), start from the div's sibling
            let curr = (heading.parentElement.classList.contains('mw-heading')) 
                       ? heading.parentElement.nextElementSibling 
                       : heading.nextElementSibling;

            while (curr) {
                if (['H1', 'H2', 'H3', 'H4', 'H5', 'H6'].includes(curr.tagName) || curr.querySelector('h1, h2, h3, h4, h5, h6')) break;
                if (curr.classList.contains('mw-heading')) break;

                const clone = curr.cloneNode(true);
                clone.querySelectorAll('.mw-editsection, .reference, .noprint, .infobox, .navbox').forEach(el => el.remove());
                
                const content = clone.innerText.trim();
                if (content) {
                    text += content + '\n';
                }
                curr = curr.nextElementSibling;
            }
            return text.trim();
        }");

        return sectionText ?? string.Empty;
    }

    /// <summary>
    /// Returns all technology name elements inside the Microsoft development tools navbox at the bottom.
    /// </summary>
    public async Task<IReadOnlyList<IElementHandle>> GetMicrosoftDevToolsTechElementsAsync()
    {
        // Look for the navbox with "Microsoft development tools"
        var handle = await Page.EvaluateHandleAsync(@"() => {
            const navbox = Array.from(document.querySelectorAll('.navbox')).find(nb => nb.innerText.includes('Microsoft development tools'));
            if (!navbox) return [];
            // Return all list items or links in this navbox
            return Array.from(navbox.querySelectorAll('li'));
        }");

        if (handle == null) return Array.Empty<IElementHandle>();
        var lengthProperty = await handle.GetPropertyAsync("length");
        var length = await lengthProperty.JsonValueAsync<int>();

        var list = new List<IElementHandle>();
        for (int i = 0; i < length; i++)
        {
            var item = await handle.GetPropertyAsync(i.ToString()) as IElementHandle;
            if (item != null) list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Returns the anchor element for each technology item (null if no link).
    /// </summary>
    public async Task<List<(string Text, bool HasLink)>> GetMicrosoftDevToolsLinkStatusAsync()
    {
        var rawResult = await Page.EvaluateAsync<string[]>(@"() => {
            const navbox = Array.from(document.querySelectorAll('.navbox')).find(nb => nb.innerText.includes('Microsoft development tools'));
            if (!navbox) return [];

            return Array.from(navbox.querySelectorAll('li')).map(li => {
                const text = li.innerText.trim();
                const hasLink = li.querySelector('a') !== null;
                return `${text}|${hasLink}`;
            });
        }");

        return rawResult?.Select(s => {
            var parts = s.Split('|');
            return (parts[0], parts[1] == "true");
        }).ToList() ?? new List<(string, bool)>();
    }

    /// <summary>
    /// Opens the color theme picker and selects "Dark".
    /// </summary>
    public async Task SetColorThemeToDarkAsync()
    {
        // Vector 2022 skin uses a specific appearance menu
        // 1. Open the appearance menu if it's not open
        var appearanceMenuButton = Page.Locator("#vector-appearance-dropdown-label");
        if (await appearanceMenuButton.IsVisibleAsync())
        {
            await appearanceMenuButton.ClickAsync();
        }
        else
        {
            // Fallback for different versions/skins
            var settingsButton = Page.Locator(".vector-appearance-toggle").Or(Page.Locator("#p-appearance-label"));
            if (await settingsButton.IsVisibleAsync())
            {
                await settingsButton.ClickAsync();
            }
        }

        await Page.WaitForTimeoutAsync(500);

        // 2. Select "Dark" theme (Night mode)
        var darkOption = Page.GetByRole(AriaRole.Radio, new() { Name = "Dark" })
                         .Or(Page.Locator("#skin-client-pref-night-mode-value-2"))
                         .Or(Page.Locator("input[value='night']"));
        
        await darkOption.ClickAsync();

        await Page.WaitForTimeoutAsync(1000);
    }

    /// <summary>
    /// Returns true if the page is currently in dark mode.
    /// </summary>
    public async Task<bool> IsDarkModeActiveAsync()
    {
        var isDark = await Page.EvaluateAsync<bool>(@"() => {
            return document.documentElement.classList.contains('skin-night-mode')
                || document.documentElement.getAttribute('data-night-mode') === '1'
                || document.body.classList.contains('skin-night-mode')
                || document.querySelector('html').style.colorScheme === 'dark';
        }");

        if (!isDark)
        {
            // Also check background color as fallback
            var bgColor = await Page.EvaluateAsync<string>(@"() =>
                window.getComputedStyle(document.body).backgroundColor");
            // Dark mode typically has dark background (low RGB values)
            isDark = IsDarkBackground(bgColor);
        }

        return isDark;
    }

    private static bool IsDarkBackground(string? rgbColor)
    {
        if (string.IsNullOrWhiteSpace(rgbColor)) return false;
        // Parse rgb(R, G, B)
        var match = System.Text.RegularExpressions.Regex.Match(rgbColor, @"rgb\((\d+),\s*(\d+),\s*(\d+)\)");
        if (!match.Success) return false;
        var r = int.Parse(match.Groups[1].Value);
        var g = int.Parse(match.Groups[2].Value);
        var b = int.Parse(match.Groups[3].Value);
        var luminance = (0.299 * r + 0.587 * g + 0.114 * b);
        return luminance < 128;
    }

    private record TechLinkInfo(string Text, bool HasLink);
}
