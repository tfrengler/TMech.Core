
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using TMech.Core.Exceptions;

namespace TMech.Core.Elements
{

    /// <summary>
    /// <para>A class for fetching HTML-elements - in the form of Element-instances - that may be sensitive to errors due to for example dynamic loading/background updates etc.<br/>
    /// Also provides conditional locating strategies (see <see cref="FetchWhen"/>) for fetching elements once they fullfill certain criteria.</para>
    /// <para>Built around the central premise that elements may not always be available the moment you search for them, and that not finding them does not by default constitute an exception.<br/>
    /// </summary>
    public sealed class ElementFactory
    {
        public TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(5.0d);
        public uint PollingInterval { get; set; } = 100;
        public TimeSpan Timeout { get; }

        /// <summary>Returns a new instance configured to use a specific context that will be used to search for elements within, using the default timeout.</summary>
        public ElementFactory(ISearchContext context)
        {
            SearchContext = context ?? throw new ArgumentNullException(nameof(context));
            Timeout = DefaultTimeout;
        }

        /// <summary>Returns a new instance configured to use a specific context that will be used to search for elements within, using the passed timeout (in seconds).</summary>
        public ElementFactory(ISearchContext context, TimeSpan timeout)
        {
            SearchContext = context ?? throw new ArgumentNullException(nameof(context));
            Timeout = timeout;
        }

        private readonly ISearchContext SearchContext;

