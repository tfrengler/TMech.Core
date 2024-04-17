using OpenQA.Selenium;
using System.Threading;

namespace TMech.Elements.Specialized
{
    /// <summary>
    /// Represents an input-checkbox or -radio element whose selected-attribute is treated as a boolean value, indicating whether it is checked or not.
    /// </summary>
    public sealed class InputRadioOrCheckboxElement : FormControlElement
    {
        internal InputRadioOrCheckboxElement(WebElement wrappedElement, FetchContext producedBy, By relatedLocator, ISearchContext relatedContext, IJavaScriptExecutor javaScriptExecutor, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, javaScriptExecutor, locatedAsMultiple)
        {
        }

        public override InputRadioOrCheckboxElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        /// <summary>
        /// Attempts to check the field if not already checked. Works for both radio- and checkbox-types.
        /// </summary>
        public void Check()
        {
            if (IsChecked()) return;
            _ = InternalRetryActionInvoker("Failed to select input-checkbox or -radio element", () =>
            {
                WrappedElement.Click();
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.Selected;
            });
        }

        /// <summary>
        /// Attempts to check or uncheck the element. Only works for checkbox-types as a radio-fields cannot directly be deselected.
        /// </summary>
        /// <param name="desiredState">Pass as <see langword="true"/> if you want the element checked, or <see langword="false"/> if you want it unchecked.</param>
        public void Check(bool desiredState)
        {
            _ = InternalRetryActionInvoker($"Failed to {(desiredState ? "" : "de")}select input-checkbox", () =>
            {
                if (WrappedElement.Selected && !desiredState || !WrappedElement.Selected && desiredState)
                {
                    WrappedElement.Click();
                }

                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return WrappedElement.Selected == desiredState;
            });
        }

        /// <summary>
        /// Attempts to uncheck the field if not already unchecked. Only works for checkbox-types as a radio-fields cannot directly be deselected.
        /// </summary>
        public void Uncheck()
        {
            if (!IsChecked()) return;
            _ = InternalRetryActionInvoker("Failed to deselect input-checkbox", () =>
            {
                WrappedElement.Click();
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return !WrappedElement.Selected;
            });
        }

        /// <summary>
        /// Checks whether the input-checkbox or -radio element is selected.
        /// </summary>
        /// <returns><see langword="true"/> if the element is selected or <see langword="false"/> if not or the element is not selectable (is not a radio-button or checkbox).</returns>
        public bool IsChecked()
        {
            bool ReturnData = InternalRetryActionInvoker("Failed to determine if input-checkbox or -radio element is selected", () =>
            {
                return WrappedElement.Selected;
            });

            return ReturnData;
        }
    }
}
