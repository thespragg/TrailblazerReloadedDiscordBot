using System.Text;
using CommandParser.Attributes;
using Discord;
using Discord.WebSocket;
using Infrastructure.Data;

namespace Interface.Commands.Commands;

[Verb]
public class AccountCommands
{
    private readonly IUserRepository _repo;

    public AccountCommands(IUserRepository repo)
        => _repo = repo;

    [Command("register", "Adds an ign the bots player list.")]
    public async Task Register(SocketMessage ctx, string name)
    {
        var user = _repo.GetUserByIgn(name);
        if (user is not null)
        {
            await ctx.Channel.SendMessageAsync("That username has already been registered.");
            return;
        }
        _repo.RegisterUser(name, ctx.Author.Id);
        await ctx.Channel.SendMessageAsync($"Added {name}.");
    }

    [Command("users", "Lists all the registered players.")]
    public async Task Users(SocketMessage ctx)
    {
        var users = _repo.GetUsers();
        var sb = new StringBuilder();
        sb.AppendLine("Registered Users:");
        sb.AppendLine();

        var guild = (ctx.Author as SocketGuildUser)?.Guild;

        foreach (var user in users)
        {
            var guildUser = guild is null ? null : await ((IGuild)guild).GetUserAsync(user.DiscordId);
            var name = guildUser?.DisplayName ?? "Error";
            sb.AppendLine(user.Username + $" - {name}");
        }

        await ctx.Channel.SendMessageAsync(sb.ToString());
    }

    [Command("remove", "Removes a registered player.")]
    public async Task Remove(SocketMessage ctx, string name)
    {
        var user = _repo.GetUserByIgn(name);
        if (user is null)
        {
            await ctx.Channel.SendMessageAsync($"No user with ign {name} found.");
            return;
        }

        var admins = new ulong[] { 140973520866770946, 121553295675228160, 587016151700078593 };
        if (ctx.Author.Id != user.DiscordId &&
            !admins.Contains(ctx.Author.Id))
        {
            var sb = new StringBuilder();
            sb.Append(
                $"You are not authorized to remove that user, the only permitted users are the registrar <@{user.DiscordId}> or a mod:");
            foreach (var admin in admins) sb.Append($" <@{admin}>");
            sb.Append('.');
            await ctx.Channel.SendMessageAsync(sb.ToString());
            return;
        }

        _repo.DeleteUser(user.Id);
        await ctx.Channel.SendMessageAsync($"User {name} removed from the rankings.");
    }
}