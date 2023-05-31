using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using TMech.Core.Elements;
using TMech.Core.Exceptions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class ElementWaiterTests
    {
        #region STATE

        private const string Category_State = "Waiter State";
        [TestCase(Category=Category_State)]
        public void Element_State_NOK_Timeout()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver, TimeSpan.FromSeconds(3));

            Webdriver.ExecuteAsyncScript(@"
                HideElement(Elements.Context1Div1());
                arguments[arguments.length - 1]();
            ");

            var Timer = Stopwatch.StartNew();
            ElementWaitException Exception = Assert.Throws<ElementWaitException>(() => ElementFactory.FetchWhen(By.Id(JSElements.Context1Div1)).IsDisplayed());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(Exception);
            Assert.IsNull(Exception.InnerException);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context1Div1)).IsDisplayed());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context1Div1)).IsNotDisplayed());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2Textarea)).IsEnabled());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2Textarea)).IsNotEnabled());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2Radio2)).IsSelected());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2Radio2)).IsNotSelected());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2InputText)).AttributeIsEqualTo("value", "Attributes_IsEqual"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2InputText)).AttributeContains("value", "es_Con"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2InputText)).AttributeEndsWith("value", "_This_Value"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context2InputText)).AttributeStartsWith("value", "Attributes_StartWith"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context3Div2)).ContentIsEqualTo("Content_IsEqual"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context3Div2)).ContentContains("nt_Co"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context3Div2)).ContentEndsWith("_This_Value"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context3Div2)).ContentStartsWith("Content_StartsWith"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context3Div2)).ContentIsNotEqualTo("Content_NotEqualTo"));
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
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
            Element? TestElement = null;
            Assert.DoesNotThrow(() => TestElement = ElementFactory.FetchWhen(By.Id(JSElements.Context3Div2)).HasContent());
            Timer.Stop();

            Webdriver.Quit();

            Assert.NotNull(TestElement);
            Assert.Greater(Timer.ElapsedMilliseconds, 2800, "Expected it to take around 3 seconds for the element content to exist");
        }

        #endregion
    }
}