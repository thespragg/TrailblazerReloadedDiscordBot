namespace CommandParser.Contracts;

public interface IVerb
{
    Type Type { get; }
    IEnumerable<ICommand> Commands { get; }
    string? VerbName { get; }
    string? HelpText { get; }
}