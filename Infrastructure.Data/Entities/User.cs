namespace Infrastructure.Data.Entities;

public class User
{
    public int Id { get; set; }
    public ulong DiscordId { get; set; }
    public required string Username { get; set; }
}