using System;
using System.Collections.Generic;

public static class UpgradeDeck
{
    // COMMAND PATTERN: each card is configured with its own concrete command.
    private static readonly UpgradeCard[] AllCards =
    {
        new UpgradeCard("Power Strike", "+5 damage", new PowerStrikeCommand()),
        new UpgradeCard("Iron Skin", "+3 armor", new IronSkinCommand()),
        new UpgradeCard("Fleet Footed", "+0.5 movement speed", new FleetFootedCommand()),
        new UpgradeCard("Rapid Attack", "+0.2 attacks per second", new RapidAttackCommand()),
        new UpgradeCard("Vitality", "+20 maximum health and heal 20", new VitalityCommand()),
        new UpgradeCard("Second Wind", "Restore 40 health", new SecondWindCommand()),
        new UpgradeCard("Treasure Hunter", "+10 shards", new TreasureHunterCommand())
    };

    public static int AvailableCount(ICollection<string> bannedTitles)
    {
        int count = 0;
        foreach (UpgradeCard card in AllCards)
            if (bannedTitles == null || !bannedTitles.Contains(card.Title)) count++;
        return count;
    }

    // Up to three distinct cards. Rerolls prefer cards not in previousOffer,
    // but allow previous cards when the pool is too small.
    public static UpgradeCard[] DrawThree(Random random, ICollection<string> bannedTitles = null,
        UpgradeCard[] previousOffer = null)
    {
        var fresh = new List<UpgradeCard>();
        var repeats = new List<UpgradeCard>();
        foreach (UpgradeCard card in AllCards)
        {
            if (bannedTitles != null && bannedTitles.Contains(card.Title)) continue;
            bool shownPreviously = false;
            if (previousOffer != null)
                foreach (UpgradeCard old in previousOffer)
                    if (old.Title == card.Title) { shownPreviously = true; break; }
            if (shownPreviously) repeats.Add(card);
            else fresh.Add(card);
        }

        var result = new List<UpgradeCard>();
        while (result.Count < 3 && (fresh.Count > 0 || repeats.Count > 0))
        {
            var pool = fresh.Count > 0 ? fresh : repeats;
            int index = random.Next(pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return result.ToArray();
    }
}
