using OpenQA.Selenium;
using System;

namespace Gdh.Art.Utils.Webdriver.Elements.Exceptions
{
    [Serializable]
    public class ElementWaitException : ElementFactoryException
    {
        public ElementWaitException(string condition, By locator, TimeSpan timeout)
            : base($"Failed to fetch element once {condition}. Locator: {locator.Mechanism} - {locator.Criteria} | timeout: {timeout}") { }

        public ElementWaitException(string condition, By locator, TimeSpan timeout, Exception inner)
            : base($"Failed to fetch element once {condition}. Locator: {locator.Mechanism} - {locator.Criteria} | timeout: {timeout}", inner) { }
    }

}