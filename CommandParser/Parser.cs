using System.Text;
using CommandParser.Attributes;
using CommandParser.Contracts;
using CommandParser.Exceptions;
using CommandParser.Extensions;
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
            var enumerable = commandParts.ToSafeList();

            if (_verbs.Count == 0 || enumerable.Count == 0) return null;

            var res = FindCommand(enumerable[0]!, enumerable[1]);
            if (res is null) throw new CommandNotFoundException();
            var (verb, cmd) = res.Value;
            var skip = 0;
            if (!string.IsNullOrEmpty(verb.VerbName)) skip += 1;
            skip += 1;
            return new ParserResult<TContext>(enumerable.Skip(skip), cmd, verb, Services);
        }
        catch (CommandNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex.Message);
            return null;
        }
    }

    private (IVerb, ICommand)? FindCommand(string commandText, string? verbName)
    {
        var verb = FindVerb(verbName) ?? FindMiscellaneousVerb(commandText);
        if (verb is null) return null;
        var cmd = FindCommand(commandText, verb);
        if (cmd is null) return null;
        return (verb, cmd);
    }

    private IVerb? FindVerb(string? verbName)
        => _verbs.FirstOrDefault(x =>
            x.VerbName?.Equals(verbName, StringComparison.InvariantCultureIgnoreCase) ?? false);

    private IVerb? FindMiscellaneousVerb(string commandName)
        => _verbs.FirstOrDefault(x => x.Commands.Any(c => c.CommandText.Equals(commandName, StringComparison.InvariantCultureIgnoreCase)));

    private static ICommand? FindCommand(string commandText, IVerb? verb)
        => verb?.Commands.FirstOrDefault(x => x.CommandText.Equals(commandText, StringComparison.InvariantCultureIgnoreCase));

    public void ConfigureHelpGenerator(Func<HelpSection, string> configure)
        => HelpMethodGenerator = configure;
    
    public IEnumerable<string> GenerateHelp()
    {
        var cmds = _verbs
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
        return cmds;
    }

    private static string GenerateHelpForVerb(HelpSection? verb)
    {
        if (verb == null) return string.Empty;
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(verb.Verb))
        {
            sb.AppendLine($"{verb.Verb} COMMANDS:");
            sb.AppendLine(verb.VerbHelpText);
            sb.AppendLine();
        }

        foreach (var cmd in verb.Commands)
        {
            sb.AppendLine(cmd.Command + " - " + cmd.CommandHelpText);
        }

        return sb.ToString();
    }
}

public class Parser : Parser<EmptyContext>
{
}