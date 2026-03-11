# Combat Rules

## Purpose
Define the structural rules of combat for the first playable version and establish the combat contract that later stats, skills, enemies, bosses, gear, automation, loot, and UX specs must follow.

---

## Scope
This spec defines:
- what combat is in this project
- core combat participants
- combat state flow
- engagement rules
- damage/defeat outcome rules at a structural level
- relationship between combat and node progression
- minimal MVP combat requirements
- constraints for later specs

This spec does not define:
- exact stat formulas
- exact damage formulas
- exact scaling numbers
- exact enemy roster
- exact boss mechanics
- exact skill list
- exact gear stat values
- exact UI layout

---

## Core statement
Combat is an automated encounter loop in which the player-controlled combat entity and hostile entities interact over time until the run reaches a resolved state.

Combat exists to:
- generate enemy kills,
- test build effectiveness,
- produce rewards,
- produce node/map progression,
- create the push/farm rhythm of the game.

Combat is not built around manual execution skill.
It is built around automated action resolution shaped by player planning.

---

## Core combat rule
Default combat rule:

`combat starts -> entities engage automatically -> attacks/actions resolve over time -> enemies are defeated and/or player side fails/succeeds -> run results are applied`

Combat should be readable, low-friction, and compatible with repeated farming.

---

## Combat participants
Combat must support at least these participant categories.

### 1. Player side
The player side is the entity or entities representing the player’s build during a run.

Minimum MVP expectation:
- one primary player-controlled combat entity under automation

Future-compatible possibilities:
- summons
- companions
- secondary attack entities
- multi-character or roster-linked support effects

### 2. Enemy side
The enemy side is the set of hostile entities spawned by the node’s combat content.

Minimum MVP expectation:
- regular hostile enemies

Future-compatible possibilities:
- elites
- bosses
- support enemies
- hazard-like hostile entities

### 3. Environment / combat context
Combat occurs inside a node-specific combat space.

Minimum MVP expectation:
- combat occurs in a bounded or otherwise valid encounter space
- encounter context can spawn enemies and resolve combat outcomes

Environment behavior should not require manual navigation skill in the core loop.

---

## Combat entity requirements
Every combat entity must be representable with at least:
- allegiance/faction (player-side or hostile)
- alive/active state
- targetability state if applicable
- one or more action capabilities
- combat-relevant stats or stat references
- position/context data if movement exists

Exact stat schema belongs to later specs.

---

## Combat state flow
Combat must follow a consistent high-level state sequence.

### 1. Initialization
- combat space is prepared
- player combat entity is created/loaded
- node combat context is loaded
- enemy spawning rules are initialized
- temporary run combat state is reset

### 2. Active combat
- enemies appear according to node rules
- automation controls player-side combat behavior
- hostile entities act according to their behavior rules
- attacks/actions resolve continuously or in repeated steps
- kills, damage, and temporary combat events occur

### 3. Resolution check
The system repeatedly checks whether the run end condition has been met.

### 4. Combat resolution
- victory / successful resolve / failure / other valid resolved state is determined
- reward and progression outputs are computed
- combat temporary state is finalized

This flow must remain stable even when later combat complexity increases.

---

## Engagement model
Combat is based on repeated automated engagements between valid participants.

Minimum engagement expectations:
- enemies become available targets
- player-side automation can engage them
- hostile entities can threaten or damage the player side
- engagements produce visible progress toward combat resolution

Combat must not stall frequently due to unclear engagement logic.

---

## Action model
Combat entities act through actions.

Minimum action categories:
- basic offensive action
- movement or positional action if movement exists
- optional special action hooks for later skills/systems

### MVP rule
The first playable version only requires enough action variety to prove:
- entities can attack,
- enemies can be defeated,
- the player side can fail,
- builds can differ in effectiveness.

### Future-compatible rule
Later systems may add:
- active/passive skill actions
- triggered effects
- AoE actions
- projectile rules
- summon actions
- support actions
- status application actions

These additions must extend, not replace, the base action model.

---

## Targeting rule
Combat must use target selection.

Minimum rule:
- offensive actions require a valid target or valid target area/context
- automation must be able to identify valid hostile targets
- hostile entities must be able to identify valid player-side targets when appropriate

Targeting details belong to `automation_ai_behavior` and later combat implementation, but combat rules must assume targeting is always part of action resolution.

---

## Damage and survival rule
Combat must support the possibility of:
- hostile entities taking damage and being defeated
- player-side entity taking damage and failing the run if defeat conditions are met

Minimum MVP rule:
- damage exchange must exist
- defeat conditions must exist
- combat outcomes must be influenced by build strength and enemy difficulty

Exact formulas belong to `stats_and_formulas`.

---

## Defeat and removal rule
When a combat entity is defeated:
- it is no longer an active combat participant unless a later revive/respawn rule applies
- it stops contributing to combat as an active entity
- if hostile, it may contribute to rewards, kill counts, and node progress
- if player-side defeat satisfies run-failure conditions, combat resolves accordingly

Exact corpse/cleanup/respawn presentation is out of scope here.

---

## Enemy kill rule
Enemy defeats are a core output of combat.

