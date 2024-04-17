using NUnit.Framework;
using OpenQA.Selenium;
using TMech.Elements;
using TMech.Elements.Exceptions;


namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class ElementTests
    {
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

                Assert.Throws<ElementInteractionException>(() => {
                    TheElement.ClickUntil(element =>
                    {
                        string TheText = element.WrappedElement.GetAttribute("name");
                        return TheText == "RegTest";
                    });
                });
            }
        }
    }
}