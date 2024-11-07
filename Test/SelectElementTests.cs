using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using TMech.Elements;
using TMech.Elements.Extensions;
using TMech.Elements.Specialized;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class SelectElementTests
    {
        private const string Category = "SelectElementTests";
        [TestCase(Category = Category)]
        public void IsMultiple()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Assert.That(TheContext.Fetch(By.Id(JSElements.Context2SelectMulti)).AsDropdown().IsMultiple(), Is.True);
                Assert.That(TheContext.Fetch(By.Id(JSElements.Context2Select)).AsDropdown().IsMultiple(), Is.False);
            }
        }

        [TestCase(Category = Category)]
        public void SelectByValue()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                Assert.Multiple(() =>
                {

                    Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2Select))
                            .AsDropdown()
                            .SelectByValue("Context2-Option2-Value")
                    );
                    Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2Select))
                            .AsDropdown()
                            .WithRobustSelection()
                            .SelectByValue("Context2-Option2-Value")
                    );
                    Chrome.JsRemoveSelectedOptions(JSElements.Context2SelectMulti);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2SelectMulti))
                            .AsDropdown()
                            .SelectByValue("Context2-Option5-Value")
                    );
                });
            }
        }

        [TestCase(Category = Category)]
        public void SelectByText_FullMatch()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                Assert.Multiple(() =>
                {

                    Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2Select))
                            .AsDropdown()
                            .SelectByText("Context2-Option2-Text")
                    );
                    Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2Select))
                            .AsDropdown()
                            .WithRobustSelection()
                            .SelectByText("Context2-Option2-Text")
                    );
                    Chrome.JsRemoveSelectedOptions(JSElements.Context2SelectMulti);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2SelectMulti))
                            .AsDropdown()
                            .SelectByText("Context2-Option5-Text")
                    );

                });
            }
        }

        [TestCase(Category = Category)]
        public void SelectByText_PartialMatch()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                Assert.Multiple(() =>
                {

                    Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2Select))
                            .AsDropdown()
                            .SelectByText("Option2", true)
                    );
                    Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2Select))
                            .AsDropdown()
                            .WithRobustSelection()
                            .SelectByText("Option2", true)
                    );
                    Chrome.JsRemoveSelectedOptions(JSElements.Context2SelectMulti);
                    Assert.DoesNotThrow(() =>
                        TheContext
                            .Fetch(By.Id(JSElements.Context2SelectMulti))
                            .AsDropdown()
                            .SelectByText("Option5", true)
                    );

                });
            }
        }

        [TestCase(Category = Category)]
        public void GetSelectedOptions()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                Chrome.JsRemoveSelectedOptions(JSElements.Context2SelectMulti);
                var TheElement = TheContext
                        .Fetch(By.Id(JSElements.Context2SelectMulti))
                        .AsDropdown();

                TheElement.SelectByText("Context2-Option4-Text");
                TheElement.SelectByText("Context2-Option6-Text");

                IList<FormControlElement>? SelectedOptions = null;
                Assert.DoesNotThrow(() => SelectedOptions = TheElement.GetSelectedOptions());

                Assert.That(SelectedOptions, Is.Not.Empty);
                Assert.That(SelectedOptions!.Count, Is.EqualTo(2));

                Assert.That(SelectedOptions[0].GetText(), Is.EqualTo("Context2-Option4-Text"));
                Assert.That(SelectedOptions[1].GetText(), Is.EqualTo("Context2-Option6-Text"));
            }
        }

        [TestCase(Category = Category)]
        public void GetSelectedOption()
        {
            using (var Chrome = new ChromeContext())
            {
                var TheContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                Chrome.JsRemoveSelectedOptions(JSElements.Context2Select);
                var TheElement = TheContext
                        .Fetch(By.Id(JSElements.Context2Select))
                        .AsDropdown();

                TheElement.SelectByText("Context2-Option2-Text");

                FormControlElement? SelectedOptions = null;
                Assert.DoesNotThrow(() => SelectedOptions = TheElement.GetSelectedOption());

                Assert.That(SelectedOptions, Is.Not.Null);
                Assert.That(SelectedOptions?.GetText(), Is.EqualTo("Context2-Option2-Text"));
            }
        }
    }
}
