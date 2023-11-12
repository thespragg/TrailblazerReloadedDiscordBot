using CommandParser.Contracts;
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

    public DiscordBot(string botPrefix, IServiceProvider services, IParser<SocketMessage> parser)
    {
        Services = services;
        _parser = parser;
        _prefix = botPrefix;
    }

    public IServiceProvider Services { get; }

    public async Task RunAsync(Func<IConfiguration, IServiceProvider, string> configure)
    {
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
        var content = arg.CleanContent;
        if (!content.StartsWith($"!{_prefix}")) return;
        content = content.Replace($"!{_prefix}", "");
        var parsed = _parser.Parse(content.Trim());
        if (parsed is null) return;
        var success = await parsed.InvokeAsync(arg);
    }
}