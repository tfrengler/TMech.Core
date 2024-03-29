using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using TMech.Core.Exceptions;

namespace TMech.Core
{
    /// <summary>
    /// Represents a context in which elements can be fetched and searched for (a browser, element etc).
    /// </summary>
    public class FetchContext : IFetchContext
    {
        /// <summary>
        /// Creates a new fetch context around a webdriver with the standard timeout. The timeout will propagate to any elements fetched within this context.
        /// </summary>
        /// <param name="webdriver">The webdriver to wrap the fetch context around.</param>
        /// <param name="pollingInterval">Optional. The amount of miliseconds to wait between fetching and performing actions on elements.</param>
        public static FetchContext Create(IWebDriver webdriver, int pollingInterval = 300)
        {
            return new FetchContext(webdriver, (IJavaScriptExecutor)webdriver, TimeSpan.FromSeconds(5.0d), pollingInterval);
        }

        /// <summary>
        /// Creates a new fetch context around a webdriver with the given timeout. The timeout will propagate to any elements fetched within this context.
        /// </summary>
        /// <param name="webdriver">The webdriver to wrap the fetch context around.</param>
        /// <param name="timeout">The max amount of time to retry actions on elements before giving up.</param>
        /// <param name="pollingInterval">Optional. The amount of miliseconds to wait between fetching and performing actions on elements.</param>
        public static FetchContext Create(IWebDriver webdriver, TimeSpan timeout, int pollingInterval = 300)
        {
            return new FetchContext(webdriver, (IJavaScriptExecutor)webdriver, timeout, pollingInterval);
        }
        
        internal FetchContext(ISearchContext context, IJavaScriptExecutor jsExecutor, TimeSpan timeout, int pollingInterval)
        {
            Debug.Assert(context is not null, $"Argument {nameof(context)} should not be null");
            Debug.Assert(jsExecutor is not null, $"Argument {nameof(jsExecutor)} should not be null");
            Debug.Assert(pollingInterval > 0, $"Argument {nameof(pollingInterval)} must be greater than 0");
            Debug.Assert(timeout != TimeSpan.Zero, $"Argument {nameof(timeout)} must be greater than 0");

            PollingInterval = pollingInterval;
            Timeout = timeout;
            SearchContext = context;
            JsExecutor = jsExecutor;
        }

        public int PollingInterval { get; }
        public TimeSpan Timeout { get; }
        /// <summary>If the context of this instance is an element rather than the browser then this holds a reference to the element-instance it was created by.</summary>
        public IElement? Parent { get; init; }
        
        private ISearchContext SearchContext;
        private readonly IJavaScriptExecutor JsExecutor;

        #region FETCH

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator and throws an exception if it cannot be found within the timeout this instance of <see cref='IFetchContext'/> is configured with.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        /// <exception cref="FetchElementException"></exception>
        public IElement Fetch(By locator)
        {
            (bool Success, Element? Element, Exception Error) = InternalTryFetch(locator, Timeout);
            if (!Success)
            {
                Debug.Assert(Error is not null);
                throw new FetchElementException(locator, Timeout, Error);
            }
            return Element!;
        }

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout this instance of <see cref='IFetchContext'/> is configured with.<br/>
        /// Will throw an exception if fewer elements than passed in <paramref name="threshold"/> have been found before the timeout.
        /// </summary>
        /// <param name="threshold">The amount of elements that must match the locator before returning.</param>
        /// <returns>A collection of elements matching the locator at or above the <paramref name="threshold"/>.</returns>
        /// <exception cref="FetchElementException"></exception>
        public IElement[] FetchAll(By locator, uint threshold = 1)
        {
            (bool Success, bool FailedDueToStalenessOnContext, Element[] elements) = InternalTryFetchAll(locator, Timeout, threshold);
            if (!Success)
            {
                if (!FailedDueToStalenessOnContext)
                {
                    throw new FetchElementException(locator, Timeout);
                }

                throw new FetchElementException(locator, Timeout, new ReacquireElementException());
            }

            return elements;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout this instance of <see cref='IFetchContext'/> is configured with.<br/>
        /// Great for fine grained control (or during development) where you can precisely trial element lookups and inspect/handle WebDriverExceptions.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered. Never null if return value is <see langword="false"/>.</param>
        /// <returns>True if the element was found within the timeout, false otherwise.</returns>
        public bool TryFetch(By locator, [NotNullWhen(true)] out IElement? element, [NotNullWhen(false)] out Exception? error)
        {
            var Result = InternalTryFetch(locator, Timeout);
            element = Result.Item2!;
            error = Result.Item3!;
            return Result.Item1;
        }

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout this instance of <see cref='IFetchContext'/> is configured with.<br/>
        /// NOTE: If you set <paramref name="threshold"/> to 0 then it effectively becomes a normal "find elements"-call, since it bypasses the retry mechanism, and whatever elements are located (or not) will be returned immediately.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is never null but will always be empty if the return value was <see langword="false"/>.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns><see langword="true"/> if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, <see langword="false"/> otherwise.</returns>
        public bool TryFetchAll(By locator, out IElement[] elements, uint threshold = 1)
        {
            var Result = InternalTryFetchAll(locator, Timeout, threshold);
            elements = Result.Item3;
            return Result.Item1;
        }

        #endregion

        /// <summary>Checks whether an element exists. Will throw an exception if the check could not be completed before the timeout.</summary>
        /// <param name="locator">The locator for the element whose existence you want to check.</param>
        /// <param name="element">If the element exists (true is returned) it will be assigned to this parameter. If the function returns false then this will be <see langword="null"/>.</param>
        /// <returns><see langword="True"/> if the element exists, <see langword="False"/> otherwise. This can only fail if a parent element is stale.</returns>
        /// <exception cref="FetchContextException"/>
        public bool Exists(By locator, [NotNullWhen(true)] out IElement? element)
        {
            var Timer = Stopwatch.StartNew();
            while(Timer.Elapsed <= Timeout)
            {
                try
                {
                    var SearchResult = SearchContext.FindElements(locator);
                    
                    if (SearchResult.Count == 0)
                    {
                        element = null;
                        return false;
                    }

                    element = new Element((WebElement)SearchResult[0], this, locator, SearchContext, JsExecutor, false);
                    return true;
                }
                catch(StaleElementReferenceException)
                {
                    if (Parent is not null)
                    {
                        Thread.Sleep(PollingInterval);
                        SearchContext = Parent.Reacquire(false).WrappedElement;
                    }
                }
            }

            throw new FetchContextException($"Timed out trying find out if element exists (timeout: {Timeout})", new ReacquireElementException());
        }

        /// <summary>Checks the amount of elements that match a given locator within this context.</summary>
        /// <returns>The amount of elements that matches the locator. This can only fail if a parent element is stale.</returns>
        /// <exception cref="FetchContextException"></exception>
        public int AmountOf(By locator)
        {
            var Timer = Stopwatch.StartNew();
            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    var SearchResult = SearchContext.FindElements(locator);
                    return SearchResult.Count;
                }
                catch (StaleElementReferenceException)
                {
                    if (Parent is not null)
                    {
                        Thread.Sleep(PollingInterval);
                        SearchContext = Parent.Reacquire(false).WrappedElement;
                    }
                }
            }

