# TL2+ Gagan - Summon Dynamic Binding

This folder contains the plain-C# Summon superclass and four subclasses:

- OrbitingSummon
- HuntingSummon
- TotemSummon
- RevengeSummon

`Summon.UpdateBehavior(context)` is virtual. Each subclass overrides it, so a
base `Summon` reference runs the behavior of its actual subclass at runtime.

`BindingDemo.cs` is the Unity-facing demonstration component. Add it to an
empty GameObject in a demo scene and press **Next Summon** during Play Mode.
