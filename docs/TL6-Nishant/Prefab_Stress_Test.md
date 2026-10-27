# Player Core prefab and stress test

This is Nishant's instantiable GameObject for the stress-test session. It exercises the existing F7 stat modifier code while many visible shrimp bodies move and collide in 2D. It is a test object, not the completed Player Movement and Core Stats feature described in the Champion document.

## Files

- Code: `Assets/src/TL6-Nishant/PlayerCoreStressBody.cs` and `PlayerCoreStressTest.cs`.
- Prefab: `Assets/prefabs/TL6-Nishant/TL6_PlayerCore.prefab`.
- Scene: `Assets/tst/TL6-Nishant/TL6_PlayerCore_Stress.unity`.
- Automated checks: `Assets/tst/TL6-Nishant/TL6_Tests/PlayerCorePrefabTests.cs`.

The prefab uses a personal-folder copy of the team's existing GameShrimp sprite, saved as `TL6_Shrimp.png`. It contains a dynamic Rigidbody2D, a solid CircleCollider2D, and Nishant's own script. It does not need another teammate's run-session manager to spawn or move.

## Demonstrate in Unity

1. Open this project with Unity **6000.5.10f1**. Make sure it is the same checkout containing these new files.
2. Open `TL6_PlayerCore_Stress.unity`, or choose **TL6 > Open Player Core Stress Scene**.
3. Enter Play mode. One moving shrimp appears automatically.
4. Choose **Spawn 10** or **Spawn 100**. The body count should rise, and copies should move and collide.
5. Choose **Switch modifier**. Base Might 10 resolves to 12 with PercentModifier or 15 with FlatModifier.
6. Choose **Start test**. It resets to one object, waits for physics to settle, and measures starting FPS over two seconds. It then adds 100 objects at a time and measures again after settling. It stops at the first stage at or below half the starting FPS, as described in the shared lecture's slides 23–24 and 35.
7. Record the starting FPS, half-FPS threshold, number of objects at the break, measured FPS from the on-screen result, and device. The Console also records the result and device information once at the end. Save a screenshot or short video as evidence. **Stop test** cancels a run; cancelling is not a measured breaking point.
8. Choose **Clear all** to remove the copies. Stop Play mode when finished.

In the Editor, you start the measured test with one button. In an exported build, this test scene starts the measurement automatically, so a phone does not need keyboard or touch controls. A 2,000-object safety limit prevents unlimited spawning; reaching it does not establish a breaking point. If necessary, increase Maximum Objects in the scene's Stress Test component and rerun. Manual Spawn buttons remain available outside a measured run.

The rule is relative to this device's starting performance, not a fixed 30-FPS target. For example, a measured starting value of 120 FPS produces a 60-FPS stopping threshold. That is an explanation of the rule, not a result measured on this project. Measure on the intended device for phone profiling because Editor overhead and frame caps affect FPS. This test records the FPS break; it does not implement the separate physics-violation measurement mentioned in the lecture.

## Run on a device

For an Android demonstration, add only this stress scene to the test build's scene list and use Build and Run with the phone connected. The course slides allow a laptop development build as the fallback for iPhone-only or no-phone teams. The existing game's shared build list has not been changed; this scene is a separate test. Record the actual device model with the result. A test run in the Editor is not phone performance evidence.

## Automated verification

Open Unity Test Runner in EditMode and run the `TL6_Tests` assembly. The original two binding tests are retained. The new tests verify that the saved prefab can be instantiated with visible artwork and dynamic physics, and that changing one instance's modifier does not change another instance's stat. A scene test enters Play mode to verify movement, spawning 100 copies, clearing, measuring a baseline, using half that baseline as the threshold, and ending a short run at its configured limit or FPS break. Headless code checks verify behaviour, not representative device performance.

## Share with the team

In GitHub Desktop, review and commit the new files under Nishant's `src`, `tst`, `prefabs`, and `docs` directories, including Unity's `.meta` files. Follow the team's main-branch workflow, pull first, and resolve any conflicts with TL1. Check the shared main branch after pushing. No Library or Temp files belong in the commit.

## Remaining F7 work

The full Champion still calls for the player model, seven-stat block, ordered stackable modifier chain, input abstraction and controlled movement, damage/armour/invulnerability, and the once-per-run PlayerDied event. This prefab does not claim to implement those systems. Keep those tasks separate from completing the session's reusable-object requirement.
