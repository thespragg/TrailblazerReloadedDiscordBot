using Microsoft.Extensions.Configuration;

namespace DiscordBuilder.Contracts;

public interface IDiscordBot
{
    IServiceProvider Services { get; }
    Task RunAsync(Func<IConfiguration, IServiceProvider, string> configure, ulong? commandChannel = null);
    Task RunAsync(string token);
    static IDiscordBotBuilder Create(string botPrefix)
        => new DiscordBotBuilder(botPrefix);
}