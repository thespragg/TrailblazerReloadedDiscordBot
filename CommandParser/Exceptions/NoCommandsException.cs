namespace CommandParser.Exceptions;

public class NoCommandsException : Exception
{
    public NoCommandsException()
    {
    }

    public NoCommandsException(string message)
        : base(message)
    {
    }

    public NoCommandsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}