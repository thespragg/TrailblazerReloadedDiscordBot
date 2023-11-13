using Domain.Models;

namespace Domain.Contracts;

public interface IPlayerService
{
    IEnumerable<PlayerEntity> GetPlayers();
    PlayerEntity? GetPlayer(string ign);
    void RegisterPlayer(string ign, PlayerStats stats, ulong registrant);
    void UpdatePlayer(PlayerEntity player);
    void DeletePlayer(string ign);
}