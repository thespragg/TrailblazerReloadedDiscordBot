using Domain.Models;

namespace Domain.Contracts;

public interface IPlayerRepository
{
    void RegisterPlayer(string username, ulong userId, PlayerStats stats);
    IEnumerable<PlayerEntity> GetPlayers();
    PlayerEntity? GetPlayerByIgn(string ign);
    void DeletePlayer(int id);
    void Update(string ign, PlayerStats stats);
}