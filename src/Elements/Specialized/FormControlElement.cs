﻿using OpenQA.Selenium;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents a form control element, such as input, radio, textarea etc. Allows access to standard methods that are more or less applicable to all form controls, and allows you to set and get its data (stored in the value-attribute) as text (string).
    /// </summary>
    public class FormControlElement : Element
    {
        public FormControlElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple)
            : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple) { }

        /// <summary>
        /// Attempts to set data in this input-text element by sending <paramref name="input"/> as keystrokes.
        /// </summary>
        public void SetValue(string input, bool clear = true)
        {
            SendKeys(input, clear);
        }

        #region DATA RETRIEVERS

        /// <summary>
        /// Attempts to retrieve the data from the form control-element by reading out its value-attribute.
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the value", () =>
            {
                return WrappedElement.GetAttribute("value");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the value of the name-attribute.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the form control name-value", () =>
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
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the form control min-value", () =>
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
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the form control max-value", () =>
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
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the form control maxlength-value", () =>
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
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the form control minlength-value", () =>
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
            string? ReturnData = InternalRetryActionInvoker("Failed to retrieve the form control src-value", () =>
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
            string? AttributeValue = InternalRetryActionInvoker("Failed to retrieve the form control step-value", () =>
            {
                return WrappedElement.GetAttribute("step");
            });

            if (int.TryParse(AttributeValue, out int ReturnData)) return ReturnData;
            return null;
        }

        #endregion

        #region STATE CHECKERS

        /// <summary>
        /// Checks whether the form control element is readonly.
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
        /// Checks whether the form control element is selected.
        /// </summary>
        /// <returns><see langword="true"/> if the element is selected or <see langword="false"/> if not or the element is not selectable (is not a radio-button or checkbox).</returns>
        public bool IsSelected()
        {
            bool ReturnData = InternalRetryActionInvoker("Failed to determine if form-element is selected", () =>
            {
                return WrappedElement.Selected;
            });

            return ReturnData;
        }

        /// <summary>
        /// Checks whether the form control element is readonly.
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
        /// Checks whether the form control element is enabled.
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
