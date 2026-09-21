Name: [Wyatt James Requa] | Feature: F6 — Dating | Mark: ____ / 30

# 1. Brief Introduction

I own F6, the Dating System & Relationship Metagame. My feature runs the between-run relationship phase: it takes the previous run's `RunSummary`, credits the items collected, lets the player interact with four characters, advances relationships through a five-stage ladder, serves stage-appropriate dialogue, and announces relationship unlocks.

My responsibilities are the relationship model and its content. I hold affection, stage, and gift history for each character; evaluate gifts; perform stage transitions in both directions; drive dialogue choices; convert the last run into the spendable item pool; and raise `UnlockEarned` when a stage transition grants a permanent bonus or summon. The public surface is `EnterDatingPhase`, `GetSuitors`, `OfferGift`, `BeginConversation`, `Advance`, `RestoreState`, `CaptureState`, and `UnlockEarned`. The Template Method is centered on `Suitor.ReceiveGift`: the shared courtship sequence stays fixed while character-specific taste evaluation and reaction selection vary through overridable steps.

I deliberately do **not** own persistence, permanent-bonus application, or combat systems. F3 owns persistence and consumes my `UnlockEarned` event; F3 also calls `RestoreState` after loading and persists the `RelationshipSnapshot` returned by `CaptureState`. F7 owns player stats and converts permanent bonus records into stat modifiers. I do not directly reference the player, enemies, weapons, or combat systems. My run-to-dating input is the `RunSummary` supplied through `EnterDatingPhase`, and my unlock output is the `RelationshipUnlock` event consumed by F3. This keeps the dating feature independent of the combat layer.

# 2. Use Case Diagram with Scenario

*(Completed in Stage 3.)*

# 3. Data Flow Diagrams

[View the team Context Diagram (Level 0) and Diagram 0 (Level 1)](Team_diagram.png)

*Team-wide Context Diagram and Diagram 0 provided for the Stage 2 data-flow documentation.*

# 4. Acceptance Tests

The tests below are derived from the F6 testing strategy and completion criteria. The relationship model is plain C# and is intended to be tested in EditMode without a Unity scene. Where the source specification does not define a concrete numeric value, the test uses a bracketed value that must be confirmed during implementation.

## Deterministic tests

