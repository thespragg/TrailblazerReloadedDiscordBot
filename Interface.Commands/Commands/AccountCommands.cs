using System.Text;
using CommandParser.Attributes;
using Discord;
using Discord.WebSocket;
using Domain.Contracts;
using Infrastructure.Data;
using JetBrains.Annotations;

namespace Interface.Commands.Commands;

[Verb]
[PublicAPI]
public class AccountCommands
{
    private readonly IPlayerService _playerService;

    public AccountCommands(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [Command("register", "Adds an ign to the bots player list.")]
    public async Task Register(SocketMessage ctx, string name)
    {
        await _playerService.RegisterPlayer(name, ctx.Author.Id);
        await ctx.Channel.SendMessageAsync($"Added {name}.");
    }
    
    [Command("register-other", "Adds an ign to the bots player list for someone else, must contain the discord id of that user.")]
    public async Task RegisterOther(SocketMessage ctx, string name)
    {
        var id = ctx.MentionedUsers.First().Id;
        await _playerService.RegisterPlayer(name, id);
        await ctx.Channel.SendMessageAsync($"Added {name}.");
    }

    [Command("players", "Lists all the registered players.")]
    public async Task Players(SocketMessage ctx)
    {
        var users = _playerService.GetPlayers();
        var sb = new StringBuilder();
        sb.AppendLine("Registered Users:");
        sb.AppendLine();

        var guild = (ctx.Author as SocketGuildUser)?.Guild;

        foreach (var user in users.Value!)
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
        var user = _playerService.GetPlayer(name);
        if (user.Value is null)
        {
            await ctx.Channel.SendMessageAsync($"No user with ign {name} found.");
            return;
        }

        var admins = new ulong[] { 140973520866770946, 121553295675228160, 587016151700078593 };
        if (ctx.Author.Id != user.Value.DiscordId &&
            !admins.Contains(ctx.Author.Id))
        {
            var sb = new StringBuilder();
            sb.Append(
                $"You are not authorized to remove that user, the only permitted users are the registrar <@{user.Value.DiscordId}> or a mod:");
            foreach (var admin in admins) sb.Append($" <@{admin}>");
            sb.Append('.');
            await ctx.Channel.SendMessageAsync(sb.ToString());
            return;
        }

        _playerService.DeletePlayer(user.Value.Username);
        await ctx.Channel.SendMessageAsync($"User {name} removed from the rankings.");
    }
}