using CommandParser.Attributes;
using CommandParser.Exceptions;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace CommandParser.Test;

public class ParserTests
{
    private static readonly string[] ValidCommandParts = { "verb", "command", "45" };
    private static readonly string[] ValidCommand2Parts = { "verb", "command2", "45" };

    [Fact]
    public void TestAddCommands_ValidType_AddsVerb()
    {
        var parser = new Parser();
        parser.AddCommands<MockCommands>();
        Assert.Single(parser.Verbs);
        Assert.Contains(parser.Verbs, x => x.VerbName == "verb");
    }
    
    [Fact]
    public void TestAddCommands_InvalidType_ThrowsNotAVerb()
    {
        var parser = new Parser();
        Assert.Throws<NotAVerbException>(() => parser.AddCommands<InvalidVerb>());
    }

    [Fact]
    public void TestAddCommands_InvalidCommands_ThrowsNoCommands()
    {
        var parser = new Parser();
        Assert.Throws<NoCommandsException>(() => parser.AddCommands<InvalidCommands>());
    }

    [Fact]
    public void Parse_ValidCommand_ParsesCorrectly()
    {
        var parser = new Parser();
        parser.AddCommands<MockCommands>();
        var res = parser.Parse(ValidCommandParts);
        Assert.NotNull(res);
    }
    
    [Fact]
    public async Task Parse_ValidCommand_ParsesCorrectly_AndInvokes()
    {
        var parser = new Parser();
        parser.AddCommands<MockCommands>();
        var res = parser.Parse(ValidCommandParts);
        Assert.NotNull(res);
        Assert.True(await res.InvokeAsync());
    }
    
    [Fact]
    public async Task Parse_ValidCommand_InjectsDependencies()
    {
        var parser = new Parser();
        parser.AddCommands<MockCommandsWithInjected>();
        var di = new ServiceCollection().AddScoped<MockCommands>().BuildServiceProvider();
        parser.AddDependencyInjection(di);
        var res = parser.Parse(ValidCommandParts);
        Assert.NotNull(res);
        Assert.True(await res.InvokeAsync());
    }
    
    [Fact]
    public async Task Parse_ValidCommand_WorksWithContext()
    {
        var parser = new Parser<Context>();
        parser.AddCommands<MockCommands>();
        
        var res = parser.Parse(ValidCommand2Parts);
        var inv = await res?.InvokeAsync(new Context())!;
        Assert.NotNull(res);
        Assert.True(inv);
    }
}

public class Context
{
    public string Message { get; set; } = "Test";
}

[Verb("verb", "Verb help text")]
[PublicAPI]
public class MockCommands
{
    [Command("command", "Command help text")]
    public void CommandMethod(int num)
    {
    }
    
    [Command("command2", "Command help text")]
    public void Command2Method(Context msg, int num)
    {
        Assert.NotNull(msg);
    }
}

[Verb("verb", "Verb help text")]
[PublicAPI]
public class MockCommandsWithInjected
{
    public MockCommandsWithInjected(MockCommands cmds)
    {   
    }
    
    [Command("command", "Command help text")]
    public void CommandMethod(int num)
    {
    }
}

[PublicAPI]
public class InvalidVerb
{
    [Command("command", "Command help text")]
    public void CommandMethod()
    {
    }
}

[PublicAPI]
[Verb]
public class InvalidCommands
{
    public void CommandMethod()
    {
    }
}