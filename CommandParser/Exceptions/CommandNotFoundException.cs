namespace CommandParser.Exceptions;

public class CommandNotFoundException : Exception
{
    public CommandNotFoundException()
    {
    }

    public CommandNotFoundException(string message)
        : base(message)
    {
    }

    public CommandNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}