﻿using System;

namespace TMech.Elements.Exceptions
{
    [Serializable]
    public class ReacquireElementException : FetchContextException
    {
        public ReacquireElementException()
            : base($"Failed to reacquire handle to the element on staleness") { }

        public ReacquireElementException(Exception inner)
            : base($"Failed to reacquire handle to the element on staleness", inner) { }
    }
}
