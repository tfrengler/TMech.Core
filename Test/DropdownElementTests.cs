using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using TMech.Core.Elements;
using TMech.Core.Elements.Extensions;
using TMech.Core.Elements.Specialized;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public sealed class DropdownElementTests
    {
        #region SETTERS

        public const string Category_Setters = "Dropdown_Setters";
        [TestCase(Category = Category_Setters)]
        public void Setter_SelectByValue()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Select)).AsDropdown();

            Assert.DoesNotThrow(() => TestElement.SelectByValue("Context2-Option2-Value"));
            var Dropdown = new SelectElement(TestElement.WrappedElement);
            Assert.AreEqual("Context2-Option2-Value", Dropdown.SelectedOption.GetAttribute("value"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setter_SelectByText()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Select)).AsDropdown();

            Assert.DoesNotThrow(() => TestElement.SelectByText("Context2-Option2-Text"));
            var Dropdown = new SelectElement(TestElement.WrappedElement);
            Assert.AreEqual("Context2-Option2-Value", Dropdown.SelectedOption.GetAttribute("value"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setter_DeselectByValue()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteScript("Select(Elements.Context2Option5());Select(Elements.Context2Option6());");
            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2SelectMulti)).AsDropdown();

            Assert.DoesNotThrow(() => TestElement.DeselectByValue("Context2-Option6-Value"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Setters)]
        public void Setter_DeselectByText()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteScript("Select(Elements.Context2Option5());Select(Elements.Context2Option6());");
            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2SelectMulti)).AsDropdown();

            Assert.DoesNotThrow(() => TestElement.DeselectByText("Context2-Option6-Text"));

            Webdriver.Quit();
        }

        #endregion

        #region GETTERS

        public const string Category_Getters = "Dropdown_Getters";
        [TestCase(Category = Category_Getters)]
        public void Getter_SelectedOption_SingleSelect()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Select)).AsDropdown();
            Webdriver.ExecuteScript(@"Select(Elements.Context2Option2());");
            FormControlElement SelectedOption = null;

            Assert.DoesNotThrow(() => SelectedOption = TestElement.GetSelectedOption());
            var Dropdown = new SelectElement(TestElement.WrappedElement);
            Assert.AreEqual(JSElements.Context2Option2, Dropdown.SelectedOption.GetAttribute("id"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Getters)]
        public void Getter_SelectedOption_MultiSelect()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2SelectMulti)).AsDropdown();
            Webdriver.ExecuteScript(@"Select(Elements.Context2Option5());");
            FormControlElement SelectedOption = null;

            Assert.DoesNotThrow(() => SelectedOption = TestElement.GetSelectedOption());
            var Dropdown = new SelectElement(TestElement.WrappedElement);
            Assert.AreEqual(JSElements.Context2Option5, Dropdown.SelectedOption.GetAttribute("id"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Getters)]
        public void Getter_Options_SingleSelect()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Select)).AsDropdown();
            IEnumerable<FormControlElement> AllOptions = null;

            Assert.DoesNotThrow(() => AllOptions = TestElement.GetOptions());
            Assert.AreEqual(4, AllOptions.Count());

            Assert.AreEqual(JSElements.Context2OptionNIL1, AllOptions.First().WrappedElement.GetAttribute("id"));
            Assert.AreEqual(JSElements.Context2Option3, AllOptions.Last().WrappedElement.GetAttribute("id"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Getters)]
        public void Getter_Options_MultiSelect()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2SelectMulti)).AsDropdown();
            IEnumerable<FormControlElement> AllOptions = null;

            Assert.DoesNotThrow(() => AllOptions = TestElement.GetOptions());
            Assert.AreEqual(4, AllOptions.Count());

            Assert.AreEqual(JSElements.Context2OptionNIL2, AllOptions.First().WrappedElement.GetAttribute("id"));
            Assert.AreEqual(JSElements.Context2Option6, AllOptions.Last().WrappedElement.GetAttribute("id"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_Getters)]
        public void Getter_IsMultiSelect()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            var TestElement1 = ElementFactory.Fetch(By.Id(JSElements.Context2SelectMulti)).AsDropdown();
            var TestElement2 = ElementFactory.Fetch(By.Id(JSElements.Context2Select)).AsDropdown();
 
            Assert.AreEqual(TestElement1.IsMultiple(), true);
            Assert.AreEqual(TestElement2.IsMultiple(), false);

            Webdriver.Quit();
        }

        #endregion
    }
}
