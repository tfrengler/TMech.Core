using System;

namespace Gdh.Art.Utils.Webdriver.Elements.Exceptions
{
    [Serializable]
    public class ElementInteractionException : ElementFactoryException
    {
        public ElementInteractionException() { }

        public ElementInteractionException(string message) : base(message) { }

        public ElementInteractionException(string message, Exception inner) : base(message, inner) { }
    }

}