using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TMech.Core
{
    public sealed partial class Element : IElement
    {
        internal Element(WebElement wrappedElement, IFetchContext producedBy, By relatedLocator, ISearchContext relatedContext, IJavaScriptExecutor jsExecutor, bool locatedAsMultiple)
        {
            Debug.Assert(wrappedElement is not null);
            Debug.Assert(producedBy is not null);
            Debug.Assert(relatedLocator is not null);
            Debug.Assert(relatedContext is not null);

            WrappedElement = wrappedElement;
            ProducedBy = producedBy;
            RelatedLocator = relatedLocator;
            RelatedContext = relatedContext;
            LocatedAsMultiple = locatedAsMultiple;
            JavaScriptExecutor = jsExecutor;
            Identifier = Guid.NewGuid();
        }

        private readonly IJavaScriptExecutor JavaScriptExecutor;

        public WebElement WrappedElement { get; private set; }
        public IFetchContext ProducedBy { get; }
        public By RelatedLocator { get; }
        public ISearchContext RelatedContext { get; private set; }
        public bool LocatedAsMultiple { get; }
        public Guid Identifier { get; }

        /// <summary>
        /// Attempts to reacquire the underlying <see cref="WebElement"/> this instance is wrapped around. Note that if the <see cref="RelatedContext"/> is another element, and that one is stale, this will fail indefinitely as staleness is not recursively resolved.<br/>
        /// Note that elements acquired via <see cref="ElementFactory.FetchAll"/> cannot be reacquired.
        /// </summary>
        /// <param name="throwOnError">Whether to throw an exception if the element cannot be reacquired.</param>
        public IElement Reacquire(bool throwOnError)
        {
            // Cannot reacquire if located through FetchAll because re-applying
            // the locator does not get us the same element since it was indexed
            // out of the resulting array
            if (LocatedAsMultiple) return this;

            try
            {
                WrappedElement = (WebElement)RelatedContext.FindElement(RelatedLocator);
            }
            catch (WebDriverException)
            {
                if (throwOnError) throw;
            }

            return this;
        }

        /// <summary>Attempts to click the element.</summary>
        public IElement Click()
        {
            _ = InternalRetryActionInvoker("Failed to click element", () =>
            {
                WrappedElement.Click();
                return true;
            });

            return this;
        }

        /// <summary>Repeatedly clicks the element until a certain condition is met.</summary>
        /// <param name="predicate">A predicate that decides when the click has succeeded. Receives a reference to the clicked element, and is expected to return a boolean where true indicates success.</param>
        public IElement ClickUntil(Func<IElement, bool> predicate)
        {
            _ = InternalRetryActionInvoker("Failed to click element", () =>
            {
                WrappedElement.Click();
                if (predicate(this)) return true;
                throw new WebDriverException("Predicate returned false");
            });

            return this;
        }

        /// <summary>
        /// Attempts to scroll the element into view. This is done using native Javascript and even if the method throws no exceptions there is still no guarantee that the element is indeed in view.
        /// </summary>
        public IElement ScrollIntoView()
        {
            _ = InternalRetryActionInvoker("Failed to scroll element into view", () =>
            {
                JavaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView({ behavior: \"instant\", block: \"center\", inline: \"center\" });", new object[] { WrappedElement });
                return true;
            });

            return this;
        }

        /// <summary>
        /// Attempts to send input to the element. This will fail if the element is not a type that accepts input (textarea, input etc).
        /// If <paramref name="input"/> is null, empty or whitespace then nothing will be sent to the element (except clearing it if <paramref name="clear"/> is <see langword="true"/>).
        /// </summary>
        /// <param name="clear">Whether to clear the value in the element first. If <see langword="false"/> then the <paramref name="input"/> is appended instead.</param>
        public IElement SendKeystrokes(string keysequence)
        {
            string InputForErrorMessage = keysequence.Length > 64 ? $"{keysequence[..64]}... TRUNCATED ({keysequence.Length})" : keysequence;
            _ = InternalRetryActionInvoker($"Failed to send keystrokes to element: {InputForErrorMessage}", () =>
            {
                if (string.IsNullOrWhiteSpace(keysequence)) return true;

                WrappedElement.SendKeys(keysequence);
                return true;
            });

            return this;
        }

        /// <summary>
        /// Retrieves the inner (nested) HTML of this element, trimmed of leading and trailing whitespace.
        /// </summary>
        public string GetInnerHTML()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the inner, nested HTML", () =>
            {
                return (string)JavaScriptExecutor.ExecuteScript("return arguments[0].innerHTML;", WrappedElement);
            });

            return ReturnData?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Returns the name of the HTML-tag this element represents, in lower-case.
        /// </summary>
        public string GetTagName()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the HTML-tag name", () =>
            {
                return WrappedElement.TagName;
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the text-content of the element, with leading and trailing whitespace removed. Never returns null.
        /// </summary>
        /// <param name="removeAdditionalWhitespace">Whether to remove additional whitespace aside from leading and trailing, such as newlines, tabs, linefeeds etc. Optional, defaults to true.</param>
        public string GetInnerText(bool removeAdditionalWhitespace = true)
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the inner text value", () =>
            {
                string ElementText = WrappedElement.Text.Trim();
                if (!removeAdditionalWhitespace) return ElementText;
                return AdditionalWhiteSpaceRegex().Replace(ElementText, "");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Returns the id of the element, as defined by its id-attribute.
        /// </summary>
        public string GetId()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the id-attribute", () =>
            {
                return WrappedElement.GetAttribute("id");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Returns the title of the element, as defined by its title-attribute.
        /// </summary>
        public string GetTitle()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the title-attribute", () =>
            {
                return WrappedElement.GetAttribute("title");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Returns the value of a given attribute.
        /// </summary>
        public string GetAttribute(string attributeName)
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the value for attribute with name: " + attributeName, () =>
            {
                return WrappedElement.GetAttribute(attributeName);
            });

            return ReturnData ?? string.Empty;
        }

        public string GetCssClass()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the css class-attribute", () =>
            {
                return WrappedElement.GetAttribute("className");
            });

            return ReturnData ?? string.Empty;
        }

        public string[] GetCssClasses()
        {
            string[]? ReturnData = InternalRetryActionInvoker("Failed to retrieve the css class-attribute", () =>
            {
                string CssClassAttribute = WrappedElement.GetAttribute("className").Trim();
                return CssClassAttribute.Split(' ', StringSplitOptions.TrimEntries);
            });

            return ReturnData ?? Array.Empty<string>();
        }

        /// <summary>
        /// Checks whether the element is displayed. The criteria for an element to be displayed typically means:
        /// <list type='bullet'>
        /// <item>Not obscured (fully or partially) by other elements</item>
        /// <item>Has a height and width greater than 0</item>
        /// </list>
        /// Note that the element does NOT have to be in the actual viewport!
        /// </summary>
        public bool IsDisplayed()
        {
            bool ReturnData = InternalRetryActionInvoker("Failed to determine if element is displayed", () =>
            {
                return WrappedElement.Displayed;
            });

            return ReturnData;
        }

        /// <summary>
        /// <para>Returns a factory that can be used to fetch elements within the context of this element. Uses the timeout of <see cref='ProducedBy'/>.</para>
        /// <para>Do be careful with extensive chaining as resolving staleness cannot be done recursively!</para>
        /// </summary>
        public IFetchContext Within()
        {
            return new FetchContext(WrappedElement, JavaScriptExecutor, ProducedBy.Timeout, ProducedBy.PollingInterval)
            {
                Parent = this
            };
        }

        private TResult? InternalRetryActionInvoker<TResult>(string errorMessage, Func<TResult> action)
        {
            Exception? LatestException = null;
            var Timeout = ProducedBy.Timeout;
            int PollingInterval = ProducedBy.PollingInterval;
            IElement? ParentElement = ProducedBy.Parent;
            var Timer = Stopwatch.StartNew();

            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    return action.Invoke();
                }
                catch (WebDriverException delegateError)
                {
                    LatestException = delegateError;
                    System.Threading.Thread.Sleep(PollingInterval);

                    // If the error is because the element reference is no longer valid then attempt to reacquire.
                    // If the context is an element try to reacquire that as well.
                    if (delegateError is StaleElementReferenceException)
                    {
                        if (ParentElement is not null)
                        {
                            RelatedContext = ParentElement.Reacquire(false).WrappedElement;
                        }
                        Reacquire(false);
                    }
                }
            }

            string FinalErrorMessage = $"{errorMessage}. Mechanism: {RelatedLocator.Mechanism} | Criteria: {RelatedLocator.Criteria} | From multiple? {LocatedAsMultiple} | Timeout: {Timeout}.";
            throw new Exceptions.ElementInteractionException(FinalErrorMessage, LatestException!);
        }

        [GeneratedRegex("[\t\n\v\f\r]")]
        private static partial Regex AdditionalWhiteSpaceRegex();
    }
}
