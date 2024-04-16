using System;

namespace Gdh.Art.Utils.Webdriver.Elements.Exceptions
{
    [Serializable]
    public class ReacquireElementException : ElementFactoryException
    {
        public ReacquireElementException()
            : base($"Failed to reacquire handle to the element on staleness") { }

        public ReacquireElementException(Exception inner)
            : base($"Failed to reacquire handle to the element on staleness", inner) { }
    }
}
