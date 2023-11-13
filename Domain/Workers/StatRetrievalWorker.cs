using Domain.Contracts;
using Microsoft.Extensions.Hosting;

namespace Domain.Workers;

public class StatRetrievalWorker : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IPlayerService _playerService;

    public StatRetrievalWorker(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(CheckStats, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
        return Task.CompletedTask;
    }

    private void CheckStats(object? state)
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