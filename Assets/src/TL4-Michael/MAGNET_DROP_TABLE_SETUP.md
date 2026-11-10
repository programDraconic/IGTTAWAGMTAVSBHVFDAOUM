# Magnet field and variable crystal drop tables

## Files changed / added
- UPDATED `CrystalPickup.cs`: crystal pickups provide crystals and XP and can be pulled by the magnet.
- UPDATED `EnemyHealth.cs`: optional weighted crystal drop table; retains the old `crystalPrefab` fallback when no drop table is assigned.
- NEW `CrystalMagnet.cs`: detects and pulls crystals toward the player.
- NEW `CrystalDropTable.cs`: configurable ScriptableObject drop table.
- The package also includes the previous level-up, reroll/banish, upgrade-source, shared PlayerStats, and XP bar scripts, plus unchanged PlayerCrystals.

## Set up the magnet
1. Put `CrystalMagnet` on the same **player** GameObject as `PlayerCrystals`.
2. Change `Magnet Radius` (default 4) and `Pull Speed` (default 9) in Inspector. Optional: assign the crystal layer to `Crystal Layers` for better performance.
3. Each crystal prefab needs `CrystalPickup` **and a Collider2D**. For ordinary walking pickups, set the Collider2D's `Is Trigger` on, and ensure the player/crystal collision arrangement has a Rigidbody2D so trigger events work. The player already has one in PlayerMovement.
4. Crystal prefabs can optionally have a Rigidbody2D; a dynamic Rigidbody2D might react to physics while being pulled, so kinematic is often simpler.
5. CrystalMagnet has public `MagnetRadius` and `PullSpeed` properties for upgrades, e.g. `magnet.MagnetRadius += 1f`.

## Set up different crystal types
1. Duplicate your crystal prefab to make e.g. SmallCrystal, LargeCrystal and RareCrystal.
2. Set the `Crystal Value` and `Xp Value` on each prefab's `CrystalPickup` component (e.g. 1/10, 3/30, 10/100).
3. Assets > Create > Roguelike > Crystal Drop Table. Open the asset and add entries, assigning **CrystalPickup components on prefab assets** (drag prefab into the field).
4. Example table: SmallCrystal weight=70 min=1 max=3; LargeCrystal weight=25 min=1 max=2; RareCrystal weight=5 min=1 max=1. `Drop Chance` sets the chance of *any* drop.
5. Assign this asset to `Crystal Drop Table` on your enemy prefab's EnemyHealth component. When set, it replaces the single `Crystal Prefab` drop. Unassigned preserves old behavior.
6. `Drop Scatter Radius` controls spawn position variation. For no scatter, set to 0.

## Behavior and limitations
- Each enemy death rolls **one** weighted crystal type and then spawns a random number of that type. To produce independent multi-type drops per death, expand the roll loop separately.
- Picking up a crystal calls `PlayerCrystals.AddCrystals(...)` and `PlayerStats.Shared.GainXP(...)` exactly once; this triggers the existing leveling system, pause and cards.
- Magnet does not run while `Time.timeScale == 0` (paused for upgrades).
- Replace existing scripts instead of keeping duplicates. **Do not copy duplicate PlayerCrystals or EnemyHealth class files into Assets.**
- Scripts use the existing Unity 2D physics APIs; no new InputSystem dependency is introduced.
