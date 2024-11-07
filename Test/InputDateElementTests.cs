using NUnit.Framework;
using OpenQA.Selenium;
using System;
using TMech.Elements;
using TMech.Elements.Extensions;
using TMech.Elements.Specialized;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class InputDateElementTests
    {
        private const string Category = "InputDateElementTests";
        [TestCase(Category = Category)]
        public void SetDate()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2DateTime));

                var Input = new DateTime(2054, 12, 14);
                var InputAsString = Input.ToString(InputDateElement.ValueAttributeFormat);

                Assert.DoesNotThrow(() => TheElement.AsInputDate().SetDate(Input, InputDateElement.ChromiumFormat));
                Assert.DoesNotThrow(() => TheElement.AsInputDate().WithRobustSelection().SetDate(Input, InputDateElement.ChromiumFormat));
                var Value = TheElement.GetAttribute("value");

                Assert.That(Value, Is.EqualTo(InputAsString));
            }
        }

        [TestCase(Category = Category)]
        public void SetDateByJS()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2DateTime));

                var Input = new DateTime(2054, 12, 14);
                var InputAsString = Input.ToString(InputDateElement.ValueAttributeFormat);

                Assert.DoesNotThrow(() => TheElement.AsInputDate().SetDateByJS(Input));
                Assert.DoesNotThrow(() => TheElement.AsInputDate().WithRobustSelection().SetDateByJS(Input));
                var Value = TheElement.GetAttribute("value");

                Assert.That(Value, Is.EqualTo(InputAsString));
            }
        }

        [TestCase(Category = Category)]
        public void GetDate()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2DateTime));

                var ExpectedOutput = new DateTime(2054, 12, 14);
                var ExpectedOutputAsString = ExpectedOutput.ToString(InputDateElement.ValueAttributeFormat);
                DateTime? ActualOutput = null;

                Chrome.JsChangeElementAttribute(JSElements.Context2DateTime, "value", string.Empty);
                Assert.That(TheElement.AsInputDate().GetDate(), Is.EqualTo(DateTime.MinValue));
                
                Chrome.JsChangeElementAttribute(JSElements.Context2DateTime, "value", ExpectedOutputAsString);                
                Assert.DoesNotThrow(() => ActualOutput = TheElement.AsInputDate().GetDate());
                Assert.That(ActualOutput, Is.EqualTo(ExpectedOutput));
            }
        }
    }
}
