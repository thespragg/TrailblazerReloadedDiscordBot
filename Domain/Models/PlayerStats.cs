namespace Domain.Models;

public class PlayerStats
{
    public List<Skill> Skills { get; set; } = new();
    public List<Activity> Activities { get; set; } = new();
    public Dictionary<string, int> SkillsAndActivities =>
        Skills
            .Select(x => (x.Name, x.Level))
            .Concat(Activities.Select(x => (x.Name, x.Score)))
            .ToDictionary(x => x.Name, x => x.Item2);
}