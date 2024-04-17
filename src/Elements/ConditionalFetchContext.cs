using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Threading;
using TMech.Elements.Exceptions;

namespace TMech.Elements
{
    /// <summary>Utility-class used to fetch elements conditionally. Is spawned by <see cref='FetchContext.FetchWhen'/> but can be instantiated manually if desired.<br/>
    /// Uses the factory to fetch elements but only returns them when certain conditions are met. All methods throw an exception upon timeout.
    /// </summary>
    public sealed class ConditionalFetchContext
    {
        internal ConditionalFetchContext(FetchContext context, IJavaScriptExecutor javaScriptExecutor, By locator, TimeSpan timeout)
        {
            Debug.Assert(context is not null);
            Debug.Assert(locator is not null);

            WrappedContext = context;
            JavaScriptExecutor = javaScriptExecutor;
            Locator = locator;
            Timeout = timeout;
        }

        public FetchContext WrappedContext { get; }
        public By Locator { get; }
        public TimeSpan Timeout { get; }
        public IJavaScriptExecutor JavaScriptExecutor { get; }

        private Element InternalFetchWhen(Func<IWebElement, bool> predicate, string actionDescriptionForTimeout)
        {
            var Timer = Stopwatch.StartNew();
            Exception? Error = null;

            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    var Element = WrappedContext.Fetch(Locator, Timeout);
                    if (!predicate(Element.UnWrap())) continue;

                    return new Element(Element.UnWrap(), WrappedContext, Locator, WrappedContext.SearchContext, JavaScriptExecutor, false);
                }
                catch (WebDriverException exception)
                {
                    Error = exception;
                    Thread.Sleep((int)WrappedContext.PollingInterval);
                }
            }

