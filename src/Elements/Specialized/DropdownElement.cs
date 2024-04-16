using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Gdh.Art.Utils.Webdriver.Elements.Specialized
{
    /// <summary>
    /// Represents a dropdown - &lt;select&gt;-element - that acts as a wrapper around <see cref="SelectElement"/>.
    /// </summary>
    public sealed class DropdownElement : FormControlElement
    {
        internal DropdownElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        /// <summary>
        /// Configure this instance to use robust selection, meaning all "setters" will try and ensure that the value/state is what you desire before returning.
        /// </summary>
        public override DropdownElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        public bool IsMultiple()
        {
            return InternalRetryActionInvoker($"Failed to determine if dropdown-element is multiple select", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                return Dropdown.IsMultiple;
            });
        }

        public void SelectByValue(string value)
        {
            _ = InternalRetryActionInvoker($"Failed to select option by value in dropdown-element (value: {value})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.SelectByValue(value);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return Dropdown.SelectedOption.GetAttribute("value") == value;
            });
        }

        public void DeselectByText(string text)
        {
            _ = InternalRetryActionInvoker($"Failed to deselect option by text in dropdown-element (text: {text})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.DeselectByText(text);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return Dropdown.SelectedOption.Text != text;
            });
        }

        public void DeselectByValue(string value)
        {
            _ = InternalRetryActionInvoker($"Failed to deselect option by value in dropdown-element (value: {value})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.DeselectByValue(value);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return Dropdown.SelectedOption.Text != value;
            });
        }

        public void SelectByText(string text, bool partialMatch = false)
        {
            _ = InternalRetryActionInvoker($"Failed to select option by text in dropdown-element (text: {text} | partial match? {partialMatch})", () =>
            {
                var Dropdown = new SelectElement(WrappedElement);
                Dropdown.SelectByText(text, partialMatch);
                if (!RobustSelection) return true;

                Thread.Sleep(RobustWaitTimeInMS);
                return partialMatch ? Dropdown.SelectedOption.Text.Contains(text) : Dropdown.SelectedOption.Text == text;
            });
        }

        public FormControlElement GetSelectedOption()
        {
            FormControlElement? ReturnData = InternalRetryActionInvoker("Failed to get selected option from dropdown-element", () =>
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
