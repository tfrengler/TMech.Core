using NUnit.Framework;
using OpenQA.Selenium;
using TMech.Elements;
using TMech.Elements.Extensions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class InputRadioOrCheckboxTests
    {
        private const string Category = "InputRadioOrCheckboxTests";
        [TestCase(Category = Category)]
        public void Check()
        {
            using(var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                Chrome.JsDeselectElement(JSElements.Context2Checkbox);

                Assert.DoesNotThrow(() =>
                    TheContext
                        .Fetch(By.Id(JSElements.Context2Checkbox))
                        .AsInputRadioOrCheckbox()
                        .Check()
                );

                Chrome.JsDeselectElement(JSElements.Context2Checkbox);

                Assert.DoesNotThrow(() =>
                    TheContext
                        .Fetch(By.Id(JSElements.Context2Checkbox))
                        .AsInputRadioOrCheckbox()
                        .WithRobustSelection()
                        .Check()
                );
            }
        }

        [TestCase(Category = Category)]
        public void Uncheck()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsSelectElement(JSElements.Context2Checkbox);

                Assert.DoesNotThrow(() =>
                    TheContext
                        .Fetch(By.Id(JSElements.Context2Checkbox))
                        .AsInputRadioOrCheckbox()
                        .Uncheck()
                );

                Chrome.JsSelectElement(JSElements.Context2Checkbox);

                Assert.DoesNotThrow(() =>
                    TheContext
                        .Fetch(By.Id(JSElements.Context2Checkbox))
                        .AsInputRadioOrCheckbox()
                        .WithRobustSelection()
                        .Uncheck()
                );
            }
        }

        [TestCase(Category = Category)]
        public void IsChecked()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsSelectElement(JSElements.Context2Checkbox);

                Assert.That(
                    TheContext.Fetch(By.Id(JSElements.Context2Checkbox)).AsInputRadioOrCheckbox().IsChecked(),
                    Is.True
                );

                Chrome.JsDeselectElement(JSElements.Context2Checkbox);

                Assert.That(
                    TheContext.Fetch(By.Id(JSElements.Context2Checkbox)).AsInputRadioOrCheckbox().IsChecked(),
                    Is.False
                );
            }
        }
    }
}
