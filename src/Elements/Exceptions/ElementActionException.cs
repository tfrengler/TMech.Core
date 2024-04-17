using System;

namespace TMech.Elements.Exceptions
{
    [Serializable]
    public class ElementInteractionException : FetchContextException
    {
        public ElementInteractionException() { }

        public ElementInteractionException(string message) : base(message) { }

        public ElementInteractionException(string message, Exception inner) : base(message, inner) { }
    }

}