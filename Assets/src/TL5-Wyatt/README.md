# TL5 / F6 — Dating System & Relationship Metagame (MVP)

Built from the TL5 SA diagrams: use case (slide 33), class diagram (slide 34),
Template Method (slide 35), dynamic binding (slide 36).

## Run it

1. Open your own scene (not `Assets/scenes/MVP.unity`).
2. Hierarchy → Create Empty → name it `DatingMVP` → Add Component → `DatingMVP`.
3. Press Play. One suitor card appears - a GameObject instantiated at run time
   from `Assets/prefabs/TL5-Wyatt/Resources/SuitorCard.prefab`. Use **< Prev** and
   **Next >** above the card to rebind it to each of the four suitors.
4. Gift a suitor, then press Advance Relationship. Unlock events are logged
   at the bottom of the screen and to the Console.

The prefab is found automatically via `Resources.Load`. You can also drag it
onto the `Suitor Card Prefab` field in the Inspector instead.

### Card size

Cards scale with the Game view width, so they stay readable from 800 px to 4K.
If they are still too small or too large, use the **A- / A+** buttons in the
top right during Play, or set **UI Scale** on the `DatingMVP` component before
pressing Play. Sizing lives in `DatingUi.cs`.

## Classes (slide 34)

| Diagram | File |
|---|---|
| `DatingController` | `DatingController.cs` |
| `Suitor` (abstract) | `Suitor.cs` |
| `Husband` (built as `VampireShrimp`), `Matriarch`, `Potential`, `Friend` | one file each |
| `RelationshipState` | `RelationshipState.cs` |
| `Gift`, `Reaction`, `RelationshipStage` | supporting types |
| — | `DatingUi.cs` (card sizing and styles) |

`SuitorView` and `DatingMVP` are the only MonoBehaviours. Everything else is
plain C#, so the whole feature is testable with no scene loaded.

## Template Method + dynamic binding

`Suitor.ReceiveGift(gift)` is the template: evaluate taste → record → select
reaction. It is **not** virtual. The two overridable steps are the binding pair:

```
protected virtual int      EvaluateTaste(Gift gift)
protected virtual Reaction SelectReaction()
```

Walk the card through all four suitors, giving each the same Shrimp Cracker:

| Suitor | Affection | Reaction |
|---|---|---|
| Vampire Shrimp | +2 | "You did not have to. But I am glad you did." |
| Matriarch | 0 (insulted) | "You are not family. Do not pretend otherwise." |
| Potential | 0 | "I do not know you well enough for this." |
| Friend | +3 | "Oh! For me? That is very kind." |

Comment out `virtual` in `Suitor.cs` and `override` in all four subclasses:
every card shows `+1` affection and "accepts it without comment". Four
characters collapse into one. That is the live oral-exam demo.

## Deviations from the diagram

Three, all forced by making the diagram runnable:

1. **`ReceiveGift` returns `Reaction`.** The diagram shows no return type, but
   the view needs the reaction to display and a test needs a value to assert on.
2. **`SelectSuitor(int index)` on `DatingController`.** `OfferGift(gift)` and
   `BeginConversation()` take no suitor parameter, so something has to say who
   the gift is for. `Converse()` on `Suitor` exists for the same reason — it is
   the "Begin Conversation" use case from slide 33.
3. **`EvaluateTaste` / `SelectReaction` are `protected` in the subclasses.**
   The diagram marks them `#` on `Suitor` and `+` on the subclasses; C# cannot
   widen access on an override, so they stay `protected` throughout.

4. **`Husband` is built as `VampireShrimp`.** Same class in the hierarchy, same
   position under `Suitor`, renamed for the in-game fiction. Relabel the box on
   slides 34 and 36 to match.

`State` is exposed read-only on `Suitor` so the view and F3 can read affection
and stage. The `relationshipState` field itself stays private, as drawn.

## Integration surface

`DatingController` is what TL1 calls. `UnlockEarned` is the "Publish Unlock
Event" use case — F3 (Summons & Persistence) subscribes to it.
`CaptureState()` / `RestoreState()` is what F3 persists.
