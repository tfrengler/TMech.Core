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
    }
}
