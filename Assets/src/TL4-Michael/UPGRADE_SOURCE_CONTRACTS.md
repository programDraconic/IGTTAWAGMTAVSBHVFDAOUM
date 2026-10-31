# Upgrade-source contracts

The **Upgrade Source** interface (`IUpgradeSource`) lets the level-up UI receive cards from more than one source without modifying its card-selection logic.

- `UpgradeSources.cs` defines `IUpgradeSource`, `LevelUpUpgradeSource` (spends pending level-up selections), and `BonusUpgradeSource` (free reward selections from quests/chests).
- `PickupBindingDemo.cs` consumes whichever source has pending choices, prioritizing level-ups. It pauses the game, supports rerolls and banishes, and resumes after all sources are resolved.
- `UpgradeCard.Apply()` still uses `IUpgradeCommand.Execute()`. The Command design pattern remains intact.
- `PlayerStats.Shared` remains the shared player model. `CrystalPickup` is included with its XP reward.

## Example: give one upgrade from a treasure chest

```csharp
BonusUpgradeSource.Shared.GrantChoices();
```

The next `PickupBindingDemo.Update()` displays three cards and pauses. Selecting a card spends one *bonus choice*, not a player level.

## To implement another upgrade source

Implement `IUpgradeSource` in a new class, then add an instance to the `upgradeSources` array inside `PickupBindingDemo.Awake()`. The contract requires `SourceName`, `PendingChoices`, `DrawChoices(...)` and `TryChoose(...)`.

## Installing

Replace the current leveling scripts with the included updated scripts and add `UpgradeSources.cs`. Do not duplicate `PlayerStats` or other existing classes. Keep `PickupBindingDemo` on an active GameObject.

The `L` debug key still uses `UnityEngine.Input.GetKeyDown(KeyCode.L)` (legacy input). If your project uses only the New Input System, configure Active Input Handling as **Both** or **Input Manager (Old)** for this debug key. The other uploaded player scripts have their own New Input System imports; this package does not modify those scripts.
