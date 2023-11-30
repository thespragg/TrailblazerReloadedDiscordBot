using Discord;
using DiscordBuilder.Contracts;
using Domain;
using Domain.Contracts;
using Domain.Workers;
using Infrastructure.Data;
using Infrastructure.Hiscores;
using Interface.Commands.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace App;

public abstract class Program
{
    private static async Task Main(string[] _)
    {
        var bot = IDiscordBot
            .Create("tb")
            .SetIntents(GatewayIntents.MessageContent | GatewayIntents.AllUnprivileged)
            .ConfigureServices(sc =>
            {
                sc.AddData();
                sc.AddHiscoreProvider();
                sc.AddDomain();
            })
            .AddCommands<AccountCommands>()
            .AddCommands<TopCommands>()
            .ConfigureApplicationLogging(logger => logger.WriteTo.Console())
            .AddDefaultDiscordLogging()
            .Build();

        var statService = bot.Services.GetRequiredService<IStatRetrievalWorker>();
        statService.Start();

        var config = bot.Services.GetRequiredService<IConfiguration>();
        var channel = ulong.Parse(config["Discord:CommandChannel"]!);
        
        Console.WriteLine($"Using command channel: {channel}");
        await bot.RunAsync(
            (cfg, sp) => cfg["Discord:Token"]!,
            channel
        );    

        
    }

}