            throw new FetchContextException($"Timed out trying find out how many elements exist (timeout: {Timeout})", new ReacquireElementException());
        }

        /// <summary>
        /// Returns a fetch context that can find elements once they fullfil certain conditions.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public ConditionalFetchContext When(By locator)
        {
            return new ConditionalFetchContext(SearchContext, this, JsExecutor, locator, Timeout, PollingInterval);
        }

        #region PRIVATE

        private Tuple<bool, bool, Element[]> InternalTryFetchAll(By locator, TimeSpan timeout, uint threshold)
        {
            var Timer = Stopwatch.StartNew();
            bool ContextIsStale = false;
            ReadOnlyCollection<IWebElement> Elements;

            while (Timer.Elapsed <= timeout)
            {
                ContextIsStale = false;
                try
                {
                    Elements = SearchContext.FindElements(locator);
                }
                catch (WebDriverException error)
                {
                    if (error is InvalidSelectorException)
                    {
                        throw new FetchElementException(locator, timeout, error);
                    }

                    if (error is not StaleElementReferenceException)
                    {
                        continue;
                    }

                    ContextIsStale = true;

                    if (Parent is not null)
                    {
                        Thread.Sleep(PollingInterval);
                        SearchContext = Parent.Reacquire(false).WrappedElement;
                    }

                    continue;
                }

                if (threshold > 0 && Elements.Count < threshold)
                {
                    Thread.Sleep(PollingInterval);
                    continue;
                }

                var ReturnData = new Element[Elements.Count];
                for (int Index = 0; Index < Elements.Count; Index++)
                {
                    ReturnData[Index] = new Element((WebElement)Elements[Index], this, locator, SearchContext, JsExecutor, true);
                }

                return new(true, false, ReturnData);
            }

            return new(false, ContextIsStale, Array.Empty<Element>());
        }

        // If return.item1 is false then return.item2 is null and return.item3 is not.
        // If return.item1 is true then return.item2 is not null and return.item3 is null.
        private Tuple<bool, Element?, WebDriverException?> InternalTryFetch(By locator, TimeSpan timeout)
        {
            var Timer = Stopwatch.StartNew();
            WebDriverException? LatestException = null;

            while (Timer.Elapsed <= timeout)
            {
                try
                {
                    IWebElement Element = SearchContext.FindElement(locator);
                    return new(true, new Element((WebElement)Element, this, locator, SearchContext, JsExecutor, false), null);
                }
                catch (WebDriverException error)
                {
                    if (error is InvalidSelectorException)
                    {
                        throw new FetchElementException(locator, timeout, error);
                    }

                    LatestException = error;
                    Thread.Sleep(PollingInterval);
                    if (error is StaleElementReferenceException && Parent is not null)
                    {
                        SearchContext = Parent.Reacquire(false).WrappedElement;
                    }
                }
            }

            return new(false, null, LatestException);
        }

        #endregion
    }
}
