using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace WikipediaAutomation.Helpers;

public static class ReportManager
{
    private static ExtentReports? _extent;
    private static readonly string ReportPath =
        Path.Combine(AppContext.BaseDirectory, "Reports", "TestReport.html");

    public static ExtentReports GetInstance()
    {
        if (_extent != null) return _extent;

        Directory.CreateDirectory(Path.GetDirectoryName(ReportPath)!);

        var reporter = new ExtentSparkReporter(ReportPath)
        {
            Config =
            {
                DocumentTitle = "Wikipedia Automation Report",
                ReportName    = "Playwright Article - Test Results",
                Theme         = AventStack.ExtentReports.Reporter.Config.Theme.Dark
            }
        };

        _extent = new ExtentReports();
        _extent.AttachReporter(reporter);
        _extent.AddSystemInfo("Tester", "QA Engineer");
        _extent.AddSystemInfo("Target", "https://en.wikipedia.org/wiki/Playwright_(software)");

        return _extent;
    }

    public static void Flush() => _extent?.Flush();
}
