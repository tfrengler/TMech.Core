using OpenQA.Selenium;
using System;

namespace TMech.Core
{
    public sealed class ConditionalFetchContext
    {
        internal ConditionalFetchContext(IFetchContext factory, By locator) { }
        public FetchContext WrappedFactory { get; }
        public By Locator { get; }
        public TimeSpan Timeout { get; }

        public ConditionalFetchContext IsDisplayed() { return default; }
        public ConditionalFetchContext IsNotDisplayed() { return default; }
        public ConditionalFetchContext IsNotEnabled() { return default; }
        public ConditionalFetchContext IsEnabled() { return default; }
        public ConditionalFetchContext IsSelected() { return default; }
        public ConditionalFetchContext IsNotSelected() { return default; }
        public ConditionalFetchContext AttributeIsEqualTo(string attributeName, string attributeValue) { return default; }
        public ConditionalFetchContext AttributeStartsWith(string attributeName, string attributeValue) { return default; }
        public ConditionalFetchContext AttributeEndsWith(string attributeName, string attributeValue) { return default; }
        public ConditionalFetchContext AttributeContains(string attributeName, string attributeValue) { return default; }
        public ConditionalFetchContext AttributeHasContent(string attributeName) { return default; }
        public ConditionalFetchContext ContentIsEqualTo(string text) { return default; }
        public ConditionalFetchContext ContentIsNotEqualTo(string text) { return default; }
        public ConditionalFetchContext ContentStartsWith(string text) { return default; }
        public ConditionalFetchContext ContentEndsWith(string text) { return default; }
        public ConditionalFetchContext ContentContains(string text) { return default; }
        public ConditionalFetchContext HasContent() { return default; }
        public ConditionalFetchContext IsClickable() { return default; }
        public ConditionalFetchContext DoesNotExist() { return default; }

        public IElement? Fetch() { return default; }
    }
}
