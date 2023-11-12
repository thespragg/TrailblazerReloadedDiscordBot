using Domain.Models;

namespace Domain.Contracts;

public interface IStatService
{
    Task<PlayerStats> GetStats(string username);
}