| ID | Test name | Exact procedure / input | Pass / fail bound |
|---|---|---|---|
| A1 | Gift evaluation by character | Run **1** EditMode test per character, **4 total runs**. For each character, submit **3 documented gift cases**: one loved, one tolerated, and one insulted gift, quantity `[1]` each. Record `AffectionDelta`. | **Pass:** 4/4 characters return the documented taste classifications; 0/12 classifications incorrect. |
| A2 | Gift threshold boundary | Run **1** test for each of the **4 characters** using a gift/quantity combination whose resulting affection is exactly `[the documented stage threshold]`. | **Pass:** 4/4 boundary cases enter the specified stage; observed stage differs from expected by **0 stages**. |
| A3 | Stage transition upward | For each of the **4 characters**, start below the next stage threshold and give the minimum documented quantity needed to cross exactly one threshold. **1 run per character.** | **Pass:** 4/4 cases advance exactly **1 stage**; no case advances 2+ stages. |
| A4 | Stage regression downward | For each character for which the specification defines a regression case, start above the bottom stage and submit the documented insulting gift once. **[N] total runs, one per documented regression case.** | **Pass:** every documented regression occurs by the specified amount; no transition may go below the bottom stage. |
| A5 | Stage clamp at top and bottom | Execute **2 runs**: one attempts to increase a character already at the top stage, and one attempts to decrease a character already at the bottom stage. | **Pass:** top remains at the maximum defined stage and bottom remains at the minimum defined stage; 0 out-of-range stages. |
| A6 | Template Method ordering | Create **1** test subclass overriding only `EvaluateTaste`. Submit **1 gift** and record the documented five-step template sequence. | **Pass:** exactly **5/5** steps execute in the specified order; no step skipped or duplicated. |
| A7 | Character-specific dynamic binding | Submit **1 common gift** to all **4** characters using the same quantity. Record each `AffectionDelta` and reaction. | **Pass:** all **4** calls dispatch through `Suitor` polymorphism and match the documented character-specific results. |
| A8 | Dialogue stage selection | For each of the **4** characters, initialize one relationship state at each of the **5 stages** and call `BeginConversation`. **20 total calls.** | **Pass:** 20/20 calls return a node permitted by the current stage; 0 invalid starts. |
| A9 | Dialogue reachability | Traverse the dialogue graph from every defined stage entry for all **4 characters**. Record unreachable nodes and choices pointing to missing nodes. | **Pass:** unreachable nodes = **0**; missing-node choice targets = **0**. |
| A10 | Dialogue termination | Follow **1 valid conversation path per stage for each character**, **20 total paths**, until `Advance` returns `null`. | **Pass:** 20/20 paths terminate with `null`; no invalid node or indefinite loop. |
| A11 | Unlock event correctness | Create **[N] test cases, one for every documented stage transition that earns an unlock**. Trigger each once and count `UnlockEarned`. | **Pass:** every unlocking transition raises exactly **1** event; non-unlocking transitions raise **0**. |
| A12 | Snapshot round trip | Populate **1** relationship state with affection, stages, and gift history for all **4 characters**. Capture, create a fresh model, restore, and compare every serialized field. | **Pass:** 100% of serialized fields match; differing fields = **0**. |
| A13 | Run-summary item conversion | Supply **1** `RunSummary` containing `[N]` spendable items, where `[N]` is the documented test quantity, then enter dating. | **Pass:** credited quantity = `[N]`; absolute difference = **0**. |
| A14 | Public-contract isolation | Compile and inspect the F6 boundary. Verify references are limited to documented F5 `RunSummary` input and F3 relationship/persistence contracts/events. | **Pass:** undocumented combat implementation references = **0**; direct references to player/enemy/weapon/combat implementation classes = **0**. |

## Bounded-random / seeded tests

The F6 specification does not define bounded-random behavior for gifts, stage transitions, or dialogue. Therefore, no F6 acceptance test assumes random behavior. If implementation introduces randomness later, its input distribution, seed procedure, and numeric pass/fail bounds must be added here before the test is accepted.

## Example output

| Test | Runs | Observed | Bound | Pass? |
|---|---:|---|---|---|
| A3 — Stage transition upward | 4 | 4/4 advanced exactly 1 stage | 4/4; 0 cases advance 2+ | Yes |
| A6 — Template Method ordering | 1 | 5/5 steps in order | Exactly 5/5 | Yes |
| A9 — Dialogue reachability | 20 paths | 1 unreachable node | 0 unreachable nodes | **No** |
| A11 — Unlock event correctness | [N] transitions | [N] correct events | [N]/[N] correct | Yes |
| A12 — Snapshot round trip | 1 | 100% fields matched | 100%; 0 differences | Yes |
| A14 — Public-contract isolation | 1 inspection | 0 undocumented combat references | 0 | Yes |

# 5. Timeline

The F6 estimate is **52 hours**: 9 hours design, 24 hours coding, 11 hours testing, 5 hours documentation, and 3 hours learning/integration support.

## Work Items

| Task | Duration (hours) | Predecessor task(s) |
|---|---:|---|
| D1 — Relationship model and public contracts | 3 | — |
| D2 — Template Method and `Suitor` hierarchy design | 2 | D1 |
| D3 — Stage ladder and gift-rule design | 2 | D1 |
| D4 — Dialogue graph architecture and content plan | 2 | D1 |
| C1 — `DatingController` and relationship-state implementation | 5 | D2, D3 |
| C2 — `Suitor` subclasses and `ReceiveGift` template | 5 | D2, D3 |
| C3 — Stage transitions and gift evaluation support | 4 | C1, C2 |
| C4 — `DialogueGraph` and `DialogueNode` implementation | 4 | D4, C1 |
| C5 — Unlock events and snapshot/restore integration | 3 | C3, C4 |
| C6 — Dating scene UI integration | 3 | C4, C5 |
| T1 — Gift evaluation tests | 3 | C2, C3 |
| T2 — Stage-transition boundary tests | 2 | C3 |
| T3 — Template Method dynamic-binding test | 2 | C2 |
| T4 — Dialogue reachability tests | 2 | C4 |
| T5 — F3 unlock integration test | 2 | C5 |
| DOC1 — Champion document and public-interface documentation | 2 | C6, T1, T2, T3, T4, T5 |
| DOC2 — Content and self-documenting-code pass | 3 | C6, T4 |
| L1 — AI interaction log and feature learning | 1 | — |
| L2 — TL1/F3 integration support | 2 | C5 |
| **Total** | **52** | |

