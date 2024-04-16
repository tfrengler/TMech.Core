using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Gdh.Art.Utils.Webdriver.Elements.Specialized
{
    /// <summary>
    /// Represents an HTML-element whose value-attribute can be treated as a number. These are typically input-elements of type 'number'.
    /// </summary>
    public sealed class NumberElement : FormControlElement
    {
        internal NumberElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple) { }

        public override NumberElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        /// <summary>
        /// The default number parsing format to use when setting and getting decimal numbers (floats and doubles).
        /// Configured as the number having a dot (.) as decimal separator, no group (thousand) separators and no currency symbol.
        /// </summary>
        public static NumberFormatInfo DefaultDecimalFormat { get; } = new NumberFormatInfo()
        {
            CurrencyDecimalDigits = 3,
            CurrencyGroupSeparator = "",
            CurrencySymbol = ""
        };

        #region SETTERS

        /// <summary>
        /// Attempts to set the value in this element to a given signed 32-bit integer.
        /// </summary>
        public void SetInteger(int input)
        {
            string ValueToSet = Convert.ToString(input);
            _ = InternalRetryActionInvoker("Failed to set integer in value-attribute of element", () =>
            {
                WrappedElement.Clear();
                WrappedElement.SendKeys(ValueToSet);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == ValueToSet;
            });
        }

        /// <summary>
        /// Attempts to set the value in this element to a given signed 64-bit integer.
        /// </summary>
        public void SetLong(long input)
        {
            string ValueToSet = Convert.ToString(input);
            _ = InternalRetryActionInvoker("Failed to set long in value-attribute of element", () =>
            {
                WrappedElement.Clear();
                WrappedElement.SendKeys(ValueToSet);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == ValueToSet;
            });
        }

        /// <summary>
        /// Attempts to set the value in this element to a given single precision (32-bit) decimal number.
        /// </summary>
        /// <param name="input">The float to send to the element. Is converted to a string according to <see cref="DefaultDecimalFormat"/>.</param>
        public void SetFloat(float input)
        {
            string ValueToSet = input.ToString("C", DefaultDecimalFormat);
            _ = InternalRetryActionInvoker("Failed to set float in value-attribute of element", () =>
            {
                WrappedElement.Clear();
                WrappedElement.SendKeys(ValueToSet);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == ValueToSet;
            });
        }

        /// <summary>
        /// Attempts to set the value in this element to a given double precision (64-bit) decimal number.
        /// </summary>
        /// <param name="input">The double to send to the element. Is converted to a string according to <see cref="DefaultDecimalFormat"/>.</param>
        public void SetDouble(double input)
        {
            string ValueToSet = input.ToString("C", DefaultDecimalFormat);
            _ = InternalRetryActionInvoker("Failed to set double in value-attribute of element", () =>
            {
                WrappedElement.Clear();
                WrappedElement.SendKeys(ValueToSet);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == ValueToSet;
            });
        }

        /// <summary>
        /// Attempts to set the value in this element to a given single precision (32-bit) decimal number.
        /// </summary>
        /// <param name="input">The float to send to the element.</param>
        /// <param name="format">The format to use to convert <paramref name="input"/> to a string before being sent to the browser.</param>
        public void SetFloat(float input, NumberFormatInfo format)
        {
            Debug.Assert(format is not null);

            string ValueToSet = input.ToString("C", format);
            _ = InternalRetryActionInvoker("Failed to set float in value-attribute of element", () =>
            {
                WrappedElement.Clear();
                WrappedElement.SendKeys(ValueToSet);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == ValueToSet;
            });
        }

        /// <summary>
        /// Attempts to set the value in this element to a given double precision (64-bit) decimal number.
        /// </summary>
        /// <param name="input">The double to send to the element.</param>
        /// <param name="format">The format to use to convert <paramref name="input"/> to a string before being sent to the browser.</param>
        public void SetDouble(double input, NumberFormatInfo format)
        {
            Debug.Assert(format is not null);

            string ValueToSet = input.ToString("C", format);
            _ = InternalRetryActionInvoker("Failed to set double in value-attribute of element", () =>
            {
                WrappedElement.Clear();
                WrappedElement.SendKeys(ValueToSet);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.GetAttribute("value") == ValueToSet;
            });
        }

        #endregion

        #region GETTERS

        /// <summary>
        /// Attempts to retrieve and parse the element's value-attribute as a signed 32-bit integer.
        /// </summary>
        /// <returns>The value as an integer, or 0 if the element's value was empty.</returns>
        public int GetInteger()
        {
            return InternalRetryActionInvoker<int>("Failed to parse the element's value-attribute as an integer", () =>
            {
                string? Value = WrappedElement.GetAttribute("value");
                return Convert.ToInt32(Value);
            }); 
        }

        /// <summary>
        /// Attempts to retrieve and parse the element's value-attribute as a signed 64-bit integer.
        /// </summary>
        /// <returns>The value as a long, or 0 if the element's value was empty.</returns>
        public long GetLong()
        {
            return InternalRetryActionInvoker<long>("Failed to parse the element's value-attribute as a long", () =>
            {
                string? Value = WrappedElement.GetAttribute("value");
                return Convert.ToInt64(Value);
            });
        }

        /// <summary>
        /// Attempts to retrieve and parse the element's value-attribute as a single precision (32-bit) decimal value. The value is parsed to a float using the rules of <see cref="DefaultDecimalFormat"/>.
        /// </summary>
        /// <returns>The value as an float with 3 decimal places, or 0 if the element's value was empty.</returns>
        /// <exception cref="FormatException">Thrown if the value-attribute did not contain a value that adheres to the format of <see cref="DefaultDecimalFormat"/>.</exception>
        public float GetFloat()
        {
            return InternalRetryActionInvoker<float>("Failed to parse the element's value-attribute as a float", () =>
            {
                string? Value = WrappedElement.GetAttribute("value");
                return Convert.ToSingle(Value, DefaultDecimalFormat);
            });
        }

        /// <summary>
        /// Attempts to retrieve and parse the element's value-attribute as a double precision (64-bit) decimal value. The value is parsed to a double using the rules of <see cref="DefaultDecimalFormat"/>.
        /// </summary>
        /// <returns>The value as a double with 3 decimal places, or 0 if the element's value was empty.</returns>
        /// <exception cref="FormatException">Thrown if the value-attribute did not contain a value that adheres to the format of <see cref="DefaultDecimalFormat"/>.</exception>
        public double GetDouble()
        {
            return InternalRetryActionInvoker<double>("Failed to parse the element's value-attribute as a double", () =>
            {
                string? Value = WrappedElement.GetAttribute("value");
                return Convert.ToDouble(Value, DefaultDecimalFormat);
            });
        }

        /// <summary>
        /// Attempts to retrieve and parse the element's value-attribute as a single precision (32-bit) decimal value.
        /// </summary>
        /// <param name="parseFormat">The format to parse the value with. This indicates how many decimals places to keep, if there are currency symbols to keep in mind, what the decimal separator is etc.</param>
        /// <returns>The value as a float as parsed according to <paramref name="parseFormat"/>, or 0 if the element's value was empty.</returns>
        /// <exception cref="FormatException">Thrown if the value-attribute did not contain a value that adheres to the format of <paramref name="parseFormat"/>.</exception>
        public float GetFloat(NumberFormatInfo parseFormat)
        {
            Debug.Assert(parseFormat is not null);

            return InternalRetryActionInvoker<float>("Failed to parse the element's value-attribute as a float", () =>
            {
                string? Value = WrappedElement.GetAttribute("value");
                return Convert.ToSingle(Value, parseFormat);
            });
        }

        /// <summary>
        /// Attempts to retrieve and parse the element's value-attribute as a double precision (64-bit) decimal value.
        /// </summary>
        /// <param name="parseFormat">The format to parse the value with. This indicates how many decimals places to keep, if there are currency symbols to keep in mind, what the decimal separator is etc.</param>
        /// <returns>The value as a double as parsed according to <paramref name="parseFormat"/>, or 0 if the element's value was empty.</returns>
        /// <exception cref="FormatException">Thrown if the value-attribute did not contain a value that adheres to the format of <paramref name="parseFormat"/>.</exception>
        public double GetDouble(NumberFormatInfo parseFormat)
        {
            Debug.Assert(parseFormat is not null);

            return InternalRetryActionInvoker<double>("Failed to parse the element's value-attribute as a double", () =>
            {
                string? Value = WrappedElement.GetAttribute("value");
                return Convert.ToDouble(Value, parseFormat);
            });
        }

        #endregion
    }
}
