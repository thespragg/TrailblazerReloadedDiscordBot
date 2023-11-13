using Domain.Contracts;
using Domain.Models;

namespace Domain.Services;

public class PlayerService : IPlayerService
{
    private readonly IStatService _statService;
    private readonly IPlayerRepository _playerRepo;

    public PlayerService(IStatService statService, IPlayerRepository playerRepo)
    {
        _statService = statService;
        _playerRepo = playerRepo;
    }
    
    public Result<IEnumerable<PlayerEntity>> GetPlayers()
        => Result<IEnumerable<PlayerEntity>>.Ok(_playerRepo.GetPlayers());

    public Result<PlayerEntity?> GetPlayer(string ign)
        => Result<PlayerEntity?>.Ok(_playerRepo.GetPlayerByIgn(ign));

    public async Task<Result<Unit>> RegisterPlayer(string ign, ulong registrant)
    {
        var playerStats = await _statService.GetStats(ign);
        if (playerStats is null)
        {
            return Result<Unit>.Failed($"Couldn't find {ign} on the hiscores, player not registered.");
        }
        
        var user = _playerRepo.GetPlayerByIgn(ign);
        if (user is not null)
        {
            return Result<Unit>.Failed("That username has already been registered.");
        }
        
        _playerRepo.RegisterPlayer(ign, registrant, playerStats);
        return Result<Unit>.Ok(new Unit());
    }

    public async Task<Result<Unit>> UpdatePlayerStats(string username)
    {
        var playerStats = await _statService.GetStats(username);
        if (playerStats is null)
        {
            return Result<Unit>.Failed($"Couldn't find {username} on the hiscores, player not registered.");
        }
        
        _playerRepo.Update(username, playerStats);
        return Result<Unit>.Ok(new Unit());
    }

    public Result<Unit> DeletePlayer(string ign)
    {
        throw new NotImplementedException();
    }
}