## Critical Path

The dependency graph has three equal 23-hour critical branches that merge at C5:

1. **D1 → D2 → C1 → C3 → C5 → C6 → DOC2 = 23 hours**
2. **D1 → D3 → C2 → C3 → C5 → C6 → DOC2 = 23 hours**
3. **D1 → D2 → C1 → C4 → C5 → C6 → DOC2 = 23 hours**

Zero-slack work is therefore D1, D2, D3, C1, C2, C3, C4, C5, C6, and DOC2. The remaining tasks have slack because they can run in parallel with the critical branches. Total labor remains 52 hours; the dependency graph's critical-path elapsed time is 23 hours.

## Gantt Timeline

```mermaid
gantt
    title F6 Dating System & Relationship Metagame — 52-hour work plan
    dateFormat  YYYY-MM-DD HH:mm
    axisFormat  %H:%M

    section Design
    D1 Relationship model and contracts        :d1, 2026-01-01 09:00, 3h
    D2 Template Method and hierarchy           :d2, after d1, 2h
    D3 Stage ladder and gift rules             :d3, after d1, 2h
    D4 Dialogue graph architecture             :d4, after d1, 2h

    section Coding
    C1 DatingController and relationship state :c1, after d2, d3, 5h
    C2 Suitors and ReceiveGift template         :c2, after d2, d3, 5h
    C3 Stage transitions and gift evaluation    :c3, after c1, c2, 4h
    C4 DialogueGraph and DialogueNode           :c4, after d4, c1, 4h
    C5 Unlocks and snapshot/restore             :c5, after c3, c4, 3h
    C6 Dating scene UI integration              :c6, after c4, c5, 3h

    section Testing
    T1 Gift evaluation tests                    :t1, after c3, 3h
    T2 Stage boundary tests                     :t2, after c3, 2h
    T3 Template Method binding test             :t3, after c2, 2h
    T4 Dialogue reachability tests              :t4, after c4, 2h
    T5 F3 unlock integration test               :t5, after c5, 2h

    section Documentation
    DOC1 Champion and interface docs            :doc1, after c6, t1, t2, t3, t4, t5, 2h
    DOC2 Content and self-documenting pass      :doc2, after c6, t4, 3h

    section Learning / Integration
    L1 AI interaction log and learning          :l1, 2026-01-01 09:00, 1h
    L2 TL1/F3 integration support               :l2, after c5, 2h
```

# AI Question Log — Summary


# Wyatt

| # | TL | Feature | Owns | GOF pattern | Hierarchy (dynamic binding) |
|---|---|---|---|---|---|
| F1 | TL1 | Run State & Flow | Game phases, run clock, pause, difficulty scalar | State | `GameState` → Boot / Run / LevelUp / Dating / GameOver |
| F2 | TL2 | Weapons | Auto-firing arsenal, projectiles, targeting, hit detection | Prototype + Strategy | `Weapon` → Projectile / Orbit / Aura / Chain |
| F3 | TL2+ | Summons & Persistence | Save profile, permanent bonuses, allied units | Memento + Façade | `Summon` → Orbiting / Hunting / Totem / Revenge |
| F4 | TL3 | Enemies | Spawn patterns, brains, damage intake, death events | Flyweight | `EnemyBrain` → Chaser / Swarmer / Ranged / Elite |
| F5 | TL4 | Experience & Draft | Gems, XP curve, level-up offers, run summary | Command | `Upgrade` → WeaponGrant / WeaponLevel / Passive / Evolution / SummonSlot |
| F6 | TL5 | Dating | Suitors, affection, stage ladder, dialogue | Template Method | `Suitor` → Husband / Matriarch / Cousin / Sibling |
| F7 | TL6 | Player Core | Movement, input, stats, health, damage — **MVP core** | Decorator | `StatModifier` → Flat / Percent / Conditional / Timed |                                                                                                         My TL# is TL5, Write the Champion document in Markdown format.

