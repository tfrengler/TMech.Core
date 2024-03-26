using OpenQA.Selenium;
using System.Diagnostics.CodeAnalysis;
using System;

namespace TMech.Core
{
    public interface IFetchContext
    {
        public int PollingInterval { get; }
        public TimeSpan Timeout { get; }
        public IElement? Parent { get; }

        public IElement Fetch(By locator);
        public IElement[] FetchAll(By locator, uint threshold = 1);
        public bool TryFetch(By locator, [NotNullWhen(true)] out IElement? element, [NotNullWhen(false)] out Exception? error);
        public bool TryFetchAll(By locator, out IElement[] elements, uint threshold = 1);

        public bool Exists(By locator, out IElement? element);
        public int AmountOf(By locator);

        public ConditionalFetchContext When(By locator);
    }
}
