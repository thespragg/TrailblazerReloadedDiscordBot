namespace Domain.Models;

public class PlayerEntity
{
    public ulong DiscordId { get; init; }
    public required string Username { get; init; }
    public PlayerStats? Stats { get; set; }
}