using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

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
            var imdbSearchTerm = GetUserInput("Enter a IMDb search term:");
            var imdbData = ScrapeImdb(driver, imdbSearchTerm);
            WriteToCSV("ImbdData.csv", imdbData);
            WriteToJson("ImbdData.json", imdbData);
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

    static List<Dictionary<string, string>> ScrapeYouTube(IWebDriver driver, string searchTerm)
    {
        //YouTube scraping

        // Navigate to YouTube, search for videos, and retrieve data
        driver.Navigate().GoToUrl("https://www.youtube.com/");

        // Accept cookies and close pop-up
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        var acceptButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Akkoord gaan met het gebruik van cookies en andere gegevens voor de beschreven doeleinden']")));

        acceptButton.Click();

        Thread.Sleep(8000);

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
            var uploader = video.FindElement(By.CssSelector("#channel-name.ytd-video-renderer")).Text;
            var views = video.FindElement(By.CssSelector("div#metadata-line > span")).Text;

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
        // ICTJob scraping
        // Navigate to ICTJob, search for jobs, and retrieve data
        driver.Navigate().GoToUrl("https://www.ictjob.be/");

        // Navigate and fill in the search box
        var searchBox = driver.FindElement(By.Id("keywords-input"));
        searchBox.SendKeys(searchTerm);

        // Click on search button
        var searchButton = driver.FindElement(By.Id("main-search-button"));
        searchButton.Click();

        Thread.Sleep(4000);

        // Sample data
        var ictJobData = new List<Dictionary<string, string>>();

        var jobs = driver.FindElements(By.ClassName("search-result-list"));

        foreach (var job in jobs)
        {
            var jobInfoElements = job.FindElements(By.CssSelector("li.search-item span.job-info")).Take(5);

            foreach (var jobInfoElement in jobInfoElements)
            {
                var title = jobInfoElement.FindElement(By.CssSelector("a.job-title")).Text;
                var employer = jobInfoElement.FindElement(By.CssSelector("span.job-company")).Text;
                var location = jobInfoElement.FindElement(By.CssSelector("span.job-location")).Text;
                var keywords = jobInfoElement.FindElement(By.CssSelector("span.job-keywords")).Text;
                var link = jobInfoElement.FindElement(By.CssSelector("a.job-title")).GetAttribute("href");

                var jobInfo = new Dictionary<string, string>
                {
                    {"Title", title},
                    {"Employer", employer},
                    {"Location", location},
                    {"Keywords", keywords},
                    {"Link", link}
                };

                // Add the job information
                ictJobData.Add(jobInfo);
            }
        }


        return ictJobData;
    }

    static List<Dictionary<string, string>> ScrapeImdb(IWebDriver driver, string searchTerm)
    {
        // IMDb scraping

        // Navigate to IMDb
        driver.Navigate().GoToUrl("https://www.imdb.com/");

        // Accept cookies
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        var acceptButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-testid='accept-button']")));

        acceptButton.Click();

        // Search
        // Fill in user input in search bar youtube
        var searchBox = driver.FindElement(By.Name("q"));
        searchBox.SendKeys(searchTerm);

        // Click on search button
        var searchButton = driver.FindElement(By.Id("suggestion-search-button"));
        searchButton.Click();

        var imdbData = new List<Dictionary<string, string>>();

        var movies = driver.FindElements(By.XPath("//section[@data-testid='find-results-section-title']"));

        foreach (var movie in movies)
        {
           var movieElements = movie.FindElements(By.CssSelector("div.ipc-metadata-list-summary-item__tc"));

            foreach (var movieElement in movieElements)
            {
                var title = movieElement.FindElement(By.CssSelector("a")).Text;
                var link = movieElement.FindElement(By.CssSelector("a")).GetAttribute("href");
                var year = movieElement.FindElement(By.CssSelector("ul.ipc-metadata-list-summary-item__tl")).Text;

                var movieInfo = new Dictionary<string, string>
                {
                    {"Title", title},
                    {"Year", year},
                    {"Link", link}
                };

                imdbData.Add(movieInfo);
            }
            
        }
        return imdbData;
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
        System.IO.File.WriteAllText(fileName, json);
    }

}
