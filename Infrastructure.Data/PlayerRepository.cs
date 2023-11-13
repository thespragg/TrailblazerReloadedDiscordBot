using Domain.Contracts;
using Domain.Models;
using Infrastructure.Data.Entities;
using LiteDB;

namespace Infrastructure.Data;

public class PlayerRepository : IPlayerRepository
{
    private ILiteCollection<Player> Users { get; }

    public PlayerRepository()
    {
        var dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.osrs";
        Directory.CreateDirectory(dir);
        var database = new LiteDatabase($"{dir}/{nameof(PlayerRepository)}.db");
        Users = database.GetCollection<Player>(nameof(Users));
        Users.EnsureIndex(x => x.Id);
    }

    public void RegisterPlayer(string username, ulong userId, PlayerStats stats)
    {
        Users.Insert(new Player
        {
            Username = username,
            DiscordId = userId,
            Stats = stats
        });
    }

    public void DeletePlayer(int id)
        => Users.Delete(id);

    public IEnumerable<PlayerEntity> GetPlayers() 
        => Users
            .FindAll()
            .Select(x=>new PlayerEntity
                {
                    Username = x.Username,
                    DiscordId = x.DiscordId,
                    Stats = x.Stats
                })
            .ToList();

    public PlayerEntity? GetPlayerByIgn(string ign)
        => Users.FindOne(x => x.Username.Equals(ign, StringComparison.InvariantCultureIgnoreCase)) is { } player
            ? new PlayerEntity
            {
                Username = player.Username,
                DiscordId = player.DiscordId,
                Stats = player.Stats
            }
            : null;
}