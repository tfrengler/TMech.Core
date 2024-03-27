using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using TMech.Core;

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
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);
                var Exists = TestContext.Exists(By.Id(JSElements.Context1), out IElement? result);

                Assert.That(Exists, Is.True);
                Assert.That(result, Is.Not.Null);
            }
        }

        [TestCase(Category = Category_Exists)]
        public void Exists_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);

                var Timer = Stopwatch.StartNew();
                var Exists = TestContext.Exists(By.Id("gnargle"), out IElement? result);
                Timer.Stop();

                Assert.That(Exists, Is.False);
                Assert.That(result, Is.Null);
            }
        }

        [TestCase(Category = Category_Exists)]
        public void Exists_Fail_Staleness()
        {
            using (var Chrome = new ChromeContext())
            {
                var InitialContext = FetchContext.Create(Chrome.ChromeDriver);
                var TestContext = InitialContext.Fetch(By.Id(JSElements.Context3Div3));
                Chrome.JsRemoveLastChildOfParent(JSElements.Context3);

                var Timer = Stopwatch.StartNew();
                bool? Exists = null;
                IElement? Result = null;

                var TheError = Assert.Throws<TMech.Core.Exceptions.FetchContextException>(() =>
                {
                    Exists = TestContext.Within().Exists(By.Id(JSElements.Context3Div3Span1), out Result);
                });

                Timer.Stop();

                Assert.That(TheError, Is.Not.Null);
                Assert.That(Exists, Is.Null);
                Assert.That(Result, Is.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
            }
        }

        #endregion

        private const string Category_Fetch = "FetchContext = Fetch";
        [TestCase(Category = Category_Fetch)]
        public void Fetch_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);
                IElement Result = TestContext.Fetch(By.Id(JSElements.Context1));

                Assert.That(Result, Is.Not.Null);
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void Fetch_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);
                var Timer = Stopwatch.StartNew();
                Assert.Throws<TMech.Core.Exceptions.FetchElementException>(() => TestContext.Fetch(By.Id("gnargle")));
                Timer.Stop();

                Assert.That(Timer.Elapsed, Is.GreaterThan(GlobalSetup.DefaultFetchContextTimeout));
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void Fetch_Delayed_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);
                Chrome.JsCopyLastChildOfParentAndAppend(JSElements.Context1, 4000);

                var Timer = Stopwatch.StartNew();
                IElement Result = TestContext.Fetch(By.Id(JSElements.Context1Div3 + '0'));
                Timer.Stop();

                Assert.That(Result, Is.Not.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(TimeSpan.FromMilliseconds(3800d)));
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void TryFetch_Delayed_Success()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);
                Chrome.JsCopyLastChildOfParentAndAppend(JSElements.Context1, 4000);

                var Timer = Stopwatch.StartNew();
                bool Success = TestContext.TryFetch(By.Id(JSElements.Context1Div3 + '0'), out IElement? element, out Exception? error);
                Timer.Stop();

                Assert.That(Success, Is.True);
                Assert.That(element, Is.Not.Null);
                Assert.That(error, Is.Null);
                Assert.That(Timer.Elapsed, Is.GreaterThan(TimeSpan.FromMilliseconds(3800d)));
            }
        }

        [TestCase(Category = Category_Fetch)]
        public void TryFetch_Fail()
        {
            using (var Chrome = new ChromeContext())
            {
                var TestContext = FetchContext.Create(Chrome.ChromeDriver);
                bool Success = TestContext.TryFetch(By.Id("gnargle"), out IElement? element, out Exception? error);

                Assert.That(Success, Is.False);
                Assert.That(element, Is.Null);
                Assert.That(error, Is.Not.Null);
            }
        }
    }
}
