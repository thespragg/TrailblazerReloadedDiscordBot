using Domain.Contracts;
using Domain.Models;

namespace Domain.Services;

public class PlayerService(IStatService statService, IPlayerRepository playerRepo) : IPlayerService
{
    private readonly IStatService _statService = statService;

    public IEnumerable<PlayerEntity> GetPlayers()
        => playerRepo.GetPlayers();

    public PlayerEntity? GetPlayer(string ign)
        => playerRepo.GetPlayerByIgn(ign);

    public void RegisterPlayer(string ign, PlayerStats stats, ulong registrant)
        => playerRepo.RegisterPlayer(ign, registrant, stats);

    public void UpdatePlayer(PlayerEntity player)
    {
        throw new NotImplementedException();
    }

    public void DeletePlayer(string ign)
    {
        throw new NotImplementedException();
    }
}