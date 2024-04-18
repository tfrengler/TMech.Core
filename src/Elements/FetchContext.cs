using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using TMech.Elements.Exceptions;

namespace TMech.Elements
{
    /// <summary>
    /// <para>Represents a context (a browser or another element) in which elements can be fetched within a timeout.
    /// Also provides conditional locating strategies (see <see cref="FetchWhen"/>) for fetching elements once they fullfill certain criteria.</para>
    /// <para>Built around the central premise that elements may not always be available the moment you search for them, and that not finding them does not by default constitute an exception.</para>
    /// </summary>
    public sealed class FetchContext
    {
        /// <summary>Creates a new context around a webdriver (browser) using the passed timeout (in seconds).</summary>
        public static FetchContext Create(IWebDriver webdriver, TimeSpan timeout)
        {
            return new FetchContext(webdriver, timeout);
        }

        /// <summary>Creates a new context around a webdriver (browser) using the default timeout (5 seconds).</summary>
        public static FetchContext Create(IWebDriver webdriver)
        {
            return new FetchContext(webdriver, TimeSpan.FromSeconds(5.0d));
        }

        /// <summary>
        /// The amount of time (in miliseconds) to wait between fetch attempts and other actions. Is propagated to fetched elements.
        /// </summary>
        public uint PollingInterval { get; set; } = 300;
        public TimeSpan Timeout { get; }
        /// <summary>
        /// The Selenium search context that elements are searched for within. When <see cref="Parent"/> is not <c>null</c> then this is a <see cref="WebElement"/>, otherwise an <see cref="IWebDriver"/>.
        /// </summary>
        public ISearchContext SearchContext { get; private set; }
        public IJavaScriptExecutor JavascriptExecutor { get; }

        /// <summary>
        /// If this instance was spawned by a call to <see cref="Element.Within()"/>, then this refers to the <see cref="Element"/>-instance it came from.
        /// </summary>
        public Element? Parent { get; init; }

        internal FetchContext(IWebDriver webdriver, TimeSpan timeout)
        {
            ArgumentNullException.ThrowIfNull(webdriver);

            SearchContext = webdriver;
            Timeout = timeout;
            JavascriptExecutor = (IJavaScriptExecutor)webdriver;
        }

        internal FetchContext(ISearchContext context, IJavaScriptExecutor javaScriptExecutor, TimeSpan timeout)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(javaScriptExecutor);

            SearchContext = context;
            Timeout = timeout;
            JavascriptExecutor = javaScriptExecutor;
        }

        #region FETCH

        /// <summary>
        /// Fetches an element that matches the <paramref name="locator"/> and throws an exception if it cannot be found within the timeout.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element Fetch(By locator)
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
        /// Fetches an element that matches the <paramref name="locator"/> and throws an exception if it cannot be found within <paramref name="timeout"/>.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element Fetch(By locator, TimeSpan timeout)
        {
            (bool Success, Element? Element, Exception Error) = InternalTryFetch(locator, timeout);
            if (!Success)
            {
                Debug.Assert(Error is not null);
                throw new FetchElementException(locator, timeout, Error);
            }

            return Element!;
        }

        /// <summary>
        /// Fetches all elements that match <paramref name="locator"/> within the timeout based on a treshold.
        /// </summary>
        /// <param name="threshold">The amount of elements that must match the locator before returning. If fewer elements match the threshold before the timeout an exception will be thrown.</param>
        /// <returns>A collection of elements matching <paramref name="locator"/> at or above the <paramref name="threshold"/>.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element[] FetchAll(By locator, uint threshold = 1)
        {
            (bool Success, bool FailedDueToStalenessOnContext, Element[] elements) = InternalTryFetchAll(locator, Timeout, threshold);
            if (!Success)
            {
                if (!FailedDueToStalenessOnContext)
                    throw new FetchElementException(locator, Timeout);

                throw new FetchElementException(locator, Timeout, new ReacquireElementException());
            }

            return elements;
        }

        /// <summary>
        /// Fetches all elements that match <paramref name="locator"/> within the <paramref name="timeout"/> based on a treshold.
        /// </summary>
        /// <param name="threshold">The amount of elements that must match the locator before returning. If fewer elements match the threshold before the <paramref name="timeout"/> an exception will be thrown.</param>
        /// <returns>A collection of elements matching <paramref name="locator"/> at or above the <paramref name="threshold"/>.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element[] FetchAll(By locator, TimeSpan timeout, uint threshold = 1)
        {
            (bool Success, bool FailedDueToStalenessOnContext, Element[] elements) = InternalTryFetchAll(locator, timeout, threshold);
            if (!Success)
            {
                if (!FailedDueToStalenessOnContext)
                    throw new FetchElementException(locator, Timeout);

                throw new FetchElementException(locator, Timeout, new ReacquireElementException());
            }

            return elements;
        }

        #endregion

        #region TRY FETCH

