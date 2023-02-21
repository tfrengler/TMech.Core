using System;

public class TimeoutException : TMechException
{
    public TimeoutException()
    {
    }

    public TimeoutException(string message)
        : base(message)
    {
    }

    public TimeoutException(string message, Exception inner)
        : base(message, inner)
    {
    }
}