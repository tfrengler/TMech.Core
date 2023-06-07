using OpenQA.Selenium;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents an input-checkbox or -radio element whose data is a boolean value, indicating whether it is checked or not.
    /// </summary>
    public sealed class BooleanElement : FormControlElement
    {
        public BooleanElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        /// <summary>
        /// Attempts to check the field if not already checked. Works for both radio- and checkbox-types.
        /// </summary>
        public void Select()
        {
            if (IsSelected()) return;
            _ = InternalRetryActionInvoker("Failed to select input-checkbox or -radio element", () =>
            {
                WrappedElement.Click();
                return true;
            });
        }

        /// <summary>
        /// Attempts to check the field if not already checked. Only works for checkbox-types as a radio-fields cannot directly be deselected.
        /// </summary>
        public void Deselect()
        {
            if (!IsSelected()) return;
            _ = InternalRetryActionInvoker("Failed to deselect input-checkbox or -radio element", () =>
            {
                WrappedElement.Click();
                return true;
            });
        }

        /// <summary>
        /// Checks whether the input-checkbox or -radio element is selected.
        /// </summary>
        /// <returns><see langword="true"/> if the element is selected or <see langword="false"/> if not or the element is not selectable (is not a radio-button or checkbox).</returns>
        public bool IsSelected()
        {
            bool ReturnData = InternalRetryActionInvoker("Failed to determine if input-checkbox or -radio element is selected", () =>
            {
                return WrappedElement.Selected;
            });

            return ReturnData;
        }
    }
}
