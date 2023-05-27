using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMech.Core;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class ElementTests
    {
        #region ACTIONS

        private const string Category_Actions = "Element Actions";
        [TestCase(Category=Category_Actions)]
        public void Actions_Click()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id("Context2-Button-Id"));

            Assert.NotNull(TestElement);
            Assert.DoesNotThrow(() => TestElement.Click());
            Assert.DoesNotThrow(() => Webdriver.SwitchTo().Alert());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_Actions)]
        public void Actions_ClickDelayed()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id("Context2-Button-Id"));
            Assert.NotNull(TestElement);
            TestElement.TryActionsThisManyTimes(50);

            Webdriver.ExecuteAsyncScript(@"
                HideElement(Elements.Context2Button());
                arguments[arguments.length - 1]();
                await Wait(2500);
                ShowElement(Elements.Context2Button());
            ");

            var Timer = Stopwatch.StartNew();
            Assert.DoesNotThrow(() => TestElement.Click());
            Timer.Stop();
            Assert.DoesNotThrow(() => Webdriver.SwitchTo().Alert());
            Assert.Greater(Timer.ElapsedMilliseconds, 2000, "Expected it to take around 2 seconds before the click succeeded!");

            Webdriver.Quit();
        }

        [TestCase(Category=Category_Actions)]
        public void Actions_ScrollIntoView()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id("Context2-Select-Id"));

            Assert.NotNull(TestElement);
            Assert.DoesNotThrow(() => TestElement.ScrollIntoView());

            Webdriver.Quit();
        }

        #endregion

        #region DATA GETTERS

        private const string Category_DataGetters = "Element Data Getters";
        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetFormControlType()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElementDateTime = ElementFactory.Fetch(By.Id(JSElements.Context2DateTimeLocal));
            Element? TestElementRadioButton = ElementFactory.Fetch(By.Id(JSElements.Context2Radio1));
            Element? TestElementSelect = ElementFactory.Fetch(By.Id(JSElements.Context2Select));
            Element? TestElementTextarea = ElementFactory.Fetch(By.Id(JSElements.Context2Textarea));

            Assert.NotNull(TestElementDateTime);
            Assert.NotNull(TestElementRadioButton);
            Assert.NotNull(TestElementSelect);
            Assert.NotNull(TestElementTextarea);

            Assert.AreEqual("input:datetime-local", TestElementDateTime.GetFormControlType());
            Assert.AreEqual("input:radio", TestElementRadioButton.GetFormControlType());
            Assert.AreEqual("select", TestElementSelect.GetFormControlType());
            Assert.AreEqual("textarea", TestElementTextarea.GetFormControlType());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetAttributeNames()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Textarea));
            Assert.NotNull(TestElement);

            var AttributeNames = TestElement.GetAttributeNames();
            Assert.AreEqual( AttributeNames.Count, 4, "Expected there to be only 4 available attributes for this textarea!" );

            string[] ExpectedPropertyNames = new string[] { "id", "name", "cols", "rows" };
            Assert.DoesNotThrow(() =>
            {
                foreach(string CurrentExpectedAttribute in ExpectedPropertyNames)
                {
                    if (!AttributeNames.Contains(CurrentExpectedAttribute))
                    {
                        throw new Exception($"Expected attribute with name '{CurrentExpectedAttribute}' to be in the list of the elements attributes! (actual: {string.Join(',', AttributeNames)})");
                    }
                }
            });

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetAttributes()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Textarea));
            Assert.NotNull(TestElement);

            var ActualAttributes = TestElement.GetAttributes();

            Dictionary<string,string> ExpectedAttributes = new() {
                { "id", JSElements.Context2Textarea },
                { "name", "Context2-Textarea1-Name" },
                { "cols", "30" },
                { "rows", "10" }
            };

            Assert.DoesNotThrow(() =>
            {
                foreach(KeyValuePair<string,string> CurrentExpectedAttribute in ExpectedAttributes)
                {
                    if (!ActualAttributes.Contains(CurrentExpectedAttribute))
                    {
                        List<string> Actual = new();
                        foreach(KeyValuePair<string,string> Current in ActualAttributes)
                        {
                            Actual.Add($"- {Current.Key} | {Current.Value}");
                        }

                        throw new Exception(
                            $"Expected attribute with name '{CurrentExpectedAttribute.Key}' and value '{CurrentExpectedAttribute.Value}' to be in the list of the elements attributes!"
                            + Environment.NewLine + "ACTUAL:" + Environment.NewLine
                            + string.Join(Environment.NewLine, Actual)
                        );
                    }
                }
            });

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetDataSet()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context3Div2));
            Assert.NotNull(TestElement);

            var ActualDataSet = TestElement.GetDataSet();

            Assert.AreEqual( ActualDataSet.Count, 2, "Expected there to be only 2 data-sets for this div!" );

            Dictionary<string,string> ExpectedDataSet = new() {
                { "data1", "Context3-Div2-Data1" },
                { "data2", "Context3-Div2-Data2" }
            };

            Assert.DoesNotThrow(() =>
            {
                foreach(KeyValuePair<string,string> CurrentExpectedAttribute in ExpectedDataSet)
                {
                    if (!ActualDataSet.Contains(CurrentExpectedAttribute))
                    {
                        List<string> Actual = new();
                        foreach(KeyValuePair<string,string> Current in ActualDataSet)
                        {
                            Actual.Add($"- {Current.Key} | {Current.Value}");
                        }

                        throw new Exception(
                            $"Expected custom attribute with name '{CurrentExpectedAttribute.Key}' and value '{CurrentExpectedAttribute.Value}' to be present in the dataset of this element!"
                            + Environment.NewLine + "ACTUAL:" + Environment.NewLine
                            + string.Join(Environment.NewLine, Actual)
                        );
                    }
                }
            });

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetHTML()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context3Div3));

            Assert.NotNull(TestElement);
            Assert.AreEqual("<span>Context3-Div3-Text</span>", TestElement.GetHTML());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetId()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context3Div3));

            Assert.NotNull(TestElement);
            Assert.AreEqual(JSElements.Context3Div3, TestElement.GetId());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetTagName()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Textarea));

            Assert.NotNull(TestElement);
            Assert.AreEqual("textarea", TestElement.GetTagName());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetText()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context1Div2));

            Assert.NotNull(TestElement);
            Assert.AreEqual("Context1-Div2-Text", TestElement.GetText());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_DataGetters)]
        public void DataGetters_GetValue()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));

            Assert.NotNull(TestElement);
            Assert.AreEqual("Context2-InputText-Value", TestElement.GetValue());

            Webdriver.Quit();
        }

        #endregion

        #region STATE CHECKERS

        private const string Category_StateCheckers = "Element State Checkers";
        [TestCase(Category=Category_StateCheckers)]
        public void StateCheckers_IsDisplayed()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            Assert.IsTrue(TestElement.IsDisplayed());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_StateCheckers)]
        public void StateCheckers_IsNotDisplayed()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Webdriver.ExecuteScript(@"HideElement(Elements.Context2InputText());");

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2InputText));
            Assert.IsFalse(TestElement.IsDisplayed());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_StateCheckers)]
        public void StateCheckers_IsSelected()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio1));
            Assert.IsTrue(TestElement.IsSelected());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_StateCheckers)]
        public void StateCheckers_IsNotSelected()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio2));
            Assert.IsFalse(TestElement.IsSelected());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_StateCheckers)]
        public void StateCheckers_IsEnabled()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio2));
            Assert.IsTrue(TestElement.IsEnabled());

            Webdriver.Quit();
        }

        [TestCase(Category=Category_StateCheckers)]
        public void StateCheckers_IsNotEnabled()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            Element? TestElement = ElementFactory.Fetch(By.Id(JSElements.Context2Radio3));
            Assert.IsFalse(TestElement.IsEnabled());

            Webdriver.Quit();
        }

        #region ELEMENT STALENESS

        private const string Category_Staleness = "Element Staleness";
        [TestCase(Category=Category_Staleness)]
        public void ReacquireElementOnStaleException()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            bool Success = ElementFactory.TryFetch(By.CssSelector("div#" + JSElements.Context1Div3), out Element? TestElement, out WebDriverException? Error);
            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);

            Webdriver.ExecuteAsyncScript($@"
                RemoveLastChildOfParent(Elements.Context1());
                arguments[arguments.length - 1]();
                await Wait(3000);
                let NewElement = document.createElement('div');
                NewElement.id = '{JSElements.Context1Div3}';
                Elements.Context1().appendChild(NewElement);
            ");

            var Timer = Stopwatch.StartNew();
            string ElementId = TestElement.GetId();
            Timer.Stop();

            Webdriver.Quit();
            Assert.AreEqual(ElementId, JSElements.Context1Div3);
            Assert.Greater(Timer.ElapsedMilliseconds, 3000, "Expected it to take around 3 seconds to reacquire the element due to the staleness");
        }

        [TestCase(Category=Category_Staleness)]
        public void DoNotReacquireElementOnStaleException()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver);

            bool Success = ElementFactory.TryFetch(By.CssSelector("div#" + JSElements.Context1Div3), out Element? TestElement, out WebDriverException? Error);
            Assert.True(Success);
            Assert.NotNull(TestElement);
            Assert.Null(Error);

            TestElement.DoNotReacquireElementIfStale();

            Webdriver.ExecuteAsyncScript($@"
                RemoveLastChildOfParent(Elements.Context1());
                arguments[arguments.length - 1]();
            ");

            var Timer = Stopwatch.StartNew();
            var Exception = Assert.Throws<TMech.Core.Exceptions.ElementInteractionException>(() => TestElement.GetId());
            Assert.That(Exception.InnerException, Is.TypeOf<StaleElementReferenceException>());
            
            Timer.Stop();

            Webdriver.Quit();
            Assert.Greater(Timer.ElapsedMilliseconds, 3000, "Expected it to take around 3 seconds for the action to fail due to the staleness");
        }

        #endregion

        #endregion
    }
}
