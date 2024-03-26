﻿using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using TMech.Core.Exceptions;

namespace TMech.Core
{
    /// <summary>
    /// Represents a context in which elements can be fetched and searched for.
    /// </summary>
    public class FetchContext : IFetchContext
    {
        public static FetchContext Create(ISearchContext context, int pollingInterval = 300)
        {
            return new FetchContext(context, TimeSpan.FromSeconds(5.0d), pollingInterval);
        }

        public static FetchContext Create(ISearchContext context, TimeSpan timeout, int pollingInterval = 300)
        {
            return new FetchContext(context, timeout, pollingInterval);
        }
        
        internal FetchContext(ISearchContext context, TimeSpan timeout, int pollingInterval)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            if (pollingInterval < 0) throw new ArgumentException($"Argument {nameof(pollingInterval)} must be greater than 0");
            if (timeout == TimeSpan.Zero) throw new ArgumentException($"Argument {nameof(timeout)} must be greater than 0");

            PollingInterval = pollingInterval;
            Timeout = timeout;
            SearchContext = context;
        }

        public int PollingInterval { get; }
        public TimeSpan Timeout { get; }
        private ISearchContext SearchContext;
        /// <summary>If the context of this instance is an element rather than the browser then this holds a reference to the element-instance it was created by.</summary>
        public IElement? Parent { get; init; }

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

        #endregion

        /// <summary>
        /// Attempts to fetch an element that matches the passed locator within the timeout this instance of <see cref='IFetchContext'/> is configured with.<br/>
        /// Great for fine grained control (or during development) where you can precisely trial element lookups and inspect/handle WebDriverExceptions.
        /// </summary>
        /// <param name="element">A reference to the element that was found, or null if the timeout was reached.</param>
        /// <param name="error">A reference to the latest exception that was captured while attempting to locate the element, or null if no exceptions were encountered. Never null if return value is <see langword="false"/>.</param>
        /// <returns>True if the element was found within the timeout, false otherwise.</returns>
        public bool TryFetch(By locator, [NotNullWhen(true)] out IElement element, [NotNullWhen(false)] out Exception error)
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
        /// <param name="elements">A reference to the collection of elements that were found. The collection is never null and contains whatever elements were found after <paramref name="threshold"/> was reached or after <see cref="Timeout"/> was reached.</param>
        /// <param name="threshold">The amount of elements that must match the locator.</param>
        /// <returns><see langword="true"/> if the amount of elements found were at or above <paramref name='threshold'/> within the timeout, <see langword="false"/> otherwise.</returns>
        public bool TryFetchAll(By locator, out IElement[] elements, uint threshold = 1)
        {
            var Result = InternalTryFetchAll(locator, Timeout, threshold);
            elements = Result.Item3;
            return Result.Item1;
        }

        /// <summary>Checks whether an element exists. Will throw an exception if the check could not be completed before the timeout.</summary>
        /// <param name="locator">The locator for the element whose existence you want to check.</param>
        /// <param name="element">If the element exists (true is returned) it will be assigned to this parameter. If the function returns false then this will be <see langword="null"/>.</param>
        /// <returns><see langword="True"/> if the element exists, <see langword="False"/> otherwise.</returns>
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

                    element = new Element((WebElement)SearchResult[0], this, locator, SearchContext, false);
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

            throw new FetchContextException($"Timed out trying find out if element exists because the context is stale (timeout: {Timeout})");
        }

        /// <summary>Returns the amount of elements that match the passed locator.</summary>
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

            throw new FetchContextException($"Timed out trying find out how many elements exist because the context is stale (timeout: {Timeout})");
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
                    Thread.Sleep((int)PollingInterval);
                    continue;
                }

                var ReturnData = new Element[Elements.Count];
                for (int Index = 0; Index < Elements.Count; Index++)
                {
                    ReturnData[Index] = new Element((WebElement)Elements[Index], this, locator, SearchContext, true);
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
                    return new(true, new Element((WebElement)Element, this, locator, SearchContext, false), null);
                }
                catch (WebDriverException error)
                {
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

        public ConditionalFetchContext When(By locator)
        {
            return new ConditionalFetchContext(this, locator);
        }
    }
}