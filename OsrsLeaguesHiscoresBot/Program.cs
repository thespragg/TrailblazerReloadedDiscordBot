using DiscordBuilder.Contracts;
using Infrastructure.Data;
using Interface.Commands.Commands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace OsrsLeaguesHiscoresBot;

public class Program
{
    private static async Task Main(string[] _)
        => await IDiscordBot
            .Create()
            .ConfigureServices(sc =>
            {
                sc.AddScoped<IUserRepository, UserRepository>();
            })
            .AddCommands<AccountCommands>()
            .ConfigureApplicationLogging(logger => logger.WriteTo.Console())
            .AddDefaultDiscordLogging()
            .Build()
            .RunAsync();
}