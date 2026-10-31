using System;
using UnityEngine;
using UnityEngine.InputSystem;
// Attach to an active GameObject to test pickups and level-up choices.
public class PickupBindingDemo : MonoBehaviour
{
    private Pickup current = new XPPickup();
    private readonly PlayerStats player = PlayerStats.Shared;
    private readonly System.Random random = new System.Random();
    private UpgradeCard[] choices;
    private int pickupIndex;
    private bool pausedForUpgrades;
    private bool selectingBanish;
    private float previousTimeScale = 1f;
    private string lastResult = "Collect XP or press L to level up.";

private void Update()
{
    // Use the Input System package for keyboard input.
    if (Keyboard.current != null &&
        Keyboard.current.lKey.wasPressedThisFrame &&
        player.PendingLevelUps == 0)
    {
        player.SetLevel(player.Level + 1);
        lastResult = "DEBUG: Level up (L).";
    }

    if (player.PendingLevelUps > 0 && choices == null)
    {
        StartCardSelection();
    }
}

    private void OnDisable() { ResumeAfterUpgrades(); }

    private void OnGUI()
    {
        if (player.PendingLevelUps > 0 && choices == null) StartCardSelection();
        GUI.Label(new Rect(20, 20, 950, 25),
            "Level: " + player.Level + " | XP: " + player.XP + "/" + player.XPToNextLevel +
            " | Pending: " + player.PendingLevelUps + " | L = test level up");
        GUI.Label(new Rect(20, 48, 950, 25),
            "HP: " + player.Health + "/" + player.MaxHealth + " | Damage: " + player.Damage +
            " | Armor: " + player.Armor + " | Move: " + player.MoveSpeed.ToString("F1") +
            " | Attacks/sec: " + player.AttackSpeed.ToString("F1"));
        GUI.Label(new Rect(20, 76, 950, 25),
            "Shards: " + player.Shards + " | Pickup: " + current.GetType().Name);
        GUI.Label(new Rect(20, 104, 1050, 25), lastResult);

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

        GUI.Box(new Rect(20, 205, 760, 255), selectingBanish
            ? "BANISH: Click a card to remove it from future draws (no upgrade selected)"
            : "LEVEL UP! Game paused - choose an upgrade");
        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            UpgradeCard card = choices[i];
            if (GUI.Button(new Rect(35 + i * 245, 245, 225, 125), card.Title + "\n\n" + card.Description))
            {
                if (selectingBanish) BanishCard(index);
                else ChooseCard(index);
            }
        }

        GUI.enabled = !selectingBanish && player.RerollsRemaining > 0 &&
                      UpgradeDeck.AvailableCount(player.BanishedCards) > 1;
        if (GUI.Button(new Rect(35, 390, 225, 45), "Reroll (" + player.RerollsRemaining + " left)"))
            RerollCards();
        GUI.enabled = !selectingBanish && player.BanishesRemaining > 0 &&
                      UpgradeDeck.AvailableCount(player.BanishedCards) > 1;
        if (GUI.Button(new Rect(280, 390, 225, 45), "Banish (" + player.BanishesRemaining + " left)"))
            selectingBanish = true;
        GUI.enabled = selectingBanish;
        if (GUI.Button(new Rect(525, 390, 225, 45), "Cancel Banish"))
            selectingBanish = false;
        GUI.enabled = true;
    }

    private void StartCardSelection()
    {
        if (player.PendingLevelUps <= 0 || choices != null) return;
        if (!pausedForUpgrades)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            pausedForUpgrades = true;
        }
        selectingBanish = false;
        choices = UpgradeDeck.DrawThree(random, player.BanishedCards);
    }

    private void RerollCards()
    {
        if (choices == null || !player.UseReroll()) return;
        choices = UpgradeDeck.DrawThree(random, player.BanishedCards, choices);
        lastResult = "Rerolled available cards.";
    }

    private void BanishCard(int index)
    {
        if (!selectingBanish || choices == null || index < 0 || index >= choices.Length) return;
        string title = choices[index].Title;
        if (!player.TryBanishCard(title)) return;
        selectingBanish = false;
        // Refresh all offered cards to avoid stale choices / banished selections.
        choices = UpgradeDeck.DrawThree(random, player.BanishedCards, choices);
        lastResult = "Banished " + title + " for this run. Select an upgrade.";
    }

    private void ChooseCard(int index)
    {
        if (selectingBanish || choices == null || index < 0 || index >= choices.Length ||
            player.PendingLevelUps <= 0) return;
        UpgradeCard selected = choices[index];
        // COMMAND PATTERN invoker: UI triggers the card's encapsulated command.
        selected.Apply(player);
        player.ConsumePendingUpgrade();
        lastResult = "Selected: " + selected.Title + " (" + selected.Description + ")";
        choices = null;
        if (player.PendingLevelUps > 0) StartCardSelection();
        else ResumeAfterUpgrades();
    }

    private void ResumeAfterUpgrades()
    {
        selectingBanish = false;
        choices = null;
        if (!pausedForUpgrades) return;
        Time.timeScale = previousTimeScale;
        pausedForUpgrades = false;
    }
}
