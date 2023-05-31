using OpenQA.Selenium;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents an element whose value-attribute is treated as a string. This is typically a textarea-element or an input-element whose type is 'text', although most inputs (such as date, url, file, number etc) can be treated as text as well.
    /// </summary>
    public sealed class TextElement : Element
    {
        public TextElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple)
            : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple) { }

        /// <summary>
        /// Attempts to retrieve the data from the text element by reading out its value-attribute.
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            return GetValue();
        }

        /// <summary>
        /// Attempts to set data in this input-text element by sending <paramref name="input"/> as keystrokes.
        /// </summary>
        public void SetData(string input, bool clear = true)
        {
            SendKeys(input, clear);
        }
    }
}
