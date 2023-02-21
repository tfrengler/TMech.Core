using System;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TMech.Core;

namespace Code
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var Webdriver = new ChromeDriver(@"C:\Temp\CVS\Webdrivers");
            Webdriver.Manage().Window.Maximize();

            try
            {
                string? ExecutingLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("ExecutingLocation: " + ExecutingLocation);
                string TestPageURL = "file:///" + new FileInfo(ExecutingLocation + @"\TestPage.html").FullName;
                Console.WriteLine("TestPageURL: " + TestPageURL);

                var ElementsFactory = new ElementFactory(Webdriver);
                Webdriver.Navigate().GoToUrl(TestPageURL);

                var Timer = Stopwatch.StartNew();
                var Test = ElementsFactory.Fetch(By.Id("Context2-Button-Id")).GetAttributes();
                Console.WriteLine($"Time taken: " + Timer.Elapsed);

                Console.WriteLine($"Attributes ({Test.Count}):");
                foreach(var Current in Test)
                    Console.WriteLine($"-| {Current.Key} - {Current.Value}");

            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                Webdriver?.Quit();
            }

            return 0;
        }
    }
}