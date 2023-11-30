using Domain.Contracts;
using Microsoft.Extensions.Hosting;

namespace Domain.Workers;

public interface IStatRetrievalWorker
{
    void Start();
    void CheckStats(object? state);
}

public class StatRetrievalWorker : IStatRetrievalWorker
{
    private Timer? _timer;
    private readonly IPlayerService _playerService;

    public StatRetrievalWorker(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    public void Start()
        => _timer = new Timer(CheckStats, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));

    public void CheckStats(object? state)
    {
        var players = _playerService.GetPlayers().Value;
        if (players is null) return;
        
        foreach (var player in players)
        {
            _playerService.UpdatePlayerStats(player.Username);
            Thread.Sleep(50);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}