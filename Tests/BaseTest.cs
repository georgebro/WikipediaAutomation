using AventStack.ExtentReports;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using WikipediaAutomation.Helpers;

namespace WikipediaAutomation.Tests;

[TestFixture]
public abstract class BaseTest : PageTest
{
    protected ExtentTest? TestReport;
    private static readonly ExtentReports Extent = ReportManager.GetInstance();

    public override BrowserNewContextOptions ContextOptions() =>
        new()
        {
            ViewportSize = new ViewportSize { Width = 1440, Height = 900 },
            Locale       = "en-US"
        };

    [SetUp]
    public void StartTest()
    {
        TestReport = Extent.CreateTest(TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public async Task EndTest()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        var message = TestContext.CurrentContext.Result.Message ?? string.Empty;

        if (outcome == TestStatus.Failed)
        {
            // Capture screenshot on failure
            var screenshotPath = Path.Combine(
                AppContext.BaseDirectory, "Reports", "Screenshots",
                $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);

            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });

            TestReport?.Fail(message)
                       .AddScreenCaptureFromPath(screenshotPath);
        }
        else if (outcome == TestStatus.Passed)
        {
            TestReport?.Pass("Test passed ✓");
        }
        else
        {
            TestReport?.Skip("Test skipped");
        }

        ReportManager.Flush();
    }
}
