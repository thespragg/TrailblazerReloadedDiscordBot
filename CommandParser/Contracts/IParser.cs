using CommandParser.Attributes;
using JetBrains.Annotations;

namespace CommandParser.Contracts;

/// <summary>
/// Defines the structure of the <see cref="IParser"/> interface.
/// </summary>
[PublicAPI]
public interface IParser<TContext>
{
    /// <summary>
    /// Add a class containing commands to the parser.
    /// </summary>
    /// <typeparam name="TCommands"></typeparam>
    void AddCommands<TCommands>();
    
    /// <summary>
    /// Add a class containing commands to the parser.
    /// </summary>
    void AddCommands(Type type);
    
    /// <summary>
    /// Parses a command in string form by checking against known verbs, which are classes decorated using <see cref="VerbAttribute"/>.
    /// </summary>
    /// <param name="command">A <see cref="string"/> array of command line arguments.</param>
    IParserResult<TContext>? Parse(string command);
    
    /// <summary>
    /// Parses a command in string form by checking against known verbs, which are classes decorated using <see cref="VerbAttribute"/>.
    /// </summary>
    /// <param name="commandParts">A <see cref="string"/> array of command line arguments.</param>
    IParserResult<TContext>? Parse(IEnumerable<string> commandParts);

    IReadOnlyList<IVerb> Verbs { get; }

    void AddDependencyInjection(IServiceProvider services);
    IEnumerable<string> GenerateHelp();
}

/// <summary>
/// Defines the structure of the <see cref="IParser"/> interface.
/// </summary>
[PublicAPI]
public interface IParser
{
    /// <summary>
    /// Add a class containing commands to the parser.
    /// </summary>
    /// <typeparam name="TCommands"></typeparam>
    void AddCommands<TCommands>();
    
    /// <summary>
    /// Add a class containing commands to the parser.
    /// </summary>
    /// <typeparam name="TCommands"></typeparam>
    void AddCommands(Type type);
    
    /// <summary>
    /// Parses a command in string form by checking against known verbs, which are classes decorated using <see cref="VerbAttribute"/>.
    /// </summary>
    /// <param name="command">A <see cref="string"/> array of command line arguments.</param>
    IParserResult? Parse(string command);
    
    /// <summary>
    /// Parses a command in string form by checking against known verbs, which are classes decorated using <see cref="VerbAttribute"/>.
    /// </summary>
    /// <param name="commandParts">A <see cref="string"/> array of command line arguments.</param>
    IParserResult? Parse(IEnumerable<string> commandParts);

    IReadOnlyList<IVerb> Verbs { get; }
}