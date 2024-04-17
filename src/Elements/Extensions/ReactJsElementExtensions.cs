using OpenQA.Selenium;
using System.Diagnostics;
using System.Threading;
using System;
using TMech.Elements.Exceptions;

namespace TMech.Elements.Extensions
{
    /// <summary>
    /// Contains special extension methods to wait for ReactJS specific conditions.
    /// </summary>
    public static class ReactJsElementExtensions
    {
        public const string FindEventHandlerScript = @"
            if (arguments.length === 0)
            {
                throw new Error('Expected to have at least two arguments');
            }

            if (!(arguments[0] && arguments[0] instanceof HTMLElement))
            {
                throw new Error('Expected arguments[0] to be an HTML-element');
            }

            if (!(arguments[1] && typeof arguments[1] === 'string' && arguments[1].length > 0))
            {
                throw new Error('Expected arguments[1] to be a string and not empty');
            }

            let TheElement = arguments[0];
            let EventHandlerName = arguments[1];
            let EventHandler, EventHandlers = undefined;

            let EventHandlerKeyName = Object.keys(TheElement).find(x => x.startsWith('__reactEventHandlers'));
            if (EventHandlerKeyName) {
                EventHandlers = TheElement[EventHandlerKeyName];
                EventHandler = EventHandlers[EventHandlerName];
            }
        ";

        /// <summary>
        /// Waits for an element to have a ReactJS onClick-handler registered before returning.
        /// </summary>
        /// <exception cref="ConditionalFetchException"></exception>
        public static Element HasOnClickHandler(this ConditionalFetchContext self)
        {
            return InternalEventHandlerChecker(self, self.JavaScriptExecutor, "onClick", "it had a ReactJS onClick-handler attached");
        }

        /// <summary>
        /// Waits for an element to have a ReactJS onChange-handler registered before returning.
        /// </summary>
        /// <exception cref="ConditionalFetchException"></exception>
        public static Element HasOnChangeHandler(this ConditionalFetchContext self)
        {
            return InternalEventHandlerChecker(self, self.JavaScriptExecutor, "onChange", "it had a ReactJS onChange-handler attached");
        }

        private static Element InternalEventHandlerChecker(ConditionalFetchContext waiter, IJavaScriptExecutor jsExecutor, string eventHandlerName, string errorMessage)
        {
            Debug.Assert(waiter is not null);
            Debug.Assert(jsExecutor is not null);
            Debug.Assert(eventHandlerName is not null);
            Debug.Assert(errorMessage is not null);

            var Timer = Stopwatch.StartNew();
            Exception? Error = null;
            string Script = FindEventHandlerScript + " return EventHandler && EventHandlers.disabled === false";

            while (Timer.Elapsed <= waiter.Timeout)
            {
                try
                {
                    var Element = waiter.WrappedContext.Fetch(waiter.Locator);
                    bool Success = (bool)jsExecutor.ExecuteScript(Script, Element.UnWrap(), eventHandlerName);
                    if (Success) return Element;
                }
                catch (WebDriverException exception)
                {
                    Error = exception;
                    Thread.Sleep((int)waiter.WrappedContext.PollingInterval);
                }
            }

            throw new ConditionalFetchException(errorMessage, waiter.Locator, waiter.Timeout, Error!);
        }
    }
}
