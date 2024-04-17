using OpenQA.Selenium;

namespace TMech.Utils
{
    /// <summary>
    /// Contains helper methods for constructing Selenium locators.
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// Creates a Selenium locator that matches one or more elements whose id-attribute ends with a specific value.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to none, thus matching any element type.</param>
        public static By ByIdEndsWith(string id, string tagName = "")
        {
            return By.CssSelector($"{tagName}[id$='{id}']");
        }

        /// <summary>
        /// Creates a Selenium locator that matches one or more elements whose id-attribute starts with a specific value.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to none, thus matching any element type.</param>
        public static By ByIdStartsWith(string id, string tagName = "")
        {
            return By.CssSelector($"{tagName}[id^='{id}']");
        }

        /// <summary>
        /// Creates a Selenium locator that matches one or more elements whose id-attribute contains a specific value.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to none, thus matching any element type.</param>
        public static By ByIdContains(string id, string tagName = "")
        {
            return By.CssSelector($"{tagName}[id*='{id}']");
        }

        /// <summary>
        /// Creates a Selenium locator that matches one or more elements whose inner text equals a specific value.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to *, thus matching any element type.</param>
        public static By ByTextEquals(string text, string tagName = "*")
        {
            return By.XPath($".//{tagName}[normalize-space(text())='{text}']");
        }

        /// <summary>
        /// Creates a Selenium locator that matches one or more elements where either its own inner text - or any of its descendants' - equals a specific value.
        /// Text is normalized before matching, so you don't have to account for leading or trailing whitespaces for example.
        /// Note that unless you specify <paramref name="tagName"/> you are likely to get the first (outermost) element in the DOM tree whose descendant has the specific <paramref name="text"/>.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to *, thus matching any element type.</param>
        public static By BySelfOrDescendantTextEquals(string text, string tagName = "*")
        {
            return By.XPath($".//{tagName}[normalize-space(.)='{text}']");
        }

        /// <summary>
        /// Creates a Selenium locator that matches one or more elements whose inner text contains a specific value.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to *, thus matching any element type.</param>
        public static By ByTextContains(string text, string tagName = "*")
        {
            return By.XPath($".//{tagName}[contains(normalize-space(text()),{text})]");
        }

        /// <summary>
        /// Creates a Selenium locator that matches one or more elements where either its own inner text - or any of its descendants' - contains a specific value.
        /// Text is normalized before matching, so you don't have to account for leading or trailing whitespaces for example.
        /// Note that unless you specify <paramref name="tagName"/> you are likely to get the first (outermost) element in the DOM tree whose descendant has the specific <paramref name="text"/>.
        /// </summary>
        /// <param name="tagName">Optional. Which HTML-tagname to limit the locator to finding. Defaults to *, thus matching any element type.</param>
        public static By BySelfOrDescendantTextContains(string text, string tagName = "*")
        {
            return By.XPath($".//{tagName}[contains(normalize-space(.),{text})]");
        }
    }
}
