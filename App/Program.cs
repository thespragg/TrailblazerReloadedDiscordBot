using Discord;
using DiscordBuilder.Contracts;
using Domain;
using Domain.Contracts;
using Infrastructure.Data;
using Infrastructure.Hiscores;
using Interface.Commands.Commands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace App;

public abstract class Program
{
    private static async Task Main(string[] _)
        => await IDiscordBot
            .Create("tb")
            .SetIntents(GatewayIntents.MessageContent | GatewayIntents.AllUnprivileged)
            .ConfigureServices(sc =>
            {
                sc.AddData();
                sc.AddHiscoreProvider();
                sc.AddDomain();
            })
            .AddCommands<AccountCommands>()
            .ConfigureApplicationLogging(logger => logger.WriteTo.Console())
            .AddDefaultDiscordLogging()
            .Build()
            .RunAsync(
                (cfg, sp) => cfg["Discord:Token"]!
            );
}