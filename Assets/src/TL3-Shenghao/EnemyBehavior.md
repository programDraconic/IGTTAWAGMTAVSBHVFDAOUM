# Shrimp Game enemy types

`EnemyBehavior.cs` adds per-prefab movement speed, chase rules, and weapon damage multipliers for a top-down Unity 2D game. Keep health, collision handling, and movement in their existing scripts. This component calculates damage and chase velocity for those scripts to use.

## Set up enemy prefabs

1. Copy `EnemyBehavior.cs` into your Unity project's `Assets` folder.
2. Add **Enemy Behavior** to the root of each enemy prefab. Set **Move Speed** in world units per second.
3. Choose **Never**, **Within Range**, or **Always**. **Within Range** checks the current distance every time, so the enemy can resume roaming when the player leaves its detection radius. **Stopping Distance** produces zero chase velocity while keeping the enemy in chase mode.
4. Under **Weapon Modifiers**, enter a case-sensitive weapon ID from your weapon code and a multiplier. Use one entry per ID. The first entry wins if there are duplicates. A multiplier of 0 means immunity, 0.5 means half damage, and 1.5 means 50% extra damage. Unknown or blank IDs use normal damage.

These settings are example values for testing. Replace the example IDs with the ones your weapons pass.

| Enemy prefab | Speed | Chase rule | Radius | `pistol` | `shotgun` |
| --- | ---: | --- | ---: | ---: | ---: |
| Scout | 3.5 | Within Range | 5 | 1.5 | 0.75 |
| Armored | 1.5 | Always | — | 0.5 | 1.5 |
| Roamer | 2.0 | Never | — | 1.0 | 1.0 |

## Connect weapons and movement

In the existing hit handler, find the component on the enemy root and pass the resulting damage to your existing health method once. Replace the example variable and method names with the names from your scripts.

```csharp
var behavior = hitCollider.GetComponentInParent<ShrimpGame.EnemyBehavior>();
float damage = behavior != null
    ? behavior.GetDamageFromWeapon(weaponId, baseDamage)
    : baseDamage;
enemyHealth.TakeDamage(damage);
```

In your existing movement script, pass in the player transform. Use chase velocity when the method returns true. Otherwise use your current random roaming direction, scaled by the enemy's speed. Apply the chosen velocity through your existing movement code once per update.

```csharp
if (!behavior.TryGetChaseVelocity(playerTransform, out Vector2 velocity))
    velocity = roamDirection.normalized * behavior.MoveSpeed;
```

If your movement code takes a displacement, multiply velocity by the timestep once. The chase rule uses direct distance in the XY plane. It does not calculate paths around obstacles.

## Play Mode checks

- Hit a Scout with 10 base pistol damage and confirm that health drops by 15. Hit an Armored enemy with the same weapon and confirm that health drops by 5.
- Move in and out of the Scout's detection radius. Confirm that it switches between chasing and roaming. Confirm that it stops within its stopping distance.
- Confirm that the Armored enemy pursues outside the Scout's radius and that the Roamer keeps roaming.
- Test a weapon with no modifier and confirm that it deals its base damage.

## AI interaction log

- The user requested Unity C# support for enemy types with weapon-specific weaknesses and resistances, different move speeds, and different chase rules.
- The assistant chose per-prefab Inspector settings and provided integration examples for the existing weapon and movement scripts. No project source, Champion Document, system diagram, or separate interaction logs were available for this task. The component was prepared as a standalone addition and has not been compiled or tested in Unity.
