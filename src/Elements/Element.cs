using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TMech.Elements
{
    /// <summary><para>
    /// Represents an HTML-element, acting as a wrapper for Selenium's WebElement-instance. Designed for ease of use and stability, with built-in retry mechanisms for all interactions.
    /// It is a base class, representing any HTML-element, and has methods that apply to most but not all all element types.</para>
    /// <para>
    /// It is built around the fact that interacting with elements on webpages can often be flaky due to the highly dynamic nature of modern webpages.
    /// All interactions are retried on Selenium exceptions and stale elements will be detected and reacquired if possible.
    /// </para>
    /// NOTE: Resolving staleness does not work recursively through an entire chain, only on the element itself and its parent. 
    /// In that case it is recommended to limit chaining, and use locators that target your specific element as much as possible.
    /// </summary>
    public class Element
    {
        internal Element(WebElement wrappedElement, FetchContext producedBy, By relatedLocator, ISearchContext relatedContext, IJavaScriptExecutor javaScriptExecutor, bool locatedAsMultiple)
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
            JavaScriptExecutor = javaScriptExecutor;
            Identifier = Guid.NewGuid();
        }

        public WebElement WrappedElement { get; private set; }
        public FetchContext ProducedBy { get; }
        public By RelatedLocator { get; }
        public ISearchContext RelatedContext { get; private set; }
        public bool LocatedAsMultiple { get; }
        public Guid Identifier { get; }
        public IJavaScriptExecutor JavaScriptExecutor { get; }

        #region PRIVATE

        protected TResult? InternalRetryActionInvoker<TResult>(string errorMessage, Func<TResult> action)
        {
            System.Threading.Thread.Sleep(100);
            Exception? LatestException = null;
            var Timer = Stopwatch.StartNew();

            while (Timer.Elapsed <= ProducedBy.Timeout)
            {
                try
                {
                    return action.Invoke();
                }
                catch (WebDriverException delegateError)
                {
                    LatestException = delegateError;
                    System.Threading.Thread.Sleep((int)ProducedBy.PollingInterval);

                    if (delegateError is StaleElementReferenceException)
                    {
                        Reacquire(false);
                    }
                }
            }

            string FinalErrorMessage = $"{errorMessage}. Mechanism: {RelatedLocator.Mechanism} | Criteria: {RelatedLocator.Criteria} | From multiple? {LocatedAsMultiple} | Timeout: {ProducedBy.Timeout}";

            if (LatestException is not null)
                throw new Exceptions.ElementInteractionException(FinalErrorMessage, LatestException);

            throw new Exceptions.ElementInteractionException(FinalErrorMessage);
        }

        #endregion

        #region ACTIONS/INTERACTIONS

        /// <summary>Attempts to click the element.</summary>
        public Element Click()
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
        public Element ClickUntil(Func<Element, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            _ = InternalRetryActionInvoker("Failed to click element until condition was met", () =>
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
        public Element ScrollIntoView()
        {
            _ = InternalRetryActionInvoker("Failed to scroll element into view", () =>
            {
                JavaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView({ behavior: \"instant\", block: \"center\", inline: \"center\" });", WrappedElement);
                return true;
            });

            return this;
        }

        /// <summary>
        /// Attempts to send input to the element as keystrokes, simulating typing. This will fail if the element is not a type that accepts input (<c>&lt;textarea&gt;</c> or <c>&lt;input&gt;</c>), is not displayed or is displayed.
        /// If <paramref name="input"/> is null, empty or whitespace then nothing will be sent to the element.
        /// </summary>
        public Element SendKeys(string input)
        {
            string InputForErrorMessage = input.Length > 64 ? $"{input[..64]}... TRUNCATED ({input.Length})" : input;
            _ = InternalRetryActionInvoker($"Failed to send keys to element: {InputForErrorMessage}", () =>
            {
                if (string.IsNullOrWhiteSpace(input)) return true;

                WrappedElement.SendKeys(input);
                return true;
            });

            return this;
        }

        /// <summary>
        /// Attempts to clear the element's text. This will fail if the element is not a type that accepts input (<c>&lt;textarea&gt;</c> or <c>&lt;input&gt;</c>). Note that attempting to clear an <c>&lt;input type='file'&gt;</c> will throw an exception.
        /// </summary>
        /// <param name="usingKeystrokes">If <see langword="False"/> it will use Selenium's standard method. If <see langword="True"/> then it will be done by using key input (<c>Ctrl + a</c>, then <c>Delete</c>).</param>
        public Element Clear(bool usingKeystrokes = false)
        {
            _ = InternalRetryActionInvoker("Failed to clear element", () =>
            {
                if (usingKeystrokes)
                {
                    WrappedElement.SendKeys(Keys.LeftControl + "a");
                    WrappedElement.SendKeys(Keys.Delete);
                    return true;
                }

                WrappedElement.Clear();
                return true;
            });

            return this;
        }

        #endregion

        #region DATA/ATTRIBUTE GETTERS

        /// <summary>
        /// Returns the name of the form control this element represents. Form control elements are:<br/>
        /// <list type='bullet'>
        /// <item><c>&lt;a&gt;</c></item>
        /// <item><c>&lt;button&gt;</c></item>
        /// <item><c>&lt;fieldset&gt;</c></item>
        /// <item><c>&lt;input&gt;</c></item>
        /// <item><c>&lt;select&gt;</c></item>
        /// <item><c>&lt;textarea&gt;</c></item>
        /// <item><c>&lt;label&gt;</c></item>
        /// <item><c>&lt;legend&gt;</c></item>
        /// <item><c>&lt;datalist&gt;</c></item>
        /// <item><c>&lt;output&gt;</c></item>
        /// <item><c>&lt;option&gt;</c></item>
        /// <item><c>&lt;optgroup&gt;</c></item>
        /// </list>
        /// </summary>
        /// <returns>The name of the element if it's a form control element, <see cref="string.Empty"/> otherwise. For input-elements the type-name is returned as well (<c>input:text</c>, <c>input:file</c> etc).</returns>
        public string GetFormControlType()
        {
            string? ReturnData = InternalRetryActionInvoker(
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
                        case "label":
                        case "legend":
                        case "datalist":
                        case "output":
                        case "option":
                        case "optgroup":
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
        /// Only returns declared attributes in the HTML markup of the page, and not in the properties of the element as found when accessing the element's properties via JavaScript.
        /// </summary>
        public IList<string> GetAttributeNames()
        {
            ReadOnlyCollection<object>? Result = InternalRetryActionInvoker("Failed to retrieve attribute names", () =>
            {
                ReadOnlyCollection<object>? AttributeArray = (ReadOnlyCollection<object>)JavaScriptExecutor.ExecuteScript("return arguments[0].getAttributeNames();", WrappedElement);
                return AttributeArray;
            });

            if (Result is null) return Array.Empty<string>();

            var ReturnData = new string[Result.Count];
            int Index = 0;

            foreach(object CurrentAttributeName in Result)
            {
                ReturnData[Index] = (string)CurrentAttributeName;
                Index++;
            }

            return ReturnData;
        }

        /// <summary>
        /// Retrieves a list of all the attributes (standard, dataset and non-standard) and their values, trimmed of leading and trailing whitespace.
        /// Only returns declared attributes in the HTML markup of the page, and not in the properties of the element as found when accessing the element's properties via JavaScript.
        /// </summary>
        public IDictionary<string, string> GetAttributes()
        {
            Dictionary<string, object>? ScriptReturnData = InternalRetryActionInvoker(
                "Failed to retrieve attributes and their values",
                () =>
                {
                    string Script = @"
                        let ReturnData = {};
                        const AttributeNames = arguments[0].getAttributeNames();
                        AttributeNames.forEach(name=> ReturnData[name] = arguments[0].getAttribute(name).trim());
                        return ReturnData;
                    ";
                    return (Dictionary<string, object>)JavaScriptExecutor.ExecuteScript(Script, WrappedElement);
                }
            );

            Debug.Assert(ScriptReturnData is not null);

            var ReturnData = new Dictionary<string, string>(ScriptReturnData.Count);
            foreach (KeyValuePair<string, object> Current in ScriptReturnData)
            {
                ReturnData.Add(Current.Key, (string)Current.Value);
            }

            return ReturnData;
        }

        /// <summary>
        /// Retrieves a list of all custom data-attributes and their values, trimmed of leading and trailing whitespace.
        /// </summary>
        public IDictionary<string, string> GetDataSet()
        {
            Dictionary<string, object>? ScriptReturnData = InternalRetryActionInvoker(
                "Failed to retrieve dataset-attrbute values",
                () =>
                {
                    string Script = @"
                        let ReturnData = {};
                        for(let Key in arguments[0].dataset)
                        {
                            ReturnData[Key] = arguments[0].dataset[Key].trim();
                        }
                        return ReturnData;
                    ";
                    return (Dictionary<string, object>)JavaScriptExecutor.ExecuteScript(Script, WrappedElement);
                }
            );

            Debug.Assert(ScriptReturnData is not null);

            var ReturnData = new Dictionary<string, string>(ScriptReturnData.Count);
            foreach (KeyValuePair<string, object> Current in ScriptReturnData)
            {
                ReturnData.Add(Current.Key, (string)Current.Value);
            }

            return ReturnData;
        }

        /// <summary>
        /// Retrieves the inner (nested) HTML of this element, trimmed of leading and trailing whitespace.
        /// </summary>
        public string GetHTML()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the inner, nested HTML", () =>
            {
                return (string)JavaScriptExecutor.ExecuteScript("return arguments[0].innerHTML.trim();", WrappedElement);
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Returns the id of the element, as defined by its <c>id</c>-attribute.
        /// </summary>
        public string GetId()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the id", () =>
            {
                return WrappedElement.GetAttribute("id");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Returns the name of the HTML-tag this element represents, in lower-case. For example for element <c>&lt;input name='foo'&gt;</c> would return <c>input</c>.
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
        /// <para>Retrieves the text-content of the element, with leading and trailing whitespace removed. Successive whitespace is reduced to one, and linebreaks are replaced with one space.</para>
        /// <para>For example an element whose inner text is:<br/>
        /// <c>"  \u000cContext1\t-Div2\n\r-Text\u000b-Value "</c><br/>
        /// would return: <c>"Context1 -Div2 -Text -Value"</c></para> 
        /// </summary>
        /// <returns>The text value of the element, or <see cref="string.Empty"/></returns>
        public string GetText()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the inner text value", () =>
            {
                return WrappedElement.Text;
            });
            
            ReturnData ??= string.Empty;
            ReturnData = new Regex("\n").Replace(ReturnData, " ").Trim();
            return new Regex("\\s{2,}").Replace(ReturnData, " ").Trim();
        }

        /// <summary>
        /// Returns the value of a given attribute. Returns both declared attributes in the HTML markup of the page, as well as in the properties of the element as found when accessing the element's properties via JavaScript.
        /// </summary>
        /// <returns>The text value of the element, or <see cref="string.Empty"/> if the attribute does not exist or its value is not set.</returns>
        public string GetAttribute(string attributeName)
        {
            ArgumentNullException.ThrowIfNull(attributeName);

            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the value for attribute with name: " + attributeName, () =>
            {
                return WrappedElement.GetAttribute(attributeName);
            });

            return ReturnData ?? string.Empty;
        }

        #endregion

        #region STATE CHECKERS

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

        #endregion

        #region SPECIALIZED

        /// <summary>
        /// <para>Returns a factory that can be used to fetch elements within the context of this element. Uses the timeout of <see cref='ProducedBy'/>.</para>
        /// <para>Do be careful with extensive chaining as resolving staleness cannot be done recursively!</para>
        /// </summary>
        public FetchContext Within()
        {
            return new FetchContext(WrappedElement, JavaScriptExecutor, ProducedBy.Timeout)
            {
                Parent = this
            };
        }

        /// <summary>
        /// <para>Returns a factory that can be used to fetch elements within the context of this element, configured to use the timeout you pass.</para>
        /// <para>Do be careful with extensive chaining as resolving staleness cannot be done recursively!</para>
        /// </summary>
        public FetchContext Within(TimeSpan timeout)
        {
            return new FetchContext(WrappedElement, JavaScriptExecutor, timeout)
            {
                Parent = this
            };
        }

        /// <summary>
        /// Returns a string representation of the element as it would look in the DOM.
        /// </summary>
        /// <returns>A string representing the element, such as this for example: <code>&lt;button data-button="" class="btn OSFillParent" type="button" style="height: 40px;" id="b41-btnSignaalNieuw"/&gt;</code></returns>
        public string GetHtmlSignature()
        {
            const string Script = @"
                let el = arguments[0];
                let wrapper = '<' + el.tagName.toLowerCase();

                for(let i = 0; i < el.attributes.length; i++)
                {
                    wrapper += ' ' + el.attributes[i].nodeName + '=""';
                    wrapper += el.attributes[i].nodeValue + '""';
                }
    
                wrapper += '/>';

                return wrapper;";

            string? ReturnData = InternalRetryActionInvoker("Failed to generate HTML signature for element", () =>
            {
                return (string)JavaScriptExecutor.ExecuteScript(Script, WrappedElement);
            });

            Debug.Assert(ReturnData is not null);
            Debug.Assert(ReturnData.Length > 0);

            return ReturnData;
        }

        /// <summary>
        /// Returns a list of all elements in the chain, with the first index being the current instance and the last being the first (top) element in the chain.<br/>
        /// This is achieved by walking the <see cref="FetchContext.Parent"/>-tree via <see cref="ProducedBy"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Element> GetElementChain()
        {
            var ElementChain = new List<Element>
            {
                this
            };
            Element? ThisElement = this;

            while (true)
            {
                FetchContext CurrentFactory = ThisElement.ProducedBy;
                ThisElement = CurrentFactory.Parent;

                if (ThisElement is null) break;
                ElementChain.Add(ThisElement);
            }

            return ElementChain;
        }

        /// <summary>
        /// Attempts to reacquire the underlying <see cref="WebElement"/> this instance is wrapped around. Note that if the <see cref="RelatedContext"/> is another element, and that one is stale, this will fail indefinitely as staleness is not recursively resolved.<br/>
        /// Note that elements acquired via <see cref="FetchContext.FetchAll"/> cannot be reacquired.
        /// </summary>
        /// <param name="throwOnError">Whether to throw an exception if the element cannot be reacquired.</param>
        public Element Reacquire(bool throwOnError)
        {
            // Cannot reacquire if located through FetchAll because re-applying
            // the locator does not get us the same element since it was indexed
            // out of the resulting array
            if (LocatedAsMultiple) return this;

            try
            {
                if (ProducedBy.Parent is not null)
                {
                    RelatedContext = ProducedBy.Parent.Reacquire(true).WrappedElement;
                }
                WrappedElement = (WebElement)RelatedContext.FindElement(RelatedLocator);
            }
            catch (WebDriverException)
            {
                if (throwOnError) throw;
            }

            return this;
        }

        /// <summary>
        /// Returns the underlying <see cref="WebElement"/> that this instance is wrapped around.
        /// </summary>
        public WebElement UnWrap()
        {
            return WrappedElement;
        }

        #endregion

    }

}