using Domain.Models;

namespace Domain.Contracts;

public interface IPlayerService
{
    Result<IEnumerable<PlayerEntity>> GetPlayers();
    Result<PlayerEntity?> GetPlayer(string ign);
    Task<Result<Unit>> RegisterPlayer(string ign, ulong registrant);
    Task<Result<Unit>> UpdatePlayerStats(string username);
    Result<Unit> DeletePlayer(string ign);
}