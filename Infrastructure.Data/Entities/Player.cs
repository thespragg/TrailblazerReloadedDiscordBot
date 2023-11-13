using Domain.Models;

namespace Infrastructure.Data.Entities;

public class Player
{
    public int Id { get; set; }
    public ulong DiscordId { get; init; }
    public required string Username { get; init; }
    public required PlayerStats Stats { get; init; }
}