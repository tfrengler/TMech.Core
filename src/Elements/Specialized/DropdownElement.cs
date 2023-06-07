using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Diagnostics;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents a dropdown - &lt;select&gt;-element - that acts as a wrapper around <see cref="SelectElement"/>, producing <see cref="FormControlElement"/>-instances instead of Selenium's <see cref="IWebDriver"/>.<br/>
    /// Be aware that the elements produced by the methods in this class cannot be reacquired on stale element errors, because they are not directly produced by <see cref="ElementFactory"/> and therefore their <see cref="Element.RelatedLocator"/> is incorrect.
    /// </summary>
    public sealed class DropdownElement : FormControlElement
    {
        public DropdownElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        public bool IsMultiple()
        {
            return InternalRetryActionInvoker<bool>($"Failed to determine if dropdown-element is multiple select", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                return Dropdown.IsMultiple;
            });
        }

        public void SelectByValue(string value)
        {
            _ = InternalRetryActionInvoker<bool>($"Failed to select option by value in dropdown-element (value: {value})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.SelectByValue(value);
                return true;
            });
        }

        public void DeselectByText(string text)
        {
            _ = InternalRetryActionInvoker<bool>($"Failed to deselect option by text in dropdown-element (text: {text})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.DeselectByText(text);
                return true;
            });
        }

        public void DeselectByValue(string value)
        {
            _ = InternalRetryActionInvoker<bool>($"Failed to deselect option by value in dropdown-element (value: {value})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.DeselectByValue(value);
                return true;
            });
        }

        public void SelectByText(string text)
        {
            _ = InternalRetryActionInvoker<bool>($"Failed to select option by text in dropdown-element (text: {text})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.SelectByText(text);
                return true;
            });
        }

        public FormControlElement GetSelectedOption()
        {
            FormControlElement? ReturnData = InternalRetryActionInvoker<FormControlElement>("Failed to get selected option from dropdown-element", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                var Element = (WebElement)Dropdown.SelectedOption;
                var Value = Element.GetAttribute("value");
                return new FormControlElement(Element, ProducedBy, By.CssSelector($"option[value='{Value}']"), RelatedContext, false);
            });

            Debug.Assert(ReturnData is not null);
            return ReturnData;
        }

        public IList<FormControlElement> GetSelectedOptions()
        {
            IList<FormControlElement>? ReturnData = InternalRetryActionInvoker<IList<FormControlElement>>("Failed to get all selected options from dropdown-element", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                var Elements = Dropdown.AllSelectedOptions;

                var ReturnData = new List<FormControlElement>(Elements.Count);
                foreach (var CurrentElement in Elements)
                {
                    var Value = CurrentElement.GetAttribute("value");
                    var NewElement = new FormControlElement((WebElement)CurrentElement, ProducedBy, By.CssSelector($"option[value='{Value}']"), RelatedContext, true);
                    ReturnData.Add(NewElement);
                }

                return ReturnData;
            });

            Debug.Assert(ReturnData is not null);
            return ReturnData;
        }

        public IList<FormControlElement> GetOptions()
        {
            IList<FormControlElement>? ReturnData = InternalRetryActionInvoker<IList<FormControlElement>>("Failed to get options from dropdown-element", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                var Elements = Dropdown.Options;

                var ReturnData = new List<FormControlElement>(Elements.Count);
                foreach (var CurrentElement in Elements)
                {
                    var Value = CurrentElement.GetAttribute("value");
                    var NewElement = new FormControlElement((WebElement)CurrentElement, ProducedBy, By.CssSelector($"option[value='{Value}']"), RelatedContext, true);
                    ReturnData.Add(NewElement);
                }

                return ReturnData;
            });

            Debug.Assert(ReturnData is not null);
            return ReturnData;
        }
    }
}
