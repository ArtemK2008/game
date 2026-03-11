# Characters

## Purpose
Define the character system for the first playable version and establish the structural rules that later skills, gear, progression, combat, automation, and UX specs must follow.

---

## Scope
This spec defines:
- what a character is in this project
- the role of characters in the core loop
- character ownership and progression at a structural level
- relationship between characters and builds
- relationship between characters and automation/combat
- minimal MVP character requirements
- constraints for later specs

This spec does not define:
- exact character roster size
- exact character names or lore
- exact stat tables
- exact skill lists
- exact gear items
- exact UI layout
- exact save serialization

---

## Core statement
Characters are persistent player-owned combat identities that carry build choices, participate in automated combat, and provide long-term progression differentiation.

Characters must:
- support the core push/farm-upgrade-repeat loop,
- create meaningful build variation,
- remain manageable in a lazy/autobattle game,
- interact cleanly with skills, stats, gear, and progression,
- avoid turning the game into a heavy party micromanagement system.

Characters are not the main source of manual gameplay complexity.
They are a major source of persistent identity and build planning.

---

## Definition of a character
A character is a persistent player-owned entity definition that can be used as the main player-side combat identity in runs.

A character must have:
- persistent identity
- base combat profile
- access to skills or skill tendencies
- access to progression and build customization
- relationship to stats and formulas
- relationship to automation behavior during combat

A character is not the same thing as:
- a temporary summon
- a run-only modifier
- a gear item
- a node-specific buff
- a pure cosmetic skin

---

## Core character rule
Default character flow:

`player owns character -> character provides base combat identity -> build/loadout/progression modify character -> character participates in automated combat runs -> run results and progression improve future character effectiveness`

Characters must reinforce the persistent progression identity of the game.

---

## Character design principles
- Characters must have a clear gameplay role.
- Characters must create meaningful build diversity.
- Characters must remain manageable and low-friction.
- Characters must be compatible with autobattle.
- Characters must support both push and farm contexts.
- Character progression must remain distinct from run-only progression.
- The first version should keep the character system structurally simple.
- The system must remain open to later roster expansion.

---

## Character system role in the game
Characters are one of the main long-term anchors of player investment.

They should help express:
- combat style differences
- build identity differences
- progression goals
- reasons to revisit content
- reasons to collect or invest in specific systems later

Characters should not be required to create constant between-run micromanagement overhead.

---

## Character ownership model
Characters are persistent account-owned entities.

Minimum persistent character aspects:
- character identity
- current unlocked state
- persistent stats or stat modifiers
- skill access or skill package reference
- build/loadout reference
- progression state

The implementation must keep character state distinct from temporary run state.

---

## Character usage model
The game must support at least one active character participating in combat runs.

### MVP rule
- one character is selected for a run
- that character acts as the primary player-side combat entity
- the selected character carries the main combat build context for that run

### Future-compatible rule
Later versions may support:
- multiple owned characters
- switching preferred character by node/goal
- roster progression
- support/synergy systems between characters

The first version must not overcommit to a heavy multi-character management model.

---

## Character count assumption
The system should support a small roster over time.

Current design direction:
- small number of meaningful characters
- each character should be distinct enough to justify existence
- roster size should remain manageable

### MVP-compatible interpretation
The first playable version may implement:
- one character only, while keeping the system extensible
or
- a very small roster if it helps prove build variation

The architecture must not assume a large roster from day one.

---

## Character identity dimensions
A character may differ from other characters through one or more of these dimensions.

### 1. Base stat profile
Examples:
- more offense-oriented
- more survivability-oriented
- more speed-oriented

### 2. Skill profile
Examples:
- different default skill package
- different access bias toward skill archetypes
- different triggered combat style

### 3. Build affinity
Examples:
- better fit for certain gear/stat/skill patterns
- different push vs farm tendencies

### 4. Progression path
Examples:
- different scaling tendencies
- different long-term specialization options later

The first version does not need to maximize all dimensions, but characters must differ through at least one meaningful gameplay axis.

---

## Relationship between characters and builds
Characters are part of build identity but do not replace the build system.

Required behavior:
- character choice must matter for how a build performs
- build/loadout choice must still matter within a given character
- characters should bias builds, not eliminate build planning
- the same node may feel different on different characters/builds

A character should not be only a cosmetic wrapper around the same build outcome.

---

## Relationship between characters and stats/formulas
Characters must integrate with the stat system.

Required behavior:
- each character must provide or reference base combat stats
- progression and build systems must be able to modify those stats
- character differences should produce visible combat differences
- character scaling must remain readable enough to debug

Characters are one of the main entry points into the stat/formula layer.

---

## Relationship between characters and skills
Characters must be compatible with the skill system without overcommitting early to one rigid ownership model.

Allowed models:
- character provides default/core skills
- character provides skill bias or affinity
- character unlocks or improves some skill categories
- skills come mostly from loadout while character changes how they scale or behave

### MVP requirement
The first version must support at least one clear connection between character identity and skill behavior.

Examples:
- different default skill package
- different baseline attack pattern
- different stat-to-skill synergy

