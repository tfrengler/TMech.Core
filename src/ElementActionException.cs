using System;

namespace TMech.Core.Exceptions
{
    [Serializable]
    public class ElementInteractionException : TMechException
    {
        public ElementInteractionException() { }

        public ElementInteractionException(string message) : base(message) { }

        public ElementInteractionException(string message, Exception inner) : base(message, inner) { }
    }

}