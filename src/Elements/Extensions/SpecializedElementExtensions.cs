using TMech.Elements.Specialized;

namespace TMech.Elements.Extensions
{
    public static class SpecializedElementExtensions
    {
        /// <summary>
        /// Treat this element as a form control element.
        /// </summary>
        public static FormControlElement AsFormControl(this Element self)
        {
            return new FormControlElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an <c>&lt;input type='date'&gt;</c> element.
        /// </summary>
        public static InputDateElement AsInputDate(this Element self)
        {
            return new InputDateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an <c>&lt;input type='date'&gt;</c> element.
        /// </summary>
        public static InputDateElement AsInputDate(this FormControlElement self)
        {
            return new InputDateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as a <c>select</c> element.
        /// </summary>
        public static SelectElement AsDropdown(this Element self)
        {
            return new SelectElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as a <c>select</c> element.
        /// </summary>
        public static SelectElement AsDropdown(this FormControlElement self)
        {
            return new SelectElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an <c>&lt;input type='radio'&gt;</c> or <c>&lt;input type='checkbox'&gt;</c> element.
        /// </summary>
        public static InputRadioOrCheckboxElement AsInputRadioOrCheckbox(this Element self)
        {
            return new InputRadioOrCheckboxElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an <c>&lt;input type='radio'&gt;</c> or <c>&lt;input type='checkbox'&gt;</c> element.
        /// </summary>
        public static InputRadioOrCheckboxElement AsInputRadioOrCheckbox(this FormControlElement self)
        {
            return new InputRadioOrCheckboxElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an <c>&lt;input type='number'&gt;</c> element.
        /// </summary>
        public static InputNumberElement AsInputNumber(this Element self)
        {
            return new InputNumberElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an <c>&lt;input type='number'&gt;</c> element.
        /// </summary>
        public static InputNumberElement AsInputNumber(this FormControlElement self)
        {
            return new InputNumberElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.JavaScriptExecutor, self.LocatedAsMultiple);
        }
    }
}
