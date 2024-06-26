﻿using System;

namespace TMech.Elements.Exceptions
{
    [Serializable]
    public class FetchContextException : Exception
    {
        public FetchContextException() { }

        public FetchContextException(string message) : base(message) { }

        public FetchContextException(string message, Exception inner) : base(message, inner) { }
    }
}
