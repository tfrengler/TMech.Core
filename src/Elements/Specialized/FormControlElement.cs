using OpenQA.Selenium;
using System.IO;
using System.Threading;

namespace TMech.Elements.Specialized
{
    /// <summary>
    /// <para>
    /// Represents a form control element, such as &lt;input&gt;, &lt;option&gt;, &lt;radio&gt;, &lt;textarea&gt; etc.
    /// </para>
    /// <para>
    /// Allows access to standard methods that are (more or less) applicable to all form controls such as getting the name-attribute, input-type, maxlength-attribute, src-attribute etc.
    /// </para>
    /// Getting and setting the data is exposed via the value-attribute which is treated as text. For more specialized elements with strong data types see other classes in this namespace.
    /// </summary>
    public class FormControlElement : Element
    {
        internal FormControlElement(WebElement wrappedElement, FetchContext producedBy, By relatedLocator, ISearchContext relatedContext, IJavaScriptExecutor javaScriptExecutor, bool locatedAsMultiple)
            : base(wrappedElement, producedBy, relatedLocator, relatedContext, javaScriptExecutor, locatedAsMultiple) { }

        protected const int RobustWaitTimeInMS = 500;

        protected bool RobustSelection = false;

        /// <summary>
        /// Configure this instance to use robust selection. This means all setters - after changing the value of the element - will wait <see cref="RobustWaitTimeInMS"/> ms and then verify that the value of the element is correct.
        /// This makes value setting more robust but does cause an overhead, and will slow down execution so use with care.
        /// </summary>
        public virtual FormControlElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        #region DATA SETTERS

        /// <summary>
        /// Attempts to set data in this formcontrol element by sending <paramref name="input"/> as keystrokes.
        /// This will fail with an exception if not an element that accepts key-input (<c>&lt;textarea&gt;</c> or <c>&lt;input&gt;</c>).
        /// </summary>
        public void SetValue(string input, bool clear = true)
        {
            _ = InternalRetryActionInvoker($"Failed to set value in formcontrol-element (clear? {clear})", () =>
            {
                if (clear) WrappedElement.Clear();
                WrappedElement.SendKeys(input);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == input;
            });
        }

        /// <summary>
        /// <para>Attempts to set data in this formcontrol element by setting <paramref name="input"/> directly in the value-attribute via Javascript.
        /// After setting the value a change-event will be dispatched to the element to trigger any potential event handlers.</para>
        /// <para>This can be significantly faster than sending input via keystrokes on large texts, but it may bypass potential front-end checks such as ignoring disabled elements, triggering dialogs, and other things. Use with care.</para>
        /// This will fail with an exception if not an element that accepts key-input (<c>&lt;textarea&gt;</c> or <c>&lt;input&gt;</c>).
        /// </summary>
        public void SetValueByJS(string input)
        {
            string ValueChangerScript = $@"
                    arguments[0].blur();
                    let Prototype = null;

                    if (arguments[0] instanceof HTMLTextAreaElement)
                        Prototype = window.HTMLTextAreaElement.prototype;
                    if (arguments[0] instanceof HTMLInputElement)
                        Prototype = window.HTMLInputElement.prototype;
                    else
                        throw new Error('Element is not a textarea or input element: ' + arguments[0].tagName);
                        
                    let NativeSetter = Object.getOwnPropertyDescriptor(Prototype, 'value').set;
                    NativeSetter.call(arguments[0], '{input.Replace("'", "\\'").Trim()}');
                    arguments[0].dispatchEvent(new Event('change', {{bubbles: true}}));
                ";

            _ = InternalRetryActionInvoker($"Failed to set value in formcontrol-element via Javascript", () =>
            {
                WrappedElement.SendKeys(input);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == input;
            });
        }

        /// <summary>
        /// Attemps to upload a local file to the formcontrol element. Will fail if the element is not <c>&lt;input type='file'&gt;</c> or <paramref name="input"/> does not exists.
        /// </summary>
        /// <param name="input">The full, absolute path of the file you wish to upload including file extension.</param>
        public void UploadFile(string input)
        {
            if (!System.IO.File.Exists(input))
            {
                throw new FileNotFoundException("Unable to upload file to form control element as file cannot be found", input);
            }

            SendKeys(input);
        }

        #endregion

        #region DATA RETRIEVERS

