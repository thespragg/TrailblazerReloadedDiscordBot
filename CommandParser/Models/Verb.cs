using CommandParser.Contracts;

namespace CommandParser.Models;

internal class Verb : IVerb
{
    public required Type Type { get; init; }
    public IEnumerable<ICommand> Commands { get; init; } = Enumerable.Empty<ICommand>();
    public string? VerbName { get; init; }
    public string? HelpText { get; init; }
}