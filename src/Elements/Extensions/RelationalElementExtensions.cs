using OpenQA.Selenium;

namespace TMech.Core.Elements.Extensions
{
    public static class RelationalElementExtensions
    {
        public static Element? FetchNextSibling(this Element self, string tagName = "*")
        {
            var Locator = By.XPath($"./following-sibling::{tagName}[1]");
            return self.Elements(self.ProducedBy.Timeout).Fetch(Locator);
        }

        public static Element? FetchParent(this Element self)
        {
            var Locator = By.XPath("./parent::*[1]");
            return self.Elements(self.ProducedBy.Timeout).Fetch(Locator);
        }

        public static Element? FetchPreviousSibling(this Element self, string tagName = "*")
        {
            var Locator = By.XPath($"./preceding-sibling::{tagName}[1]");
            return self.Elements(self.ProducedBy.Timeout).Fetch(Locator);
        }

        public static Element? FetchAncestor(this Element self, string tagName = "*")
        {
            var Locator = By.XPath($"./ancestor::{tagName}[1]");
            return self.Elements(self.ProducedBy.Timeout).Fetch(Locator);
        }

        public static Element[] FetchDescendants(this Element self, string tagName = "*", uint threshold = 1)
        {
            return InternalGetChildrenOrDescendants(self, tagName, false, threshold);
        }

        public static Element[] FetchChildren(this Element self, string tagName = "*", uint threshold = 1)
        {
            return InternalGetChildrenOrDescendants(self, tagName, true, threshold);
        }

        private static Element[] InternalGetChildrenOrDescendants(Element self, string tagName, bool children, uint threshold)
        {
            string Axis = children ? "child" : "descendant";
            var Locator = By.XPath($"./{Axis}::{tagName}");

            Element[] ReturnData = self.Elements(self.ProducedBy.Timeout).FetchAll(Locator, threshold);
            return ReturnData;
        }
    }
}
