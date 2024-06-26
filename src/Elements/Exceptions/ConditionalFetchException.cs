﻿using OpenQA.Selenium;
using System;

namespace TMech.Elements.Exceptions
{
    [Serializable]
    public class ConditionalFetchException : FetchContextException
    {
        public ConditionalFetchException(string condition, By locator, TimeSpan timeout)
            : base($"Failed to fetch element once {condition}. Locator: {locator.Mechanism} - {locator.Criteria} | timeout: {timeout}") { }

        public ConditionalFetchException(string condition, By locator, TimeSpan timeout, Exception inner)
            : base($"Failed to fetch element once {condition}. Locator: {locator.Mechanism} - {locator.Criteria} | timeout: {timeout}", inner) { }
    }

}