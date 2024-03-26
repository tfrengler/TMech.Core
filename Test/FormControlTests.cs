using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading;
/*
namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public sealed class FormControlTests
    {
        #region STATE CHECKERS

        public const string Category_StateCheckers = "FormControl_StateCheckers";
        [TestCase(Category = Category_StateCheckers)]
        public void StateCheckers_IsEnabled()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio2)).AsFormControl();
            Assert.IsTrue(TestElement.IsEnabled());

            Webdriver.Quit();
        }

        [TestCase(Category = Category_StateCheckers)]
        public void StateCheckers_IsNotEnabled()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio3)).AsFormControl();
            Assert.IsFalse(TestElement.IsEnabled());

            Webdriver.Quit();
        }

        [TestCase(Category = Category_StateCheckers)]
        public void StateCheckers_ReadOnly()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                Elements.Context2InputNumber().readOnly = true;
                arguments[arguments.length - 1]();
                await Wait(2000);
                Elements.Context2InputNumber().readOnly = false;
            ");

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            Assert.IsTrue(TestElement.IsReadOnly());
            Thread.Sleep(3000);
            Assert.IsFalse(TestElement.IsReadOnly());

            Webdriver.Quit();
        }

        [TestCase(Category = Category_StateCheckers)]
        public void StateCheckers_Required()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteAsyncScript(@"
                Elements.Context2InputNumber().required = true;
                arguments[arguments.length - 1]();
                await Wait(2000);
                Elements.Context2InputNumber().required = false;
            ");

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            Assert.IsTrue(TestElement.IsRequired());
            Thread.Sleep(3000);
            Assert.IsFalse(TestElement.IsRequired());

            Webdriver.Quit();
        }

        #endregion

        #region DATA SETTERS

        public const string Category_DataSetters = "FormControl_DataSetters";
        [TestCase(Category = Category_DataSetters)]
        public void DataSetter_SetValue()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            FormControlElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsFormControl());

            Assert.DoesNotThrow(() => TestElement.SetValue("this is a test"));
            Assert.AreEqual("this is a test", TestElement.WrappedElement.GetAttribute("value"));

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataSetters)]
        public void DataSetter_GetValue()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element BaseElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            FormControlElement TestElement = null;

            Assert.NotNull(BaseElement);
            Assert.DoesNotThrow(() => TestElement = BaseElement.AsFormControl());

            Assert.AreEqual("Context2-InputText-Value", TestElement.GetValue());

            Webdriver.Quit();
        }

        #endregion

        #region DATA GETTERS

        public const string Category_DataGetters = "FormControl_DataGetters";
        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Name()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            string Name = null;

            Assert.DoesNotThrow(() => Name = TestElement.GetName());
            Assert.AreEqual(Name, "Context2-InputNumber-Name");

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_InputType()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            string Type = null;

            Assert.DoesNotThrow(() => Type = TestElement.GetInputType());
            Assert.AreEqual(Type, "number");

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Min()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            int? Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMin());
            Assert.AreEqual(Actual, 2);

            TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText)).AsFormControl();
            Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMin());
            Assert.IsNull(Actual);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Max()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            int? Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMax());
            Assert.AreEqual(Actual, 102);

            TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText)).AsFormControl();
            Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMax());
            Assert.IsNull(Actual);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Minlength()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText)).AsFormControl();
            int? Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMinLength());
            Assert.AreEqual(Actual, 2);

            TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio1)).AsFormControl();
            Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMinLength());
            Assert.IsNull(Actual);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Maxlength()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText)).AsFormControl();
            int? Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMaxLength());
            Assert.AreEqual(Actual, 123);

            TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio1)).AsFormControl();
            Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetMaxLength());
            Assert.IsNull(Actual);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Step()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputNumber)).AsFormControl();
            int? Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetStep());
            Assert.AreEqual(Actual, 1);

            TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText)).AsFormControl();
            Actual = 0;

            Assert.DoesNotThrow(() => Actual = TestElement.GetStep());
            Assert.IsNull(Actual);

            Webdriver.Quit();
        }

        [TestCase(Category = Category_DataGetters)]
        public void DataGetter_Source()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            FormControlElement TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Image)).AsFormControl();
            string Actual = null;

            Assert.DoesNotThrow(() => Actual = TestElement.GetSource());
            Assert.AreEqual(Actual, "http://somewhere.com/doesnotexist.jpeg");

            Webdriver.Quit();
        }

        #endregion
    }
}
*/