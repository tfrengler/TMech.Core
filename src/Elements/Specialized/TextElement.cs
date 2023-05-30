using OpenQA.Selenium;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents either a textarea-element or an input-element whose type is 'text'.
    /// </summary>
    public sealed class TextElement : Element, ISetsSingleData<string>, IHasSingleData<string>
    {
        public TextElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple)
            : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple) { }

        /// <summary>
        /// Attempts to retrieve the data from the input-text element by reading out its value-attribute.
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            var ReturnData = InternalRetryActionInvoker<string>("Failed to get data from text-element", () =>
            {
                return WrappedElement.GetAttribute("value");
            });

            return ReturnData ?? string.Empty;
        }

        /// <summary>
        /// Attempts to set data in this input-text element by simply typing in it.
        /// </summary>
        public void SetData(string input)
        {
            SendKeys(input);
        }
    }
}
