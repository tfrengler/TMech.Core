using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TMech.Core.Elements.Specialized;
using TMech.Core.Elements;
using TMech.Core.Elements.Extensions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public sealed class IntegerElementTests
    {
        public const string Category_Setters = "Integer_Setters";
        [TestCase(Category = Category_Setters)]
        public void Setters_ByInteger()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            IntegerElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsInteger();
            Assert.DoesNotThrow(()=> TestElement.SetNumber(666));
            Assert.AreEqual("666", TestElement.WrappedElement.GetAttribute("value"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setters_ByString_OK()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            IntegerElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsInteger();
            Assert.DoesNotThrow(() => TestElement.SetNumber("999"));
            Assert.AreEqual("999", TestElement.WrappedElement.GetAttribute("value"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setters_ByString_NOK()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            IntegerElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsInteger();
            Assert.Throws<System.ArgumentException>(() => TestElement.SetNumber("test"));

            Webdriver.Quit();
        }

        public const string Category_Getters = "Integer_Getters";
        [TestCase(Category = Category_Getters)]
        public void Getters_Get_OK()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            int? Actual = null;

            IntegerElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsInteger();
            Assert.DoesNotThrow(() => Actual = TestElement.GetNumber());
            Assert.AreEqual(42, Actual);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Getters)]
        public void Getters_Get_NOK()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);
            int? Actual = null;

            IntegerElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText)).AsInteger();
            Assert.DoesNotThrow(() => Actual = TestElement.GetNumber());
            Assert.AreEqual(null, Actual);

            Webdriver.Quit();
        }
    }
}
