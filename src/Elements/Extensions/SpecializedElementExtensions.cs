using TMech.Core.Elements.Specialized;

namespace TMech.Core.Elements.Extensions
{
    public static class SpecializedElementExtensions
    {
        public static FormControlElement AsFormControl(this Element self)
        {
            return new FormControlElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static DateElement AsDate(this Element self)
        {
            return new DateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static DateElement AsDate(this FormControlElement self)
        {
            return new DateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static DropdownElement AsDropdown(this Element self)
        {
            return new DropdownElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static DropdownElement AsDropdown(this FormControlElement self)
        {
            return new DropdownElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static BooleanElement AsBoolean(this Element self)
        {
            return new BooleanElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static BooleanElement AsBoolean(this FormControlElement self)
        {
            return new BooleanElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static IntegerElement AsInteger(this Element self)
        {
            return new IntegerElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static IntegerElement AsInteger(this FormControlElement self)
        {
            return new IntegerElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }
    }
}
