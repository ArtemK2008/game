# Bosses

## Purpose
Define the boss system for the first playable version and establish the structural rules that later combat, enemies, progression, rewards, world, automation, and UX specs must follow.

---

## Scope
This spec defines:
- what a boss is in this project
- the role of bosses in the core loop
- how bosses differ from ordinary enemies
- relationship between bosses and progression/rewards/world structure
- minimal MVP boss requirements
- constraints for later specs

This spec does not define:
- exact boss roster
- exact boss names/themes
- exact stat values
- exact attack scripts
- exact reward values
- exact UI presentation
- exact save serialization

---

## Core statement
Bosses are high-importance hostile encounters that serve as major progression checkpoints, stronger build tests, and concentrated reward moments.

Bosses must:
- feel meaningfully different from ordinary enemies,
- reinforce world and route progression,
- remain compatible with autobattle,
- create stronger push pressure than normal combat,
- justify their importance through progression and/or reward outcomes.

Bosses are not just larger ordinary enemies.
They are milestone-level combat entities or encounters.

---

## Definition of a boss
A boss is a high-importance hostile combat entity or tightly scoped boss encounter that acts as a stronger progression gate or milestone than ordinary node combat.

A boss must have:
- high progression importance
- stronger combat significance than ordinary enemies
- explicit relation to node/route/world progression
- explicit defeat outcome
- explicit reward/progression meaning

A boss is not the same thing as:
- an elite enemy only
- a standard enemy with slightly higher stats only
- a pure cutscene gate
- a service/progression node without combat

---

## Core boss rule
Default boss flow:

`boss encounter becomes available -> player enters boss/gate content -> autobattle-compatible boss combat resolves -> boss defeat or failure produces major progression/reward result`

Bosses must reinforce the push side of the push/farm-upgrade-repeat loop.

---

## Boss design principles
- Bosses must have clear progression purpose.
- Bosses must feel more important than ordinary enemies.
- Bosses must remain readable under autobattle.
- Bosses must test build quality more strongly than ordinary content.
- Bosses must not require manual execution skill to be fair in the core design.
- The first version should keep boss structure simple and strong.
- Boss rewards/progression outputs must justify encounter importance.
- Bosses must not become the only meaningful progression source.

---

## Boss system role in the game
Bosses are one of the main ways the game expresses:
- region or route checkpoints
- stronger build tests
- progression milestones
- concentrated reward spikes
- transition between local progression stages

Bosses should create memorable pressure without turning the game into a manual action game.

---

## Boss structural categories
The system must support at least one boss category in MVP and remain extensible for more.

### 1. Gate boss
Primary role:
- block or gate major forward progression
- mark an important region or route step

Expected outcomes:
- unlock next route segment
- unlock next region or major node path
- grant stronger milestone reward than ordinary nodes

This is the most important boss category for the game’s structure.

### 2. Optional challenge boss
Primary role:
- provide higher-risk optional challenge
- grant special or concentrated reward

This is future-compatible and not required for MVP.

### 3. Farmable boss or repeatable milestone boss
Primary role:
- remain replayable for rewards or efficiency goals after first clear

This is also future-compatible and may be introduced later.

---

## Boss versus enemy distinction
Bosses must differ from ordinary enemies through one or more meaningful dimensions.

Allowed distinction dimensions:
- much higher progression importance
- stronger durability/threat profile
- more complex combat pattern
- more concentrated encounter focus
- stronger reward or unlock output
- unique or rarer encounter context

Disallowed baseline pattern:
- calling a standard enemy a boss with no meaningful mechanical or progression difference

---

## Relationship between bosses and world structure
Bosses must be compatible with world/node progression.

Required behavior:
- bosses should usually appear as special node roles or special route checkpoints
- bosses should help define region or route structure
- boss defeat should affect reachable world space or progression state in a visible way

Bosses should reinforce the feeling of moving deeper into the world.

---

## Relationship between bosses and progression
Bosses are milestone progression encounters.

Required behavior:
- boss defeat should produce a major progression result
- boss attempt/failure may still produce some value if aligned with broader run rules
- bosses should create a stronger build-readiness check than ordinary nodes
- bosses should not replace ordinary kill-based progression as the backbone of the game

Bosses are major gates, not the only gate system.

---

## Relationship between bosses and rewards
Bosses should provide stronger or more concentrated reward meaning than ordinary enemies/nodes.

Possible reward roles:
- milestone progression currency/material
- stronger first-clear reward
- unlock-linked reward bundle
- concentrated resource output
- unique reward category later

### MVP rule
Even if reward variety is simple, boss defeat must feel more valuable than an ordinary combat node clear.

---

## Relationship between bosses and combat
Bosses use the same combat foundation as the rest of the game, but with higher significance.

Required behavior:
- bosses must be valid hostile combat participants or encounters
- bosses must function under autobattle-compatible combat rules
- bosses must be able to defeat or pressure underprepared builds
- stronger builds should perform visibly better against bosses

Boss combat should feel like a stronger build check, not a different genre.

---

## Relationship between bosses and automation
Bosses must remain fair and readable under automation.

