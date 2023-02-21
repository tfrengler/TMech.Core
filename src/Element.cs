using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace TMech.Core
{
    /// <summary>Represents an HTML-element, acting as a wrapper for an IWebElement-instance. It is designed for ease of use and stability, and not for speed.<br/>
    /// It is a base class, representing any HTML-element, and has methods that apply to (almost) all element types.<br/>
    /// It is built around the idea that interacting with raw elements can often be unstable, and easily cause exceptions when attempting to interact with it.<br/>
    /// Therefore interactions - such as clicking and/or reading out attributes etc - should be more robust, and not fail immediately. These methods will typically retry their actions before finally throwing an exception.<br/>
    /// Also no methods that return data should return null unless specifically designed for it, and they should explicitly be marked as nullable.</summary>
    public class Element
    {
        public Element(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple)
        {
            if (wrappedElement is null) throw new ArgumentNullException(nameof(wrappedElement));
            if (producedBy is null) throw new ArgumentNullException(nameof(producedBy));
            if (relatedLocator is null) throw new ArgumentNullException(nameof(relatedLocator));
            if (relatedContext is null) throw new ArgumentNullException(nameof(relatedContext));

            WrappedElement = wrappedElement;
            ProducedBy = producedBy;
            RelatedLocator = relatedLocator;
            RelatedContext = relatedContext;
            LocatedAsMultiple = locatedAsMultiple;
            JavaScriptExecutor = (IJavaScriptExecutor)wrappedElement.WrappedDriver;
        }

        /// <summary>Represents the element</summary>
        public WebElement WrappedElement {get; private set;}
        public ElementFactory ProducedBy {get;}
        public By RelatedLocator {get;}
        public ISearchContext RelatedContext {get;}
        public uint ActionAttempts {get; private set;} = 1;

        #region PRIVATE

        private readonly IJavaScriptExecutor JavaScriptExecutor;
        private readonly bool LocatedAsMultiple;
        private Element() {}

        private void ReAcquireElementHandle()
        {
            WrappedElement = (WebElement)RelatedContext.FindElement(RelatedLocator);
        }

        private TResult RetryActionOnFailureInvoker<TResult>(string errorMessage, Func<TResult> action)
        {
            Exception? LatestException = null;
            uint RemainingAttempts = ActionAttempts;

            while(RemainingAttempts > 0)
            {
                RemainingAttempts--;

                try {
                    return action.Invoke();
                }
                catch(WebDriverException delegateError)
                {
                    LatestException = delegateError;
                    if (RemainingAttempts == 0) break;

                    System.Threading.Thread.Sleep(500);
                    if (LocatedAsMultiple) continue;

                    if (delegateError is StaleElementReferenceException)
                    {
                        try
                        {
                            ReAcquireElementHandle();
                        }
                        catch (WebDriverException reacquireError)
                        {
                            LatestException = reacquireError;
                        }
                    }
                }
            }

            string FinalErrorMessage = string.Empty;
            if (LatestException is not null) FinalErrorMessage = ":" + Environment.NewLine + "---------------| LAST EXCEPTION:" + Environment.NewLine + LatestException.Message;
            throw new ElementActionException($"{errorMessage}. Element locator: {RelatedLocator.Mechanism} - {RelatedLocator.Criteria}. Tried {ActionAttempts} time(s)" + FinalErrorMessage);
        }

        #endregion

        /// <summary>
        /// Instructs this instance to retry actions - such as clicking and fetching attributes etc - a certain number of times before throwing an exception.<br/>
        /// There's a 500 ms delay between attempts so if for example you want to wait max 5 seconds before stopping then set this to 10.
        /// </summary>
        /// <param name="attempts">The amount of attempts to make. If passed as 0 it will be set to 1.</param>
        public Element TryActionsThisManyTimes(uint attempts)
        {
            ActionAttempts = attempts > 0 ? attempts : 1;
            return this;
        }

        #region ACTIONS/INTERACTIONS

        /// <summary>Attempts to click the element. Will retry on exceptions, equal to the amount defined in <see cref='ActionAttempts'/></summary>
        public void Click()
        {
            _ = RetryActionOnFailureInvoker<bool>("Unable to click element", ()=> {
                WrappedElement.Click();
                return true;
            });
        }

        public void ScrollIntoView()
        {
            _ = RetryActionOnFailureInvoker<bool>("Unable to click element", ()=> {
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
            string ReturnData =  RetryActionOnFailureInvoker<string>(
                "Unable to retrieve the name of the form control type",
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
            var ReturnData =  RetryActionOnFailureInvoker<ReadOnlyCollection<object>>("Unable to retrieve attribute names", ()=> {
                ReadOnlyCollection<object>? AttributeArray = JavaScriptExecutor.ExecuteScript("return arguments[0].getAttributeNames();", new object[] { WrappedElement }) as ReadOnlyCollection<object>;
                return AttributeArray ?? new ReadOnlyCollection<object>(Array.Empty<string>());
            });

            return ReturnData.Select(attributeName=> (string)attributeName).ToImmutableList();
        }

        /// <summary>
        /// Retrieves a list of all the attributes (standard, dataset and non-standard) and their values.
        /// </summary>
        public ImmutableDictionary<string, string> GetAttributes()
        {
            Dictionary<string,object>? ScriptReturnData = RetryActionOnFailureInvoker<Dictionary<string,object>>(
                "Unable to retrieve attributes and their values",
                ()=> {
                    string Script = @"
                        let ReturnData = {};
                        const AttributeNames = arguments[0].getAttributeNames();
                        AttributeNames.forEach(name=> ReturnData[name] = arguments[0].getAttribute(name));
                        return ReturnData;
                    ";
                    return JavaScriptExecutor.ExecuteScript(Script, new object[] { WrappedElement }) as Dictionary<string,object>;
                }
            );

            if (ScriptReturnData is null) return ImmutableDictionary.Create<string, string>();

            var ReturnData = ImmutableDictionary.CreateBuilder<string, string>();
            foreach(KeyValuePair<string,object> Current in ScriptReturnData)
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
            Dictionary<string,object>? ScriptReturnData = RetryActionOnFailureInvoker<Dictionary<string,object>?>(
                "Unable to retrieve dataset-attrbute values",
                ()=> {
                    string Script = @"
                        let ReturnData = {};
                        for(let Key in arguments[0].dataset)
                            ReturnData[Key] = arguments[0].dataset[Key];
                        return ReturnData;
                    ";
                    return JavaScriptExecutor.ExecuteScript(Script, new object[] { WrappedElement }) as Dictionary<string,object>;
                }
            );

            if (ScriptReturnData is null) return ImmutableDictionary.Create<string, string>();

            var ReturnData = ImmutableDictionary.CreateBuilder<string, string>();
            foreach(KeyValuePair<string,object> Current in ScriptReturnData)
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
            string ReturnData =  RetryActionOnFailureInvoker<string>("Unable to retrieve the inner, nested HTML", ()=> {
                string? HTML = JavaScriptExecutor.ExecuteScript("return arguments[0].innerHTML;", new object[] { WrappedElement }) as string;
                return HTML?.Trim() ?? string.Empty;
            });

            return ReturnData;
        }

        /// <summary>
        /// Returns the id of the element, as defined by its id-attribute.
        /// </summary>
        public string GetId()
        {
            string ReturnData =  RetryActionOnFailureInvoker<string>("Unable to retrieve the id", ()=> {
                return WrappedElement.GetAttribute("id");
            });

            return ReturnData;
        }

        /// <summary>
        /// Returns the name of the HTML-tag this element represents, in lower-case.
        /// </summary>
        public string GetTagName()
        {
            string ReturnData =  RetryActionOnFailureInvoker<string>("Unable to retrieve the HTML-tag name", ()=> {
                return WrappedElement.TagName;
            });

            return ReturnData;
        }

        /// <summary>
        /// Retrieves the text-content of the element, with leading and trailing whitespace removed.
        /// </summary>
        /// <param name="removeAdditionalWhitespace">Whether to remove additional whitespace aside from leading and trailing, such as newlines, tabs, linefeeds etc. Optional, defaults to true.</param>
        public string GetText(bool removeAdditionalWhitespace = true)
        {
            string ReturnData =  RetryActionOnFailureInvoker<string>("Unable to retrieve the inner text value", ()=> {
                string ElementText = WrappedElement.Text;
                if (!removeAdditionalWhitespace) return ElementText;
                return new Regex("[\t\n\v\f\r\u0020]").Replace(ElementText, "").Trim();
            });

            return ReturnData;
        }

        /// <summary>
        /// Retrieves the trimmed value of the HTML-element. If it does not have value-attribute and empty string is returned.
        /// </summary>
        public string GetValue()
        {
            string ReturnData =  RetryActionOnFailureInvoker<string>("Unable to retrieve the value", ()=> {
                return WrappedElement.GetAttribute("value");
            });

            return ReturnData.Trim();
        }

        #endregion

        #region STATE CHECKERS

        /// <summary>
        /// Checks whether the element is enabled (equivalent to checking the 'disable'-attribute). Only relevant for form-controls but safe to call on any element.
        /// </summary>
        /// <returns>True if the element is enabled or if the element does not support the disabled-attribute, false otherwise.</returns>
        public bool IsEnabled()
        {
            bool ReturnData =  RetryActionOnFailureInvoker<bool>("Unable to determine if element is enabled", ()=> {
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
            bool ReturnData =  RetryActionOnFailureInvoker<bool>("Unable to determine if element is displayed", ()=> {
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
            bool ReturnData =  RetryActionOnFailureInvoker<bool>("Unable to determine if element is selected", ()=> {
                return WrappedElement.Selected;
            });

            return ReturnData;
        }

        #endregion

        #region FETCH RELATED ELEMENTS

        public Element? GetNextSibling()
        {
            throw new NotImplementedException();
        }

        public Element? GetParent()
        {
            throw new NotImplementedException();
        }

        public Element? GetPreviousSibling()
        {
            throw new NotImplementedException();
        }

        public Element[] GetChildren()
        {
            throw new NotImplementedException();
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