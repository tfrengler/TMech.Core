using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.Linq;
using TMech.Core.Elements;
using TMech.Core.Elements.Extensions;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public sealed class CompoundTests
    {
        private const string Category_CompoundTests = "CompoundTests";
        [TestCase(Category = Category_CompoundTests)]
        public void NestedFetches_WithWaits()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver, TimeSpan.FromSeconds(5.0d));

            Element TestElement = null;

            Webdriver.ExecuteAsyncScript(@"
                let LabelElement = document.querySelector('label[for=\'Context2-DateTimeLocal-Id\']');
                HideElement(Elements.Context2());
                ChangeText(LabelElement, '');
                arguments[arguments.length - 1]();
                await Wait(3100);
                ShowElement(Elements.Context2());
                await Wait(3100);
                ChangeText(LabelElement, 'test');
            ");

            var Timer = Stopwatch.StartNew();

            Assert.DoesNotThrow(() =>
            {
                TestElement = ElementFactory
                    .FetchWhen(By.Id(JSElements.Context2))
                        .IsDisplayed()
                    .Elements()
                    .FetchAll(By.TagName("div")).Last()
                    .Elements()
                    .FetchWhen(By.TagName("label"))
                        .HasContent();
            });

            Timer.Stop();

            Assert.Greater(Timer.ElapsedMilliseconds, 6000, "Expected it to take around or more than 6 seconds to complete the chained calls");
            Assert.AreEqual(TestElement.WrappedElement.GetAttribute("for"), JSElements.Context2DateTimeLocal);
        }

        [TestCase(Category = Category_CompoundTests)]
        public void NestedFetches_WithRelationalExtensions()
        {
            ChromeDriver Webdriver = Shared.SetUpWebdriverAndGoToTestPage();
            var ElementFactory = new ElementFactory(Webdriver, TimeSpan.FromSeconds(5.0d));

            Element TestElement = null;

            Webdriver.ExecuteAsyncScript(@"
                HideElement(Elements.Context2());
                HideElement(Elements.Context2DateTimeLocal());
                arguments[arguments.length - 1]();
                await Wait(3100);
                ShowElement(Elements.Context2());
            ");

            var Timer = Stopwatch.StartNew();

            Assert.DoesNotThrow(() =>
            {
                TestElement = ElementFactory
                    .FetchWhen(By.Id(JSElements.Context2))
                        .IsDisplayed()
                    .FetchDescendants("div")
                        .Last()
                    .FetchChildren("label")
                        .First()
                    .FetchNextSibling("input");

            });

            Timer.Stop();

            Assert.Greater(Timer.ElapsedMilliseconds, 3000, "Expected it to take around or more than 3 seconds to complete the chained calls");
            Assert.AreEqual(TestElement.GetId(), JSElements.Context2DateTimeLocal);
        }
    }
}
