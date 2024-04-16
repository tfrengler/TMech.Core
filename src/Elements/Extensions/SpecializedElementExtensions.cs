using Gdh.Art.Utils.Webdriver.Elements.Specialized;

namespace Gdh.Art.Utils.Webdriver.Elements.Extensions
{
    public static class SpecializedElementExtensions
    {
        /// <summary>
        /// Treat this element as a form control (input, textarea etc) whose value-attribute can be read and manipulated, along with other formcontrol specific attributes.
        /// </summary>
        public static FormControlElement AsFormControl(this Element self)
        {
            return new FormControlElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be parsed as a <see cref="System.DateTime"/>.
        /// </summary>
        public static DateElement AsDate(this Element self)
        {
            return new DateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this formcontrol element as an element whose value can be parsed as a <see cref="System.DateTime"/>.
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
        /// Treat this formcontrol element as an element that can be interacted with as a dropdown (select-tag).
        /// </summary>
        public static DropdownElement AsDropdown(this FormControlElement self)
        {
            return new DropdownElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value can be parsed as a boolean.
        /// </summary>
        public static BooleanElement AsBoolean(this Element self)
        {
            return new BooleanElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this formcontrol element as an element whose value can be parsed as a boolean.
        /// </summary>
        public static BooleanElement AsBoolean(this FormControlElement self)
        {
            return new BooleanElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this element as an element whose value-attribute can be parsed as a number.
        /// </summary>
        public static NumberElement AsNumeric(this Element self)
        {
            return new NumberElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        /// <summary>
        /// Treat this formcontrol element as an element whose value-attribute can be parsed as a number.
        /// </summary>
        public static NumberElement AsNumeric(this FormControlElement self)
        {
            return new NumberElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }
    }
}
