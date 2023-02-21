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
        #region STATE

        private const string Category_State = "Waiter State";
        [TestCase(Category=Category_State)]
        public void Element_State_NOK_Timeout_NoThrow()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver, TimeSpan.FromSeconds(3));

            Webdriver.ExecuteAsyncScript(@"
                HideElement(Elements.Context1Div1());
                arguments[arguments.length - 1]();
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context1Div1)).IsDisplayed(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.False(Success);
            Assert.Null(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the timeout to be reached");
        }

        [TestCase(Category=Category_State)]
        public void Element_State_NOK_Timeout_DoThrow()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver, TimeSpan.FromSeconds(3));

            Webdriver.ExecuteAsyncScript(@"
                HideElement(Elements.Context1Div1());
                arguments[arguments.length - 1]();
            ");

            var Timer = Stopwatch.StartNew();
            Assert.Throws<TimeoutException>(() => ElementFactory.TryFetchWhen(By.Id(JSElements.Context1Div1)).ThrowOnTimeout().IsDisplayed(out _, out _));
            Timer.Stop();

            Webdriver.Quit();

            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the timeout to be reached");
        }

        [TestCase(Category=Category_State)]
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

        [TestCase(Category=Category_State)]
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

        [TestCase(Category=Category_State)]
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

        [TestCase(Category=Category_State)]
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
        #endregion

        #region ATTRIBUTES

        private const string Category_Attributes = "Waiter Attributes";
        [TestCase(Category=Category_Attributes)]
        public void Attributes_IsEqual()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeAttribute(Elements.Context2InputText(), 'value', 'Attributes_IsEqual');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2InputText)).AttributeIsEqualTo(out ExceptionDispatchInfo? Error, out Element? TestElement, "value", "Attributes_IsEqual");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element attribute to be equal to value");
        }

        [TestCase(Category=Category_Attributes)]
        public void Attributes_Contain()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeAttribute(Elements.Context2InputText(), 'value', 'Attributes_Contain');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2InputText)).AttributeContains(out ExceptionDispatchInfo? Error, out Element? TestElement, "value", "es_Con");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element attribute to contain value");
        }

        [TestCase(Category=Category_Attributes)]
        public void Attributes_EndsWith()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeAttribute(Elements.Context2InputText(), 'value', 'Attributes_EndsWith_This_Value');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2InputText)).AttributeEndsWith(out ExceptionDispatchInfo? Error, out Element? TestElement, "value", "_This_Value");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element attribute to end with value");
        }

        [TestCase(Category=Category_Attributes)]
        public void Attributes_StartWith()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeAttribute(Elements.Context2InputText(), 'value', 'Attributes_StartWith_This_Value');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context2InputText)).AttributeStartsWith(out ExceptionDispatchInfo? Error, out Element? TestElement, "value", "Attributes_StartWith");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element attribute to start with value");
        }

        #endregion

        #region CONTENT

        private const string Category_Content = "Waiter Content";
        [TestCase(Category=Category_Content)]
        public void Content_IsEqual()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeText(Elements.Context3Div2(), 'Content_IsEqual');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context3Div2)).ContentIsEqualTo(out ExceptionDispatchInfo? Error, out Element? TestElement, "Content_IsEqual");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content to be equal to");
        }

        [TestCase(Category=Category_Content)]
        public void Content_Contains()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeText(Elements.Context3Div2(), 'Content_Contains');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context3Div2)).ContentContains(out ExceptionDispatchInfo? Error, out Element? TestElement, "nt_Co");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content to contain");
        }

        [TestCase(Category=Category_Content)]
        public void Content_EndsWith()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeText(Elements.Context3Div2(), 'Content_EndsWith_This_Value');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context3Div2)).ContentEndsWith(out ExceptionDispatchInfo? Error, out Element? TestElement, "_This_Value");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content to end with");
        }

        [TestCase(Category=Category_Content)]
        public void Content_StartWith()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeText(Elements.Context3Div2(), 'Content_StartsWith_This_Value');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context3Div2)).ContentStartsWith(out ExceptionDispatchInfo? Error, out Element? TestElement, "Content_StartsWith");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content to start with");
        }

        [TestCase(Category=Category_Content)]
        public void Content_NotEqualTo()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                ChangeText(Elements.Context3Div2(), 'Content_NotEqualTo');
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeText(Elements.Context3Div2(), 'SomethingElse');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context3Div2)).ContentIsNotEqualTo(out ExceptionDispatchInfo? Error, out Element? TestElement, "Content_NotEqualTo");
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content not to be equal to");
        }

        [TestCase(Category=Category_Content)]
        public void Content_Exists()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                ChangeText(Elements.Context3Div2(), '');
                arguments[arguments.length - 1]();
                await Wait(3000);
                ChangeText(Elements.Context3Div2(), 'Content_Exists');
            ");

            var Timer = Stopwatch.StartNew();
            bool Success = ElementFactory.TryFetchWhen(By.Id(JSElements.Context3Div2)).HasContent(out ExceptionDispatchInfo? Error, out Element? TestElement);
            Timer.Stop();

            Webdriver.Quit();

            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content to exist");
        }

        #endregion
    }
}