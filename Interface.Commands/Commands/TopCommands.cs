using System.Text;
using CommandParser.Attributes;
using Discord;
using Discord.WebSocket;
using Domain.Contracts;
using Domain.Extensions;

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
            await ctx.Channel.SendMessageAsync(
                "No users have been registered, the bot doesn't know what skills exist yet.");
            return;
        }

        var skills = player.Stats!.Skills.Select(x => x.Name.Replace(" ", "")).ToArray();
        var activities = player.Stats.Activities.Select(x => x.Name.Replace(" ", "")).ToArray();

        var embed = new EmbedBuilder
        {
            Description = new StringBuilder()
                .AppendLine("Misc:")
                .AppendLine()
                .AppendLine(string.Join(", ", "Total Level", "League Points"))
                .AppendLine()
                .AppendLine("Skills:")
                .AppendLine()
                .AppendLine(string.Join(", ", skills))
                .AppendLine()
                .AppendLine("Activities/Bosses:")
                .AppendLine()
                .AppendLine(string.Join(", ", activities))
                .ToString()
        };

        await ctx.Channel.SendMessageAsync(embed: embed.Build());
    }

    [Command("top", "Shows the top 10 for any skill, points, or boss kc.")]
    public async Task Top(SocketMessage ctx, string skill)
    {
        var players = _playerService.GetPlayers().Value;
        if (skill.ToLower().Equals("totallevel"))
        {
            var totalLvls = players!.Select(x => (x.Username, x.Stats?.Skills.Sum(j => j.Level) ?? 0));
            await ctx.Channel.SendMessageAsync(GetTopTen("Total Level", totalLvls));
            return;
        }

        var playerWithStats = players?.FirstOrDefault(x => x.Stats is not null && x.Stats.Skills.Count > 0)?.Stats;
        if (playerWithStats is null)
        {
            await ctx.Channel.SendMessageAsync("Couldn't find a player in the database to compare stats.");
            return;
        }

        var stats = players
            .Select(x => (x.Username,
                x.Stats.SkillsAndActivities.FirstOrDefault(j => j.Key.Replace(" ", "").ToLower().Equals(skill.ToLower())).Value));

        await ctx.Channel.SendMessageAsync(GetTopTen(skill.ToTitleCase(), stats));
    }

    private string GetTopTen(string skill, IEnumerable<(string, int)> values)
    {
        var topTen = values.OrderBy(x => x.Item2).Take(10).ToList();
        var sb = new StringBuilder();

        var first = topTen.Take(1).FirstOrDefault();
        var second = topTen.Skip(1).Take(1).FirstOrDefault();
        var third = topTen.Skip(2).Take(1).FirstOrDefault();

        sb.AppendLine($"Top 10 players for {skill}").AppendLine();
        if (first != default)
            sb.AppendLine("\ud83e\udd47 1st").AppendLine($"**{first.Item1}** - {first.Item2}")
                .AppendLine();
        if (second != default)
            sb.AppendLine("\ud83e\udd48 1st").AppendLine($"**{second.Item1}** - {second.Item2}")
                .AppendLine();
        if (third != default)
            sb.AppendLine("\ud83e\udd49 1st").AppendLine($"**{third.Item1}** - {third.Item2}")
                .AppendLine();

        var position = 4;
        foreach (var player in topTen.Skip(3))
        {
            sb.AppendLine($"{position}. **{player.Item1}** - {player.Item2}");
            position++;
        }

        return sb.ToString();
    }
}