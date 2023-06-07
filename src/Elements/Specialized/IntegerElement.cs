using OpenQA.Selenium;
using System;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents an input-element whose value-attribute can be treated as a numeric, integer value. These are typically input-number or -range elements.
    /// </summary>
    public sealed class IntegerElement : FormControlElement
    {
        public IntegerElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        /// <summary>
        /// Returns the value as an integer. Returns null if the value cannot be parsed as an integer.
        /// </summary>
        public int? GetNumber()
        {
            return InternalRetryActionInvoker<int?>("", () =>
            {
                string Value = WrappedElement.GetAttribute("value");
                if (int.TryParse(Value, out int ReturnData))
                {
                    return ReturnData;
                }

                return null;
            });
        }

        /// <summary>
        /// Attempts to write the given number to the field.
        /// </summary>
        public void SetNumber(int number)
        {
            SetValue(Convert.ToString(number), true);
        }

        /// <summary>
        /// Attempts to write the given number to the field, represented as a string. Throws an exception if <paramref name="number"/> cannot be converted to an integer.
        /// </summary>
        public void SetNumber(string number)
        {
            if (!int.TryParse(number, out int ParsedInteger))
            {
                throw new ArgumentException("Cannot parsed argument as number: " + number, nameof(number));
            }

            SetNumber(ParsedInteger);
        }
    }
}