# ChatGPT

I drafted the TL5/F6 Champion document from the provided course template and project plan, keeping Sections 2 and 3 as headings only and grounding Sections 1, 4, and 5 in the F6 specification.  

[Download the Markdown Champion document](sandbox:/mnt/data/F6_TL5_Champion.md)

I marked unspecified test values with `[brackets]` as required by the template.

# Wyatt

Create two images, a context diagram and a diagram 0, both for the whole team

# ChatGPT

I created both team-wide diagrams as a single image: the **Context Diagram (Level 0)** and **Diagram 0 (Level 1)**, using the Gane–Sarson structure specified in your prompt. 

The image includes all seven team features/TL owners, external agents, data stores, labeled data flows, and process descriptions.

# Wyatt

Does the Champion document meet all requirements?

# ChatGPT

Not quite. **The Champion document meets most of the template requirements, but I would fix several things before submitting it.** I checked it against the Champion outline and the F6 specification in the project plan.  

### Requirement check

| Requirement                            | Status | Notes                                                                                                  |
| -------------------------------------- | ------ | ------------------------------------------------------------------------------------------------------ |
| Header                                 | ✅      | Correct structure; you still need to replace `[Your Name]`.                                            |
| Section 1: 2–4 first-person paragraphs | ✅      | Covers ownership, responsibilities, non-responsibilities, dependencies, and public interface.          |
| Section 2 heading only                 | ✅      | Correct: `(Completed in Stage 3.)`                                                                     |
| Section 3 heading only                 | ✅      | Correct for the current Champion stage: `(Completed in Stage 2...)`                                    |
| Deterministic acceptance tests         | ✅      | Detailed procedures and numeric bounds are provided.                                                   |
| Bounded-random tests                   | ✅      | Correctly explains that F6 has no specified random behavior.                                           |
| Example output table                   | ✅      | Has the required columns and a failing result.                                                         |
| Timeline                               | ⚠️     | 52 hours total, but the category split doesn't exactly match the F6 estimate.                          |
| Critical path                          | ⚠️     | The calculation is reasonable, but one predecessor is missing from the table/Gantt.                    |
| Gantt                                  | ⚠️     | Some dependencies in the table aren't carried into the Mermaid Gantt.                                  |
| AI Question Log                        | ⚠️     | The template asks for a heading-only placeholder; yours does that, but it needs your actual log later. |
| Use of source facts only               | ⚠️     | A few implementation details were invented rather than marked `[bracketed]`.                           |

### The biggest issues

**1. The effort breakdown should match F6 exactly.**

The F6 specification says:

* Design: **9h**
* Coding: **24h**
* Testing: **11h**
* Documentation: **5h**
* Learning/integration: **3h**
* Total: **52h** 

Your document actually has 9h design, **24h coding, 11h testing, 5h documentation, and 3h learning**, so the totals are correct. This is actually fine—the concern is only that the earlier general project-plan snippets for other features use different splits. **For F6 specifically, your split matches the source.** So I would mark this one ✅ after checking the exact F6 source.

**2. The C3 predecessor is wrong/incomplete.**

Your table has:

> C3 — Stage transitions and GiftLedger | 4 | C1

But C3 depends conceptually on both the relationship-state implementation **and the Suitor/ReceiveGift implementation (C2)**. Your own critical-path explanation says the C2 branch is critical, but the dependency table doesn't establish that.

It should be:

> **C3 — Stage transitions and GiftLedger | 4 | C1, C2**

That also makes the critical-path calculation internally consistent.

**3. The Mermaid Gantt doesn't completely respect the table's predecessors.**

