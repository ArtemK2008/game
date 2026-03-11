# Saving / Persistence / Offline Progress

## Purpose
Define the save, persistence, and offline-progress model for the first playable version and establish the structural rules that later run/session, world, progression, towns, automation, UX, and technical specs must follow.

---

## Scope
This spec defines:
- what game state must persist
- what does not need to persist between runs or sessions
- safe save points
- save behavior expectations
- offline progress role and limits
- relationship between persistence and the core loop
- minimal MVP save/persistence requirements
- constraints for later specs

This spec does not define:
- exact save-file format
- exact database/storage technology
- exact anti-cheat strategy
- exact cloud-sync implementation
- exact platform SDK integration
- exact UI art/layout for save/offline screens

---

## Core statement
The game must preserve meaningful player progress across sessions without requiring long uninterrupted play.

Saving and persistence must:
- protect the core push/farm/progression loop,
- make short sessions viable,
- keep post-run state safe and reliable,
- preserve persistent growth cleanly,
- support low-friction return to play,
- allow offline/AFK progress later without undermining active progression.

Saving is not only a technical feature.
It is part of the game’s trust model.

---

## Core persistence rule
Default persistence flow:

`player resolves gameplay state -> important persistent changes are committed -> player can leave safely -> game later restores persistent world/account state -> player resumes from a clear world-level context`

The system must prioritize persistence of resolved progress over persistence of unresolved temporary activity.

---

## Persistence design principles
- Resolved progress must be durable.
- Temporary run state must remain distinct from persistent progression state.
- The player should have frequent safe stopping points.
- The first version should prefer simple and reliable persistence boundaries.
- Offline progress must reinforce the loop, not replace it.
- Save behavior must be predictable enough for players and developers.
- The architecture must support later expansion without redefining core save boundaries.

---

## Persistence categories
The system must distinguish clearly between persistent and non-persistent state.

### 1. Persistent account state
Must survive across sessions.

Examples:
- unlocked nodes/regions
- persistent currencies/resources/materials
- account-wide upgrades
- character state
- gear ownership/equipment state
- progression system state
- town/service unlock state
- settings and preferences if relevant

### 2. Persistent world state
Must survive across sessions where relevant.

Examples:
- node clear/progress state
- world map reachability state
- boss gate cleared state
- available route changes
- town/service availability state

### 3. Temporary run state
May reset when a run ends and does not need to survive ordinary exit unless a later mid-run suspend feature is added.

Examples:
- current in-run level-up picks
- temporary run modifiers
- active combat-state variables
- current transient enemy/combat-space state

### 4. Session-only convenience state
May persist optionally but is not required to define core progress.

Examples:
- last selected node
- recent farming node
- recent push target
- current viewed menu/tab

The implementation must not blur these categories.

---

## Save boundary rule
The default save boundary is the resolved post-run world-level state.

Meaning:
- after a run resolves and its results are applied,
- the game should be able to save a stable persistent state,
- the player should be able to exit safely,
- on return, the game restores a clear post-run/world-level context.

This is the main safe-save model for MVP.

---

## Safe save points
The system must provide predictable safe save points.

### Required safe save point
- after a run resolves and all persistent changes are applied

### Strongly recommended additional safe save points
- after a progression/town/service transaction is confirmed
- after a node/world unlock state changes
- after a settings/profile change if settings are persistent

### Optional later safe save points
- after entering a stable town/service context
- on explicit user-triggered save if ever exposed
- mid-run suspend state if later supported

MVP does not require full mid-run save.

---

## Save timing expectations
The save model should favor frequent commits of important persistent changes.

Required principles:
- saving should occur automatically at safe state transitions
- resolved rewards and progression changes should not remain uncommitted for long
- loss of already-resolved progress should be minimized
- the player should not need to manually remember to save

The default expectation is autosave, not manual save management.

---

## Relationship between save model and run/session structure
Save behavior must support the run/session structure.

