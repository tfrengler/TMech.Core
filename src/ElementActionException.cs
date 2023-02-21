using System;

public class ElementActionException : TMechException
{
    public ElementActionException()
    {
    }

    public ElementActionException(string message)
        : base(message)
    {
    }

    public ElementActionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}