        /// <summary>
        /// Attempts to fetch an element that matches the <paramref name="locator"/> within the timeout.
        /// Great for fine grained control (during development of debugging) where you can precisely trial element lookups and inspect/handle exceptions.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or <c>null</c> if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or <c>null</c> if no exceptions were encountered. Will never be <c>null</c> if return value is <see langword="false"/>.</param>
        /// <returns><see langword="True"/> if the element was found within the timeout, <see langword="False"/> otherwise.</returns>
        public bool TryFetch(By locator, [NotNullWhen(true)] out Element? element, [NotNullWhen(false)] out Exception? error)
        {
            var Result = InternalTryFetch(locator, Timeout);
            element = Result.Item2;
            error = Result.Item3;
            return Result.Item1;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the <paramref name="locator"/> within the <paramref name="timeout"/>.
        /// Great for fine grained control (during development of debugging) where you can precisely trial element lookups and inspect/handle exceptions.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered. Never null if return value is <see langword="false"/>.</param>
        /// <returns><see langword="True"/> if the element was found within the timeout, <see langword="False"/> otherwise.</returns>
        public bool TryFetch(By locator, TimeSpan timeout, [NotNullWhen(true)] out Element? element, [NotNullWhen(false)] out Exception? error)
        {
            var Result = InternalTryFetch(locator, timeout);
            element = Result.Item2;
            error = Result.Item3;
            return Result.Item1;
        }

        #endregion

        #region TRY FETCH ALL

        /// <summary>
        /// Attempts to fetch all elements that match the <paramref name="locator"/> within the timeout.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is never <c>null</c> and contains whatever elements were found after <paramref name="threshold"/> was reached or after <see cref="Timeout"/> was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns><see langword="True"/> if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, <see langword="False"/> otherwise.</returns>
        public bool TryFetchAll(By locator, out Element[] elements, uint threshold = 1)
        {
            var Result = InternalTryFetchAll(locator, Timeout, threshold);
            elements = Result.Item3;
            return Result.Item1;
        }

        /// <summary>
        /// Attempts to fetch all elements that match the <paramref name="locator"/> within the <paramref name="timeout"/>.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is never <c>null</c> and contains whatever elements were found after <paramref name="threshold"/> was reached or after <see cref="Timeout"/> was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns><see langword="True"/> if the amount of elements found were at or above <paramref name='threshold'/> within the <paramref name="timeout"/>, <see langword="False"/> otherwise.</returns>
        public bool TryFetchAll(By locator, TimeSpan timeout, out Element[] elements, uint threshold = 1)
        {
            var Result = InternalTryFetchAll(locator, timeout, threshold);
            elements = Result.Item3;
            return Result.Item1;
        }

        #endregion

        #region FETCH WHEN

        /// <summary>
        /// Returns a context in which you can fetch elements by certain conditions.
        /// </summary>
        public ConditionalFetchContext FetchWhen(By locator)
        {
            return new ConditionalFetchContext(this, JavascriptExecutor, locator, Timeout);
        }

        /// <summary>
        /// Returns a context in which you can fetch elements by certain conditions, and a certain <paramref name="timeout"/>.
        /// </summary>
        public ConditionalFetchContext FetchWhen(By locator, TimeSpan timeout)
        {
            return new ConditionalFetchContext(this, JavascriptExecutor, locator, timeout);
        }

        #endregion

        #region EXISTS AND AMOUNTOF

        /// <summary>Checks whether one or more elements exist that match the <paramref name="locator"/>.</summary>
        public bool Exists(By locator)
        {
            return SearchContext.FindElements(locator).Count > 0;
        }

        /// <summary>Checks whether an element exists, and retrieves it if it does. Will throw an exception if the check could not be completed before the timeout.</summary>
        /// <param name="locator">The locator for the element whose existence you want to check.</param>
        /// <param name="element">If the element exists (true is returned) it will be assigned to this parameter. If the function returns false then this will be <see langword="null"/>.</param>
        /// <returns><see langword="True"/> if the element exists, <see langword="False"/> otherwise. This can only fail if a parent element is stale.</returns>
        /// <exception cref="FetchElementException"/>
        public bool Exists(By locator, [NotNullWhen(true)] out Element? element)
        {
            var Timer = Stopwatch.StartNew();
            while (Timer.Elapsed <= Timeout)
            {
                try
                {
                    var SearchResult = SearchContext.FindElements(locator);

                    if (SearchResult.Count == 0)
                    {
                        element = null;
                        return false;
                    }

                    element = new Element((WebElement)SearchResult[0], this, locator, SearchContext, JavascriptExecutor, false);
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    Debug.Assert(SearchContext is WebElement);
                    Debug.Assert(Parent is not null);
                    
                    Thread.Sleep((int)PollingInterval);
                    SearchContext = Parent.Reacquire(false).WrappedElement;
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
                    Debug.Assert(SearchContext is WebElement);
                    Debug.Assert(Parent is not null);

                    Thread.Sleep((int)PollingInterval);
                    SearchContext = Parent!.Reacquire(false).WrappedElement;
                }
            }

            throw new FetchContextException($"Timed out trying find out how many elements exist (timeout: {Timeout})", new ReacquireElementException());
        }

        #endregion

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

                    Debug.Assert(SearchContext is WebElement);
                    Debug.Assert(Parent is not null);

                    Thread.Sleep((int)PollingInterval);
                    SearchContext = Parent!.Reacquire(false).WrappedElement;

                    continue;
                }

                if (threshold > 0 && Elements.Count < threshold)
                {
                    Thread.Sleep((int)PollingInterval);
                    continue;
                }

                var ReturnData = new Element[Elements.Count];
                for (int Index = 0; Index < Elements.Count; Index++)
                {
                    ReturnData[Index] = new Element((WebElement)Elements[Index], this, locator, SearchContext, JavascriptExecutor, true);
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
                    return new(true, new Element((WebElement)Element, this, locator, SearchContext, JavascriptExecutor, false), null);
                }
                catch (WebDriverException error)
                {
                    if (error is InvalidSelectorException)
                    {
                        throw new FetchElementException(locator, timeout, error);
                    }

                    LatestException = error;
                    Thread.Sleep((int)PollingInterval);
                    if (error is StaleElementReferenceException)
                    {
                        Debug.Assert(SearchContext is WebElement);
                        Debug.Assert(Parent is not null);

                        SearchContext = Parent!.Reacquire(false).WrappedElement;
                    }
                }
            }

            return new(false, null, LatestException);
        }

        #endregion
    }
}
