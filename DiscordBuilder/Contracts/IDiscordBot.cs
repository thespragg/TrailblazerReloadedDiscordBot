using Microsoft.Extensions.Configuration;

namespace DiscordBuilder.Contracts;

public interface IDiscordBot
{
    Task RunAsync(Func<IConfiguration, IServiceProvider, string> configure);
    Task RunAsync(string token);
    static IDiscordBotBuilder Create(string botPrefix)
        => new DiscordBotBuilder(botPrefix);
}