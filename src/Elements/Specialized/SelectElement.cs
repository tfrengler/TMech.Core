using OpenQA.Selenium;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TMech.Elements.Specialized
{
    /// <summary>
    /// Represents a dropdown, a &lt;select&gt;-element.
    /// </summary>
    public sealed class SelectElement : FormControlElement
    {
        internal SelectElement(WebElement wrappedElement, FetchContext producedBy, By relatedLocator, ISearchContext relatedContext, IJavaScriptExecutor javaScriptExecutor, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, javaScriptExecutor, locatedAsMultiple)
        {
        }

        public override SelectElement WithRobustSelection()
        {
            RobustSelection = true;
            return this;
        }

        public bool IsMultiple()
        {
            return InternalRetryActionInvoker($"Failed to determine if dropdown-element is multiple select", () =>
            {
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
                return Dropdown.IsMultiple;
            });
        }

        public void SelectByValue(string value)
        {
            _ = InternalRetryActionInvoker($"Failed to select option by value in dropdown-element (value: {value})", () =>
            {
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
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
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
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
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
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
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
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
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
                var Element = (WebElement)Dropdown.SelectedOption;
                string? Value = Element.GetAttribute("value");
                Debug.Assert(Value is not null);

                return new FormControlElement(Element, ProducedBy, By.CssSelector($"option[value='{Value}']"), RelatedContext, JavaScriptExecutor, false);
            });

            Debug.Assert(ReturnData is not null);
            return ReturnData;
        }

        public IList<FormControlElement> GetSelectedOptions()
        {
            IList<FormControlElement>? ReturnData = InternalRetryActionInvoker<IList<FormControlElement>>("Failed to get all selected options from dropdown-element", () =>
            {
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
                var Elements = Dropdown.AllSelectedOptions;

                var ReturnData = new List<FormControlElement>(Elements.Count);
                foreach (var CurrentElement in Elements)
                {
                    string? Value = CurrentElement.GetAttribute("value");
                    Debug.Assert(Value is not null);
                    var NewElement = new FormControlElement((WebElement)CurrentElement, ProducedBy, By.CssSelector($"option[value='{Value}']"), RelatedContext, JavaScriptExecutor, true);
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
                var Dropdown = new OpenQA.Selenium.Support.UI.SelectElement(WrappedElement);
                var Elements = Dropdown.Options;

                var ReturnData = new List<FormControlElement>(Elements.Count);
                foreach (var CurrentElement in Elements)
                {
                    string? Value = CurrentElement.GetAttribute("value");
                    Debug.Assert(Value is not null);
                    var NewElement = new FormControlElement((WebElement)CurrentElement, ProducedBy, By.CssSelector($"option[value='{Value}']"), RelatedContext, JavaScriptExecutor, true);
                    ReturnData.Add(NewElement);
                }

                return ReturnData;
            });

            Debug.Assert(ReturnData is not null);
            return ReturnData;
        }
    }
}
