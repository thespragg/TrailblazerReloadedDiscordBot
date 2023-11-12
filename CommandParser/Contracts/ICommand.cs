using System.Reflection;

namespace CommandParser.Contracts;

public interface ICommand
{
    Type VerbType { get; }
    string CommandText { get; }
    MethodInfo Method { get; }
    string? HelpText { get; }
}