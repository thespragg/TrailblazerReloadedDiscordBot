namespace CommandParser.Models;

public class HelpSection
{
    public required string Verb { get; init; }
    public string? VerbHelpText { get; init; }
    public IEnumerable<CommandSection> Commands { get; init; } = Enumerable.Empty<CommandSection>();
}

public class CommandSection
{
    public required string Command { get; init; }
    public string? CommandHelpText { get; init; }
}