Required behavior:
- player can stop safely after resolved runs
- short sessions remain meaningful because progress is persisted cleanly
- long sessions can be interrupted at natural points without punishing loss
- unresolved temporary run state does not need to be the primary persistence unit in MVP

Saving should reinforce the post-run decision state as a core structural anchor.

---

## Relationship between save model and world structure
The save system must preserve the world as a persistent progression space.

Required behavior:
- node states persist
- world reachability persists
- region progression persists
- backtracking availability persists according to world/progression rules

The world must not feel ephemeral between sessions.

---

## Relationship between save model and progression
Persistent progression changes must be durable.

Required behavior:
- resources/currencies/materials gained from resolved states persist
- progression purchases/upgrades persist
- character/build/gear changes persist when confirmed
- cleared maps and unlocks persist

The save model must make long-term growth trustworthy.

---

## Relationship between save model and towns/service systems
Towns and service layers often change persistent state.

Required behavior:
- town/service transactions should commit cleanly
- partial or ambiguous application of upgrade/conversion actions should be avoided
- the player should not lose confirmed town/progression actions after exit/crash if a safe save point was reached

This is especially important for low-friction progression spending.

---

## Relationship between save model and automation
Automation-related persistent changes may need to persist.

Examples:
- automation unlocks
- auto-pick preference if implemented persistently
- saved farming preferences later
- convenience presets later

Required principle:
- automation convenience settings may persist, but temporary run automation state does not need to persist mid-run in MVP

---

## Exit/resume model
The game should assume the player can leave after a stable state and come back later.

Required resume behavior:
- restore persistent world/account state
- restore player to a clear world-level or service-level context
- make next available actions understandable immediately

MVP does not need to resume into an unresolved combat moment.
It needs to resume into a safe decision state.

---

## Offline progress definition
Offline progress means progress or rewards granted for elapsed real-world time while the player is not actively playing, based on persistent systems and prior state.

Offline progress is optional in MVP but the architecture should remain compatible with it.

Offline progress is not the same thing as:
- active manual play replacement
- unresolved run continuation by default
- instant free progression with no relation to prior systems

---

## Offline progress design principles
If offline progress exists, it must:
- reinforce the game’s farming/automation identity,
- remain subordinate to active progression and deliberate build choices,
- depend on persistent state or qualified farming context,
- be readable and claimable,
- not trivialize the core world progression loop.

Offline progress should feel like support value, not the only smart way to play.

---

## Offline progress source model
Offline progress should come from already-established systems.

Preferred source relationships:
- mastered or farm-ready content
- automation-friendly systems
- persistent farming setups
- progression systems that explicitly support offline gain

Disallowed baseline pattern:
- arbitrary passive gain with no relationship to the world, farming, or persistent setup

Offline value should be tied to player-established capability.

---

## Offline progress output categories
Offline progress may later grant one or more of:
- soft currency
- materials/resources
- limited progression materials
- farming summary rewards
- automation-related efficiency value later

### Recommended restriction
Offline progress should not be the default source of major first-clear progression or boss/gate advancement in MVP.

Offline progress should mainly support farming/value accumulation, not replace active push progression.

---

## Offline progress claim model
If offline progress exists, it should be surfaced through a clear claim/review step.

Preferred behavior:
- game detects elapsed time and relevant offline source state
- computes offline summary rewards
- presents readable summary to player
- grants or confirms grant into persistent inventory/state
- returns player to ordinary world/session flow

This keeps offline value readable and debuggable.

---

## Offline progress limits
Offline progress must have structural limits.

Recommended limits:
- tied to eligible content/state only
- weaker or less flexible than strong active farming
- not the primary source of major unlocks
- bounded by time, eligibility, or efficiency rules

The goal is to support comfort, not to bypass the core loop.

---

## MVP interpretation rule
The first playable version may choose one of these approaches.

