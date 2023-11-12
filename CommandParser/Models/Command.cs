using System.Reflection;
using CommandParser.Contracts;

namespace CommandParser.Models;

internal class Command : ICommand
{
    public required Type VerbType { get; init;  }
    public required string CommandText { get; init; }
    public required MethodInfo Method { get; init; }
    public string? HelpText { get; init; }
}