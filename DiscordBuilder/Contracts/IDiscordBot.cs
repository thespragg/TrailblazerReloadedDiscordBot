namespace DiscordBuilder.Contracts;

public interface IDiscordBot
{
    Task RunAsync();
    static IDiscordBotBuilder Create()
        => new DiscordBotBuilder();
}