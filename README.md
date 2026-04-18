# Wikipedia Automation Framework

Этот проект представляет собой легковесный фреймворк для автоматизации тестирования на языке C# с использованием Microsoft Playwright. Целью тестирования является страница Википедии о [Playwright](https://en.wikipedia.org/wiki/Playwright_(software)).

## Описание проекта

Фреймворк построен по классической архитектуре **Page Object Model (POM)** и предназначен для выполнения трех основных задач (кейсов):
1. **Сравнение количества слов:** Автоматическое сравнение уникальных слов в секции "Debugging features", полученных через пользовательский интерфейс (UI) и через MediaWiki API.
2. **Проверка ссылок:** Валидация того, что все технологии в навигационном блоке "Microsoft development tools" являются кликабельными ссылками.
3. **Темный режим:** Проверка корректности переключения сайта в темную тему оформления через настройки внешнего вида Википедии.

## Архитектура

```
WikipediaAutomation/
├── Pages/
│   ├── BasePage.cs                   # Базовый класс для навигации
│   └── WikipediaPlaywrightPage.cs    # Page Object для статьи Википедии (локаторы и действия)
├── Helpers/
│   ├── TextNormalizer.cs             # Нормализация текста и подсчет уникальных слов
│   ├── WikipediaApiClient.cs         # Клиент для работы с MediaWiki API
│   └── ReportManager.cs              # Настройка HTML-отчетов ExtentReports
├── Tests/
│   ├── BaseTest.cs                   # Базовый класс тестов (Setup/Teardown, скриншоты при ошибках)
│   ├── Task1_WordCountComparisonTest.cs
│   ├── Task2_MicrosoftDevToolsLinksTest.cs
│   └── Task3_DarkModeTest.cs
└── WikipediaAutomation.csproj        # Файл проекта с зависимостями
```

## Основные возможности
- **ExtentReports 5:** Генерация наглядных HTML-отчетов с результатами тестов.
- **Скриншоты:** Автоматическое создание скриншотов при падении тестов.
- **Кросс-браузерность:** Поддержка Chromium, Firefox и WebKit (по умолчанию используется Chromium).
- **MediaWiki API:** Прямое взаимодействие с API Википедии для верификации данных.

---

## Как запустить тесты

### 1. Подготовка окружения
Убедитесь, что у вас установлен .NET SDK 8.0 или выше.
```powershell
dotnet restore
dotnet build
```

### 2. Установка браузеров Playwright
```powershell
dotnet playwright install
```

### 3. Запуск тестов
```powershell
dotnet test
```

Результаты тестов и отчет будут доступны в папке:
`bin/Debug/net8.0/Reports/TestReport.html`

---

## Стек технологий
- **C# / .NET 8**
- **NUnit 3** – тестовый движок
- **Microsoft Playwright** – автоматизация браузера
- **ExtentReports 5** – отчетность
- **Newtonsoft.Json** – работа с JSON API
