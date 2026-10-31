# Crystal pickup XP update

`ShardPickup.Collect(PlayerStats player)` now grants **3 shards and 25 XP**.

- XP is awarded using `player.GainXP(XPPerPickup)` rather than changing `XP` directly.
- This triggers the existing shared leveling flow (`PlayerStats.Shared`), pending upgrade cards, pause, rerolls and banishes via `PickupBindingDemo`.
- Adjust `ShardPickup.XPPerPickup` to tune the XP reward.
- Any world crystal GameObject must call `new ShardPickup().Collect(PlayerStats.Shared)` (or use its existing `ShardPickup` instance). These are plain C# pickup types, not `MonoBehaviour` components.
- In the demo, click `Swap Pickup` until `ShardPickup` is selected, then `Collect`. The L test key is unchanged and uses legacy `Input.GetKeyDown`.

## Installation
Replace the old `ShardPickup.cs`. All other scripts in this archive are included for convenience; do not keep multiple copies of the same class in your Unity project.

**Note:** if your crystals use a different world collection script rather than `ShardPickup`, that script must similarly invoke `PlayerStats.Shared.GainXP(amount)` when collected.
