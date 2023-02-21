
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace TMech.Core
{

    /// <summary>
    /// <para>A class for fetching HTML-elements - in the form of Element-instances - that may be sensitive to errors due to for example dynamic loading/background updates etc.<br/>
    /// Also provides conditional locating strategies (see TryFetchWhen) for fetching elements once they fullfill certain criteria.</para>
    /// <para>Built around the central premise that elements may not always be available the moment you search for them, and that not finding them does not by default constitute an exception.<br/>
    /// This puts the responsibility of handling not finding an element - and any eventual exceptions - in your hands.</para>
    /// </summary>
    public sealed class ElementFactory
    {
        public TimeSpan DefaultTimeout {get;} = TimeSpan.FromSeconds(5.0d);
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

        private ElementFactory() {}
        private readonly ISearchContext SearchContext;

        #region GET

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator and throws an exception if it cannot be found within the timeout this instance of <see cref='ElementFactory'/> is configured with.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        public Element? Fetch(By locator)
        {
            (bool Success, Element? Element, ExceptionDispatchInfo? Error) = InternalTryFetch(locator, Timeout);
            if (!Success) Error.Throw(); // Should be guaranteed NOT to be null if Success is false
            return Element;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator and throws an exception if it cannot be found within the timeout you pass.
        /// </summary>
        /// <returns>The element matching <paramref name='locator'/>. If an exception is thrown, then it will be null.</returns>
        public Element? Fetch(By locator, TimeSpan timeout)
        {
            (bool Success, Element? Element, ExceptionDispatchInfo? Error) = InternalTryFetch(locator, timeout);
            if (!Success) Error.Throw();
            return Element;
        }

        #endregion

        #region TRY FETCH

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout this instance of <see cref='ElementFactory'/> is configured with.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered.</param>
        /// <returns>True if the element was found within the timeout, false otherwise.</returns>
        public bool TryFetch(By locator, out Element? element, out ExceptionDispatchInfo? error)
        {
            (bool Success, element, error) = InternalTryFetch(locator, Timeout);
            return Success;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout you pass.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered.</param>
        /// <returns>True if the element was found within the timeout, false otherwise.</returns>
        public bool TryFetch(By locator, TimeSpan timeout, out Element? element, out ExceptionDispatchInfo? error)
        {
            (bool Success, element, error) = InternalTryFetch(locator, timeout);
            return Success;
        }

        #endregion

        #region TRY FETCH ALL

        /// <summary>
        /// Attempts to fetch all elements that match the passed locator within the timeout this instance of <see cref='ElementFactory'/> is configured with.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is empty if the amount of elements that were found were below <paramref name='threshold'/> or if the timeout was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns>True if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, false otherwise.</returns>
        public bool TryFetchAll(By locator, out Element[] elements, uint threshold = 1)
        {
            (bool Success, elements) = InternalTryFetchAll(locator, Timeout, threshold);
            return Success;
        }

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout you pass.
        /// </summary>
        /// <param name="elements">A reference to the collection of elements that were found. The collection is empty if the amount of elements that were found were below <paramref name='threshold'/> or if the timeout was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns>True if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, false otherwise.</returns>
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
        public ElementWaiter TryFetchWhen(By locator)
        {
            return new ElementWaiter(this, locator, SearchContext, Timeout);
        }

        /// <summary>
        /// Produces a waiter that is configured to use this <see cref='ElementFactory'/>-instance to find the element matching <paramref name='locator'/> when it reaches certain conditions.
        /// </summary>
        public ElementWaiter TryFetchWhen(By locator, TimeSpan timeout)
        {
            return new ElementWaiter(this, locator, SearchContext, timeout);
        }

        #endregion

        /// <summary>Checks whether one or more elements exist that match the passed locator.</summary>
        public bool Exists(By locator)
        {
            return SearchContext.FindElements(locator).Count > 0;
        }

        #region PRIVATE

        private Tuple<bool, Element[]> InternalTryFetchAll(By locator, TimeSpan timeout, uint threshold)
        {
            var Timer = Stopwatch.StartNew();

            while (Timer.Elapsed <= timeout)
            {
                ReadOnlyCollection<IWebElement> Elements = SearchContext.FindElements(locator);

                if (Elements.Count < threshold)
                {
                    Thread.Sleep((int)PollingInterval);
                    continue;
                }

                var ReturnData = new Element[Elements.Count];
                for(int Index = 0; Index < Elements.Count; Index++)
                    ReturnData[Index] = new Element((WebElement)Elements[Index], this, locator, SearchContext, true);

                return new (true, ReturnData);
            }

            return new(false, Array.Empty<Element>());
        }

        // If return.item1 is false then return.item2 is null and return.item3 is not.
        // If return.item1 is true then return.item2 is not null and return.item3 is null.
        private Tuple<bool, Element?, ExceptionDispatchInfo?> InternalTryFetch(By locator, TimeSpan timeout)
        {
            var Timer = Stopwatch.StartNew();
            ExceptionDispatchInfo? LatestException = null;

            while (Timer.Elapsed <= timeout)
            {
                try
                {
                    IWebElement Element = SearchContext.FindElement(locator);
                    return new(true, new Element((WebElement)Element, this, locator, SearchContext, false), null);
                }
                catch (Exception error) when (error is WebDriverException)
                {
                    LatestException = ExceptionDispatchInfo.Capture(error);
                    Thread.Sleep((int)PollingInterval);
                }
            }

            return new(false, null, LatestException);
        }

        #endregion
    }
}
