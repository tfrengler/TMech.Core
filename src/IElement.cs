using OpenQA.Selenium;
using System;

namespace TMech.Core
{
    public interface IElement
    {
        public WebElement WrappedElement { get; }

        internal IElement Reacquire(bool throwOnError);
        public IElement Click();
        public IElement ClickUntil(Func<IElement, bool> predicate);
        public IElement ScrollIntoView();
        public IElement SendKeystrokes(string keysequence);

        public string GetInnerHTML();
        public string GetTagName();
        public string GetInnerText(bool removeAdditionalWhitespace = true);
        public string GetId();
        public string GetTitle();
        public string GetAttribute(string name);
        public string GetCssClass();
        public string[] GetCssClasses();
        public bool IsDisplayed();

        public IFetchContext Within();
    }
}
