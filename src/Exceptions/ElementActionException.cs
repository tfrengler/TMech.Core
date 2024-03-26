using System;

namespace TMech.Core.Exceptions
{
    [Serializable]
    public class ElementInteractionException : FetchContextException
    {
        public ElementInteractionException() { }

        public ElementInteractionException(string message) : base(message) { }

        public ElementInteractionException(string message, Exception inner) : base(message, inner) { }
    }

}