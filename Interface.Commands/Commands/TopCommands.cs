using CommandParser.Attributes;
using Discord.WebSocket;
using Domain.Contracts;

namespace Interface.Commands.Commands;

[Verb]
public class TopCommands
{
    private readonly IPlayerService _playerService;

    public TopCommands(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    [Command("top-options", "Shows what the choices for top are.")]
    public async Task TopOptions(SocketMessage ctx)
    {
        var player = _playerService.GetPlayers().Value!.FirstOrDefault(x => x.Stats is not null);
        if (player is null)
        {
            await ctx.Channel.SendMessageAsync("No users have been registered, the bot doesn't know what skills exist yet.");
            return;
        }
        var skills = player.Stats!.Skills.Select(x => x.Name);
        var activities = player.Stats.Activities.Select(x => x.Name);
        await ctx.Channel.SendMessageAsync(string.Join(",", skills.Concat(activities)));
    }
    
    [Command("top", "Shows the top 10 for any skill, points, or boss kc.")]
    public void Top(SocketMessage ctx)
    {
        
    }
}