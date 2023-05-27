using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V106.Debugger;

namespace TMech.Core
{
    /// <summary><para>
    /// Represents an HTML-element, acting as a wrapper for an IWebElement-instance. Designed for ease of use and stability, with built-in retry mechanisms for all (inter)actions.<br/>
    /// It is a base class, representing any HTML-element, and has methods that apply to (almost) all element types.</para>
    /// <para>
    /// It is built around the fact that interacting with elements on webpages can often be flaky due to the highly dynamic nature of modern webpages.<br/>
    /// All interactions are retried on Selenium exceptions (configureable), and stale elements will be detected and reacquired (if possible).
    /// </para>
    /// NOTE: Reacquiring stale elements only works on THIS element! If the <see cref="RelatedContext"/> is another element, and that one is stale, you will get a <see cref="StaleElementReferenceException"/> as staleness cannot be resolved recursively.<br/>
    /// In such a situation it is recommended to use a more precise locator that can get you a <see cref="Element"/>-instance that exists directly within the webdriver context.
    /// </summary>
    public class Element
    {
        public Element(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple)
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
            JavaScriptExecutor = (IJavaScriptExecutor)wrappedElement.WrappedDriver;
        }

        public WebElement WrappedElement { get; private set; }
        public ElementFactory ProducedBy { get; }
        public By RelatedLocator { get; }
        public ISearchContext RelatedContext { get; }
        public uint ActionAttempts { get; private set; } = 50;

        #region PRIVATE

        private bool DoNotReacquireElementOnException;
        private readonly IJavaScriptExecutor JavaScriptExecutor;
        private readonly bool LocatedAsMultiple;

        private TResult? InternalRetryActionInvoker<TResult>(string errorMessage, Func<TResult> action)
        {
            WebDriverException? LatestException = null;
            uint RemainingAttempts = ActionAttempts;

            while (RemainingAttempts > 0)
            {
                RemainingAttempts--;

                try
                {
                    return action.Invoke();
                }
                catch (WebDriverException delegateError)
                {
                    LatestException = delegateError;
                    if (RemainingAttempts == 0) break;

                    System.Threading.Thread.Sleep(100);
                    if (LocatedAsMultiple) continue;

                    // If the error is because the element reference is no longer valid then attempt to reacquire
                    if (!DoNotReacquireElementOnException && delegateError is StaleElementReferenceException)
                    {
                        try
                        {
                            // This could fail consistently until the timeout is reached if RelatedContext is another element
                            // that no longer exists or is in an invalid state (obscured, hidden etc)
                            WrappedElement = (WebElement)RelatedContext.FindElement(RelatedLocator);
                        }
                        catch (WebDriverException reacquireError)
                        {
                            LatestException = reacquireError;
                        }
                    }
                }
            }

            string FinalErrorMessage = $"{errorMessage}. Element locator: {RelatedLocator.Mechanism} | {RelatedLocator.Criteria}. Tried {ActionAttempts} time(s)";

            if (LatestException is not null)
                throw new Exceptions.ElementInteractionException(FinalErrorMessage, LatestException);

            throw new Exceptions.ElementInteractionException(FinalErrorMessage);
        }

        #endregion

        /// <summary>
        /// Instructs this instance to retry actions a specific number of times before throwing an exception.<br/>
        /// There's a 100 ms delay between attempts so you need 10 attempts per 1 second (50 for 5 seconds, 100 for 10 seconds etc).
        /// </summary>
        /// <param name="attempts">The amount of attempts to make. If passed as 0 it will be set to 1.</param>
        public Element TryActionsThisManyTimes(uint attempts)
        {
            ActionAttempts = attempts > 0 ? attempts : 1;
            return this;
        }

        /// <summary>
        /// Disables the behaviour where if an action fails - and it is because a StaleElementReferenceException - it would try to reacquire the element reference again.<br/>
        /// This can be beneficial if your locator is not precise enough, and there's a decent chance that another element that matches your locator would be fetched intstead, which can lead to undesired or hard to debug behaviour.
        /// </summary>
        /// <param name="attempts"></param>
        public Element DoNotReacquireElementIfStale()
        {
            DoNotReacquireElementOnException = true;
            return this;
        }

        #region ACTIONS/INTERACTIONS

        /// <summary>Attempts to click the element. Will retry on exceptions, equal to the amount defined in <see cref='ActionAttempts'/></summary>
        public void Click()
        {
            _ = InternalRetryActionInvoker<bool>("Failed to click element", () =>
            {
                WrappedElement.Click();
                return true;
            });
        }

        public void ScrollIntoView()
        {
            _ = InternalRetryActionInvoker<bool>("Failed scroll element into view", () =>
            {
                JavaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView();", new object[] { WrappedElement });
                return true;
            });
        }

        #endregion

        #region DATA/ATTRIBUTE GETTERS