            throw new ConditionalFetchException(actionDescriptionForTimeout, Locator, Timeout, Error!);
        }

        /// <summary>
        /// Fetches the element once it is considered displayed. This typically means:
        /// <list type='bullet'>
        /// <item>Not obscured (fully or partially) by other elements</item>
        /// <item>Has a height and width greater than 0</item>
        /// </list>
        /// NOTE: The element does NOT have to be in the viewport!
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is displayed</returns>
        public Element IsDisplayed()
        {
            return InternalFetchWhen(element => element.Displayed, "it was displayed");
        }

        /// <summary>
        /// Fetches the element once it is no longer displayed. Note that elements not in the viewport are still considered visible.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is no longer displayed</returns>
        public Element IsNotDisplayed()
        {
            return InternalFetchWhen(element => !element.Displayed, "it was not displayed");
        }

        /// <summary>
        /// Fetches the element once it is disabled. This is only relevant for elements that support the disabled-attribute.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is disabled</returns>
        public Element IsNotEnabled()
        {
            return InternalFetchWhen(element => !element.Enabled, "it was disabled");
        }

        /// <summary>
        /// Fetches the element once it is enabled. This is only relevant for elements that support the disabled-attribute.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is enabled</returns>
        public Element IsEnabled()
        {
            return InternalFetchWhen(element => element.Enabled, "it was enabled");
        }

        /// <summary>
        /// Fetches the element once it has been selected. This is only relevant for elements that can be selected, such as radio-buttons and checkboxes.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is selected</returns>
        public Element IsSelected()
        {
            return InternalFetchWhen(element => element.Selected, "it was selected");
        }

        /// <summary>
        /// Fetches the element once it has been deselected. This is only relevant for elements that can be selected, such as radio-buttons and checkboxes.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is deselected</returns>
        public Element IsNotSelected()
        {
            return InternalFetchWhen(element => !element.Selected, "it was deselected");
        }

        /// <summary>
        /// Fetches the element once a given attribute is equal to the value you passed.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The value the attribute must be equal to.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its attribute is equal to the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element AttributeIsEqualTo(string attributeName, string attributeValue)
        {
            ArgumentNullException.ThrowIfNull(attributeName);
            ArgumentNullException.ThrowIfNull(attributeValue);

            return InternalFetchWhen(element => element.GetAttribute(attributeName) == attributeValue, $"once attribute '{attributeName}' was equal to '{attributeValue}'");
        }

        /// <summary>
        /// Fetches the element once a given attribute starts with the value you passed.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The value the attribute must start with.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its attribute starts with the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element AttributeStartsWith(string attributeName, string attributeValue)
        {
            ArgumentNullException.ThrowIfNull(attributeName);
            ArgumentNullException.ThrowIfNull(attributeValue);

            return InternalFetchWhen(element => element.GetAttribute(attributeName).StartsWith(attributeValue), $"once attribute '{attributeName}' started with '{attributeValue}'");
        }

        /// <summary>
        /// Fetches the element once a given attribute is ends with the value you passed.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The value the attribute must end with.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its attribute ends with the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element AttributeEndsWith(string attributeName, string attributeValue)
        {
            ArgumentNullException.ThrowIfNull(attributeName);
            ArgumentNullException.ThrowIfNull(attributeValue);

            return InternalFetchWhen(element => element.GetAttribute(attributeName).EndsWith(attributeValue), $"once attribute '{attributeName}' ended with '{attributeValue}'");
        }

        /// <summary>
        /// Fetches the element once a given attribute contains the value you passed.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The value the attribute must contain.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its attribute contains the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element AttributeContains(string attributeName, string attributeValue)
        {
            ArgumentNullException.ThrowIfNull(attributeName);
            ArgumentNullException.ThrowIfNull(attributeValue);

            return InternalFetchWhen(element => element.GetAttribute(attributeName).Contains(attributeValue), $"once attribute '{attributeName}' contained '{attributeValue}'");
        }

        /// <summary>
        /// Fetches the element once a given attribute has content, ie. is not empty (ignoring whitespace).
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its attribute has a value.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element AttributeHasContent(string attributeName)
        {
            ArgumentNullException.ThrowIfNull(attributeName);

            return InternalFetchWhen(element => element.GetAttribute(attributeName).Trim().Length > 0, $"once attribute '{attributeName}' had a value");
        }

        /// <summary>
        /// Fetches the element once its text (not counting inner HTML) is equal to the value you pass.
        /// </summary>
        /// <param name="text">The value the text content must be equal to.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its text content is equal to the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element ContentIsEqualTo(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            return InternalFetchWhen(element => element.Text == text, $"once its content was equal to '{text}'");
        }

        /// <summary>
        /// Fetches the element once its text (not counting inner HTML) is not equal to the value you pass.
        /// </summary>
        /// <param name="text">The value the text content must not be equal to.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its text content is not equal to the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element ContentIsNotEqualTo(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            return InternalFetchWhen(element => element.Text != text, $"once its content was not equal to '{text}'");
        }

        /// <summary>
        /// Fetches the element once its text (not counting inner HTML) starts with the value you pass.
        /// </summary>
        /// <param name="text">The value the text content must start with.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its text content starts with the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element ContentStartsWith(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            return InternalFetchWhen(element => element.Text.StartsWith(text), $"once its content started with '{text}'");
        }

        /// <summary>
        /// Fetches the element once its text (not counting inner HTML) ends with the value you pass.
        /// </summary>
        /// <param name="text">The value the text content must end with.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its text content ends with the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element ContentEndsWith(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            return InternalFetchWhen(element => element.Text.EndsWith(text), $"once its content ended with '{text}'");
        }

        /// <summary>
        /// Fetches the element once its text (not counting inner HTML) contains the value you pass.
        /// </summary>
        /// <param name="text">The value the text content must contain.</param>
        /// <returns>A reference to the <see cref="Element"/>-instance once its text content contains the value you passed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Element ContentContains(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            return InternalFetchWhen(element => element.Text.Contains(text), $"once its content contained '{text}'");
        }

        /// <summary>
        /// Fetches the element once its text (not counting inner HTML) is not empty, ignoring whitespace.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it has text content.</returns>
        public Element HasContent()
        {
            return InternalFetchWhen(element => element.Text.Trim().Length > 0, $"once its content was not empty");
        }

        /// <summary>
        /// Fetches the element once it is considered clickable (displayed and enabled). Note that this is still not a guarantee that the element is in fact clickable.
        /// </summary>
        /// <returns>A reference to the <see cref="Element"/>-instance once it is considered clickable.</returns>
        public Element IsClickable()
        {
            return InternalFetchWhen(element => (element.Displayed && element.Enabled), $"once it was clickable (displayed and enabled)");
        }
    }
}