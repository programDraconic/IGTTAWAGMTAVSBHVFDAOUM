using System;
using System.Collections.Generic;

// UPGRADE-SOURCE CONTRACT: any system that offers upgrade cards implements this.
// The UI knows only this interface, not whether the offer came from XP, a chest, etc.
public interface IUpgradeSource
{
    string SourceName { get; }
    int PendingChoices { get; }
    UpgradeCard[] DrawChoices(Random random, ICollection<string> bannedTitles,
        UpgradeCard[] previousOffer = null);
    bool TryChoose(PlayerStats player, UpgradeCard card);
}

// XP level-ups consume one pending choice per selected upgrade.
public sealed class LevelUpUpgradeSource : IUpgradeSource
{
    private readonly PlayerStats player;
    public LevelUpUpgradeSource(PlayerStats player) { this.player = player; }
    public string SourceName => "LEVEL UP";
    public int PendingChoices => player.PendingLevelUps;

    public UpgradeCard[] DrawChoices(Random random, ICollection<string> bannedTitles,
        UpgradeCard[] previousOffer = null)
    {
        return UpgradeDeck.DrawThree(random, bannedTitles, previousOffer);
    }

    public bool TryChoose(PlayerStats target, UpgradeCard card)
    {
        if (target != player || card == null || PendingChoices <= 0) return false;
        if (!player.ConsumePendingUpgrade()) return false;
        card.Apply(player); // COMMAND PATTERN: execute the chosen card's command.
        return true;
    }
}

// Example second source: quests/shops/chests can queue a free card choice
// without increasing the player's level or consuming a pending level-up.
public sealed class BonusUpgradeSource : IUpgradeSource
{
    public static BonusUpgradeSource Shared { get; } = new BonusUpgradeSource();
    private int pending;
    private BonusUpgradeSource() { }
    public string SourceName => "BONUS UPGRADE";
    public int PendingChoices => pending;

    public void GrantChoices(int amount = 1)
    {
        if (amount > 0 && pending <= int.MaxValue - amount) pending += amount;
    }

    public UpgradeCard[] DrawChoices(Random random, ICollection<string> bannedTitles,
        UpgradeCard[] previousOffer = null)
    {
        return UpgradeDeck.DrawThree(random, bannedTitles, previousOffer);
    }

    public bool TryChoose(PlayerStats player, UpgradeCard card)
    {
        if (player == null || card == null || pending <= 0) return false;
        pending--;
        card.Apply(player);
        return true;
    }

    public void Clear() { pending = 0; }
}
