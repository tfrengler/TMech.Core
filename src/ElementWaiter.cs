
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TMech.Core
{
    /// <summary>Utility-class used to fetch elements conditionally. Is spawned by ElementFactory.TryFetchWhen and is not meant to be instantiated directly.</summary>
    public sealed class ElementWaiter
    {
        public ElementWaiter(ElementFactory factory, By locator, ISearchContext context, TimeSpan timeout)
        {
            WrappedFactory = factory;
            Locator = locator;
            SearchContext = context;
            Timeout = timeout;
        }

        private ElementWaiter() {}

        private readonly ElementFactory WrappedFactory;
        private readonly By Locator;
        private readonly ISearchContext SearchContext;
        private readonly TimeSpan Timeout;

        // 'error' can be non-null regardless of whether the timeout is reached, but 'element' is always null if the timeout is reached
        // and the method return true only if predicate returns true before the timeout, otherwise false
        private bool InternalFetchWhen(out ExceptionDispatchInfo? error, out Element? element, Func<IWebElement, bool> predicate)
        {
            var Timer = Stopwatch.StartNew();
            error = null;
            element = null;

            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    IWebElement Element = SearchContext.FindElement(Locator);
                    if (!predicate(Element)) continue;

                    element = new Element((WebElement)Element, WrappedFactory, Locator, SearchContext, false);
                    return true;
                }
                catch (WebDriverException exception)
                {
                    error = ExceptionDispatchInfo.Capture(exception);
                    Thread.Sleep(100);
                }
            }

            return false;
        }

        public bool IsDisplayed(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Displayed);
        }

        public bool IsNotDisplayed(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => !element.Displayed);
        }

        public bool IsNotEnabled(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => !element.Enabled);
        }

        public bool IsEnabled(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Enabled);
        }

        public bool IsSelected(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Selected);
        }

        public bool IsNotSelected(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => !element.Selected);
        }

        public bool AttributeIsEqualTo(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName) == attributeValue);
        }

        public bool AttributeStartsWith(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName).StartsWith(attributeValue));
        }

        public bool AttributeEndsWith(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName).EndsWith(attributeValue));
        }

        public bool AttributeContains(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName).Contains(attributeValue));
        }

        public bool ContentIsEqualTo(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text == text);
        }

        public bool ContentIsNotEqualTo(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text != text);
        }

        public bool ContentStartsWith(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.StartsWith(text));
        }

        public bool ContentEndsWith(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.EndsWith(text));
        }

        public bool ContentContains(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.Contains(text));
        }

        public bool HasContent(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.Trim().Length > 0);
        }

    }

}