Required effects of enemy kills:
- contribute to node/map progression according to progression rules
- contribute to rewards according to reward rules
- contribute to combat-state advancement if the node depends on kill flow

The combat system must treat enemy kills as one of the main meaningful outputs of a run.

---

## Combat and run progression relationship
Combat is the main mechanism through which a run produces progression value.

Required combat outputs:
- enemy kills
- run rewards
- progress toward map clear
- evidence of build performance
- possible failure/success state for the run

Combat should therefore produce meaningful output even when the map is not fully cleared in one run.

---

## Combat pacing rule
Combat must support repeated runs and long-term farming.

Required pacing properties:
- combat should become readable quickly after start
- combat should generate visible progress over time
- combat should not require constant manual intervention
- combat should remain meaningful during repeated farming

Exact pacing numbers belong to later specs.

---

## Push vs farm combat expectations
Combat should feel different in push and farm contexts.

### Push combat expectations
- lower stability
- higher risk of failure or slowdown
- stronger test of build strength
- more valuable for unlock progress

### Farm combat expectations
- higher consistency
- lower attention requirement
- better suitability for automation
- reliable reward generation

These differences should come from content difficulty, build readiness, and progression state, not from a completely different combat system.

---

## Relationship between combat and automation
Combat is primarily automation-driven.

Required relationship:
- movement, targeting, and attack execution are largely handled by automation
- the player’s main influence is pre-run setup and in-run upgrade decisions where applicable
- combat must remain functional without direct manual execution

Combat rules must therefore remain compatible with AI-driven control at all times.

---

## Relationship between combat and builds
Combat must express build differences.

Required behavior:
- stronger or better-suited builds should perform better
- weaker or mismatched builds should perform worse
- progression and setup choices should materially affect combat outcomes
- combat should reflect planning quality, not manual dexterity

If combat outcomes barely change across builds, the combat system is failing its role.

---

## Relationship between combat and node roles
Combat rules must be compatible with multiple node roles.

### Combat nodes
- full standard combat flow required

### Farming on cleared combat nodes
- same combat foundation, lower practical difficulty expectation

### Elite / boss / gate nodes
- same combat foundation, with harder or more specialized content

### Non-combat/service nodes
- may bypass active combat entirely

The combat system is the default run interaction model, not the required model for every node role.

---

## Combat resolution states
Minimum valid resolved combat states:
- successful resolve
- failed resolve
- partial-value resolve if the run ends without full success but still grants rewards/progress

This is required because the game loop depends on incomplete runs still often producing value.

Exact naming can vary in implementation.

---

## MVP requirements
The first playable version must support:
- one player-side combat entity
- hostile enemies that can spawn and be defeated
- automated targeting
- automated attacks
- automated movement if movement exists in MVP
- damage exchange between player side and enemy side
- defeat conditions for enemies and player side
- enemy kills contributing to rewards and map progress
- combat resolving into a valid run result

MVP may omit:
- bosses
- elites
- advanced skill triggers
- complex status systems
- multi-entity player parties
- advanced hazard systems

---

## MVP priorities
Focus on:
- stable automated combat loop
- readable kill generation
- clear relationship between build strength and combat outcome
- clear success/failure resolution
- repeated farming viability
- partial-value results on unsuccessful runs when appropriate

Avoid in MVP:
- high combat complexity before baseline readability is proven
- too many entity types
- too many action types
- too many special-case combat rules
- mechanics that require direct action skill to function correctly

---

## Data model requirements
Minimum combat-related data categories:
- run combat context
- player combat entity state
- enemy entity state
- target relationships or target selection context
- active combat-state flags
- kill/death events
- combat resolution result

Optional later data:
- status effect state
- summon state
- projectile state
- area effect state
- threat/aggro state
- combat metrics for summaries

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not require direct manual execution for core combat success.
- Do not remove enemy kills as a major output of combat.
- Do not make combat outcomes mostly independent of build quality.
- Do not make ordinary combat rely on rare scripted one-off rules.
- Do not make failure states so punitive that partial progression becomes rare by default.
- Do not make combat unreadable under automation.
- Do not create combat systems that break repeated farming viability.
- Do not make node progression structurally disconnected from combat output.

---

## Extension points
The combat system must support later addition of:
- elites
- bosses
- skills and triggered effects
- area attacks
- projectiles
- summons/companions
- status effects
- advanced automation heuristics
- encounter-specific combat modifiers

These additions must extend the core automated engagement model, not replace it.

---

## Out of scope
This spec does not define:
- exact DPS formulas
- exact stat list
- exact movement/pathing algorithm
- exact enemy behaviors
- exact boss rules
- exact skill library
- exact loot formulas
- exact combat HUD

---

## Validation checklist
- [ ] Combat can initialize from a node run.
- [ ] Player-side and hostile entities can engage automatically.
- [ ] Attacks/actions can resolve over time.
- [ ] Hostile enemies can be defeated.
- [ ] Player-side failure is possible.
- [ ] Enemy kills contribute to rewards and node progress.
- [ ] Combat produces a resolved run result.
- [ ] Combat outcomes meaningfully reflect build quality.
- [ ] Combat remains compatible with repeated farming and automation.
- [ ] The system can later support elites, bosses, skills, and statuses without redesign.

