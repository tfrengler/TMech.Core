using NUnit.Framework;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TMech.Core;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class ElementFactoryTests
    {

        private const string Category_Exists = "ElementFactory Exists";
        [TestCase(Category=Category_Exists)]
        public void Fetch_WebdriverContext_Exists()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            
            Stopwatch Timer = Stopwatch.StartNew();
            bool Exists = ElementFactory.Exists(By.Id(JSElements.Context1Div1));
            Timer.Stop();

            Webdriver.Quit();
            Assert.IsTrue(Exists);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to determine that the element exists");
        }

        [TestCase(Category=Category_Exists)]
        public void Fetch_WebdriverContext_NotExists()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            
            Stopwatch Timer = Stopwatch.StartNew();
            bool Exists = ElementFactory.Exists(By.Id("gnargle"));
            Timer.Stop();

            Webdriver.Quit();
            Assert.IsFalse(Exists);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to determine that the element does not exist");
        }

        private const string Category_Fetch = "ElementFactory Fetch";
        [TestCase(Category=Category_Fetch)]
        public void Fetch_WebdriverContext_ImmediatelyAvailable()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            Element? TestElement = null;
            Stopwatch Timer = Stopwatch.StartNew();

            Assert.DoesNotThrow(()=>
            {
                TestElement = ElementFactory.Fetch(By.Id("Context1"));
            });

            Timer.Stop();
            Webdriver.Quit();
            Assert.NotNull(TestElement);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to fetch the element!");
        }

        [TestCase(Category=Category_Fetch)]
        public void Fetch_WebdriverContext_DelayedAvailability()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            Element? TestElement = null;
            Stopwatch Timer = null;

            Webdriver.ExecuteAsyncScript("arguments[arguments.length - 1]();await Wait(3000);CopyLastChildOfParentAndAppend(Elements.Context1());;");

            Timer = Stopwatch.StartNew();

            Assert.DoesNotThrow(()=>
            {
                TestElement = ElementFactory.Fetch(By.Id("Context1-Div3-Id0"));
            });

            Timer.Stop();

            Webdriver.Quit();
            Assert.NotNull(TestElement);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds to fetch the element!");
        }

        [TestCase(Category=Category_Fetch)]
        public void Fetch_WebdriverContext_NotFound()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            Element? TestElement = null;

            Assert.Throws(typeof(NoSuchElementException), ()=> {
                TestElement = ElementFactory.Fetch(By.Id("DoesNotExist"));
            });
            Webdriver.Quit();
            Assert.IsNull(TestElement);
        }

        private const string Category_TryFetch_WebdriverContext = "ElementFactory TryFetch WebdriverContext";
        [TestCase(Category=Category_TryFetch_WebdriverContext)]
        public void TryFetch_WebdriverContext_ImmediatelyAvailable()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetch(By.Id("Context1-Div3-Id"), out Element? TestElement, out var Error);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to fetch the element!");
        }

        [TestCase(Category=Category_TryFetch_WebdriverContext)]
        public void TryFetch_WebdriverContext_DelayedAvailability()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript("arguments[arguments.length - 1]();await Wait(3000);CopyLastChildOfParentAndAppend(Elements.Context1());;");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetch(By.Id("Context1-Div3-Id0"), out Element? TestElement, out var Error);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds to fetch the element!");
        }

        [TestCase(Category=Category_TryFetch_WebdriverContext)]
        public void TryFetch_WebdriverContext_DelayedAvailability_CustomTimeout()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver, TimeSpan.FromSeconds(10.0d));

            Webdriver.ExecuteAsyncScript("arguments[arguments.length - 1]();await Wait(9000);CopyLastChildOfParentAndAppend(Elements.Context1());;");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetch(By.Id("Context1-Div3-Id0"), out Element? TestElement, out var Error);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success, "Expected the element to exist after 9 seconds!");
            Assert.NotNull(TestElement);
            Assert.Greater(Timer.ElapsedMilliseconds, 8800, "Expected it to take around 9 seconds to fetch the element!");
        }

        [TestCase(Category=Category_TryFetch_WebdriverContext)]
        public void TryFetch_WebdriverContext_NotFound()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetch(By.Id("DoesNotExist"), out Element? TestElement, out var Error);
            Timer.Stop();

            Webdriver.Quit();

            Assert.False(Success);
            Assert.Null(TestElement);
            Assert.NotNull(Error);
            Assert.AreEqual(typeof(NoSuchElementException), Error.SourceException.GetType());
            Assert.Greater(Timer.ElapsedMilliseconds, 4800, "Expected it to take at least 5 seconds before the fetch failed, since that is the default");
        }

        private const string Category_TryFetch_ElementContext = "ElementFactory TryFetch ElementContext";
        [TestCase(Category=Category_TryFetch_ElementContext)]
        public void TryFetch_ElementContext_ImmediatelyAvailable()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            bool ParentSuccess = ElementFactory.TryFetch(By.Id("Context1"), out Element? ParentElement, out var ParentError);
            Assert.True(ParentSuccess);
            Assert.NotNull(ParentElement);

            var Timer = Stopwatch.StartNew();
            bool ChildSuccess = ParentElement.Elements().TryFetch(By.Id("Context1-Div3-Id"), out Element? ChildElement, out var ChildError);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(ChildSuccess);
            Assert.NotNull(ChildElement);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to fetch the child element!");
        }

        [TestCase(Category=Category_TryFetch_ElementContext)]
        public void TryFetch_ElementContext_NotFound()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            bool ParentSuccess = ElementFactory.TryFetch(By.Id("Context3"), out Element? ParentElement, out var ParentError);
            Assert.True(ParentSuccess);
            Assert.NotNull(ParentElement);

            var Timer = Stopwatch.StartNew();
            bool ChildSuccess = ParentElement.Elements().TryFetch(By.Id("Context1-Div3-Id"), out Element? ChildElement, out var ChildError);
            Timer.Stop();

            Webdriver.Quit();

            Assert.False(ChildSuccess);
            Assert.Null(ChildElement);
            Assert.NotNull(ChildError);
            Assert.AreEqual(typeof(NoSuchElementException), ChildError.SourceException.GetType());
            Assert.Greater(Timer.ElapsedMilliseconds, 4800, "Expected it to take at least 5 seconds before the fetch failed, since that is the default");
        }

        private const string Category_TryFetchAll = "ElementFactory TryFetchAll";
        [TestCase(Category=Category_TryFetchAll)]
        public void TryFetchAll_WebdriverContext_ImmediatelyAvailable()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchAll(By.Id("Context3-Div3-Id"), out Element[] TestElements);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.Greater(TestElements.Length, 0);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to fetch the element!");
        }

        [TestCase(Category=Category_TryFetchAll)]
        public void TryFetchAll_ElementContext_ImmediatelyAvailable()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            bool ParentSuccess = ElementFactory.TryFetchAll(By.Id("Context3"), out Element[] ParentElements);
            Assert.True(ParentSuccess);
            Assert.Greater(ParentElements.Length, 0);

            var Timer = Stopwatch.StartNew();
            bool ChildrenSuccess = ParentElements[0].Elements().TryFetchAll(By.CssSelector("[id^='Context3-']"), out Element[] ChildElements);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(ChildrenSuccess);
            Assert.AreEqual(ChildElements.Length, 3);
            Assert.Less(Timer.ElapsedMilliseconds, 300, "Expected it to be more or less instant to fetch the child elements!");
        }

        [TestCase(Category=Category_TryFetchAll)]
        public void TryFetchAll_WebdriverContext_DelayedAvailability()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(2000);
                CopyLastChildOfParentAndAppend(Elements.Context3());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchAll(By.Id("Context3-Div3-Id0"), out Element[] TestElements);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.AreEqual(TestElements.Length, 1, "Expected 1 elements because we added it to the DOM!");
            Assert.Greater(Timer.ElapsedMilliseconds, 1800, "Expected it to take around 2 seconds to fetch the elements!");
        }

        [TestCase(Category=Category_TryFetchAll)]
        public void TryFetchAll_WebdriverContext_CustomThreshold()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(1000);
                CopyLastChildOfParentAndAppend(Elements.Context3());
                await Wait(1000);
                CopyLastChildOfParentAndAppend(Elements.Context3());
                await Wait(1000);
                CopyLastChildOfParentAndAppend(Elements.Context3());
            ");

            int ExpectedElementAmount = 4;
            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchAll(By.CssSelector("[id^='Context3-Div3-Id']"), out Element[] TestElements, (uint)ExpectedElementAmount);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.AreEqual(TestElements.Length, ExpectedElementAmount, $"Expected {ExpectedElementAmount} elements because we added 2 beyond the original 1!");
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds to fetch the elements!");
        }
    }
}