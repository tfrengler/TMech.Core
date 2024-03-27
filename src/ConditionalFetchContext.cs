using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMech.Core.Exceptions;

namespace TMech.Core
{
    public sealed class ConditionalFetchContext
    {
        internal ConditionalFetchContext(ISearchContext searchContext, IFetchContext fetchContext, IJavaScriptExecutor jsExecutor, By locator, TimeSpan timeout, int pollingInterval)
        {
            SearchContext = searchContext;
            WrappedContext = fetchContext;
            Locator = locator;
            Timeout = fetchContext.Timeout;
            PollingInterval = fetchContext.PollingInterval;
            JsExecutor = jsExecutor;

            Constraints = new();
        }

        private readonly IFetchContext WrappedContext;
        private readonly By Locator;
        private readonly TimeSpan Timeout;
        private readonly int PollingInterval;
        private readonly IJavaScriptExecutor JsExecutor;
        private ISearchContext SearchContext;

        private readonly List<Func<IWebElement,IJavaScriptExecutor,bool>> Constraints;
        /*
        public ConditionalFetchContext IsDisplayed()
        {
            Constraints.Add(element =>
            {
                return element.Displayed;
            });

            return this;
        }

        public ConditionalFetchContext IsNotDisplayed()
        {
            Constraints.Add(element =>
            {
                return !element.Displayed;
            });

            return this;
        }

        public ConditionalFetchContext IsNotEnabled()
        {
            Constraints.Add(element =>
            {
                return !element.Enabled;
            });

            return this;
        }

        public ConditionalFetchContext IsEnabled()
        {
            Constraints.Add(element =>
            {
                return element.Enabled;
            });

            return this;
        }

        public ConditionalFetchContext IsSelected()
        {
            Constraints.Add(element =>
            {
                return element.Selected;
            });

            return this;
        }

        public ConditionalFetchContext IsNotSelected()
        {
            Constraints.Add(element =>
            {
                return !element.Selected;
            });

            return this;
        }

        public ConditionalFetchContext AttributeIsEqualTo(string attributeName, string attributeValue)
        {
            Constraints.Add(element =>
            {
                string Value = element.GetAttribute(attributeName);
                return Value.Equals(attributeValue, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext AttributeStartsWith(string attributeName, string attributeValue)
        {
            Constraints.Add(element =>
            {
                string Value = element.GetAttribute(attributeName);
                return Value.StartsWith(attributeValue, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext AttributeEndsWith(string attributeName, string attributeValue)
        {
            Constraints.Add(element =>
            {
                string Value = element.GetAttribute(attributeName);
                return Value.EndsWith(attributeValue, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext AttributeContains(string attributeName, string attributeValue)
        {
            Constraints.Add(element =>
            {
                string Value = element.GetAttribute(attributeName);
                return Value.Contains(attributeValue, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext AttributeHasContent(string attributeName)
        {
            Constraints.Add(element =>
            {
                string Value = element.GetAttribute(attributeName);
                return Value.Trim().Length > 0;
            });

            return this;
        }

        public ConditionalFetchContext ContentIsEqualTo(string text)
        {
            Constraints.Add(element =>
            {
                return element.Text.Equals(text, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext ContentIsNotEqualTo(string text)
        {
            Constraints.Add(element =>
            {
                return !element.Text.Equals(text, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext ContentStartsWith(string text)
        {
            Constraints.Add(element =>
            {
                return element.Text.StartsWith(text, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext ContentEndsWith(string text)
        {
            Constraints.Add(element =>
            {
                return element.Text.EndsWith(text, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext ContentContains(string text)
        {
            Constraints.Add(element =>
            {
                return element.Text.Contains(text, StringComparison.InvariantCulture);
            });

            return this;
        }

        public ConditionalFetchContext HasContent()
        {
            Constraints.Add(element =>
            {
                return element.Text.Trim().Length > 0;
            });

            return this;
        }

        public ConditionalFetchContext IsClickable()
        {
            Constraints.Add(element =>
            {
                return element.Enabled && element.Displayed;
            });

            return this;
        }*/

        public ConditionalFetchContext DoesNotExist()
        {
            Constraints.Add((element,jsExecutor) =>
            {
                return SearchContext.FindElements(Locator).Count == 0;
            });

            return this;
        }

        public IElement? Fetch()
        {
            var Timer = Stopwatch.StartNew();
            WebDriverException? LatestException = null;

            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    IWebElement Element = SearchContext.FindElement(Locator);
                    bool ConstraintsSatisfied = true;

                    for(int Index = 0; Index < Constraints.Count; Index++)
                    {
                        ConstraintsSatisfied = Constraints[Index](Element, JsExecutor);
                        if (ConstraintsSatisfied == false)
                        {
                            continue;
                        }
                    }

                    return new Element((WebElement)Element, WrappedContext, Locator, SearchContext, JsExecutor, false);
                }
                catch (WebDriverException error)
                {
                    LatestException = error;
                    Thread.Sleep(PollingInterval);
                    if (error is StaleElementReferenceException && WrappedContext.Parent is not null)
                    {
                        SearchContext = WrappedContext.Parent.Reacquire(false).WrappedElement;
                    }
                }
            }

            throw new FetchElementException(Locator, Timeout, LatestException!);
        }
    }
}