### Option A: no real offline progress yet
- persistent save/resume exists
- no elapsed-time farming rewards yet
- architecture remains compatible with offline progress later

### Option B: minimal offline progress
- limited passive gain from clearly eligible farming state
- simple summary on return
- no major push/unlock progress granted offline

Both are valid MVP directions.
Option A is safer if implementation simplicity is the priority.

---

## Persistence consistency rule
Persistent changes should be committed atomically enough to avoid ambiguous half-applied state.

Required principle:
- reward grant, progression update, and unlock update caused by one resolved gameplay event should be committed as one coherent state change as much as practical

The exact technical mechanism is implementation-defined, but the design must assume consistency matters.

---

## Failure/crash tolerance principles
The save model should minimize harmful state loss.

Required behavior:
- already-resolved rewards should not be easy to lose
- already-cleared progression should not revert unexpectedly
- the player should usually restart from the last stable committed state

MVP does not require perfect crash recovery at arbitrary points, but must protect ordinary resolved progress well.

---

## Settings/preferences persistence
Non-gameplay settings may also persist.

Examples:
- sound/music settings
- control preferences if relevant
- automation preferences later
- UI preferences later

These settings are secondary to gameplay persistence, but should still follow predictable save behavior.

---

## MVP requirements
The first playable version must support:
- autosave at stable resolved progression points
- persistence of world state
- persistence of progression/resources/currencies
- persistence of character/build/gear state
- safe exit after resolved runs
- clean resume into world-level or service-level context
- architecture compatible with later offline progress

MVP may omit:
- true offline farming rewards
- mid-run suspend/resume
- multiple save slots if not needed
- cloud sync
- detailed save-history/versioning UI

---

## MVP priorities
Focus on:
- reliable autosave after resolved runs
- clear distinction between persistent and temporary state
- safe resume into understandable context
- minimal risk of losing resolved progress
- future-ready compatibility with offline rewards later

Avoid in MVP:
- heavy manual save management
- mid-run persistence complexity before the post-run model is proven
- offline progression that competes with active push gameplay
- many save modes with unclear behavior
- hidden persistence rules the player cannot trust

---

## Data model requirements
Minimum required persistence-related data categories:
- world progression state
- node state data
- persistent inventory/resources/currencies
- character state
- build/loadout/equipment state
- progression/powerup/town system state
- last safe context for resume
- settings/preferences if persisted

Optional later data:
- offline progress eligibility state
- offline progress timestamp references
- recent farming target
- automation presets
- suspend-run snapshot data
- multiple profile/save-slot metadata

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make ordinary progress depend on long uninterrupted sessions.
- Do not make resolved rewards/progression easy to lose.
- Do not blur temporary run state and persistent state without clear reason.
- Do not require mid-run save in order for the game to feel fair in MVP.
- Do not make offline progress the primary source of major push advancement.
- Do not create systems whose persistence rules are unclear to the player.
- Do not make towns/progression purchases ambiguous in save behavior.
- Do not force manual saving as the normal way to protect progress.

---

## Extension points
The persistence model must support later addition of:
- offline farming rewards
- offline progress claim summaries
- automation-linked passive gain
- quick resume helpers
- mid-run suspend/resume
- cloud sync
- multiple save slots/profiles
- more detailed save-state metadata

These additions must extend the stable post-run persistence foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact save-file schema
- exact serialization format
- exact storage backend
- exact anti-tamper measures
- exact cloud provider integration
- exact offline reward formula

---

## Validation checklist
- [ ] World progression persists across sessions.
- [ ] Resources/currencies/materials persist across sessions.
- [ ] Character/build/gear state persists across sessions.
- [ ] Resolved runs can be exited safely after their results are applied.
- [ ] The game resumes into a clear world/service context.
- [ ] Temporary run state is kept distinct from persistent state.
- [ ] The architecture can later support offline progress without redesign.
- [ ] Offline progress, if added later, can remain subordinate to active push p