Required behavior:
- boss difficulty should come from stats, pattern structure, pressure, and progression context
- bosses must not depend on precise manual dodging or manual button timing for core fairness
- player influence should still come mainly from build choice, upgrades, progression, and route readiness

Bosses may require more attention in observing/upgrading around the run, but not direct manual execution.

---

## Relationship between bosses and builds
Bosses must make build quality matter.

Required behavior:
- poor builds should struggle more noticeably against bosses
- strong builds should gain visible reliability/speed advantages
- boss encounters should help reveal whether a build is ready for deeper progression
- different build priorities may later perform differently against different bosses

Bosses are one of the strongest tests of build identity in the game.

---

## Relationship between bosses and push/farm rhythm
Bosses primarily belong to push progression, not routine farming.

### Push role
- stronger progression checkpoint
- stronger build validation
- higher-value milestone

### Farm role later
- some bosses may become replayable for rewards
- repeated boss farming may exist later for specific goals

### MVP rule
Bosses do not need to be major farm content in the first version.
Their main role is progression pressure and milestone payoff.

---

## Boss encounter model
A boss encounter may be represented as:
- one boss entity
- one boss plus limited support entities
- one tightly scoped encounter with boss-centered combat

### MVP rule
The first playable version should prefer:
- one clear boss entity or one simple boss encounter model

Avoid requiring highly complex multi-phase encounter architecture in MVP.

---

## Boss complexity principles
Bosses should be more complex than ordinary enemies, but complexity must stay readable.

Allowed forms of added complexity:
- higher durability and pressure
- more dangerous behavior profile
- stronger attack pattern
- limited phase or threshold behavior later
- support enemy interaction later

Disallowed MVP baseline:
- highly complex script logic that obscures why the encounter succeeds or fails
- complexity that relies on manual execution skill rather than build readiness

---

## Boss state model
Minimum boss-related state should represent:
- active/alive state
- defeat state
- combat stats or stat reference
- boss role/tag
- encounter context reference
- progression result definition
- reward result definition

Optional later state:
- phase state
- first-clear flag
- replay reward state
- optional challenge modifiers
- support-entity relations

---

## Boss defeat model
When a boss is defeated:
- the encounter resolves successfully
- boss-related rewards are granted
- boss-related progression results are applied
- route/world/node state may change accordingly

Boss defeat must always feel structurally meaningful.
It should never be only cosmetic.

---

## Boss failure model
When the player fails a boss encounter:
- the run resolves as failed or partially successful according to run rules
- the boss gate remains uncleared
- some ordinary partial-value rules may still apply if allowed by the broader run design

### Required principle
Boss failure should create a clear signal that more build/progression/farming is needed.
It should not feel random or mechanically unfair.

---

## MVP requirements
The first playable version must support:
- at least one boss or gate-boss encounter
- boss participation through normal combat foundations
- boss defeat causing a major progression result
- boss defeat causing meaningful reward output
- boss encounter difficulty clearly above ordinary enemy content
- boss design that remains readable under autobattle

MVP may omit:
- multiple bosses
- optional challenge bosses
- replay-focused boss farming
- multi-phase boss logic
- support-add-heavy boss encounters
- unique boss-only currencies

---

## MVP priorities
Focus on:
- one clear gate-boss role
- visible difference from ordinary enemies
- strong progression meaning on defeat
- strong build-test value
- readable autobattle-compatible design
- simple but memorable milestone function

Avoid in MVP:
- many bosses before one works well
- high script complexity
- manual-dodge-dependent mechanics
- reward structures that are too complex before baseline value is proven
- boss encounters that duplicate ordinary nodes with only inflated stats and no milestone meaning

---

## Data model requirements
Minimum required boss-related data:
- boss id/type
- stat reference or values
- boss role/tag
- encounter node/context reference
- reward output reference
- progression output reference
- defeat/failure resolution hooks

Optional later data:
- phase data
- first-clear reward state
- replay behavior flags
- optional challenge tags
- region-gate tags
- support-entity composition data

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make bosses rely on manual execution skill for core fairness.
- Do not make bosses meaningless stat inflations of ordinary enemies.
- Do not make boss defeat produce trivial progression value.
- Do not make bosses the only meaningful progression source.
- Do not make boss design unreadable under autobattle.
- Do not overcomplicate boss encounters before the baseline boss role is proven.
- Do not disconnect bosses from world/route progression structure.

---

## Extension points
The boss system must support later addition of:
- multiple boss families
- optional challenge bosses
- replayable/farmable bosses
- multi-phase boss logic
- support-add boss encounters
- unique boss rewards
- region-ending bosses
- branch-specific bosses

These additions must extend the milestone boss foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact boss list
- exact boss names/themes
- exact stat numbers
- exact skill scripts
- exact reward tables
- exact boss UI widgets
- exact VFX/cutscene presentation

---

## Validation checklist
- [ ] The game can support at least one boss encounter.
- [ ] A boss is structurally distinct from an ordinary enemy.
- [ ] Boss defeat produces a major progression result.
- [ ] Boss defeat produces meaningful reward output.
- [ ] Bosses use the normal combat foundation.
- [ ] Boss difficulty is readable under autobattle.
- [ ] Bosses act as stronger build checks than ordinary content.
- [ ] The system can later expand to multiple bosses, phases, and optional challen