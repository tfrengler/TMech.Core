using Gdh.Art.Utils.Webdriver.Elements.Exceptions;
using OpenQA.Selenium;
using System.Diagnostics;
using System.Threading;
using System;

namespace Gdh.Art.Utils.Webdriver.Elements.Extensions
{
    /// <summary>
    /// Contains special extension methods to wait for ReactJS specific conditions.
    /// </summary>
    public static class ReactJsElementExtensions
    {
        private const string EventHandlerCheckScript = @"
            let eventHandlerKeyName = Object.keys(arguments[0]).find(current => current.startsWith(""__reactEventHandlers""))
            if (!eventHandlerKeyName) return false;

            let eventHandlers = arguments[0][eventHandlerKeyName];
            let eventHandler = eventHandlers[arguments[1]];

            return eventHandler !== undefined && eventHandlers.disabled === false";

        /// <summary>
        /// Waits for an element to have a ReactJS onClick-handler registered before returning.
        /// </summary>
        /// <exception cref="ElementWaitException"></exception>
        public static Element HasOnClickHandler(this ElementWaiter self, IJavaScriptExecutor jsExecutor)
        {
            return InternalEventHandlerChecker(self, jsExecutor, "onClick", "it had a ReactJS onClick-handler attached");
        }

        /// <summary>
        /// Waits for an element to have a ReactJS onChange-handler registered before returning.
        /// </summary>
        /// <exception cref="ElementWaitException"></exception>
        public static Element HasOnChangeHandler(this ElementWaiter self, IJavaScriptExecutor jsExecutor)
        {
            return InternalEventHandlerChecker(self, jsExecutor, "onChange", "it had a ReactJS onChange-handler attached");
        }

        private static Element InternalEventHandlerChecker(ElementWaiter waiter, IJavaScriptExecutor jsExecutor, string eventHandlerName, string errorMessage)
        {
            Debug.Assert(waiter is not null);
            Debug.Assert(jsExecutor is not null);
            Debug.Assert(eventHandlerName is not null);
            Debug.Assert(errorMessage is not null);

            var Timer = Stopwatch.StartNew();
            Exception? Error = null;

            while (Timer.Elapsed <= waiter.Timeout)
            {
                try
                {
                    var Element = waiter.WrappedFactory.Fetch(waiter.Locator);
                    bool Success = (bool)jsExecutor.ExecuteScript(EventHandlerCheckScript, new object[] { Element.UnWrap(), eventHandlerName });
                    if (Success) return Element;
                }
                catch (WebDriverException exception)
                {
                    Error = exception;
                    Thread.Sleep((int)waiter.WrappedFactory.PollingInterval);
                }
            }

            throw new ElementWaitException(errorMessage, waiter.Locator, waiter.Timeout, Error!);
        }
    }
}
