using OpenQA.Selenium;
using System;
using System.Globalization;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents an element whose value-attribute is treated as a 'date'. This is meant for an input-element of type 'date', but can also be used by input-text elements that contain date-strings.
    /// </summary>
    public sealed class DateElement : FormControlElement
    {
        public DateElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        public const string ValueAttributeFormat = "yyyy-MM-dd";
        public const string ChromiumFormat = "ddMMyyyy";
        public const string FirefoxFormat = "MMddyyyy";

        /// <summary>
        /// Identical to <see cref="GetData(string)"/>, using <see cref="ValueAttributeFormat"/> as the date format.
        /// </summary>
        public DateTime GetData()
        {
            return GetData(ValueAttributeFormat);
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
        public DateTime GetData(string format)
        {
            var Data = InternalRetryActionInvoker<string>("Failed to get data from date-element", () =>
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
        public void SetDataByKeystroke(DateTime input, string format)
        {
            _ = InternalRetryActionInvoker<bool>("Failed to set data in date-element", () =>
            {
                WrappedElement.Clear();
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });
                WrappedElement.SendKeys(input.ToString(format));
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });

                return true;
            });
        }

        /// <summary>
        /// Attempts to set the date in the element by setting <paramref name="input"/> directly in the value-attribute using Javascript (in the format <see cref="ValueAttributeFormat"/>).<br/>
        /// This solution is browser agnostic, and should work for all input-date elements.
        /// </summary>
        public void SetDataByJS(DateTime input)
        {
            _ = InternalRetryActionInvoker<bool>("Failed to set data in date-element", () =>
            {
                WrappedElement.Clear();
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });
                JavaScriptExecutor.ExecuteScript($"arguments[0].value = '{input.ToString(ValueAttributeFormat)}'", new object[] { WrappedElement });

                if (WrappedElement.GetAttribute("type") == "date")
                {
                    // Since scripted events aren't trusted by for example some ReactJS applications we need to interact with the field natively via the keyboard.
                    // Since we know that the year is ALWAYS last (only DD/MM vary between browsers) we simple move two steps right and write the year out.
                    WrappedElement.SendKeys(Keys.ArrowRight + Keys.ArrowRight + input.Year);
                }

                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });

                return true;
            });
        }
        
    }
}
