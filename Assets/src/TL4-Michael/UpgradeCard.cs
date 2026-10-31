using System;

// Represents a single upgrade shown as one of three card choices.
public class UpgradeCard
{
    public readonly string Title;
    public readonly string Description;
    private readonly Action<PlayerStats> effect;

    public UpgradeCard(string title, string description, Action<PlayerStats> effect)
    {
        Title = title;
        Description = description;
        this.effect = effect;
    }

    public void Apply(PlayerStats player)
    {
        effect(player);
    }
}
