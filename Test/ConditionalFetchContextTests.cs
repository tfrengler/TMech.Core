using NUnit.Framework;
using OpenQA.Selenium;
using System.Diagnostics;
using TMech.Elements;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class ConditionalFetchContextTests
    {
        private static System.TimeSpan ExpectedDelayTime = System.TimeSpan.FromMilliseconds(GlobalSetup.FetchContextTimeoutMinus1Sec - 100);

        #region DISPLAYED

        [TestCase(Category = "ConditionalFetchContext = Displayed")]
        public void IsDisplayed()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsHideElement(JSElements.Context1Div1);
                Chrome.JsShowElement(JSElements.Context1Div1, GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div1))
                                    .IsDisplayed();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = "ConditionalFetchContext = Displayed")]
        public void IsNotDisplayed()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsHideElement(JSElements.Context1Div1, GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div1))
                                    .IsNotDisplayed();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        #endregion

        #region ENABLED

        [TestCase(Category = "ConditionalFetchContext = Enabled")]
        public void IsEnabled()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsDisableElement(JSElements.Context2Textarea);
                Chrome.JsEnableElement(JSElements.Context2Textarea, GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context2Textarea))
                                    .IsEnabled();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = "ConditionalFetchContext = Enabled")]
        public void IsNotEnabled()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsDisableElement(JSElements.Context2Textarea, GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context2Textarea))
                                    .IsNotEnabled();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        #endregion

        #region SELECTED

        [TestCase(Category = "ConditionalFetchContext = Selected")]
        public void IsSelected()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsDeselectElement(JSElements.Context2Checkbox);
                Chrome.JsSelectElement(JSElements.Context2Checkbox, GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context2Checkbox))
                                    .IsSelected();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = "ConditionalFetchContext = Selected")]
        public void IsDeselected()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsSelectElement(JSElements.Context2Checkbox);
                Chrome.JsDeselectElement(JSElements.Context2Checkbox, GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context2Checkbox))
                                    .IsNotSelected();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        #endregion

        #region ATTRIBUTES

        const string Category_Attributes = "ConditionalFetchContext = Attributes";
        [TestCase(Category = Category_Attributes)]
        public void AttributeEquals()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementAttribute(JSElements.Context1Div2, "data-regtest", "RegTest", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div2))
                                    .AttributeIsEqualTo("data-regtest", "RegTest");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Attributes)]
        public void AttributeStartsWith()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementAttribute(JSElements.Context1Div2, "data-regtest", "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div2))
                                    .AttributeStartsWith("data-regtest", "1-2");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Attributes)]
        public void AttributeEndsWith()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementAttribute(JSElements.Context1Div2, "data-regtest", "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div2))
                                    .AttributeEndsWith("data-regtest", "2-3");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Attributes)]
        public void AttributeContains()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementAttribute(JSElements.Context1Div2, "data-regtest", "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div2))
                                    .AttributeContains("data-regtest", "-2-");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Attributes)]
        public void AttributeHasContent()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementAttribute(JSElements.Context1Div2, "data-regtest", "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div2))
                                    .AttributeHasContent("data-regtest");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        #endregion

        #region CONTENT

        const string Category_Content = "ConditionalFetchContext = Content";
        [TestCase(Category = Category_Content)]
        public void ContentIsEqualTo()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementText(JSElements.Context1Div3, "RegTest", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div3))
                                    .ContentIsEqualTo("RegTest");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Content)]
        public void ContentIsNotEqualTo()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementText(JSElements.Context1Div3, "RegTest");
                Chrome.JsChangeElementText(JSElements.Context1Div3, "SomethingElse", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div3))
                                    .ContentIsNotEqualTo("RegTest");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Content)]
        public void ContentStartsWith()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementText(JSElements.Context1Div3, "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div3))
                                    .ContentStartsWith("1-2");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Content)]
        public void ContentEndsWith()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementText(JSElements.Context1Div3, "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div3))
                                    .ContentEndsWith("2-3");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Content)]
        public void ContentContains()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementText(JSElements.Context1Div3, "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div3))
                                    .ContentContains("-2-");
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        [TestCase(Category = Category_Content)]
        public void HasContent()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsChangeElementText(JSElements.Context1Div3, string.Empty);
                Chrome.JsChangeElementText(JSElements.Context1Div3, "1-2-3", GlobalSetup.FetchContextTimeoutMinus1Sec);

                Element? TheElement = null;
                var Timer = Stopwatch.StartNew();

                Assert.DoesNotThrow(() => {
                    TheElement = TestContext
                                    .FetchWhen(By.Id(JSElements.Context1Div3))
                                    .HasContent();
                });

                Assert.That(Timer.Elapsed, Is.GreaterThan(ExpectedDelayTime));
            }
        }

        #endregion
    }
}