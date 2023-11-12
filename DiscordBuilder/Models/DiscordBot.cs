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

    public DiscordBot(IServiceProvider services, IParser<SocketMessage> parser)
    {
        Services = services;
        _parser = parser;
    }

    public IServiceProvider Services { get; }

    public async Task RunAsync()
    {
        var client = Services.GetRequiredService<DiscordSocketClient>();
        var config = Services.GetRequiredService<IConfiguration>();
        
        await client.LoginAsync(TokenType.Bot, config["Discord:Token"]);
        await client.StartAsync();
        
        client.MessageReceived += ClientOnMessageReceived;
        
        await Task.Delay(Timeout.Infinite);
    }

    private Task ClientOnMessageReceived(SocketMessage arg)
        => Task.FromResult(_parser.Parse(arg.Content)?.InvokeAsync(arg));
}