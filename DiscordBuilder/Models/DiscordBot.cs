using System.Text;
using CommandParser.Contracts;
using CommandParser.Exceptions;
using Discord;
using Discord.WebSocket;
using DiscordBuilder.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBuilder.Models;

internal class DiscordBot : IDiscordBot
{
    private readonly IParser<SocketMessage> _parser;
    private readonly string _prefix;
    private ulong? _commandChannel;

    public DiscordBot(string botPrefix, IServiceProvider services, IParser<SocketMessage> parser)
    {
        Services = services;
        _parser = parser;
        _prefix = botPrefix;
    }

    public IServiceProvider Services { get; }

    public async Task RunAsync(Func<IConfiguration, IServiceProvider, string> configure, ulong? commandChannel = null)
    {
        _commandChannel = commandChannel;
        var token = configure(Services.GetRequiredService<IConfiguration>(), Services);
        await RunAsync(token);
    }

    public async Task RunAsync(string token)
    {
        var client = Services.GetRequiredService<DiscordSocketClient>();

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        client.MessageReceived += ClientOnMessageReceived;
        
        await Task.Delay(Timeout.Infinite);
    }

    private async Task ClientOnMessageReceived(SocketMessage arg)
    {
        if (_commandChannel is not null && arg.Channel.Id != _commandChannel) return;
        
        var content = arg.CleanContent;
        if (!content.StartsWith($"!{_prefix}")) return;
        content = content.Replace($"!{_prefix}", "");
        try
        {
            var parsed = _parser.Parse(content.Trim());
            if (parsed is null) return;
            await parsed.InvokeAsync(arg);
        }
        catch (CommandNotFoundException)
        {
            var help = _parser.GenerateHelp();
            var sb = new StringBuilder();
            sb.AppendLine($"All commands must be prefixed by !{_prefix}, e.g !tb players");
            sb.AppendLine();
            foreach (var line in help)
            {
                sb.AppendLine(line);
            }
            await arg.Channel.SendMessageAsync(sb.ToString());
        }
    }
}