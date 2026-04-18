using Microsoft.Playwright;

namespace WikipediaAutomation.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    public async Task NavigateToAsync(string url)
    {
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }
}
