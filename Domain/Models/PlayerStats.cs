namespace Domain.Models;

public class PlayerStats
{
    public List<Skill> Skills { get; set; } = new();
    public List<Activity> Activities { get; set; } = new();
}