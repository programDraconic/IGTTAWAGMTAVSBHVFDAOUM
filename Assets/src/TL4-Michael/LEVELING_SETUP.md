# Shared leveling system (Unity C#)

Replace `PlayerStats.cs` and `PickupBindingDemo.cs` with these versions. Keep the existing `XPPickup.cs`, `UpgradeCard.cs`, and `UpgradeDeck.cs` from the included package.

The player data is available to **every script** using the same object:

```csharp
int currentLevel = PlayerStats.Shared.Level;
int currentXP = PlayerStats.Shared.XP;
int requiredXP = PlayerStats.Shared.XPToNextLevel;

PlayerStats.Shared.GainXP(25); // Add XP; triggers levels automatically
PlayerStats.Shared.SetXP(80);  // Set current XP progress
PlayerStats.Shared.SetLevel(5); // Set level directly and queue cards for gained levels
```

To listen for changes from a MonoBehaviour:

```csharp
private void OnEnable()
{
    PlayerStats.Shared.LevelChanged += OnLevelChanged;
}
private void OnDisable()
{
    PlayerStats.Shared.LevelChanged -= OnLevelChanged;
}
private void OnLevelChanged(int newLevel)
{
    UnityEngine.Debug.Log("Player is now level " + newLevel);
}
```

`XPChanged` is also available as `Action<int, int>` with `(currentXP, requiredXP)` arguments.

The demonstration's `PickupBindingDemo` now uses `PlayerStats.Shared`, not a private new instance. It checks every GUI frame for pending upgrades, so XP awarded by any script also opens the card picker as long as `PickupBindingDemo` is active in the scene.

**Notes:** This is a single-player, in-memory shared object (not a saved game or network synchronization system). Do not use `new PlayerStats()` for the shared player; always refer to `PlayerStats.Shared`. `SetXP` means total progress within the current level (and handles overflow), not lifetime XP. When reducing a level via `SetLevel`, pending upgrade selections are cleared; upgrades already chosen are not rolled back. Card stat bonuses must still be used by the real movement/combat scripts.

## Test leveling with the keyboard

With `PickupBindingDemo` attached to an active GameObject, enter Play Mode and press **L** in the Game view to immediately gain one level. This calls `PlayerStats.Shared.SetLevel(PlayerStats.Shared.Level + 1)` and queues an upgrade, so the three-card selection opens automatically. Pressing L again before selecting a card queues another upgrade selection. Works with Unity's new Input System or legacy Input Manager via conditional compilation.
