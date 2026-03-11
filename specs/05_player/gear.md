# Gear

## Purpose
Define the gear system for the first playable version and establish the structural rules that later characters, stats, loot, progression, crafting, and UX specs must follow.

---

## Scope
This spec defines:
- what gear is in this project
- the role of gear in build identity
- gear categories at a structural level
- relationship between gear and characters/stats/skills/progression
- minimal MVP gear requirements
- constraints for later specs

This spec does not define:
- exact item list
- exact item names/themes
- exact numeric values
- exact drop rates
- exact crafting recipes
- exact UI layout
- exact save serialization

---

## Core statement
Gear is a persistent build-modification system that changes how a character performs in combat and farming.

Gear must:
- provide meaningful build variation,
- interact cleanly with stats and skills,
- reinforce long-term progression,
- remain readable enough for an autobattle game,
- avoid turning MVP into an overcomplicated loot simulator.

Gear is a major build layer, but it must not replace characters, skills, or persistent progression as independent systems.

---

## Definition of gear
Gear is a persistent player-owned equipment element that can be attached to a character or build and modifies gameplay-relevant behavior.

Gear may provide:
- stat modifiers
- skill interaction modifiers
- build-specific bonuses
- utility/efficiency bonuses later

Gear is not the same thing as:
- a temporary run-only power-up
- a character itself
- a permanent account upgrade sink
- a pure cosmetic item
- a one-time consumable

---

## Core gear rule
Default gear flow:

`player acquires gear -> gear becomes persistent inventory/build option -> player equips/selects gear -> gear modifies character/build performance -> future runs are affected`

Gear must reinforce the core loop by converting rewards and progression into stronger future runs.

---

## Gear design principles
- Gear must have a clear gameplay purpose.
- Gear must create meaningful build differences.
- Gear must be compatible with automated combat.
- Gear must remain readable enough to plan around.
- The first version should keep gear structurally simple.
- Gear should amplify character/build identity, not erase it.
- Gear should support both push and farm contexts.
- Gear progression must remain distinct from temporary run progression.

---

## Gear system role in the game
Gear is one of the main persistent build layers.

Gear should help express:
- offensive vs defensive preference
- farming vs pushing preference
- synergy with skill packages
- long-term improvement from loot/progression
- reasons to revisit content for better equipment later

Gear should not require heavy per-run management in the core loop.

---

## Gear ownership model
Gear is persistent account-owned or character-usable equipment state.

Minimum persistent gear aspects:
- item identity
- availability/ownership state
- equip state or build assignment state
- modifier/effect definition
- relationship to one or more valid users/builds

The implementation must keep gear state distinct from temporary run state.

---

## Gear usage model
Gear affects the selected character/build before a run starts.

Minimum usage rule:
- gear must be selectable/equippable before combat
- equipped/assigned gear must influence future runs until changed

MVP does not require in-run manual gear swapping.

---

## Gear categories
The first version should use a small set of structural gear categories.

### 1. Primary combat gear
Purpose:
- strongly influence offensive behavior or core combat output

Possible examples structurally:
- weapon-like item
- main offense slot
- baseline attack pattern modifier

### 2. Secondary/support gear
Purpose:
- influence survivability, utility, or combat support profile

Possible examples structurally:
- defense/support slot
- accessory-like modifier source
- passive combat enhancer

### 3. General modifier gear
Purpose:
- provide broad stat bonuses or build-shaping effects

Possible examples structurally:
- accessory slot
- charm/relic-like slot
- generic passive bonus item

The exact slot names are not important for the spec.
The structural roles are.

---

## MVP gear model
The first playable version should keep gear simple.

### MVP-compatible models
- one primary gear slot + one secondary modifier slot
- one or a few generic equipment slots
- a very small equipment set that proves build differentiation

### MVP requirement
The system only needs enough gear structure to prove:
- persistent equipment exists
- equipping different gear changes combat outcomes visibly
- gear interacts with character/build identity

MVP does not need a large slot taxonomy.

---

## Relationship between gear and characters
Gear must work with characters without making characters irrelevant.

Required behavior:
- characters can equip/use gear
- gear modifies character performance
- different characters may later benefit differently from some gear
- gear should amplify or bias character identity, not overwrite it completely

A character should not feel identical after gear changes unless the gear itself is intentionally low-impact.

---

## Relationship between gear and stats/formulas
Gear should primarily interact through the stat/formula layer.

Required behavior:
- gear can modify stats directly
- gear may also modify coefficients/effects through clearly defined hooks
- gear impact must be visible in combat outcomes
- gear impact must remain debug-friendly

Default expectation:
- most gear value should be expressible through stat channels and explicit effect channels

---

## Relationship between gear and skills
Gear may influence skills.

Allowed relationships:
- improve skill scaling
- add modifiers to activation or effect behavior
- bias the build toward certain skill packages
- alter baseline attack pattern or triggered effects

Required principle:
- gear/skill synergy should create real build planning value
- gear should not require an excessively complex skill system to matter in MVP

---

## Relationship between gear and progression
Gear is part of persistent progression, but not the only path.

Required behavior:
- acquiring or improving gear should improve future runs
- gear should be one sink/source relationship for rewards/loot later
- gear progression should coexist with account-wide and character-specific progression
- gear should not be the only reason farming matters

