using System;

public class TMechException : Exception
{
    public TMechException()
    {
    }

    public TMechException(string message)
        : base(message)
    {
    }

    public TMechException(string message, Exception inner)
        : base(message, inner)
    {
    }
}