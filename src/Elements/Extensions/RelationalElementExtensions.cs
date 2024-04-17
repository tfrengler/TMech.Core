using OpenQA.Selenium;

namespace TMech.Elements.Extensions
{
    public static class RelationalElementExtensions
    {
        /// <summary>
        /// <para>
        /// Attempts to get the sibling of the current element that comes after it in the hierarchy. A sibling is an element that is on the same level in the same nested DOM tree.
        /// </para>
        /// Example:<br/>
        /// <code>
        /// &lt;div id='parent'&gt;<br/>
        /// &#32;&lt;span id='child1'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant1'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &#32;&lt;span id='child2'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant2'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &lt;/div&gt;
        /// </code>
        /// If you have a handle to <b>span#child1</b> and you call this method, you should get <b>span#child2</b>
        /// </summary>
        public static Element FetchNextSibling(this Element self, string tagName = "*")
        {
            var Locator = By.XPath($"(./following-sibling::{tagName})[1]");
            return self.Within(self.ProducedBy.Timeout).Fetch(Locator);
        }


        /// <summary>
        /// <para>
        /// Attempts to get the parent of the current element. A parent is the element that is one level above in the DOM tree that the current element is nested inside.
        /// </para>
        /// Example:<br/>
        /// <code>
        /// &lt;div id='parent'&gt;<br/>
        /// &#32;&lt;span id='child1'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant1'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &#32;&lt;span id='child2'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant2'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &lt;/div&gt;
        /// </code>
        /// If you have a handle to <b>span#child1</b> and you call this method, you should get <b>div#parent</b><br/>
        /// If you have a handle to <b>p#descendant1</b> and you call this method, you should get <b>span#child1</b>
        /// </summary>
        public static Element FetchParent(this Element self)
        {
            var Locator = By.XPath("(./parent::*)[1]");
            return self.Within(self.ProducedBy.Timeout).Fetch(Locator);
        }

        /// <summary>
        /// <para>
        /// Attempts to get the sibling of the current element that comes before it in the hierarchy. A sibling is an element that is on the same level in the same nested DOM tree.
        /// </para>
        /// Example:<br/>
        /// <code>
        /// &lt;div id='parent'&gt;<br/>
        /// &#32;&lt;span id='child1'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant1'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &#32;&lt;span id='child2'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant2'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &lt;/div&gt;
        /// </code>
        /// If you have a handle to <b>span#child2</b> and you call this method, you should get <b>span#child1</b>
        /// </summary>
        public static Element FetchPreviousSibling(this Element self, string tagName = "*")
        {
            var Locator = By.XPath($"(./preceding-sibling::{tagName})[1]");
            return self.Within(self.ProducedBy.Timeout).Fetch(Locator);
        }


        /// <summary>
        /// <para>
        /// Attempts to get an ancestor of the current element. An ancestor is an element that is on a level above the current element in the same nested DOM tree.
        /// </para>
        /// Example:<br/>
        /// <code>
        /// &lt;div id='parent'&gt;<br/>
        /// &#32;&lt;span id='child1'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant1'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &#32;&lt;span id='child2'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant2'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &lt;/div&gt;
        /// </code>
        /// If you have a handle to <b>p#descendant2</b> and you call this method, you should get <b>span#child2</b>.<br/>
        /// If you have a handle to <b>p#descendant2</b> and you call this method with <paramref name="tagName"/> = <b>div</b>, you should get <b>div#parent</b>.
        /// </summary>
        public static Element FetchAncestor(this Element self, string tagName = "*")
        {
            var Locator = By.XPath($"(./ancestor::{tagName})[1]");
            return self.Within(self.ProducedBy.Timeout).Fetch(Locator);
        }

        /// <summary>
        /// <para>
        /// Attempts to get the descendants of the current element. A descendant is an element that is nested anywhere within the current element.
        /// </para>
        /// Example:<br/>
        /// <code>
        /// &lt;div id='parent'&gt;<br/>
        /// &#32;&lt;span id='child1'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant1'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &#32;&lt;span id='child2'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant2'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &lt;/div&gt;
        /// </code>
        /// If you have a handle to <b>div#parent</b> and you call this method, you should get <b>span#child1</b>, <b>span#child2</b>, <b>p#descendant1</b> and <b>p#descendant2</b><br/>
        /// If you have a handle to <b>div#parent</b> and you call this method with <paramref name="tagName"/> = <b>p</b>, you should get <b>p#descendant1</b> and <b>p#descendant2</b><br/>
        /// </summary>
        public static Element[] FetchDescendants(this Element self, string tagName = "*", uint threshold = 1)
        {
            return InternalGetChildrenOrDescendants(self, tagName, false, threshold);
        }

        /// <summary>
        /// <para>
        /// Attempts to get the children of the current element. A child is an element that is nested immediately within the current element.
        /// </para>
        /// Example:<br/>
        /// <code>
        /// &lt;div id='parent'&gt;<br/>
        /// &#32;&lt;span id='child1'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant1'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &#32;&lt;span id='child2'&gt;<br/>
        /// &#32;&#32;&#32;&lt;p id='descendant2'&gt;&lt;/p&gt;<br/>
        /// &#32;&lt;/span&gt;<br/>
        /// &lt;/div&gt;
        /// </code>
        /// If you have a handle to <b>div#parent</b> and you call this method, you should get <b>span#child1</b> and <b>span#child2</b>.<br/>
        /// If you have a handle to <b>div#parent</b> and you call this method with <paramref name="tagName"/> = <b>p</b>, you will get nothing.<br/>
        /// </summary>
        public static Element[] FetchChildren(this Element self, string tagName = "*", uint threshold = 1)
        {
            return InternalGetChildrenOrDescendants(self, tagName, true, threshold);
        }

        private static Element[] InternalGetChildrenOrDescendants(Element self, string tagName, bool children, uint threshold)
        {
            string Axis = children ? "child" : "descendant";
            var Locator = By.XPath($"./{Axis}::{tagName}");

            Element[] ReturnData = self.Within(self.ProducedBy.Timeout).FetchAll(Locator, threshold);
            return ReturnData;
        }
    }
}