        /// <summary>
        /// Returns the name of the form control this element represents. Form control elements are:<br/>
        /// <list type='bullet'>
        /// <item>a</item>
        /// <item>button</item>
        /// <item>fieldset</item>
        /// <item>input</item>
        /// <item>select</item>
        /// <item>textarea</item>
        /// </list>
        /// </summary>
        /// <returns>The name of the element if it's a form control element. For input-elements the type-name is returned as well (input:text, input:file etc). Returns empty string otherwise.</returns>
        public string GetFormControlType()
        {
            string? ReturnData = InternalRetryActionInvoker<string>(
                "Failed to retrieve the name of the form control type",
                () =>
                {
                    string TagName = WrappedElement.TagName;
                    switch (WrappedElement.TagName)
                    {
                        case "a":
                        case "button":
                        case "fieldset":
                        case "select":
                        case "textarea":
                            return TagName;

                        case "input":
                            return "input:" + WrappedElement.GetAttribute("type");

                        default: return string.Empty;
                    }
                }
            );
            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves a list of the names of all the attributes (standard, dataset and non-standard).
        /// </summary>
        public ImmutableList<string> GetAttributeNames()
        {
            ReadOnlyCollection<object>? ReturnData = InternalRetryActionInvoker("Failed to retrieve attribute names", () =>
            {
                ReadOnlyCollection<object>? AttributeArray = JavaScriptExecutor.ExecuteScript("return arguments[0].getAttributeNames();", new object[] { WrappedElement }) as ReadOnlyCollection<object>;
                return AttributeArray ?? new ReadOnlyCollection<object>(Array.Empty<string>());
            });

#pragma warning disable CS8604 // InternalRetryActionInvoker may return null but the delegate we pass ensure we never get null values so ignore warning
            return ReturnData.Select(attributeName => (string)attributeName).ToImmutableList();
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Retrieves a list of all the attributes (standard, dataset and non-standard) and their values.
        /// </summary>
        public ImmutableDictionary<string, string> GetAttributes()
        {
            Dictionary<string, object>? ScriptReturnData = InternalRetryActionInvoker<Dictionary<string, object>>(
                "Failed to retrieve attributes and their values",
                () =>
                {
                    string Script = @"
                        let ReturnData = {};
                        const AttributeNames = arguments[0].getAttributeNames();
                        AttributeNames.forEach(name=> ReturnData[name] = arguments[0].getAttribute(name));
                        return ReturnData;
                    ";
                    return (Dictionary<string, object>)JavaScriptExecutor.ExecuteScript(Script, new object[] { WrappedElement });
                }
            );

            if (ScriptReturnData is null) return ImmutableDictionary.Create<string, string>();

            var ReturnData = ImmutableDictionary.CreateBuilder<string, string>();
            foreach (KeyValuePair<string, object> Current in ScriptReturnData)
            {
                ReturnData.Add(Current.Key, (string)Current.Value);
            }

            return ReturnData.ToImmutable();
        }

        /// <summary>
        /// Retrieves a list of all custom data-attributes and their values.
        /// </summary>
        public ImmutableDictionary<string, string> GetDataSet()
        {
            Dictionary<string, object>? ScriptReturnData = InternalRetryActionInvoker<Dictionary<string, object>?>(
                "Failed to retrieve dataset-attrbute values",
                () =>
                {
                    string Script = @"
                        let ReturnData = {};
                        for(let Key in arguments[0].dataset)
                            ReturnData[Key] = arguments[0].dataset[Key];
                        return ReturnData;
                    ";
                    return JavaScriptExecutor.ExecuteScript(Script, new object[] { WrappedElement }) as Dictionary<string, object>;
                }
            );

            if (ScriptReturnData is null) return ImmutableDictionary.Create<string, string>();

            var ReturnData = ImmutableDictionary.CreateBuilder<string, string>();
            foreach (KeyValuePair<string, object> Current in ScriptReturnData)
            {
                ReturnData.Add(Current.Key, (string)Current.Value);
            }

            return ReturnData.ToImmutable();
        }

        /// <summary>
        /// Retrieves the inner (nested) HTML of this element, trimmed of leading and trailing whitespace.
        /// </summary>
        public string GetHTML()
        {
            string? ReturnData = InternalRetryActionInvoker<string>("Failed to retrieve the inner, nested HTML", () =>
            {
                return (string)JavaScriptExecutor.ExecuteScript("return arguments[0].innerHTML;", new object[] { WrappedElement });
            });

            return ReturnData?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Returns the id of the element, as defined by its id-attribute.
        /// </summary>
        public string GetId()
        {
            string? ReturnData = InternalRetryActionInvoker<string>("Failed to retrieve the id", () =>
            {
                return WrappedElement.GetAttribute("id");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Returns the name of the HTML-tag this element represents, in lower-case.
        /// </summary>
        public string GetTagName()
        {
            string? ReturnData = InternalRetryActionInvoker<string>("Failed to retrieve the HTML-tag name", () =>
            {
                return WrappedElement.TagName;
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the text-content of the element, with leading and trailing whitespace removed.
        /// </summary>
        /// <param name="removeAdditionalWhitespace">Whether to remove additional whitespace aside from leading and trailing, such as newlines, tabs, linefeeds etc. Optional, defaults to true.</param>
        public string GetText(bool removeAdditionalWhitespace = true)
        {
            string? ReturnData = InternalRetryActionInvoker<string>("Failed to retrieve the inner text value", () =>
            {
                string ElementText = WrappedElement.Text;
                if (!removeAdditionalWhitespace) return ElementText;
                return new Regex("[\t\n\v\f\r]").Replace(ElementText, "").Trim();
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the trimmed value of the HTML-element. If it does not have value-attribute and empty string is returned.
        /// </summary>
        public string GetValue()
        {
            string? ReturnData = InternalRetryActionInvoker<string>("Failed to retrieve the value", () =>
            {
                return WrappedElement.GetAttribute("value");
            });

            return ReturnData ?? string.Empty;
        }

        #endregion

        #region STATE CHECKERS

        /// <summary>
        /// Checks whether the element is enabled (equivalent to checking the 'disable'-attribute). Only relevant for form-controls but safe to call on any element.
        /// </summary>
        /// <returns>True if the element is enabled or if the element does not support the disabled-attribute, false otherwise.</returns>
        public bool IsEnabled()
        {
            bool ReturnData = InternalRetryActionInvoker<bool>("Failed to determine if element is enabled", () =>
            {
                return WrappedElement.Enabled;
            });

            return ReturnData;
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
            bool ReturnData = InternalRetryActionInvoker<bool>("Failed to determine if element is displayed", () =>
            {
                return WrappedElement.Displayed;
            });

            return ReturnData;
        }

        /// <summary>
        /// Checks whether the element is selected (equivalent to checking for the 'checked'-attribute). Only relevant for input checkbox-elements but safe to call on any element.
        /// </summary>
        /// <returns>True if the element is an input checkbox and is selected, false otherwise or if the element does not support the checked-attribute.</returns>
        public bool IsSelected()
        {
            bool ReturnData = InternalRetryActionInvoker<bool>("Failed to determine if element is selected", () =>
            {
                return WrappedElement.Selected;
            });

            return ReturnData;
        }

        #endregion

        #region FETCH RELATED ELEMENTS

        public Element? FetchNextSibling(string tagName = "*")
        {
            var Locator = By.XPath($"./following-sibling::{tagName}[1]");
            return ProducedBy.Fetch(Locator);
        }

        public Element? FetchParent()
        {
            var Locator = By.XPath("./parent::*[1]");
            return ProducedBy.Fetch(Locator);
        }

        public Element? FetchPreviousSibling(string tagName = "*")
        {
            var Locator = By.XPath($"./preceding-sibling::{tagName}[1]");
            return ProducedBy.Fetch(Locator);
        }

        public Element? FetchAncestor(string tagName = "*")
        {
            var Locator = By.XPath($"./ancestor::{tagName}[1]");
            return ProducedBy.Fetch(Locator);
        }

        public Element[] FetchDescendants(string tagName = "*", uint threshold = 1)
        {
            return InternalGetChildrenOrDescendants(tagName, false, threshold);
        }

        public Element[] FetchChildren(string tagName = "*", uint threshold = 1)
        {
            return InternalGetChildrenOrDescendants(tagName, true, threshold);
        }

        private Element[] InternalGetChildrenOrDescendants(string tagName, bool children, uint threshold)
        {
            string Axis = children ? "child" : "descendant";
            var Locator = By.XPath($"./{Axis}::{tagName}");

            Element[] ReturnData = ProducedBy.FetchAll(Locator, threshold);
            return ReturnData;
        }

        #endregion

        #region SPECIALIZED

        /// <summary>Returns a factory that can be used to fetch elements within the context of this element. Uses the default timeout of <see cref='ElementFactory'/>.</summary>
        public ElementFactory Elements()
        {
            return new ElementFactory(WrappedElement);
        }

        /// <summary>Returns a factory that can be used to fetch elements within the context of this element, configured to use the timeout you pass.</summary>
        public ElementFactory Elements(TimeSpan timeout)
        {
            return new ElementFactory(WrappedElement, timeout);
        }

        /// <summary>Attempts to returns text of all the label-elements associated with this element.<br/>
        /// This is done by:
        /// <list type='bullet'>
        /// <item>Looking at the parent-element to see if it's a label OR</item>
        /// <item>Searching all label-elements whose for-attribute value matches the id of this element</item>
        /// </list>
        /// Be warned that it can only search for labels within the context that this element was fetched!
        ///</summary>
        /// <returns>A list of the text values from all the label-elements associated with this element. Guaranteed to never return null.</returns>
        public IReadOnlyList<string> GetLabels()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}