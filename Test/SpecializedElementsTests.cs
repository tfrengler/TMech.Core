using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using TMech.Core.Elements;
using TMech.Core.Elements.Extensions;
using TMech.Core.Elements.Specialized;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class SpecializedElementsTests
    {
        private const string Category_TextElement = "Category_TextElement";
        [TestCase(Category = Category_TextElement)]
        public void TestElement_SetData()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            TextElement TestElement = null;
            
            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsText());

            Assert.DoesNotThrow(() => TestElement.SetData("this is a test"));
            Assert.AreEqual("this is a test", BaseElement.GetValue());

            Webdriver.Quit();
        }

        [TestCase(Category = Category_TextElement)]
        public void TestElement_GetData()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            TextElement TestElement = null;
            
            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsText());

            Assert.AreEqual("Context2-InputText-Value", TestElement.GetData());

            Webdriver.Quit();
        }

        private const string Category_DateElement = "Category_DateElement";
        [TestCase(Category = Category_DateElement)]
        public void DateElement_SetByKeystrokeAndGetData()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2DateTime));
            DateElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsDate());

            var TestDate = DateTime.Now.Date.AddDays(5);
            Assert.DoesNotThrow(() => TestElement.SetDataByKeystroke(TestDate, DateElement.ChromiumFormat));

            DateTime Data = new DateTime(2050, 12, 31).Date;

            Assert.DoesNotThrow(() => Data = TestElement.GetData());
            Assert.AreEqual(TestDate, Data);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DateElement)]
        public void DateElement_SetByJSAndGetData()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2DateTime));
            DateElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsDate());

            var TestDate = DateTime.Now.Date.AddDays(5);
            Assert.DoesNotThrow(() => TestElement.SetDataByJS(TestDate));

            DateTime Data = new DateTime(2050,12,31).Date;

            Assert.DoesNotThrow(() => Data = TestElement.GetData());
            Assert.AreEqual(TestDate, Data);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DateElement)]
        public void DateElement_SetAndGetDataByCustomFormat()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            DateElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsDate());

            var TestDate = DateTime.Now.Date.AddDays(5);
            Assert.DoesNotThrow(() => TestElement.SetDataByKeystroke(TestDate, "yyyy#M#dd"));

            DateTime Data = new DateTime(2050, 12, 31).Date;

            Assert.DoesNotThrow(() => Data = TestElement.GetData("yyyy#M#dd"));
            Assert.AreEqual(TestDate, Data);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DateElement)]
        public void DateElement_ParseFailure()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2DateTime));
            DateElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsDate());

            Assert.AreEqual(default(DateTime), TestElement.GetData());

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DateElement)]
        public void DateElement_ParseFailureWithCustomFormat()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            DateElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsDate());

            var TestDate = DateTime.Now.Date.AddDays(5);
            Assert.DoesNotThrow(() => TestElement.SetDataByKeystroke(TestDate, "yyyy#M#dd"));

            Assert.AreEqual(default(DateTime), TestElement.GetData("MM-dd-yyyy"));
            Webdriver.Quit();
        }
    }
}
