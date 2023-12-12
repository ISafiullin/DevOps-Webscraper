using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

class Program
{
    static void Main()
    {
        // Assuming chromedriver.exe is one directory higher than the application
        var driverPath = @"D:\school 23-24\devops\chromedriver.exe";
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("start-maximized"); // Full window
        options.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        var driver = new ChromeDriver(driverPath, options);

        try
        {
            // YouTube scraping
            var youtubeSearchTerm = GetUserInput("Enter YouTube search term:");
            var youtubeData = ScrapeYouTube(driver, youtubeSearchTerm);
            WriteToCSV("YouTubeData.csv", youtubeData);
            WriteToJson("YouTubeData.json", youtubeData);

            // ICTJob scraping
            var ictJobSearchTerm = GetUserInput("Enter ICTJob search term:");
            var ictJobData = ScrapeICTJob(driver, ictJobSearchTerm);
            WriteToCSV("ICTJobData.csv", ictJobData);
            WriteToJson("ICTJobData.json", ictJobData);

            // User-selected site scraping
            var userSiteUrl = GetUserInput("Enter the URL of the user-selected site:");
            var userSiteSearchTerm = GetUserInput("Enter the search term for the user-selected site:");
            var userSiteData = ScrapeUserSite(driver, userSiteUrl, userSiteSearchTerm);
            WriteToCSV("UserSiteData.csv", userSiteData);
            WriteToJson("UserSiteData.json", userSiteData);
        }
        finally
        {
            // Close the browser
            driver.Quit();
        }
    }

    static string GetUserInput(string prompt)
    {
        Console.Write(prompt + " ");
        return Console.ReadLine();
    }

    static void AcceptCookies(IWebDriver driver)
    {
        try
        {
            // Wait for the cookie pop-up to appear (you may need to adjust the time based on your internet speed)
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementExists(By.Id("cookie-banner")));

            // Find the "Accept All Cookies" button and click it
            var acceptButton = driver.FindElement(By.CssSelector("#cookie-banner #button"));
            acceptButton.Click();
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Error handling cookies: {ex.Message}");
        }
    }

    static List<Dictionary<string, string>> ScrapeYouTube(IWebDriver driver, string searchTerm)
    {
        //YouTube scraping

        // Navigate to YouTube, search for videos, and retrieve data
        driver.Navigate().GoToUrl("https://www.youtube.com/");

        // Accept cookies and close pop-up
        AcceptCookies(driver);

        // Fill in user input in search bar youtube
        var searchBox = driver.FindElement(By.Name("search_query"));
        searchBox.SendKeys(searchTerm);

        // Click on search button
        var searchButton = driver.FindElement(By.Id("search-icon-legacy"));
        searchButton.Click();

        // Wait 8000ms
        Thread.Sleep(8000);

        // Sample data
        var vidData = new List<Dictionary<string, string>>();

        var searchResults = driver.FindElements(By.CssSelector("ytd-video-renderer"));
        for (int i = 0; i < Math.Min(5, searchResults.Count); i++)
        {
            var video = searchResults[i];
            var title = video.FindElement(By.Id("video-title")).Text;
            var link = video.FindElement(By.Id("video-title")).GetAttribute("href");
            var uploader = video.FindElement(By.CssSelector("#byline a")).Text;
            var views = video.FindElement(By.CssSelector("#metadata-line span:nth-child(1)")).Text;

            var vidInfo = new Dictionary<string, string>
        {
            {"Link", link},
            {"Title", title},
            {"Uploader", uploader},
            {"Views", views}
        };

            vidData.Add(vidInfo);
        }

        return vidData;
    }

    static List<Dictionary<string, string>> ScrapeICTJob(IWebDriver driver, string searchTerm)
    {
        // Your ICTJob scraping
        // Navigate to ICTJob, search for jobs, and retrieve data

        // Sample data
        var ictJobData = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                {"Title", "Sample Job 1"},
                {"Company", "Sample Company 1"},
                {"Location", "Sample Location 1"},
                {"Keywords", "Sample Keywords 1"},
                {"Link", "https://www.ictjob.be/en/job/sample-job-1/12345"}
            },
            // Add data for other jobs
        };

        return ictJobData;
    }

    static List<Dictionary<string, string>> ScrapeUserSite(IWebDriver driver, string siteUrl, string searchTerm)
    {
        //user-selected site scraping

        // Navigate to the user-selected site, perform the search, and retrieve data

        // Sample data
        var userSiteData = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                {"CustomField1", "Sample Data 1"},
                {"CustomField2", "Sample Data 2"},
                {"Link", "https://www.example.com/sample-page"}
            },
            // Add data for other entries
        };

        return userSiteData;
    }

    static void WriteToCSV(string fileName, List<Dictionary<string, string>> data)
    {
        using (var writer = new StreamWriter(fileName))
        {
            // Write header
            var header = string.Join(",", data[0].Keys);
            writer.WriteLine(header);

            // Write data
            foreach (var entry in data)
            {
                var line = string.Join(",", entry.Values);
                writer.WriteLine(line);
            }
        }
    }

    static void WriteToJson(string fileName, List<Dictionary<string, string>> data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(fileName, json);
    }

}
