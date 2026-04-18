# Wikipedia Automation Framework

A lightweight test automation framework built with C# and Microsoft Playwright, targeting the [Playwright (software)](https://en.wikipedia.org/wiki/Playwright_(software)) Wikipedia article.

## Project Overview

The framework follows the **Page Object Model (POM)** pattern and covers three main automation tasks:
1. **Word Count Comparison:** Compares unique word counts in the "Debugging features" section extracted via UI vs. MediaWiki API.
2. **Link Validation:** Validates that all items under the "Microsoft development tools" section are clickable hyperlinks.
3. **Dark Mode Verification:** Automates switching the site to "Dark" mode and verifies the visual change.

## Architecture

```
WikipediaAutomation/
├── Pages/
│   ├── BasePage.cs                   # Navigation base class
│   └── WikipediaPlaywrightPage.cs    # POM for the Wikipedia article (locators & actions)
├── Helpers/
│   ├── TextNormalizer.cs             # Text processing & unique word counting
│   ├── WikipediaApiClient.cs         # MediaWiki Parse API client
│   └── ReportManager.cs              # ExtentReports HTML reporting setup
├── Tests/
│   ├── BaseTest.cs                   # Test base (Setup/Teardown, screenshots on failure)
│   ├── Task1_WordCountComparisonTest.cs
│   ├── Task2_MicrosoftDevToolsLinksTest.cs
│   └── Task3_DarkModeTest.cs
└── WikipediaAutomation.csproj        # Project file with dependencies
```

## Key Features
- **ExtentReports 5:** Generates comprehensive HTML reports.
- **Auto-Screenshots:** Automatically captures screenshots on test failures.
- **Cross-Browser:** Supports Chromium, Firefox, and WebKit (Chromium used by default).
- **API Integration:** Direct interaction with MediaWiki API for data verification.

---

## How to Run the Tests

### 1. Prerequisites
Ensure you have the .NET SDK 8.0 or newer installed.
```powershell
dotnet restore
dotnet build
```

### 2. Install Playwright Browsers
```powershell
dotnet playwright install
```

### 3. Execute Tests
```powershell
dotnet test
```

Test results and reports are generated at:
`bin/Debug/net8.0/Reports/TestReport.html`

---

## Tech Stack
- **C# / .NET 8**
- **NUnit 3** – Test Engine
- **Microsoft Playwright** – Browser Automation
- **ExtentReports 5** – Reporting
- **Newtonsoft.Json** – API JSON Handling
