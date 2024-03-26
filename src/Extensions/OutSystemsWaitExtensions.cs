using OpenQA.Selenium;
using System.Diagnostics;
using System.Threading;
using System;
/*
namespace Gdh.Art.Utils.Webdriver.Elements.Extensions
{
    /// <summary>
    /// Contains special extension methods to wait for OutSystems specific conditions
    /// </summary>
    public static class OutSystemsWaitExtensions
    {
        private const string EventHandlerCheckScript = @"
            let eventHandlerKeyName = Object.keys(arguments[0]).find(current => current.startsWith(""__reactEventHandlers""))
            if (!eventHandlerKeyName) return false;

            let eventHandlers = arguments[0][eventHandlerKeyName];
            let eventHandler = eventHandlers[arguments[1]];

            return eventHandler !== undefined;";

        /// <summary>
        /// Waits for an element to have a ReactJS onClick-handler registered before returning.
        /// </summary>
        /// <exception cref="ElementWaitException"></exception>
        public static Element HasOnClickHandler(this ElementWaiter self, IJavaScriptExecutor jsExecutor)
        {
            Debug.Assert(jsExecutor is not null);

            var Timer = Stopwatch.StartNew();
            Exception? Error = null;

            while (Timer.Elapsed <= self.Timeout)
            {
                try
                {
                    var Element = self.WrappedFactory.Fetch(self.Locator);
                    bool Success = (bool)jsExecutor.ExecuteScript(EventHandlerCheckScript, new object[] { Element.UnWrap(), "onClick" });
                    if (Success) return Element;
                }
                catch (WebDriverException exception)
                {
                    Error = exception;
                    Thread.Sleep((int)self.WrappedFactory.PollingInterval);
                }
            }

            throw new ElementWaitException("it had an onClick-handler attached", self.Locator, self.Timeout, Error!);
        }
    }
}
*/