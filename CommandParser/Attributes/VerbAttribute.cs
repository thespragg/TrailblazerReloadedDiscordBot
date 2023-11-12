using JetBrains.Annotations;

namespace CommandParser.Attributes;

/// <summary>
/// Attribute for usage on classes to denote a verb
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[PublicAPI]
public class VerbAttribute : Attribute
{
    /// <summary>
    /// The name of the class as it should be referenced in the command.
    /// </summary>
    public string? Verb { get; set; }

    /// <summary>
    ///  Text to return when the parsing fails.
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="VerbAttribute"/> class.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="helpText"></param>
    public VerbAttribute(string verb, string helpText) => (Verb, HelpText) = (verb, helpText);

    /// <summary>
    /// Creates a new instance of the <see cref="VerbAttribute"/> class.
    /// </summary>
    public VerbAttribute()
    {
    }
}