For example, your table has dependencies involving testing/documentation, while the Mermaid schedule uses simplified `after` relationships. The template explicitly requires the Gantt to respect the predecessors.

So this should be corrected before submission.

**4. Some test details are more specific than the source supports.**

The F6 source explicitly supports:

* four characters
* five relationship stages
* distinct gift tastes
* transitions in both directions
* dialogue for four characters/five stages
* three lines per stage
* one reaction line per taste tier
* unlocks
* Template Method
* independent model testing with no scene loaded. 

But things like:

> “validate the gift, evaluate taste, apply affection, check stage transition, select a reaction”

are reasonable because the source describes the Template Method's five-step sequence, so that's supported. 

However, invented concrete values such as:

> `RunSummary` containing `[10]` spendable items

should technically be marked **`[10]`** or explicitly described as a proposed test value, because the source doesn't specify that exact quantity.

Similarly, the document says:

> “stage 1” through “stage 5”

which is strongly implied by the five-stage requirement, but if the actual numeric stage labels aren't explicitly defined, `[1–5]` would be safer under the template's “don't invent” rule.

**5. Your Champion is missing an explicit statement of the Template Method details.**

The template itself doesn't require a separate pattern section, but your F6 specification specifically identifies Template Method as the GOF pattern and the dynamic-binding pair:

* `Suitor.ReceiveGift`
* `EvaluateTaste(...)`
* `SelectReaction(...)`

The acceptance tests mention this, but the Champion would be stronger if Section 1 briefly identified that `Suitor.ReceiveGift` is the fixed template and the two virtual steps provide the character-specific variation. The F6 specification explicitly defines this design. 

### One thing you did correctly

You **should not add a PERT diagram yet**. The Champion prompt specifically says:

> “Do NOT draw a PERT chart in this stage — that comes in a later prompt.”

So the absence of PERT is correct. 

### Bottom line

I'd call the current document **about 85–90% submission-ready**.

The main things I would fix are:

1. **Correct C3's predecessors to `C1, C2`.**
2. **Make the Mermaid Gantt exactly match the dependency table.**
3. **Mark invented test quantities/values with `[brackets]` where the F6 specification doesn't define them.**
4. **Replace `[Your Name]`.**
5. Later, when you complete the Stage 2/3 portions, insert the **team Context Diagram and Diagram 0** you just created rather than leaving Sections 2/3 as placeholders.

The underlying F6 design itself is well aligned with the project plan: four characters, five stages, Template Method, relationship state, dialogue, unlock events, F3 persistence boundary, and no direct ownership of combat or stat application. 

If you want, I can **fix the Markdown Champion document now so it is fully aligned with the requirements and the F6 source**, including correcting the Gantt dependencies and test values.

# Wyatt

Yes please

# ChatGPT

Done. I revised the Champion document to address the issues we identified:

* Corrected the **C3 predecessor** to include both C1 and C2.
* Tightened the **acceptance tests** so unspecified values are explicitly bracketed.
* Clarified the **Template Method** implementation.
* Kept the exact **F6 52-hour effort breakdown**.
* Updated the **critical-path explanation**.
* Made the Gantt dependencies more consistent with the work-item table.
* Preserved Sections 2 and 3 as placeholders for the diagrams you complete in the appropriate stages.
* Kept the required failing example test.

[**Download the revised F6 TL5 Champion document**](sandbox:/mnt/data/F6_TL5_Champion_Final_Review_Updated.md)

You only need to replace **`[Your Name]`** and eventually insert your completed diagrams where Sections 2 and 3 indicate.

# Wyatt

Add the team diagram to the champion document by including a link to the image.

# ChatGPT

Done. I added a **Markdown link to the team Context Diagram + Diagram 0** under Section 3 of the Champion document.

* [Download the updated Champion document](sandbox:/mnt/data/F6_TL5_Champion_Final_Review_Updated_With_Team_Diagram.md)
* [View/download the team diagram image](sandbox:/mnt/data/Team_diagram.png)

The link in the document points to `Team_diagram.png`, so keep the Markdown file and image in the same folder for the relative link to work.

