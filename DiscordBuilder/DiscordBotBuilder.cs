using CommandParser;
using CommandParser.Contracts;
using Discord;
using Discord.WebSocket;
using DiscordBuilder.Contracts;
using DiscordBuilder.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DiscordBuilder;

public class DiscordBotBuilder : IDiscordBotBuilder
{
    private readonly DiscordSocketConfig _discordConfig;
    private bool DefaultLogging { get; set; }
    private readonly IParser<SocketMessage> _parser = new Parser<SocketMessage>();

    internal DiscordBotBuilder()
    {
        _discordConfig = new DiscordSocketConfig();
        Configuration = new ConfigurationManager()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.custom.json", true)
            .AddEnvironmentVariables()
            .Build();

        Services = new ServiceCollection();
    }

    private IServiceCollection Services { get; }
    public IConfiguration Configuration { get; }

    public IDiscordBotBuilder Configure(Action<DiscordSocketConfig> configure)
    {
        configure.Invoke(_discordConfig);
        return this;
    }

    public IDiscordBotBuilder AddCommands<TCommands>()
    {
        _parser.AddCommands(typeof(TCommands));
        return this;
    }

    public IDiscordBot Build()
    {
        Services
            .AddSingleton(Configuration)
            .AddSingleton(_discordConfig)
            .AddSingleton<DiscordSocketClient>();

        var sp = Services.BuildServiceProvider();
        var bot = new DiscordBot(sp, _parser);
        if (!DefaultLogging) return bot;

        var client = bot.Services.GetRequiredService<DiscordSocketClient>();
        client.Log += (msg) => LogMessage(msg, bot.Services);

        return bot;
    }

    private static async Task LogMessage(LogMessage msg, IServiceProvider sp)
    {
        await Task.CompletedTask;
        var logger = sp.GetRequiredService<ILogger<IDiscordBot>>();
        var logLevel = msg.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(msg.Severity))
        };
        logger.Log(logLevel, msg.Exception, msg.Message);
    }

    public IDiscordBotBuilder ConfigureApplicationLogging(Action<LoggerConfiguration> configure)
    {
        var logger = new LoggerConfiguration();
        configure.Invoke(logger);

        Services.AddLogging(builder =>
        {
            Log.Logger = logger.CreateLogger();
            builder.AddSerilog(dispose: true);
        });

        return this;
    }

    public IDiscordBotBuilder AddDefaultDiscordLogging()
    {
        DefaultLogging = true;
        return this;
    }

    public IDiscordBotBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        services.Invoke(Services);
        return this;
    }
}