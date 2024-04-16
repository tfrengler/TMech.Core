using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Threading;

namespace Gdh.Art.Utils.Webdriver.Elements.Specialized
{
    /// <summary>
    /// Represents an element whose value-attribute is treated as a 'date'. This is meant for an input-element of type 'date', but can also be used by input-text elements that contain date-strings.
    /// </summary>
    public sealed class DateElement : FormControlElement
    {
        internal DateElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        /// <summary>
        /// Configure this instance to use robust selection, meaning all "setters" will try and ensure that the value/state is what you desire before returning.
        /// </summary>
        public override DateElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        public const string ValueAttributeFormat = "yyyy-MM-dd";
        public const string ChromiumFormat = "ddMMyyyy";
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
        /// Attempts to read the data from this input-date element and parse it as a <see cref="DateTime"/>-instance where only the date-component is set (time is set to 00:00:00).<br/>
        /// If it fails for whatever reason to parse the value as a valid date then <see cref="DateTime.MinValue"/> is returned.
        /// </para>
        /// Common causes for parse failure:
        /// <list type="bullet">
        ///     <item>Leading or trailing whitespace in the value</item>
        ///     <item>Value being an empty string or pure whitespace</item>
        ///     <item>Value being an invalid date</item>
        ///     <item>Value being in a format that does not match the date-format in <paramref name="format"/></item>
        ///     <item><paramref name="format"/> is empty or null</item>
        ///     <item>Invalid date format passed in <paramref name="format"/></item>
        /// </list>
        /// </summary>
        public DateTime GetDate(string format = ValueAttributeFormat)
        {
            var Data = InternalRetryActionInvoker("Failed to get data from date-element", () =>
            {
                return WrappedElement.GetAttribute("value") ?? string.Empty;
            });

            if (DateTime.TryParseExact(Data, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ReturnData))
            {
                return ReturnData.Date;
            }

            return default;
        }

        /// <summary>
        /// Attempts to set the date in the element by sending <paramref name="input"/> as keystrokes in the format you specify.<br/>
        /// NOTE: input-date elements expect dates in a format that is browser dependant! See <see cref="ChromiumFormat"/> and <see cref="FirefoxFormat"/> for Chrome and Firefox respectively.
        /// </summary>
        public void SetDateByKeystroke(DateTime input, string format)
        {
            string InputAsString = input.ToString(ValueAttributeFormat);

            _ = InternalRetryActionInvoker("Failed to set data in date-element", () =>
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
        /// Same as <see cref="SetDateByKeystroke(DateTime, string)"/> but using <see cref="DateOnly"/> instead.
        /// </summary>
        public void SetDateByKeystroke(DateOnly input, string format)
        {
            SetDateByKeystroke(input.ToDateTime(TimeOnly.MinValue), format);
        }

        /// <summary>
        /// Attempts to set the date in the element by setting <paramref name="input"/> directly in the value-attribute using Javascript (in the format <see cref="ValueAttributeFormat"/>).
        /// This solution is browser agnostic, and should work for all input-date elements. It is also ReactJS friendly which normally does not update its internal state if you manipulate the value directly via JS.
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

            _ = InternalRetryActionInvoker("Failed to set data in date-element", () =>
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
