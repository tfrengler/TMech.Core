
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Threading;
using TMech.Core.Exceptions;

namespace TMech.Core.Elements
{
    /// <summary>Utility-class used to fetch elements conditionally. Is spawned by <see cref='ElementFactory.FetchWhen'/> but can be instantiated manually if desired.<br/>
    /// Uses the factory to fetch elements but only returns them when certain conditions are met. All methods throw an exception upon timeout.
    /// </summary>
    public sealed class ElementWaiter
    {
        public ElementWaiter(ElementFactory factory, By locator, ISearchContext context, TimeSpan timeout)
        {
            Debug.Assert(factory is not null);
            Debug.Assert(locator is not null);
            Debug.Assert(context is not null);

            WrappedFactory = factory;
            Locator = locator;
            SearchContext = context;
            Timeout = timeout;
        }

        private readonly ElementFactory WrappedFactory;
        private readonly By Locator;
        private readonly ISearchContext SearchContext;
        private readonly TimeSpan Timeout;

        private Element? InternalFetchWhen(Func<IWebElement, bool> predicate, string actionDescriptionForTimeout)
        {
            var Timer = Stopwatch.StartNew();
            FetchElementException? Error = null;

            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    var Element = WrappedFactory.Fetch(Locator, Timeout);
                    var WrappedElement = Element.WrappedElement;

                    if (!predicate(WrappedElement)) continue;

                    return new Element(WrappedElement, WrappedFactory, Locator, SearchContext, false);
                }
                catch (FetchElementException exception)
                {
                    Error = exception;
                    Thread.Sleep(100);
                }
            }

            if (Error is null)
                throw new ElementWaitException(actionDescriptionForTimeout, Locator, Timeout);

            throw new ElementWaitException(actionDescriptionForTimeout, Locator, Timeout, Error);
        }

        public Element? IsDisplayed()
        {
            return InternalFetchWhen(element => element.Displayed, "it was displayed");
        }

        public Element? IsNotDisplayed()
        {
            return InternalFetchWhen(element => !element.Displayed, "it was not displayed");
        }

        public Element? IsNotEnabled()
        {
            return InternalFetchWhen(element => !element.Enabled, "it was disabled");
        }

        public Element? IsEnabled()
        {
            return InternalFetchWhen(element => element.Enabled, "it was enabled");
        }

        public Element? IsSelected()
        {
            return InternalFetchWhen(element => element.Selected, "it was selected");
        }

        public Element? IsNotSelected()
        {
            return InternalFetchWhen(element => !element.Selected, "it was deselected");
        }

        public Element? AttributeIsEqualTo(string attributeName, string attributeValue)
        {
            if (attributeName is null) throw new ArgumentNullException(nameof(attributeName));
            if (attributeValue is null) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(element => element.GetAttribute(attributeName) == attributeValue, $"once attribute '{attributeName}' was equal to '{attributeValue}'");
        }

        public Element? AttributeStartsWith(string attributeName, string attributeValue)
        {
            if (attributeName is null) throw new ArgumentNullException(nameof(attributeName));
            if (attributeValue is null) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(element => element.GetAttribute(attributeName).StartsWith(attributeValue), $"once attribute '{attributeName}' started with '{attributeValue}'");
        }

        public Element? AttributeEndsWith(string attributeName, string attributeValue)
        {
            if (attributeName is null) throw new ArgumentNullException(nameof(attributeName));
            if (attributeValue is null) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(element => element.GetAttribute(attributeName).EndsWith(attributeValue), $"once attribute '{attributeName}' ended with '{attributeValue}'");
        }

        public Element? AttributeContains(string attributeName, string attributeValue)
        {
            if (attributeName is null) throw new ArgumentNullException(nameof(attributeName));
            if (attributeValue is null) throw new ArgumentNullException(nameof(attributeValue));

            return InternalFetchWhen(element => element.GetAttribute(attributeName).Contains(attributeValue), $"once attribute '{attributeName}' contained '{attributeValue}'");
        }

        public Element? ContentIsEqualTo(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));

            return InternalFetchWhen(element => element.Text == text, $"once its content was equal to '{text}'");
        }

        public Element? ContentIsNotEqualTo(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));

            return InternalFetchWhen(element => element.Text != text, $"once its content was not equal to '{text}'");
        }

        public Element? ContentStartsWith(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));

            return InternalFetchWhen(element => element.Text.StartsWith(text), $"once its content started with '{text}'");
        }

        public Element? ContentEndsWith(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));

            return InternalFetchWhen(element => element.Text.EndsWith(text), $"once its content ended with '{text}'");
        }

        public Element? ContentContains(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));

            return InternalFetchWhen(element => element.Text.Contains(text), $"once its content contained '{text}'");
        }

        public Element? HasContent()
        {
            return InternalFetchWhen(element => element.Text.Trim().Length > 0, $"once its content was not empty");
        }

    }

}