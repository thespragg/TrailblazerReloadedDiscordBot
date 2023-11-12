namespace CommandParser.Exceptions;

public class NotAVerbException : Exception
{
    public NotAVerbException()
    {
    }

    public NotAVerbException(string message)
        : base(message)
    {
    }

    public NotAVerbException(string message, Exception inner)
        : base(message, inner)
    {
    }
}