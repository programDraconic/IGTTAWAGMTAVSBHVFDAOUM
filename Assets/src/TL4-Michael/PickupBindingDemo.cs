using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
#endif

// Attach to an empty GameObject in the scene to test pickups and card level-ups.
public class PickupBindingDemo : MonoBehaviour
{
    private Pickup current = new XPPickup();
    private readonly PlayerStats player = PlayerStats.Shared;
    private readonly System.Random random = new System.Random();
    private UpgradeCard[] choices;
    private int pickupIndex = 0;
    private string lastResult = "Collect XP to level up and choose an upgrade.";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerStats.Shared.SetLevel(
                PlayerStats.Shared.Level + 1
            );
        }
    }

    private void OnGUI()
    {
        // Also show cards when another script awards XP or changes level.
        if (player.PendingLevelUps > 0 && choices == null) StartCardSelection();
        GUI.Label(new Rect(20, 20, 750, 25),
            "Level: " + player.Level + " | XP: " + player.XP + "/" + player.XPToNextLevel +
            " | Pending upgrades: " + player.PendingLevelUps + " | Press L to level up");
        GUI.Label(new Rect(20, 48, 850, 25),
            "HP: " + player.Health + "/" + player.MaxHealth + " | Damage: " + player.Damage +
            " | Armor: " + player.Armor + " | Move: " + player.MoveSpeed.ToString("F1") +
            " | Attacks/sec: " + player.AttackSpeed.ToString("F1"));
        GUI.Label(new Rect(20, 76, 750, 25),
            "Shards: " + player.Shards + " | Other: " + player.OtherRewards +
            " | Pickup: " + current.GetType().Name);
        GUI.Label(new Rect(20, 104, 850, 25), lastResult);

        // Don't allow more pickups until every pending upgrade is chosen.
        GUI.enabled = player.PendingLevelUps == 0;
        if (GUI.Button(new Rect(20, 140, 160, 40), "Collect"))
        {
            lastResult = current.Collect(player);
            if (player.PendingLevelUps > 0) StartCardSelection();
        }
        if (GUI.Button(new Rect(190, 140, 160, 40), "Swap Pickup"))
        {
            pickupIndex = (pickupIndex + 1) % 4;
            switch (pickupIndex)
            {
                case 0: current = new XPPickup(); break;
                case 1: current = new ShardPickup(); break;
                case 2: current = new HealthPickup(); break;
                default: current = new OtherPickup(); break;
            }
            lastResult = "Switched to " + current.GetType().Name;
        }
        GUI.enabled = true;

        if (choices == null) return;

        GUI.Box(new Rect(20, 205, 760, 200), "LEVEL UP! Choose one upgrade");
        for (int i = 0; i < choices.Length; i++)
        {
            int cardIndex = i;
            UpgradeCard card = choices[i];
            Rect cardRect = new Rect(35 + i * 245, 245, 225, 140);
            if (GUI.Button(cardRect, card.Title + "\n\n" + card.Description))
                ChooseCard(cardIndex);
        }
    }

    private void StartCardSelection()
    {
        choices = UpgradeDeck.DrawThree(random);
    }

    private void ChooseCard(int index)
    {
        if (choices == null || player.PendingLevelUps <= 0) return;
        UpgradeCard selected = choices[index];
        selected.Apply(player);
        player.ConsumePendingUpgrade();
        lastResult = "Selected: " + selected.Title + " (" + selected.Description + ")";
        choices = null;
        if (player.PendingLevelUps > 0) StartCardSelection();
    }
}
