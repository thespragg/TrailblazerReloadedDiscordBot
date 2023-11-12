using Discord;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DiscordBuilder.Contracts;

[PublicAPI]
public interface IDiscordBotBuilder
{
    IConfiguration Configuration { get; }
    IDiscordBotBuilder Configure(Action<DiscordSocketConfig> configure);
    IDiscordBotBuilder AddCommands<TCommands>();
    IDiscordBot Build();
    IDiscordBotBuilder ConfigureApplicationLogging(Action<LoggerConfiguration> configure);
    IDiscordBotBuilder AddDefaultDiscordLogging();
    IDiscordBotBuilder ConfigureServices(Action<IServiceCollection> services);
    IDiscordBotBuilder SetIntents(GatewayIntents intents);
}