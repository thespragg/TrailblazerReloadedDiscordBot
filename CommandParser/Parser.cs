using System.Text;
using CommandParser.Attributes;
using CommandParser.Contracts;
using CommandParser.Exceptions;
using CommandParser.Models;
using JetBrains.Annotations;

namespace CommandParser;

/// <summary>
/// Contains argument parsing method and necessary data
/// </summary>
[PublicAPI]
public class Parser<TContext> : IParser<TContext> where TContext : class
{
    /// <summary>
    /// Declared types for classes that contain commands
    /// </summary>
    private List<IVerb> _verbs = new();
    public IReadOnlyList<IVerb> Verbs => _verbs;
    private Func<HelpSection, string>? HelpMethodGenerator { get; set; }
    private IServiceProvider? Services { get; set; }
    public Action<string>? OnError { get; set; }

    public void AddCommands(Type type)
    {
        if (type.GetCustomAttributes(false).FirstOrDefault(x => x is VerbAttribute) is not VerbAttribute verb)
            throw new NotAVerbException("The provided type is doesn't have a [Verb] attribute.");

        var commands = type
            .GetMethods()
            .Where(
                methodInfo =>
                    methodInfo.GetCustomAttributes(false).FirstOrDefault(x => x is CommandAttribute) is CommandAttribute
            )
            .Select(x =>
            {
                var attr = x.GetCustomAttributes(false)
                    .FirstOrDefault(j => j is CommandAttribute) as CommandAttribute;
                return new Command
                {
                    VerbType = x.DeclaringType!,
                    Method = x,
                    CommandText = attr!.Method,
                    HelpText = attr.HelpText
                };
            })
            .ToList();
        if (commands.Count == 0) throw new NoCommandsException("No commands found in the provided verb class.");

        _verbs.Add(new Verb
        {
            Commands = commands,
            VerbName = verb.Verb,
            HelpText = verb.HelpText,
            Type = type
        });
    }

    public void AddCommands<TCommands>()
        => AddCommands(typeof(TCommands));

    public void AddDependencyInjection(IServiceProvider services)
        => Services = services;

    public IParserResult<TContext>? Parse(string command)
        => Parse(command.Split(" "));

    public IParserResult<TContext>? Parse(IEnumerable<string> commandParts)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(commandParts);
            var enumerable = commandParts as string[] ?? commandParts.ToArray();

            if (_verbs.Count == 0 || enumerable.Length == 0) return null;

            var skip = 0;
            var verb = FindVerb(enumerable[0]);
            if (verb != null) skip += 1;
            var command = FindCommand(enumerable[1], verb);
            command ??= FindMiscellaneousCommand(enumerable[1]);
            skip += 1;
            return command is null
                ? throw new Exception("Failed to find command")
                : new ParserResult<TContext>(enumerable.Skip(skip), command, verb, Services);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex.Message);
            return null;
        }
    }

    private IVerb? FindVerb(string verbName)
        => _verbs.FirstOrDefault(x =>
            x.VerbName?.Equals(verbName, StringComparison.InvariantCultureIgnoreCase) ?? false);

    private static ICommand? FindCommand(string commandText, IVerb? verb)
        => verb?.Commands.FirstOrDefault(x => x.CommandText == commandText);

    private ICommand? FindMiscellaneousCommand(string command)
        => _verbs
            .FirstOrDefault(x =>
                string.IsNullOrEmpty(x.VerbName)
                && x.Commands.Any(j => j.CommandText == command)
            )?
            .Commands
            .FirstOrDefault(x => x.CommandText == command);

    public void ConfigureHelpGenerator(Func<HelpSection, string> configure)
        => HelpMethodGenerator = configure;

    // Needs to also handle misc section
    public IEnumerable<string> GenerateHelp()
        => _verbs
            .Where(x => !string.IsNullOrEmpty(x.VerbName))
            .Select(x => new HelpSection
            {
                Verb = x.VerbName!,
                VerbHelpText = x.HelpText,
                Commands = x.Commands.Select(j => new CommandSection
                {
                    Command = j.CommandText,
                    CommandHelpText = j.HelpText
                })
            })
            .Select(HelpMethodGenerator ?? GenerateHelpForVerb)
            .ToList();

    private static string GenerateHelpForVerb(HelpSection? verb)
    {
        if (verb == null) return string.Empty;
        var sb = new StringBuilder();

        sb.AppendLine($"{verb.Verb} COMMANDS:");
        sb.AppendLine();
        sb.AppendLine(verb.VerbHelpText);
        sb.AppendLine();

        foreach (var cmd in verb.Commands)
        {
            sb.AppendLine(cmd.Command + " - " + cmd.CommandHelpText);
        }

        return sb.ToString();
    }
}

public class Parser: Parser<EmptyContext> { }