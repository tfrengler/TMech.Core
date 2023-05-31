using OpenQA.Selenium;
using System;

namespace TMech.Core.Exceptions
{
    [Serializable]
    public class FetchElementException : TMechException
    {
        public FetchElementException(By locator, TimeSpan timeout)
            : base($"Failed to fetch element(s) by locator: {locator.Mechanism} - {locator.Criteria} | timeout: {timeout}") { }

        public FetchElementException(By locator, TimeSpan timeout, Exception inner)
            : base($"Failed to fetch element(s) by locator: {locator.Mechanism} - {locator.Criteria} | timeout: {timeout}", inner) { }
    }

}