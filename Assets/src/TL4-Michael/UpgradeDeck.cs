using System;
using System.Collections.Generic;

public static class UpgradeDeck
{
    private static readonly UpgradeCard[] AllCards =
    {
        new UpgradeCard("Power Strike", "+5 damage", p => p.Damage += 5),
        new UpgradeCard("Iron Skin", "+3 armor", p => p.Armor += 3),
        new UpgradeCard("Fleet Footed", "+0.5 movement speed", p => p.MoveSpeed += 0.5f),
        new UpgradeCard("Rapid Attack", "+0.2 attacks per second", p => p.AttackSpeed += 0.2f),
        new UpgradeCard("Vitality", "+20 maximum health and heal 20", p => { p.MaxHealth += 20; p.Heal(20); }),
        new UpgradeCard("Second Wind", "Restore 40 health", p => p.Heal(40)),
        new UpgradeCard("Treasure Hunter", "+10 shards", p => p.Shards += 10)
    };

    // Samples three distinct cards without modifying the shared deck.
    public static UpgradeCard[] DrawThree(Random random)
    {
        var indexes = new List<int>();
        for (int i = 0; i < AllCards.Length; i++) indexes.Add(i);

        var selection = new UpgradeCard[3];
        for (int i = 0; i < selection.Length; i++)
        {
            int pick = random.Next(indexes.Count);
            selection[i] = AllCards[indexes[pick]];
            indexes.RemoveAt(pick);
        }
        return selection;
    }
}
