# Player health — F7 foundation

Nishant Patel / TL6. This adds damage and health rules on top of Michael's
existing `PlayerStats.Shared`, so pickups, upgrades and health use one set of
numbers. It does not add a second PlayerStats or PlayerModel health store.

## Run the demo

1. In Unity choose **TL6 > Open Player Health Demo**.
2. Press Play. The demo starts a fresh run at maximum HP.
3. Click **Take 10 damage**. HP decreases; hits during the next 0.5 seconds
   are ignored. Armour reduces incoming damage, with a minimum of zero.
4. Click **Heal 20**. HP cannot exceed the current maximum.
5. Take enough damage to die. One `PlayerDied` event is logged.
6. Click **Start new run** to reset health, protection and the death event guard.

Scene: `Assets/tst/TL6-Nishant/PlayerHealthDemo.unity`.

## Connect to gameplay

Add one `TL6.PlayerCore.PlayerHealthController` to the player object. It uses
`PlayerStats.Shared` in Awake without resetting the run's health. Keep one
active health controller per player. Call `Initialise()` only when starting
a fresh run; preserve it with the player across room changes. Unity advances
the protection timer using scaled time, so a timeScale pause pauses the timer.

Enemy integration can pass raw damage into this component:

```csharp
using TL6.PlayerCore;

PlayerHealthController health = target.GetComponent<PlayerHealthController>();
if (health != null)
    health.ApplyDamage(attackDamage);
```

TL1 can subscribe to `PlayerDied` to end the run and unsubscribe when its
owner is destroyed. This commit provides the event; it does not change TL1's
state machine, Shenghao's attack-preview code or the shared gameplay scenes.

Armour and maximum HP are read live, so upgrades take effect without copying
stats. A fully blocked hit does not start protection. `ApplyDamage` and `Heal`
return the actual HP lost or gained. Non-positive amounts are ignored.
This health path prevents healing after death until `Initialise`; older code
that writes `PlayerStats.Health` or calls its existing `Heal` directly can
bypass those guards and should eventually route through the health component.

## Files and validation

- `PlayerHealth.cs`: plain C# health, armour, healing, protection and death rules.
- `PlayerHealthController.cs`: connects the rules to Unity and shared stats.
- `PlayerHealthDemo.cs`: a small manual demonstration.
- `HealthTests`: rule tests plus a saved-scene test using the real shared stats.

The runtime assembly now references `Michael.Game`. No teammate source code
is changed. This is a health foundation, not the complete F7 Champion contract;
input, stat chains and the full loadout/DamagePacket interface remain separate work.

Validated with Unity 6000.5.10f1: all 13 selected TL6 tests passed (eight health
tests, including a Play Mode scene check, and the five existing binding/prefab/
stress tests). The scene was generated with Unity's serializer. These checks
do not claim that enemy damage or the game-over flow is wired into the main game.

## AI assistance

Codex drafted this implementation, demo and tests at Nishant's request on
November 2, 2026. Validation results and the pushed commit are recorded in the
accompanying chat. Nishant's next learning task is explaining these files
line by line and tracing damage through the shared data before the oral exam.