Gear is a progression vector, not the whole progression model.

---

## Relationship between gear and loot
Gear must be compatible with a loot-driven acquisition model.

Required behavior:
- gear can be obtained as a reward source or progression output
- gear can later vary by quality, source, or identity
- loot systems can generate gear or gear-improving materials

MVP may keep acquisition simple, but the architecture must not block later loot-driven expansion.

---

## Relationship between gear and automation
Gear must be compatible with autobattle.

Required behavior:
- gear value must appear through automated combat outcomes
- gear should improve stability, efficiency, or power in ways readable under automation
- gear should not depend on manual execution skill to realize its value

Autobattle should make gear differences visible, not flatten them.

---

## Relationship between gear and push/farm roles
Gear should support different strategic goals.

### Push-oriented gear tendencies
Possible emphasis:
- higher survivability
- stronger focused damage
- stronger difficult-content reliability
- better elite/boss readiness later

### Farm-oriented gear tendencies
Possible emphasis:
- higher clear speed
- better area coverage or consistency
- stronger low-friction efficiency
- reward/resource utility later

### MVP requirement
Even if only a small gear set exists, the system must be compatible with later push/farm differentiation.

---

## Gear progression model
Gear may improve through:
- acquiring better gear
- upgrading existing gear
- replacing gear with stronger/synergistic options
- attaching modifiers later

### MVP rule
The first version only needs enough gear progression to prove:
- persistent gear acquisition exists or can exist
- changing gear changes run outcomes
- gear is part of long-term growth

The first version does not need a deep enhancement economy.

---

## Gear quality / rarity model
Optional in MVP.

Future-compatible possibilities:
- rarity tiers
- quality levels
- affix counts
- upgrade tiers
- source-specific item classes

### MVP rule
Rarity/quality may be absent or very simple in the first version.
Do not require a full ARPG rarity system to prove gear value.

---

## Build assignment model
Gear must belong to build planning.

Minimum rule:
- player can define a run with a character + gear combination
- the selected/equipped combination remains stable through the run

Optional later behavior:
- saved loadouts
- per-node loadout presets
- push vs farm build presets

MVP does not require advanced loadout management.

---

## Gear state model
Minimum gear-related persistent state should represent:
- owned/not owned state
- item identity
- assigned/equipped state or build inclusion state
- effect/modifier data or reference
- valid user/build relation if relevant

Optional later state:
- upgrade level
- rarity/tier
- affixes
- favorite/locked state
- source metadata

---

## Acquisition model
Gear may be acquired through:
- node rewards
- loot drops
- milestone rewards
- shops/services later
- crafting/construction/progression systems later

### MVP rule
The first version may use simple deterministic acquisition or a small reward pool.
The gear system must still remain compatible with richer acquisition later.

---

## MVP requirements
The first playable version must support:
- at least one persistent gear category
- gear assignment/equip before runs
- gear impact on combat via stats or explicit effects
- visibly different outcomes from different gear choices
- character-compatible gear usage
- compatibility with future gear acquisition/upgrade systems

MVP may omit:
- many slots
- rarity tiers
- affix-heavy items
- crafting-heavy gear systems
- advanced upgrade trees
- gear set bonuses

---

## MVP priorities
Focus on:
- one clear gear data model
- visible gameplay effect from equipped gear
- simple slot/use structure
- clean integration with characters, skills, and stats
- future-ready compatibility with loot and progression systems

Avoid in MVP:
- too many slots
- many near-duplicate item types
- highly random affix systems before baseline value is proven
- itemization complexity that hides build meaning
- turning gear into the only important progression layer

---

## Data model requirements
Minimum required gear-related data:
- gear id
- gear category/type
- ownership state
- equip/build assignment state
- modifier/effect definition or reference
- valid user/build compatibility rule if needed

Optional later data:
- rarity
- upgrade level
- affixes
- source tags
- set membership
- push/farm usage tags
- item score or shorthand rating if needed

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make gear purely cosmetic.
- Do not make gear the only meaningful build layer.
- Do not make characters irrelevant through gear overdominance.
- Do not require gear complexity beyond what autobattle readability can support.
- Do not require a deep rarity/affix system in order for gear to matter.
- Do not make all gear choices numerically or functionally identical.
- Do not make gear progression depend entirely on late systems before baseline gear exists.

---

## Extension points
The gear system must support later addition of:
- more slots/categories
- rarity/quality tiers
- affix systems
- upgrade/enhancement levels
- gear crafting
- gear conversion/salvage
- set or synergy effects
- push/farm loadout presets
- character-specific gear affinities

These additions must extend the persistent gear foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact item names
- exact stat values
- exact rarity distribution
- exact loot rates
- exact crafting recipes
- exact gear UI screens

---

## Validation checklist
- [ ] The game has a persistent gear system.
- [ ] Gear can be assigned/equipped before runs.
- [ ] Gear affects combat outcomes visibly.
- [ ] Gear integrates with characters.
- [ ] Gear integrates with stats/formulas.
- [ ] Gear can interact with skills in a defined way.
- [ ] Different gear choices can support different build identities.
- [ ] The system can later expand to rarity, upgrades, and crafting without redesign.
