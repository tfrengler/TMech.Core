using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using TMech.Elements;
using TMech.Elements.Exceptions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class ElementTests
    {
        #region INTERACTIONS

        private const string Category_Actions = "Element = Actions";
        [TestCase(Category = Category_Actions)]
        public void Click()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Button));

                Assert.DoesNotThrow(() => TheElement.Click());
                Assert.DoesNotThrow(() => {
                    var TheAlert = Chrome.ChromeDriver.SwitchTo().Alert();
                    TheAlert.Dismiss();
                });
            }
        }

        [TestCase(Category = Category_Actions)]
        public void Click_Until_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Radio2));
                Chrome.JsChangeElementAttribute(JSElements.Context2Radio2, "name", "RegTest", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Assert.DoesNotThrow(() => {
                    TheElement.ClickUntil(element =>
                    {
                        string TheText = element.WrappedElement.GetAttribute("name");
                        return TheText == "RegTest";
                    });
                });
            }
        }

        [TestCase(Category = Category_Actions)]
        public void Click_Until_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Radio2));

                ElementInteractionException? Exception = Assert.Throws<ElementInteractionException>(() => {
                    TheElement.ClickUntil(element =>
                    {
                        string TheText = element.WrappedElement.GetAttribute("name");
                        return TheText == "RegTest";
                    });
                });

                Assert.That(Exception!.InnerException, Is.Not.Null);
                Assert.That(Exception!.InnerException, Is.TypeOf(typeof(WebDriverException)));
            }
        }

        [TestCase(Category = Category_Actions)]
        public void ScrollIntoView()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Radio2));

                Assert.DoesNotThrow(() => TheElement.ScrollIntoView());
            }
        }

        [TestCase(Category = Category_Actions)]
        public void SendKeys()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TextareaElement = TestContext.Fetch(By.Id(JSElements.Context2Textarea));
                var InputTextElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                Assert.DoesNotThrow(() => TextareaElement.SendKeys("TextareaRegTest"));
                Assert.DoesNotThrow(() => InputTextElement.SendKeys("RegTest"));

                string TextareaElementValue = TextareaElement.WrappedElement.GetAttribute("value");
                string InputTextElementValue = InputTextElement.WrappedElement.GetAttribute("value");

                Assert.That(TextareaElementValue, Is.EqualTo("TextareaRegTest"));
                Assert.That(InputTextElementValue, Is.EqualTo("Context2-InputText-ValueRegTest"));
            }
        }

        [TestCase(Category = Category_Actions)]
        public void Clear()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TextareaElement = TestContext.Fetch(By.Id(JSElements.Context2Textarea));
                var InputTextElement = TestContext.Fetch(By.Id(JSElements.Context2InputText));

                Chrome.JsChangeElementAttribute(JSElements.Context2Textarea, "value", "RegTest");
                Chrome.JsChangeElementAttribute(JSElements.Context2InputText, "value", "RegTest");

                Assert.DoesNotThrow(() => TextareaElement.Clear());
                Assert.DoesNotThrow(() => InputTextElement.Clear());

                string TextareaElementValue = TextareaElement.WrappedElement.GetAttribute("value");
                string InputTextElementValue = InputTextElement.WrappedElement.GetAttribute("value");

                Assert.That(TextareaElementValue, Is.Empty);
                Assert.That(InputTextElementValue, Is.Empty);
            }
        }

        #endregion

        #region GETTERS

        private const string Category_Getters = "Element = Getters";
        [TestCase(Category = Category_Getters)]
        public void GetFormControlType()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TextareaResult = TestContext.Fetch(By.Id(JSElements.Context2Textarea)).GetFormControlType();
                var InputTextResult = TestContext.Fetch(By.Id(JSElements.Context2InputText)).GetFormControlType();
                var InputNumberResult = TestContext.Fetch(By.Id(JSElements.Context2InputNumber)).GetFormControlType();
                var InputDateResult = TestContext.Fetch(By.Id(JSElements.Context2DateTime)).GetFormControlType();
                var DropdownResult = TestContext.Fetch(By.Id(JSElements.Context2Select)).GetFormControlType();
                var RadioResult = TestContext.Fetch(By.Id(JSElements.Context2Radio1)).GetFormControlType();
                var HyperlinkResult = TestContext.Fetch(By.Id(JSElements.Context2Hyperlink)).GetFormControlType();

                Assert.Multiple(() =>
                {
                    Assert.That(TextareaResult, Is.EqualTo("textarea"));
                    Assert.That(InputTextResult, Is.EqualTo("input:text"));
                    Assert.That(InputNumberResult, Is.EqualTo("input:number"));
                    Assert.That(InputDateResult, Is.EqualTo("input:date"));
                    Assert.That(DropdownResult, Is.EqualTo("select"));
                    Assert.That(RadioResult, Is.EqualTo("input:radio"));
                    Assert.That(HyperlinkResult, Is.EqualTo("a"));
                });
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetAttributeNames()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                
                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Radio1));
                // <input id="Context2-Radio1-Id" checked type="radio" name="Context2-Radio-Name" value="Context2-Radio1-Value" />

                IList<string>? AttributeNames = null;

                Assert.DoesNotThrow(() =>
                {
                    AttributeNames = TheElement.GetAttributeNames();
                });

                var ExpectedAttributes = new string[] { "id", "checked", "type", "name", "value" };

                Assert.That(AttributeNames, Is.EquivalentTo(ExpectedAttributes));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetAttributes()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement = TestContext.Fetch(By.Id(JSElements.Context2Radio1));
                // <input id="Context2-Radio1-Id" checked type="radio" name="Context2-Radio-Name" value="Context2-Radio1-Value" />

                IDictionary<string,string>? Attributes = null;

                Assert.DoesNotThrow(() =>
                {
                    Attributes = TheElement.GetAttributes();
                });

                var ExpectedAttributes = new Dictionary<string,string>()
                {
                    { "id", "Context2-Radio1-Id" },
                    { "checked", "" },
                    { "type", "radio" },
                    { "name", "Context2-Radio-Name" },
                    { "value", "Context2-Radio1-Value" }
                };

                Assert.That(Attributes, Is.EquivalentTo(ExpectedAttributes));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetDataSet()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement = TestContext.Fetch(By.Id(JSElements.Context3Div2));
                // <div id="Context3-Div2-Id" class="Context3-Div2-Class1 Context3-Div2-Class2" data-data1="Context3-Div2-Data1" data-data2="Context3-Div2-Data2" />

                IDictionary<string, string>? DataAttributes = null;

                Assert.DoesNotThrow(() =>
                {
                    DataAttributes = TheElement.GetDataSet();
                });

                var ExpectedAttributes = new Dictionary<string, string>()
                {
                    { "data1", "Context3-Div2-Data1" },
                    { "data2", "Context3-Div2-Data2" }
                };

                Assert.That(DataAttributes, Is.EquivalentTo(ExpectedAttributes));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetHTML()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement = TestContext.Fetch(By.Id(JSElements.StaleContextChild3));

                string ExpectedInnerHTML = "<span id=\"StaleChild4\">Initial text</span>";
                string? ActualInnerHTML = null;

                Assert.DoesNotThrow(() =>
                {
                    ActualInnerHTML = TheElement.GetHTML();
                });

                Assert.That(ActualInnerHTML, Is.EqualTo(ExpectedInnerHTML));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetId()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement = TestContext.Fetch(By.Id(JSElements.Context1Div1));

                string ExpectedId = JSElements.Context1Div1;
                string? ActualId = null;

                Assert.DoesNotThrow(() =>
                {
                    ActualId = TheElement.GetId();
                });

                Assert.That(ActualId, Is.EqualTo(ExpectedId));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetTagName()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement = TestContext.Fetch(By.Id(JSElements.Context1Div1));

                string ExpectedTagName = "div";
                string? ActualTagName = null;

                Assert.DoesNotThrow(() =>
                {
                    ActualTagName = TheElement.GetTagName();
                });

                Assert.That(ActualTagName, Is.EqualTo(ExpectedTagName));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetText()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement = TestContext.Fetch(By.Id(JSElements.Context1Div2));

                // &nbsp;  &#12;Context1&#09;-Div2&#10;&#13;-Text&#11;-Value&nbsp; 
                string ExpectedText = "Context1 -Div2 -Text -Value";
                string? ActualText = null;

                Assert.DoesNotThrow(() =>
                {
                    ActualText = TheElement.GetText();
                });

                Assert.That(ActualText, Is.EqualTo(ExpectedText));
            }
        }

        [TestCase(Category = Category_Getters)]
        public void GetAttribute()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var TheElement1 = TestContext.Fetch(By.Id(JSElements.Context2InputText));
                var TheElement2 = TestContext.Fetch(By.CssSelector($"#{JSElements.Context2FileWrapper} label"));

                string? TheElement1AttributeValue = null;
                string? TheElement2AttributeValue = null;

                Assert.DoesNotThrow(() =>
                {
                    TheElement1AttributeValue = TheElement1.GetAttribute("value");
                    TheElement2AttributeValue = TheElement2.GetAttribute("for");
                });

                Assert.That(TheElement1AttributeValue, Is.EqualTo("Context2-InputText-Value"));
                Assert.That(TheElement2AttributeValue, Is.EqualTo(JSElements.Context2File));
            }
        }

        #endregion

        #region STATE CHECKERS

        const string Category_StateCheckers = "Element = StateCheckers";
        [TestCase(Category = Category_StateCheckers)]
        public void IsDisplayed()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsHideElement(JSElements.Context1Div2);

                var TheElement1 = TestContext.Fetch(By.Id(JSElements.Context1Div1));
                var TheElement2 = TestContext.Fetch(By.Id(JSElements.Context1Div2));
                
                bool? Element1IsDisplayed = null;
                bool? Element2IsDisplayed = null;

                Assert.DoesNotThrow(() => Element1IsDisplayed = TheElement1.IsDisplayed());
                Assert.DoesNotThrow(() => Element2IsDisplayed = TheElement2.IsDisplayed());

                Assert.That(Element1IsDisplayed, Is.Not.Null);
                Assert.That(Element1IsDisplayed, Is.True);
                Assert.That(Element2IsDisplayed, Is.Not.Null);
                Assert.That(Element2IsDisplayed, Is.False);
            }
        }

        #endregion
    }
}