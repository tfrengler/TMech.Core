using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Threading;
using TMech.Elements;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class FetchContextTests
    {
        #region EXISTS

        private const string Category_Exists = "FetchContext = Exists";
        [TestCase(Category = Category_Exists)]
        public void Exists_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var Exists = TestContext.Exists(By.Id(JSElements.Context1), out Element? result);

                Assert.That(Exists, Is.True);
                Assert.That(result, Is.Not.Null);
            }
        }

        [TestCase(Category = Category_Exists)]
        public void Exists_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var Timer = Stopwatch.StartNew();
                var Exists = TestContext.Exists(By.Id("gnargle"), out Element? result);

                Assert.That(Exists, Is.False);
                Assert.That(result, Is.Null);
            }
        }

        [TestCase(Category = Category_Exists)]
        public void Exists_Fail_Staleness()
        {
            using (var Chrome = new ChromeContext())
            {
                var InitialContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TestContext = InitialContext.Fetch(By.Id(JSElements.Context3Div3));
                Chrome.JsRemoveLastChildOfParent(JSElements.Context3);

                var Timer = Stopwatch.StartNew();
                bool? Exists = null;
                Element? Result = null;

                var TheError = Assert.Throws<TMech.Elements.Exceptions.FetchContextException>(() =>
                {
                    Exists = TestContext.Within().Exists(By.Id(JSElements.Context3Div3Span1), out Result);
                });

                Assert.That(TheError, Is.Not.Null);
                Assert.That(Exists, Is.Null);
                Assert.That(Result, Is.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
            }
        }

        #endregion

        #region FETCH

        private const string Category_Fetch = "FetchContext = Fetch";
        [TestCase(Category = Category_Fetch)]
        public void Fetch_Success_Immediate()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Element Result = TestContext.Fetch(By.Id(JSElements.Context1));

                Assert.That(Result, Is.Not.Null);
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void Fetch_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var Timer = Stopwatch.StartNew();
                var TheError = Assert.Throws<TMech.Elements.Exceptions.FetchElementException>(() => TestContext.Fetch(By.Id("gnargle")));

                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
                Assert.That(TheError!.InnerException, Is.Not.Null);
                Assert.That(TheError!.InnerException!.GetType(), Is.EqualTo(typeof(OpenQA.Selenium.NoSuchElementException)));
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void Fetch_Delayed_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsCopyLastChildOfParentAndAppend(JSElements.Context1, GlobalSetup.FetchContextTimeoutMinus1Sec);

                var Timer = Stopwatch.StartNew();
                Element Result = TestContext.Fetch(By.Id(JSElements.Context1Div3 + '0'));

                Assert.That(Result, Is.Not.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(TimeSpan.FromMilliseconds(GlobalSetup.FetchContextTimeoutMinus1Sec)));
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void TryFetch_Delayed_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsCopyLastChildOfParentAndAppend(JSElements.Context1, GlobalSetup.FetchContextTimeoutMinus1Sec);

                var Timer = Stopwatch.StartNew();
                bool Success = TestContext.TryFetch(By.Id(JSElements.Context1Div3 + '0'), out Element? element, out Exception? error);

                Assert.That(Success, Is.True);
                Assert.That(element, Is.Not.Null);
                Assert.That(error, Is.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(TimeSpan.FromMilliseconds(GlobalSetup.FetchContextTimeoutMinus1Sec)));
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void TryFetch_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                bool Success = TestContext.TryFetch(By.Id("gnargle"), out Element? element, out Exception? error);

                Assert.That(Success, Is.False);
                Assert.That(element, Is.Null);
                Assert.That(error, Is.Not.Null);
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void Fetch_Within()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, TimeSpan.FromSeconds(1.0d));
                Element Container = TestContext.Fetch(By.Id(JSElements.StaleContext));

                Assert.Multiple(() =>
                {
                    Assert.That(Container, Is.Not.Null);
                    Assert.DoesNotThrow(() =>
                    {
                        Container.Within().Fetch(By.Id(JSElements.StaleContextChild4));
                    });
                    Assert.Throws<TMech.Elements.Exceptions.FetchElementException>(() =>
                    {
                        Container.Within().Fetch(By.Id(JSElements.Context3Div3));
                    });
                });
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void Fetch_ResolveStaleness_Recursive()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Element TheElement = TestContext
                    .Fetch(By.Id(JSElements.StaleContext))
                    .Within()
                    .Fetch(By.Id(JSElements.StaleContextChild1))
                    .Within()
                    .Fetch(By.Id(JSElements.StaleContextChild2))
                    .Within()
                    .Fetch(By.Id(JSElements.StaleContextChild3))
                    .Within()
                    .Fetch(By.Id(JSElements.StaleContextChild4));

                Assert.That(TheElement, Is.Not.Null);

                Chrome.JsKillAndReRenderStaleContext(0, "RegTest");
                string TheText = string.Empty;
                Thread.Sleep(1000);

                Assert.Throws<OpenQA.Selenium.StaleElementReferenceException>(() =>
                {
                    TheElement.WrappedElement.GetAttribute("textContent");
                });
                Assert.DoesNotThrow(() => {
                    TheText = TheElement.GetAttribute("textContent");
                });
                Assert.That(TheText, Is.EqualTo("RegTest"));
            }
        }

        #endregion

        #region FETCH ALL

        private const string Category_FetchAll = "FetchContext = FetchAll";
        [TestCase(Category = Category_FetchAll)]
        public void FetchAll_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var Results = TestContext.FetchAll(By.CssSelector("div[id^='Context3-Div']"), 3);

                Assert.That(Results, Is.Not.Null);
                Assert.That(Results.Length, Is.EqualTo(3));
                Assert.That(Results, Is.All.Not.Null);
            }
        }

        [TestCase(Category = Category_FetchAll)]
        public void FetchAll_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var Error = Assert.Throws<TMech.Elements.Exceptions.FetchElementException>(() => TestContext.FetchAll(By.CssSelector("div[id^='Context3-Div']"), 4));

                Assert.That(Error!.InnerException, Is.Null);
            }
        }

        [TestCase(Category = Category_FetchAll)]
        public void FetchAll_Delayed_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsCopyLastChildOfParentAndAppend(JSElements.Context3, GlobalSetup.FetchContextTimeoutMinus1Sec);
                
                var Timer = Stopwatch.StartNew();
                var Results = TestContext.FetchAll(By.CssSelector("div[id^='Context3-Div']"), 4);

                Assert.That(Results, Is.Not.Null);
                Assert.That(Results.Length, Is.EqualTo(4));
                Assert.That(Results, Is.All.Not.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(TimeSpan.FromMilliseconds(GlobalSetup.FetchContextTimeoutMinus1Sec)));
            }
        }

        [TestCase(Category = Category_FetchAll)]
        public void TryFetchAll_Delayed_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                Chrome.JsCopyLastChildOfParentAndAppend(JSElements.Context3, GlobalSetup.FetchContextTimeoutMinus1Sec);

                var Timer = Stopwatch.StartNew();
                bool Success = TestContext.TryFetchAll(By.CssSelector("div[id^='Context3-Div']"), out Element[]? Results, 4);

                Assert.That(Success, Is.True);
                Assert.That(Results, Is.Not.Null);
                Assert.That(Results.Length, Is.EqualTo(4));
                Assert.That(Results, Is.Not.All.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(TimeSpan.FromMilliseconds(GlobalSetup.FetchContextTimeoutMinus1Sec)));
            }
        }

        [TestCase(Category = Category_FetchAll)]
        public void TryFetchAll_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);

                var Timer = Stopwatch.StartNew();
                bool Success = TestContext.TryFetchAll(By.CssSelector("div[id^='Context3-Div']"), out Element[] Results, 4);

                Assert.That(Success, Is.False);
                Assert.That(Results, Is.Not.Null);
                Assert.That(Results.Length, Is.EqualTo(0));
                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
            }
        }

        [TestCase(Category = Category_FetchAll)]
        public void FetchAll_Fail_Staleness()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var DueToBeStaleContext = TestContext.Fetch(By.Id(JSElements.Context3Div3));
                Chrome.JsRemoveLastChildOfParent(JSElements.Context3);

                var Timer = Stopwatch.StartNew();
                var Error = Assert.Throws<TMech.Elements.Exceptions.FetchElementException>(() => DueToBeStaleContext.Within().FetchAll(By.TagName("span")));

                Assert.That(Error!.InnerException, Is.Not.Null);
                Assert.That(Error.InnerException!.GetType(), Is.EqualTo(typeof(TMech.Elements.Exceptions.ReacquireElementException)));
                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
            }
        }

        #endregion

        #region AMOUNT OF

        private const string Category_AmountOf = "FetchContext = Amount of";
        [TestCase(Category = Category_AmountOf)]
        public void AmountOf_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                int AmountOf = TestContext.AmountOf(By.CssSelector("div[id^='Context3-Div']"));

                Assert.That(AmountOf, Is.EqualTo(3));
            }
        }

        [TestCase(Category = Category_AmountOf)]
        public void AmountOf_Fail_Staleness()
        {
            using (var Chrome = new ChromeContext())
            {
                var InitialContext = FetchContext.Create(Chrome.ChromeDriver, GlobalSetup.DefaultFetchContextTimeout);
                var TestContext = InitialContext.Fetch(By.Id(JSElements.Context3Div3));
                Chrome.JsRemoveLastChildOfParent(JSElements.Context3);

                var Timer = Stopwatch.StartNew();
                int? AmountOf = null;

                var TheError = Assert.Throws<TMech.Elements.Exceptions.FetchContextException>(() =>
                {
                    AmountOf = TestContext.Within().AmountOf(By.Id(JSElements.Context3Div3Span1));
                });

                Timer.Stop();

                Assert.That(TheError, Is.Not.Null);
                Assert.That(TheError!.InnerException, Is.Not.Null);
                Assert.That(TheError!.InnerException!.GetType(), Is.EqualTo(typeof(TMech.Elements.Exceptions.ReacquireElementException)));
                Assert.That(AmountOf, Is.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
            }
        }


        #endregion
    }
}