        #region FETCH

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator and throws an exception if it cannot be found within the timeout this instance of <see cref='ElementFactory'/> is configured with.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element? Fetch(By locator)
        {
            (bool Success, Element? Element, WebDriverException? Error) = InternalTryFetch(locator, Timeout);
            if (!Success)
            {
                Debug.Assert(Error is not null);
                throw new FetchElementException(locator, Timeout, Error);
            }
            return Element;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator and throws an exception if it cannot be found within the timeout you pass.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element? Fetch(By locator, TimeSpan timeout)
        {
            (bool Success, Element? Element, WebDriverException? Error) = InternalTryFetch(locator, timeout);
            if (!Success)
            {
                Debug.Assert(Error is not null);
                throw new FetchElementException(locator, timeout, Error);
            }

            return Element;
        }

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout this instance of <see cref='ElementFactory'/> is configured with.<br/>
        /// Will throw an exception if fewer elements than passed in <paramref name="threshold"/> have been found before the timeout.
        /// </summary>
        /// <param name="threshold">The amount of elements that must match the locator before returning.</param>
        /// <returns>A collection of elements matching the locator at or above the <paramref name="threshold"/>.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element[] FetchAll(By locator, uint threshold = 1)
        {
            (bool Success, Element[] elements) = InternalTryFetchAll(locator, Timeout, threshold);
            if (!Success) throw new FetchElementException(locator, Timeout);

            return elements;
        }

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout specified.<br/>
        /// Will throw an exception if fewer elements than passed in <paramref name="threshold"/> have been found before the timeout.
        /// </summary>
        /// <param name="threshold">The amount of elements that must match the locator before returning.</param>
        /// <returns>A collection of elements matching the locator at or above the <paramref name="threshold"/>.</returns>
        /// <exception cref="FetchElementException"></exception>
        public Element[] FetchAll(By locator, TimeSpan timeout, uint threshold = 1)
        {
            (bool Success, Element[] elements) = InternalTryFetchAll(locator, timeout, threshold);
            if (!Success) throw new FetchElementException(locator, timeout);

            return elements;
        }

        #endregion

        #region TRY FETCH

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout this instance of <see cref='ElementFactory'/> is configured with.<br/>
        /// Great for fine grained control (or during development) where you can precisely trial element lookups and inspect/handle WebDriverExceptions.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered. Never null if return value is <see langword="false"/>.</param>
        /// <returns>True if the element was found within the timeout, false otherwise.</returns>
        public bool TryFetch(By locator, [NotNullWhen(true)] out Element? element, [NotNullWhen(false)] out WebDriverException? error)
        {
            (bool Success, element, error) = InternalTryFetch(locator, Timeout);
            return Success;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout you pass.<br/>
        /// Great for fine grained control (or during development) where you can precisely trial element lookups and inspect/handle WebDriverExceptions.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered. Never null if return value is <see langword="false"/>.</param>
        /// <returns>True if the element was found within the timeout, false otherwise.</returns>
        public bool TryFetch(By locator, TimeSpan timeout, [NotNullWhen(true)] out Element? element, [NotNullWhen(false)] out WebDriverException? error)
        {
            (bool Success, element, error) = InternalTryFetch(locator, timeout);
            return Success;
        }

        #endregion

        #region TRY FETCH ALL

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout this instance of <see cref='ElementFactory'/> is configured with.<br/>
        /// NOTE: If you set <paramref name="threshold"/> to 0 then it effectively becomes a normal "find elements"-call, since it bypasses the retry mechanism, and whatever elements are located (or not) will be returned immediately.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is never null and contains whatever elements were found after <paramref name="threshold"/> was reached or after <see cref="Timeout"/> was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns><see langword="true"/> if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, <see langword="false"/> otherwise.</returns>
        public bool TryFetchAll(By locator, out Element[] elements, uint threshold = 1)
        {
            (bool Success, elements) = InternalTryFetchAll(locator, Timeout, threshold);
            return Success;
        }

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout you pass.<br/>
        /// NOTE: If you set <paramref name="threshold"/> to 0 then it effectively becomes a normal "find elements"-call, since it bypasses the retry mechanism, and whatever elements are located (or not) will be returned immediately.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is never null and contains whatever elements were found after <paramref name="threshold"/> was reached or after <paramref name="timeout"/> was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns><see langword="true"/> if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, <see langword="false"/> otherwise.</returns>
        public bool TryFetchAll(By locator, TimeSpan timeout, out Element[] elements, uint threshold = 1)
        {
            (bool Success, elements) = InternalTryFetchAll(locator, timeout, threshold);
            return Success;
        }

        #endregion

        #region TRY FETCH WHEN

        /// <summary>
        /// Produces a waiter that is configured to use this <see cref='ElementFactory'/>-instance to find the element matching <paramref name='locator'/> when it reaches certain conditions.
        /// </summary>
        public ElementWaiter FetchWhen(By locator)
        {
            return new ElementWaiter(this, locator, SearchContext, Timeout);
        }

        /// <summary>
        /// Produces a waiter that is configured to use this <see cref='ElementFactory'/>-instance to find the element matching <paramref name='locator'/> when it reaches certain conditions.
        /// </summary>
        public ElementWaiter FetchWhen(By locator, TimeSpan timeout)
        {
            return new ElementWaiter(this, locator, SearchContext, timeout);
        }

        #endregion

        /// <summary>Checks whether one or more elements exist that match the passed locator.</summary>
        public bool Exists(By locator)
        {
            return SearchContext.FindElements(locator).Count > 0;
        }

        /// <summary>Returns the amount of elements that match the passed locator.</summary>
        public int Count(By locator)
        {
            return SearchContext.FindElements(locator).Count;
        }

        #region PRIVATE

        private Tuple<bool, Element[]> InternalTryFetchAll(By locator, TimeSpan timeout, uint threshold)
        {
            var Timer = Stopwatch.StartNew();

            while (Timer.Elapsed <= timeout)
            {
                ReadOnlyCollection<IWebElement> Elements = SearchContext.FindElements(locator);

                if (threshold > 0 && Elements.Count < threshold)
                {
                    Thread.Sleep((int)PollingInterval);
                    continue;
                }

                var ReturnData = new Element[Elements.Count];
                for (int Index = 0; Index < Elements.Count; Index++)
                    ReturnData[Index] = new Element((WebElement)Elements[Index], this, locator, SearchContext, true);

                return new(true, ReturnData);
            }

            return new(false, Array.Empty<Element>());
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
                    return new(true, new Element((WebElement)Element, this, locator, SearchContext, false), null);
                }
                catch (WebDriverException error)
                {
                    LatestException = error;
                    Thread.Sleep((int)PollingInterval);
                }
            }

            return new(false, null, LatestException);
        }

        #endregion
    }
}
