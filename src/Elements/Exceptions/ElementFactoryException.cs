using System;

namespace Gdh.Art.Utils.Webdriver.Elements.Exceptions
{
    [Serializable]
    public class ElementFactoryException : Exception
    {
        public ElementFactoryException() { }

        public ElementFactoryException(string message) : base(message) { }

        public ElementFactoryException(string message, Exception inner) : base(message, inner) { }
    }
}