        /// <summary>
        /// Attempts to retrieve the data from the form control-element by reading out its value-attribute.
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the value of the form control element", () =>
            {
                return WrappedElement.GetAttribute("value");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the value of the name-attribute. Used by all form control elements to uniquely identify them within a form, and is the identifier used when the form is submitted.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the value of the form control's name-attribute", () =>
            {
                return WrappedElement.GetAttribute("name");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the input-type, such as 'text', 'checkbox', 'date' etc. Returns an empty string if the element is not an input-element.
        /// </summary>
        /// <returns></returns>
        public string GetInputType()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to determine form control input-type", () =>
            {
                return WrappedElement.GetAttribute("type");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the value of the min-attribute. This is only relevant for form controls of type date, month, week, time, datetime-local, number, and range.
        /// </summary>
        /// <returns>The value parsed as an integer or <see langword="null"/> if the element does not have the min-attribute OR the value can not be parsed.</returns>
        public int? GetMin()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the value of the form control's min-attribute", () =>
            {
                return WrappedElement.GetAttribute("min");
            });

            if (int.TryParse(AttributeValue, out int ReturnData)) return ReturnData;
            return null;
        }

        /// <summary>
        /// Retrieves the value of the max-attribute. This is only relevant for form controls of type date, month, week, time, datetime-local, number, and range.
        /// </summary>
        /// <returns>The value parsed as an integer or <see langword="null"/> if the element does not have the max-attribute OR the value can not be parsed.</returns>
        public int? GetMax()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the value of the form control's max-attribute", () =>
            {
                return WrappedElement.GetAttribute("max");
            });

            if (int.TryParse(AttributeValue, out int ReturnData)) return ReturnData;
            return null;
        }

        /// <summary>
        /// Retrieves the value of the maxlength-attribute. This is only relevant for form controls of type text, search, url, tel, email, and password.
        /// </summary>
        /// <returns>The value as an integer or <see langword="null"/> if the element does not have the maxlength-attribute OR the value can not be parsed.</returns>
        public int? GetMaxLength()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the value of the form control's maxlength-attribute", () =>
            {
                return WrappedElement.GetAttribute("maxlength");
            });

            if (int.TryParse(AttributeValue, out int ReturnData)) return ReturnData;
            return null;
        }

        /// <summary>
        /// Retrieves the value of the minlength-attribute. This is only relevant for form controls of type text, search, url, tel, email, and password.
        /// </summary>
        /// <returns>The value as an integer or <see langword="null"/> if the element does not have the minlength-attribute OR the value can not be parsed.</returns>
        public int? GetMinLength()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the value of the form control's minlength-attribute", () =>
            {
                return WrappedElement.GetAttribute("minlength");
            });

            if (int.TryParse(AttributeValue, out int ReturnData)) return ReturnData;
            return null;
        }

        /// <summary>
        /// Retrieves the value of the src-attribute. This is only relevant for form controls of type image but is used by other elements that are not form controls (embed, video, audio etc).
        /// </summary>
        /// <returns>The value of the src-attribute or an empty string if the element does not have a src-attribute.</returns>
        public string GetSource()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the value of the form control's src-attribute", () =>
            {
                return WrappedElement.GetAttribute("src");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the value of the step-attribute. This is only relevant for form controls of type date, month, week, time, datetime-local, number, and range.
        /// </summary>
        /// <returns>The value parsed as an integer or <see langword="null"/> if the element does not have the step-attribute OR the value can not be parsed.</returns>
        public int? GetStep()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the value of the form control's step-attribute", () =>
            {
                return WrappedElement.GetAttribute("step");
            });

            if (int.TryParse(AttributeValue, out int ReturnData)) return ReturnData;
            return null;
        }

        #endregion

        #region STATE CHECKERS

        /// <summary>
        /// Checks whether the form control element is readonly meaning the element not editable by the user.
        /// </summary>
        /// <returns><see langword="true"/> if the element is required or <see langword="false"/> if not or the element does not support the required-attribute.</returns>
        public bool IsRequired()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to determine if form-element is required", () =>
            {
                return WrappedElement.GetAttribute("required");
            });

            if (bool.TryParse(AttributeValue, out bool ReturnData)) return ReturnData;
            return false;
        }

        /// <summary>
        /// Checks whether the form control element is readonly. Is supported by the text, search, url, tel, email, date, month, week, time, datetime-local, number, and password input types
        /// </summary>
        /// <returns><see langword="true"/> if the element is enabled or <see langword="false"/> if not or the element does not support the readonly-attribute.</returns>
        public bool IsReadOnly()
        {
            string? AttributeValue = InternalRetryActionInvoker("Failed to determine if form-element is readonly", () =>
            {
                return WrappedElement.GetAttribute("readonly");
            });

            if (bool.TryParse(AttributeValue, out bool ReturnData)) return ReturnData;
            return false;
        }

        /// <summary>
        /// Checks whether the form control element is enabled. Specifically, disabled inputs do not receive the click event, and disabled inputs are not submitted with the form.
        /// </summary>
        /// <returns><see langword="true"/> if the element is enabled or if the element does not support the disabled-attribute, <see langword="false"/> otherwise.</returns>
        public bool IsEnabled()
        {
            bool ReturnData = InternalRetryActionInvoker("Failed to determine if element is enabled", () =>
            {
                return WrappedElement.Enabled;
            });

            return ReturnData;
        }

        #endregion
    }
}
