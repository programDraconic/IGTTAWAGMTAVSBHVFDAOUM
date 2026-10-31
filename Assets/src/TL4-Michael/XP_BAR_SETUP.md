# XP bar setup

1. Copy `XPBarHUD.cs` to your Unity project's scripts folder.
2. Create an empty GameObject in your gameplay scene called `XP HUD`.
3. Add the `XPBarHUD` component to that GameObject.
4. Enter Play Mode. The bar appears centered at the bottom of the Game view.
5. Collect a crystal from an enemy or press **L** to test a level-up.

The bar reads `PlayerStats.Shared.XP`, `PlayerStats.Shared.Level`, and `PlayerStats.Shared.XPToNextLevel` each GUI frame. It therefore tracks XP from any script that calls `PlayerStats.Shared.GainXP(amount)` and automatically resets its fill on level-up (carrying over excess XP).

Change `Bar Width`, `Bar Height`, `Bottom Padding`, and colors in the Inspector. The component does not need a Canvas, TextMeshPro, or the New Input System package.

**Integration:** This archive is based on the previous Upgrade Source Contracts version, and includes the `CrystalPickup.cs` implementation that grants XP from enemy-dropped crystals. Replace old versions instead of keeping duplicate C# classes. The component has not been compiled inside your Unity project.
