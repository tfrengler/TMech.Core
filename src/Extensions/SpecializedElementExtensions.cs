/*namespace TMech.Core.Extensions
{
    public static class SpecializedElementExtensions
    {
        /// <summary>
        /// Treat this element as an element that can be interacted with as a form control (input, textarea etc).
        /// </summary>
        public static FormControlElement AsFormControl(this Element self)
        {
            return new FormControlElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as a date.
        /// </summary>
        public static DateElement AsDate(this Element self)
        {
            return new DateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as a date.
        /// </summary>
        public static DateElement AsDate(this FormControlElement self)
        {
            return new DateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element that can be interacted with as a dropdown (select-tag).
        /// </summary>
        public static DropdownElement AsDropdown(this Element self)
        {
            return new DropdownElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element that can be interacted with as a dropdown (select-tag).
        /// </summary>
        public static DropdownElement AsDropdown(this FormControlElement self)
        {
            return new DropdownElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as a boolean.
        /// </summary>
        public static BooleanElement AsBoolean(this Element self)
        {
            return new BooleanElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as a boolean.
        /// </summary>
        public static BooleanElement AsBoolean(this FormControlElement self)
        {
            return new BooleanElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as an integer.
        /// </summary>
        public static IntegerElement AsInteger(this Element self)
        {
            return new IntegerElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as an integer.
        /// </summary>
        public static IntegerElement AsInteger(this FormControlElement self)
        {
            return new IntegerElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as a decimal.
        /// </summary>
        public static DecimalElement AsDecimal(this FormControlElement self)
        {
            return new DecimalElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be treated as a decimal.
        /// </summary>
        public static DecimalElement AsDecimal(this Element self)
        {
            return new DecimalElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }
    }
}
*/