---

## Relationship between characters and gear
Characters must be compatible with gear without making gear the only thing that matters.

Required behavior:
- gear can modify character performance
- characters may differ in how well they use certain gear types later
- gear should amplify character identity, not erase it

The system must allow character + gear builds, not just raw gear stacking.

---

## Relationship between characters and automation
Characters must function under automated combat.

Required behavior:
- characters can complete runs without manual movement or manual attacks
- character differences should still be visible in autobattle outcomes
- character design must not depend on complex manual execution skill
- automation should express character build identity, not flatten it

Characters must fit the lazy-combat philosophy of the game.

---

## Relationship between characters and progression
Characters are persistent progression targets.

Character progression may later include:
- stat growth
- skill unlocks/improvements
- specialization
- stronger build access
- better farming efficiency
- better push readiness

Required principle:
- character progression must improve long-term effectiveness without replacing account-wide progression systems

Characters are one progression layer, not the only one.

---

## Relationship between characters and world progression
Characters must support world progression rather than compete with it.

Required behavior:
- stronger characters help push deeper nodes
- different characters may later be better for different region/node goals
- old content may remain useful for character progression materials or efficiency gains

Character growth should give more reasons to use the world, not bypass it too early.

---

## Push vs farm relationship
Characters should be able to differ in push and farm suitability.

Possible later roles:
- safer progression character
- faster farming character
- stronger burst/elite character
- more reliable all-rounder

### MVP requirement
Even if only one character exists in MVP, the character system must be compatible with later push/farm role differentiation.

---

## Character progression model
Character progression must be persistent and separable from run-only growth.

Allowed forms:
- leveling or rank-like growth
- stat growth
- skill unlock/improvement
- character-specific upgrade nodes
- gear/loadout expansion

### MVP rule
The first version only needs enough character progression to prove:
- persistent growth exists
- characters can become stronger over time
- character investment affects future runs

---

## Character state model
Minimum character state should be able to represent:
- locked/unlocked ownership state
- active/selectable state
- persistent progression state
- current loadout/build state
- combat-relevant stat reference
- skill access reference

Optional later state:
- specialization path
- affinity tags
- mastery state
- role tags
- farming suitability markers

---

## Character selection model
The player should be able to choose which character is used for a run if more than one exists.

### MVP-compatible rule
If only one character exists in MVP, the system may treat selection as implicit.

### Future-compatible rule
If multiple characters exist later, the character selection step should occur before run start and integrate cleanly with build/loadout selection.

---

## Character unlock model
Characters may be:
- available from the start
- unlocked through progression
- unlocked through progression milestones or systems later

### MVP rule
The first version may start with all implemented characters unlocked or only one character available.
The architecture must still support future unlockable characters.

---

## MVP requirements
The first playable version must support:
- at least one persistent playable character
- character participation as the primary player-side combat entity
- character linkage to stats/formulas
- character linkage to skills or baseline attack behavior
- character-compatible build/loadout data
- persistent character progression or strengthening in some form

MVP may omit:
- roster switching depth
- character-specific talent trees
- many distinct progression paths
- support-character systems
- heavy multi-character micromanagement

---

## MVP priorities
Focus on:
- one clear character data model
- visible character contribution to combat identity
- clean connection to skills, stats, and build setup
- persistent strengthening over time
- future-ready support for a small roster

Avoid in MVP:
- many characters before one is implemented well
- deep roster management overhead
- character systems that duplicate gear/progression systems without adding clear value
- heavy specialization trees before baseline character identity is proven

---

## Data model requirements
Minimum required character-related data:
- character id
- unlock state
- selectable/active state
- base stat reference or values
- skill package or skill reference
- loadout/build reference
- persistent progression state

Optional later data:
- role/archetype tags
- specialization state
- affinity tags
- mastery data
- per-character automation preferences
- region/node performance hints

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make characters purely cosmetic.
- Do not make characters the only meaningful progression layer.
- Do not make character management too heavy for the lazy/autobattle core.
- Do not require manual execution skill to realize character value.
- Do not make all characters numerically identical in practice.
- Do not overcommit the game too early to a large active roster system.
- Do not let gear completely erase character identity.
- Do not tie character usefulness only to late-game systems.

---

## Extension points
The character system must support later addition of:
- small roster expansion
- character unlock conditions
- character specialization paths
- character-specific skill packages
- push vs farm role differentiation
- character-specific progression systems
- roster-wide bonuses or synergies
- character-linked automation presets

These additions must extend the persistent character foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact roster size
- exact character concepts/themes
- exact stat numbers
- exact progression trees
- exact unlock costs
- exact character UI screens

---

## Validation checklist
- [ ] The game has at least one persistent playable character.
- [ ] Character state is distinct from temporary run state.
- [ ] Characters participate in combat as the player-side identity.
- [ ] Characters connect to stats/formulas.
- [ ] Characters connect to skills or baseline attack behavior.
- [ ] Characters can become stronger over time.
- [ ] Character identity affects build/combat behavior in a visible way.
- [ ] The system can later support a small roster without redesign.
