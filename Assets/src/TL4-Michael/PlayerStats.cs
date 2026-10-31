using System;
using System.Collections.Generic;

// Shared runtime player data. Any script can read PlayerStats.Shared.Level / XP
// and write through GainXP, SetXP, or SetLevel.
public class PlayerStats
{
    public static PlayerStats Shared { get; } = new PlayerStats();

    public int Level { get; private set; } = 1;
    public int XP { get; private set; } = 0; // XP toward the next level
    public int XPToNextLevel => 100 + (Level - 1) * 50;
    public int PendingLevelUps { get; private set; } = 0;

    // Shared, run-wide card resources; other scripts may read them.
    public int RerollsRemaining { get; private set; } = 3;
    public int BanishesRemaining { get; private set; } = 3;
    private readonly HashSet<string> banishedCards = new HashSet<string>();
    public ICollection<string> BanishedCards => new List<string>(banishedCards).AsReadOnly();

    public bool UseReroll(bool hasActiveChoice = false)
    {
        if ((!hasActiveChoice && PendingLevelUps <= 0) || RerollsRemaining <= 0) return false;
        RerollsRemaining--;
        return true;
    }

    public bool TryBanishCard(string title, bool hasActiveChoice = false)
    {
        if ((!hasActiveChoice && PendingLevelUps <= 0) || BanishesRemaining <= 0 ||
            string.IsNullOrEmpty(title) || banishedCards.Contains(title) ||
            UpgradeDeck.AvailableCount(banishedCards) <= 1) return false;
        BanishesRemaining--;
        banishedCards.Add(title);
        return true;
    }

    // Invoke when starting a fresh run (not between levels).
    public void ResetCardResources()
    {
        RerollsRemaining = 3;
        BanishesRemaining = 3;
        banishedCards.Clear();
    }

    // Other systems can subscribe to these notifications to refresh UI, etc.
    public event Action<int> LevelChanged;
    public event Action<int, int> XPChanged; // current XP, next threshold

    public int Shards = 0;
    public int Health = 50;
    public int MaxHealth = 100;
    public int OtherRewards = 0;

    public int Damage = 10;
    public float MoveSpeed = 5f;
    public float AttackSpeed = 1f;
    public int Armor = 0;

    public void GainXP(int amount)
    {
        if (amount <= 0) return;
        SetXP(XP + amount);
    }

    // Set progress within the current level. Values over the requirement
    // automatically grant levels and queue one card choice per level gained.
    public void SetXP(int amount)
    {
        XP = Math.Max(0, amount);
        while (XP >= XPToNextLevel)
        {
            XP -= XPToNextLevel;
            Level++;
            PendingLevelUps++;
            LevelChanged?.Invoke(Level);
        }
        XPChanged?.Invoke(XP, XPToNextLevel);
    }

    // For milestones, debugging, or scripted rewards. Increasing the level
    // queues the corresponding number of upgrade choices.
    public void SetLevel(int newLevel)
    {
        newLevel = Math.Max(1, newLevel);
        if (newLevel == Level) return;
        if (newLevel > Level) PendingLevelUps += newLevel - Level;
        else PendingLevelUps = 0; // no outdated choices after a level reduction
        Level = newLevel;
        XP = Math.Min(XP, XPToNextLevel - 1);
        LevelChanged?.Invoke(Level);
        XPChanged?.Invoke(XP, XPToNextLevel);
    }

    public bool ConsumePendingUpgrade()
    {
        if (PendingLevelUps <= 0) return false;
        PendingLevelUps--;
        return true;
    }

    public void Heal(int amount)
    {
        if (amount > 0)
            Health = Math.Min(MaxHealth, Health + amount);
    }
}
