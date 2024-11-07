using NUnit.Framework;
using OpenQA.Selenium;
using System.IO;
using TMech.Elements;
using TMech.Elements.Extensions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class FormControlElementTests
    {
        #region SETTERS

        private const string Category_Setters = "FormControlElement = Setters";
        [TestCase(Category = Category_Setters)]
        public void SetValue()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                Assert.DoesNotThrow(() => TheElement.AsFormControl().SetValue("RegTest1"));
                Assert.DoesNotThrow(() => TheElement.AsFormControl().WithRobustSelection().SetValue("RegTest2"));
            }
        }

        [TestCase(Category = Category_Setters)]
        public void SetValueByJS()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                Assert.DoesNotThrow(() => TheElement.AsFormControl().SetValueByJS("RegTest1"));
                Assert.DoesNotThrow(() => TheElement.AsFormControl().WithRobustSelection().SetValueByJS("RegTest2"));
            }
        }

        [TestCase(Category = Category_Setters)]
        public void UploadFile()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2File));

                Assert.DoesNotThrow(() => TheElement.AsFormControl().UploadFile(Path.Combine(GlobalSetup.ChromeLocation.DirectoryName!, "ABOUT")));
                Assert.Throws<FileNotFoundException>(() => TheElement.AsFormControl().UploadFile("C:/DoesNotExist.zip"));
                //Assert.Throws<Exception>(() => TestContext.Fetch(By.Id(JSElements.Context2Textarea)).AsFormControl().UploadFile("C:/Temp/Chromium/VERSION"));
            }
        }

        #endregion

        #region GETTERS

        private const string Category_Getters = "FormControlElement = Getters";
        [TestCase(Category = Category_Getters)]
        public void GetValue()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                string? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetValue());
                Assert.That(Value, Is.EqualTo("Context2-InputText-Value"));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetName()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                string? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetName());
                Assert.That(Value, Is.EqualTo("Context2-InputText-Name"));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetInputType()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                string? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetInputType());
                Assert.That(Value, Is.EqualTo("text"));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetMin()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputNumber));

                int? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetMin());
                Assert.That(Value, Is.EqualTo(2));
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().GetMin(), Is.Null);
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetMax()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputNumber));

                int? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetMax());
                Assert.That(Value, Is.EqualTo(102));
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().GetMax(), Is.Null);
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetMinLength()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                int? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetMinLength());
                Assert.That(Value, Is.EqualTo(2));
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().GetMinLength(), Is.Null);
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetMaxLength()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                int? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetMaxLength());
                Assert.That(Value, Is.EqualTo(123));
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().GetMaxLength(), Is.Null);
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetStep()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2InputNumber));

                int? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetStep());
                Assert.That(Value, Is.EqualTo(1));
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().GetStep(), Is.Null);
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetSource()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Image));

                string? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().GetSource());
                Assert.That(Value, Is.EqualTo("http://somewhere.com/doesnotexist.jpeg"));
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().GetSource(), Is.Empty);
            }
        }

        #endregion

        #region STATE CHECKING

        private const string Category_StateCheckers = "FormControlElement = StateCheckers";
        [TestCase(Category = Category_StateCheckers)]
        public void IsRequired()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2File));
                Chrome.JsChangeElementAttribute(JSElements.Context2File, "required", "true");

                bool? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().IsRequired());
                Assert.That(Value, Is.True);
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().IsRequired(), Is.False);
            }
        }

        [TestCase(Category = Category_StateCheckers)]
        public void IsReadOnly()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2File));
                Chrome.JsChangeElementAttribute(JSElements.Context2File, "readonly", "true");

                bool? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().IsReadOnly());
                Assert.That(Value, Is.True);
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().IsReadOnly(), Is.False);
            }
        }

        [TestCase(Category = Category_StateCheckers)]
        public void IsEnabled()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Radio3));

                bool? Value = null;

                Assert.DoesNotThrow(() => Value = TheElement.AsFormControl().IsEnabled());
                Assert.That(Value, Is.False);
                Assert.That(TestContext.Fetch(By.Id(JSElements.Context1Div1)).AsFormControl().IsEnabled(), Is.True);
            }
        }

        #endregion
    }
}
