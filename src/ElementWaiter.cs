
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using OpenQA.Selenium;

namespace TMech.Core
{
    /// <summary>Utility-class used to fetch elements conditionally. Is spawned by <see cref='ElementFactory.TryFetchWhen'/> but can be instantiated manually if desired.<br/>
    /// This class is ideal not only for fetching elements according to common conditions but for building assertions on top of.
    /// </summary>
    public sealed class ElementWaiter
    {
        public ElementWaiter(ElementFactory factory, By locator, ISearchContext context, TimeSpan timeout)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (locator is null) throw new ArgumentNullException(nameof(locator));
            if (context is null) throw new ArgumentNullException(nameof(context));

            WrappedFactory = factory;
            Locator = locator;
            SearchContext = context;
            Timeout = timeout;
        }

        private ElementWaiter() {}

        private bool ThrowExceptionOnTimeout;
        private readonly ElementFactory WrappedFactory;
        private readonly By Locator;
        private readonly ISearchContext SearchContext;
        private readonly TimeSpan Timeout;

        // Out param 'error' can be non-null regardless of whether the timeout is reached (whether the method returns true or false)
        // Out param 'element' is always null if the timeout is reached (method returns false)
        // The method returns false ONLY if predicate returns true before the timeout, otherwise true to indicate success
        private bool InternalFetchWhen(out ExceptionDispatchInfo? error, out Element? element, Func<IWebElement, bool> predicate, string actionDescriptionForTimeout)
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

            if (!ThrowExceptionOnTimeout) return false;

            string FinalErrorMessage = string.Empty;
            if (error is not null) FinalErrorMessage = ":" + Environment.NewLine + "---------------| LAST EXCEPTION:" + Environment.NewLine + error.SourceException.Message;
            throw new TimeoutException($"Timed out ({Timeout}) trying to fetch element ({Locator.Mechanism} | {Locator.Criteria}) once {actionDescriptionForTimeout}{FinalErrorMessage}");
        }

        /// <summary>
        /// Configures this instance to throw a <see cref='TimeoutException'/> if the timeout is reached instead of returning false.
        /// </summary>
        public ElementWaiter ThrowOnTimeout()
        {
            ThrowExceptionOnTimeout = true;
            return this;
        }

        public bool IsDisplayed(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Displayed, "it was displayed");
        }

        public bool IsNotDisplayed(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => !element.Displayed, "it was not displayed");
        }

        public bool IsNotEnabled(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => !element.Enabled, "it was disabled");
        }

        public bool IsEnabled(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Enabled, "it was enabled");
        }

        public bool IsSelected(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Selected, "it was selected");
        }

        public bool IsNotSelected(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => !element.Selected, "it was deselected");
        }

        public bool AttributeIsEqualTo(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName) == attributeValue, $"once attribute '{attributeName}' was equal to '{attributeValue}'");
        }

        public bool AttributeStartsWith(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName).StartsWith(attributeValue), $"once attribute '{attributeName}' started with '{attributeValue}'");
        }

        public bool AttributeEndsWith(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName).EndsWith(attributeValue), $"once attribute '{attributeName}' ended with '{attributeValue}'");
        }

        public bool AttributeContains(out ExceptionDispatchInfo? error, out Element? element, string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(out error, out element, element => element.GetAttribute(attributeName).Contains(attributeValue), $"once attribute '{attributeName}' contained '{attributeValue}'");
        }

        public bool ContentIsEqualTo(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text == text, $"once its content was equal to '{text}'");
        }

        public bool ContentIsNotEqualTo(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text != text, $"once its content was not equal to '{text}'");
        }

        public bool ContentStartsWith(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.StartsWith(text), $"once its content started with '{text}'");
        }

        public bool ContentEndsWith(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.EndsWith(text), $"once its content ended with '{text}'");
        }

        public bool ContentContains(out ExceptionDispatchInfo? error, out Element? element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            return InternalFetchWhen(out error, out element, element => element.Text.Contains(text), $"once its content contained '{text}'");
        }

        public bool HasContent(out ExceptionDispatchInfo? error, out Element? element)
        {
            return InternalFetchWhen(out error, out element, element => element.Text.Trim().Length > 0, $"once its content was not empty");
        }

    }

}