using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using TMech.Core.Elements.Specialized;
using TMech.Core.Elements;
using TMech.Core.Elements.Extensions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public sealed class BooleanElementTests
    {
        public const string Category_StateCheckers = "Boolean_StateCheckers";
        [TestCase(Category = Category_StateCheckers)]
        public void StateCheckers_IsSelected()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            BooleanElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio1)).AsBoolean();
            Assert.IsTrue(TestElement.IsSelected());

            Webdriver.Quit();
        }

        [TestCase(Category = Category_StateCheckers)]
        public void StateCheckers_IsNotSelected()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            BooleanElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio2)).AsBoolean();
            Assert.IsFalse(TestElement.IsSelected());

            Webdriver.Quit();
        }

        public const string Category_Setters = "Boolean_Setters";
        [TestCase(Category = Category_Setters)]
        public void Setters_Select_Radio()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            BooleanElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio1)).AsBoolean();
            Assert.DoesNotThrow(() => TestElement.Select());
            Assert.IsTrue(TestElement.WrappedElement.Selected);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setters_Select_Checkbox()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            BooleanElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Checkbox)).AsBoolean();
            Assert.DoesNotThrow(() => TestElement.Select());
            Assert.IsTrue(TestElement.WrappedElement.Selected);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setters_Deselect_Checkbox()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteScript("Select(Elements.Context2Checkbox())");

            BooleanElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Checkbox)).AsBoolean();
            Assert.DoesNotThrow(() => TestElement.Deselect());
            Assert.IsTrue(!TestElement.WrappedElement.Selected);

            Webdriver.Quit();
        }
    }
}
