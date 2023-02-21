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
    public class ElementWaiterTests
    {
        private const string Category_State = "Waiter State";
        // [TestCase(Category=Category_State)]
        public void State_IsDisplayed()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                HideElement(Elements.Context1Div1());
                arguments[arguments.length - 1]();
                await Wait(3000);
                ShowElement(Elements.Context1Div1());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context1Div1)).IsDisplayed(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element to be displayed");
        }

        // [TestCase(Category=Category_State)]
        public void State_IsNotDisplayed()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                HideElement(Elements.Context1Div1());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context1Div1)).IsNotDisplayed(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element to become invisible");
        }

        // [TestCase(Category=Category_State)]
        public void State_IsEnabled()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                Disable(Elements.Context2Textarea());
                arguments[arguments.length - 1]();
                await Wait(3000);
                Enable(Elements.Context2Textarea());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2Textarea)).IsEnabled(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element to be enabled");
        }

        // [TestCase(Category=Category_State)]
        public void State_IsNotEnabled()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                Disable(Elements.Context2Textarea());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2Textarea)).IsNotEnabled(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element to be disabled");
        }

        [TestCase(Category=Category_State)]
        public void State_IsSelected()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                Deselect(Elements.Context2Radio2());
                arguments[arguments.length - 1]();
                await Wait(3000);
                Select(Elements.Context2Radio2());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2Radio2)).IsSelected(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element to be selected");
        }

        [TestCase(Category=Category_State)]
        public void State_IsNotSelected()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                Select(Elements.Context2Radio2());
                arguments[arguments.length - 1]();
                await Wait(3000);
                Deselect(Elements.Context2Radio2());
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2Radio2)).IsNotSelected(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element to be deselected");
        }


        private const string Category_Attributes = "Attributes";

        private const string Category_Content = "Content";
    }
}