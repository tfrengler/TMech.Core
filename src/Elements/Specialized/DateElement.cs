using OpenQA.Selenium;
using System;

namespace TMech.Core.Elements.Specialized
{
    /// <summary>
    /// Represents an input-element of type 'date'.
    /// </summary>
    public sealed class DateElement : Element, IHasSingleData<DateTime>, ISetsSingleData<DateTime>
    {
        public DateElement(WebElement wrappedElement, ElementFactory producedBy, By relatedLocator, ISearchContext relatedContext, bool locatedAsMultiple) : base(wrappedElement, producedBy, relatedLocator, relatedContext, locatedAsMultiple)
        {
        }

        /// <summary>
        /// Attempts to read the data from this input-date element and parse it as a <see cref="DateTime"/>. If it fails to parse then <see cref="DateTime.MinValue"/> is returned.
        /// </summary>
        /// <returns></returns>
        public DateTime GetData()
        {
            var Data = InternalRetryActionInvoker<string>("Failed to get data from date-element", () =>
            {
                return WrappedElement.GetAttribute("value") ?? string.Empty;
            });


            if (DateTime.TryParse(Data, out DateTime ReturnData))
            {
                return ReturnData;
            }

            return default;
        }

        public void SetData(DateTime input)
        {
            _ = InternalRetryActionInvoker<bool>("Failed to set data in date-element", () =>
            {
                WrappedElement.Clear();
                // We will blur (unfocus) the element just to be safe, in case a previous action has caused the cursor to be in this field already
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });
                // Use JS to change the field's value-attribute directly using the US format (which is what ALL input-date fields use under the hood)
                JavaScriptExecutor.ExecuteScript($"arguments[0].value = '{input.ToString("yyyy-MM-dd")}'", new object[] { WrappedElement });

                // Since scripted events aren't usually trusted they don't trigger for exaxmple ReactJS to actually update the field hence we need to interact with the field natively
                // Since we know that the year is ALWAYS last (only DD/MM vary between browsers) we simple move two steps right and write the year out
                WrappedElement.SendKeys(Keys.ArrowRight + Keys.ArrowRight + input.Year);
                JavaScriptExecutor.ExecuteScript("arguments[0].blur();", new object[] { WrappedElement });

                return true;
            });
        }
    }
}
