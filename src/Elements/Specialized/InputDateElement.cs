using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Threading;

namespace TMech.Elements.Specialized
{
    /// <summary>
    /// Represents an <c>input[type='date']</c> whose value-attribute can be treated and parsed as a date.
    /// </summary>
    public sealed class InputDateElement : FormControlElement
    {
        internal InputDateElement(WebElement wrappedElement, FetchContext producedBy, By relatedLocator, ISearchContext relatedContext, IJavaScriptExecutor javaScriptExecutor, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, javaScriptExecutor, locatedAsMultiple)
        {
        }

        public override InputDateElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        /// <summary>
        /// The standard, browser-agnostic format that the value-attribute accepts and returns date-strings in.
        /// </summary>
        public const string ValueAttributeFormat = "yyyy-MM-dd";
        /// <summary>
        /// The format that Chromium-based browsers accepts date-strings in when setting the value via keystrokes.
        /// </summary>
        public const string ChromiumFormat = "ddMMyyyy";
        /// <summary>
        /// The format that Firefox accepts date-strings in when setting the value via keystrokes.
        /// </summary>
        public const string FirefoxFormat = "MMddyyyy";

        /// <summary>
        /// Identical to <see cref="GetDate(string)"/>, using <see cref="ValueAttributeFormat"/> as the date format.
        /// </summary>
        public DateTime GetDate()
        {
            return GetDate(ValueAttributeFormat);
        }

        /// <summary>
        /// <para>
        /// Reads the <c>value-attribute</c> of this element and parses it as a <see cref="DateTime"/>-instance where only the date-component is set (time is set to 00:00:00).<br/>
        /// </para>
        /// Common causes for parse failure:
        /// <list type="bullet">
        ///     <item>Leading or trailing whitespace in the value</item>
        ///     <item>Value being an empty string or pure whitespace</item>
        ///     <item>Value being an invalid date</item>
        ///     <item>Value being in a format that does not match the parsing format in <paramref name="format"/></item>
        /// </list>
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representing the value from the <c>value</c>-attribute. If it fails for whatever reason to parse the value as a valid date then <see cref="DateTime.MinValue"/> is returned instead.</returns>
        public DateTime GetDate(string format = ValueAttributeFormat)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(format);

            string? Data = InternalRetryActionInvoker("Failed to get value and parse it as date from input[type='date']-element", () =>
            {
                return WrappedElement.GetAttribute("value") ?? string.Empty;
            });

            if (DateTime.TryParseExact(Data, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ReturnData))
            {
                return ReturnData.Date;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// <para>Attempts to set the value in the element by sending <paramref name="input"/> as keystrokes in the format you specify.</para>
        /// <para><b>NOTE:</b> input-date elements expect dates in a format that is browser dependant! See <see cref="ChromiumFormat"/> and <see cref="FirefoxFormat"/> for Chrome and Firefox respectively.</para>
        /// </summary>
        public void SetDate(DateTime input, string format)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(format);
            string InputAsString = input.ToString(format);

            _ = InternalRetryActionInvoker("Failed to set data in input[type='date']-element by keystrokes", () =>
            {
                WrappedElement.Clear();
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });
                WrappedElement.SendKeys(input.ToString(format));
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });

                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == InputAsString;
            });
        }

        /// <summary>
        /// Same as <see cref="SetDate(DateTime, string)"/> but using <see cref="DateOnly"/> instead.
        /// </summary>
        public void SetDate(DateOnly input, string format)
        {
            SetDate(input.ToDateTime(TimeOnly.MinValue), format);
        }

        /// <summary>
        /// <para>Attempts to set the value in the element by setting <paramref name="input"/> directly in the <c>value</c>-attribute using Javascript (in the format <see cref="ValueAttributeFormat"/>).</para>
        /// <para>This solution is browser agnostic, and should work for all input-date elements. It is also ReactJS friendly which normally does not update its internal state if you manipulate the value directly via Javascript.</para>
        /// </summary>
        public void SetDateByJS(DateTime input)
        {
            string InputAsString = input.ToString(ValueAttributeFormat);

            string ValueChangerScript = $@"
                    arguments[0].blur();

                    if (!(arguments[0] instanceof HTMLInputElement))
                        throw new Error('Element is not an input element: ' + arguments[0].tagName);

                    let NativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
                    NativeSetter.call(arguments[0], '{InputAsString}');
                    arguments[0].dispatchEvent(new Event('change', {{bubbles: true}}));
                ";

            string ValueCheckerScript = $@"
                    let eventHandlerKeyName = Object.keys(arguments[0]).find(current => current.startsWith(""__reactEventHandlers""));
                    if (!eventHandlerKeyName) return arguments[0].value === '{InputAsString}';

                    let eventHandlers = arguments[0][eventHandlerKeyName];
                    return (eventHandlers.value === '{InputAsString}');
                ";

            _ = InternalRetryActionInvoker("Failed to set value in input[type='date']-element by Javascript", () =>
            {
                JavaScriptExecutor.ExecuteScript(ValueChangerScript, WrappedElement);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return (bool)JavaScriptExecutor.ExecuteScript(ValueCheckerScript, WrappedElement);
            });
        }

        /// <summary>
        /// Attempts to set the date in the element by setting <paramref name="input"/> directly in the value-attribute using Javascript (in the format <see cref="ValueAttributeFormat"/>).
        /// This solution is browser agnostic, and should work for all input-date elements. It is also ReactJS friendly which normally does not update its internal state if you manipulate the value directly via JS.
        /// </summary>
        public void SetDateByJS(DateOnly input)
        {
            SetDateByJS(input.ToDateTime(TimeOnly.MinValue));
        }
    }
}
