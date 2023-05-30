using TMech.Core.Elements.Specialized;

namespace TMech.Core.Elements.Extensions
{
    public static class SpecializedElementExtensions
    {
        public static TextElement AsText(this Element self)
        {
            return new TextElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }

        public static DateElement AsDate(this Element self)
        {
            return new DateElement(self.WrappedElement, self.ProducedBy, self.RelatedLocator, self.RelatedContext, self.LocatedAsMultiple);
        }
    }
}
