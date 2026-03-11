# Enemies

## Purpose
Define the enemy system for the first playable version and establish the structural rules that later combat, stats, bosses, loot, world, progression, and UX specs must follow.

---

## Scope
This spec defines:
- what an enemy is in this project
- enemy roles in the core loop
- enemy categories at a structural level
- relationship between enemies and combat/progression/rewards
- minimal MVP enemy requirements
- constraints for later specs

This spec does not define:
- exact enemy roster
- exact enemy names/themes
- exact stat values
- exact AI implementation details
- exact drop tables
- exact regional enemy assignments
- exact UI presentation

---

## Core statement
Enemies are the hostile combat participants that drive node progression, test build effectiveness, and generate rewards.

Enemies must:
- provide the main kill output of runs,
- create visible differences between push and farm content,
- remain readable under autobattle,
- support repeated farming without becoming meaningless,
- scale with world progression without making old content irrelevant.

Enemies are one of the main ways the game expresses difficulty and progression pressure.

---

## Definition of an enemy
An enemy is a hostile combat entity that appears within node combat content and can threaten the player-side combat entity.

An enemy must have:
- hostile allegiance
- combat-relevant stats
- targetability/interaction state if applicable
- behavior rules
- defeat state
- relationship to reward and progression outputs

An enemy is not the same thing as:
- a boss gate encounter definition as a whole
- an environmental modifier only
- a node progression rule
- a temporary hazard with no combat-entity behavior unless later explicitly modeled that way

---

## Core enemy rule
Default enemy flow:

`enemy spawns or becomes active -> enemy participates in combat -> enemy can threaten player side -> enemy can be defeated -> enemy defeat contributes to rewards and node progress`

This flow is the default meaning of an ordinary enemy in the game.

---

## Enemy design principles
- Enemies must have a clear combat purpose.
- Enemy behavior must remain readable under automation.
- Enemies must create meaningful combat differences across content.
- The first version should use a small readable set of enemy roles.
- Enemies must support both push and farm contexts.
- Enemy difficulty must come from understandable factors.
- Enemy defeat must matter because kills are core progression output.
- Enemy variety should come from role differences, not only visuals.

---

## Enemy system role in the game
Enemies are one of the main drivers of:
- kill-based node progression
- combat pacing
- build validation
- risk level
- farm efficiency
- reward generation

If enemy design is weak, then:
- push/farm distinction becomes weak,
- build differences become harder to read,
- node progression becomes less satisfying.

---

## Required enemy categories
The first version should support a small set of structural enemy categories.

### 1. Standard enemy
Purpose:
- baseline hostile unit
- primary source of kill count
- main input into ordinary map/node progression

Expected properties:
- relatively common
- readable behavior
- lower individual complexity than later elite/boss content

This category is required in MVP.

### 2. Durable or dangerous variant
Purpose:
- create meaningful variation inside ordinary combat content
- increase pressure beyond only adding more standard enemies

Possible structural differences:
- higher survivability
- higher damage threat
- higher movement pressure
- more disruptive combat pattern

This can be implemented in MVP as a second enemy profile or may be simplified if scope is tight.

### 3. Future elite/boss-compatible category
The enemy system must remain extensible to support elites and bosses later.

MVP does not need full elite/boss implementation in this spec, but ordinary enemy design must not block later enemy hierarchy expansion.

---

## Enemy role dimensions
Enemies may differ along one or more role dimensions.

### 1. Durability profile
Examples:
- fragile
- balanced
- tanky

### 2. Threat profile
Examples:
- low sustained threat
- high burst threat
- pressure through persistence

### 3. Mobility/approach profile
Examples:
- slow pressure
- fast engage
- area control tendency later

### 4. Combat purpose
Examples:
- filler kill target
- pressure source
- build check
- farming speed limiter

The first version does not need many dimensions, but enemies must differ through at least one meaningful gameplay axis.

---

## Relationship between enemies and combat
Enemies are core combat participants.

Required behavior:
- enemies must be valid hostile targets
- enemies must be able to threaten or damage the player side
- enemies must be defeatable through normal combat rules
- enemy defeat must produce meaningful combat output

Enemy design must remain compatible with automated targeting, movement, and attack execution.

---

## Relationship between enemies and progression
Enemy defeats are one of the main progression outputs of a run.

Required behavior:
- defeating enemies increases node/map progress according to progression rules
- harder content may use stronger or more punishing enemies
- enemy composition should help create visible world progression differences

Enemies are not only obstacles.
They are also the main producers of kill-driven advancement.

---

## Relationship between enemies and rewards
Enemies are an economic source.

Required behavior:
- enemy defeats may grant or contribute to rewards
- enemy presence must therefore matter economically
- more difficult or rarer enemy types may later be associated with better reward intensity

MVP may keep reward differentiation simple, but enemy kills must still matter for run value.

---

## Relationship between enemies and node roles
Enemy expectations vary by node role.

### Standard combat nodes
- enemies should form the baseline repeatable combat population
- kill flow should feel stable and readable

### Farming on cleared nodes
- enemy composition should support repeated efficient kills
- content should become more manageable as progression improves

### Harder push nodes
- enemies may be denser, tougher, or more threatening
- build weaknesses should become more visible

### Elite / boss / gate nodes later
- enemy system must support stronger hierarchy and more specialized threats

The same enemy foundation should support all of these contexts.

---

## Relationship between enemies and world structure
Enemies should support world progression identity.

Required behavior:
- different nodes/regions may later emphasize different enemy profiles
- enemy differences should help regions feel mechanically distinct over time
- older region enemies should remain useful for farming and progression support even if they are less threatening later

MVP does not need deep regional enemy ecology, but the system must allow it later.

---

## Relationship between enemies and automation
Enemies must be designed for autobattle readability.

Required behavior:
- enemies should be targetable and understandable under AI-driven combat
- enemy difficulty should emerge from stats, behavior, density, and context rather than hidden manual-reaction requirements
- enemies should not require precise player steering to interact correctly in the core loop

The enemy system must support the game’s lazy-combat identity.

---

## Relationship between enemies and builds
Enemies must create reasons for builds to matter.

Required behavior:
- some enemy profiles should reward offensive efficiency
- some enemy profiles should reward survivability or stability
- enemy pressure should reveal when a build is underprepared
- stronger builds should visibly handle ordinary enemies better

If all enemies are effectively identical in practice, build differentiation becomes weaker.

---

## Push vs farm relationship
Enemies should feel different in push and farm contexts without requiring a completely different enemy system.

### Push expectations
- enemies are more likely to expose weaknesses
- kills may be slower
- incoming pressure may be more dangerous
- build mismatch is more visible

### Farm expectations
- enemies become more manageable
- kill flow becomes smoother
- automation becomes more stable
- content remains rewarding but less threatening

This distinction should come from progression state, scaling, and composition, not from replacing the core enemy model.

---

## Enemy scaling principles
Enemy scaling must support long-term progression.

Required principles:
- later content can field stronger enemies
- earlier enemies should become easier over time
- scaling must preserve farm usefulness of older content
- scaling should not make every new node feel impossible by default
- scaling should remain readable enough for balancing and debugging

Exact scaling formulas belong to `stats_and_formulas` and balancing work.

---

## Enemy state model
Minimum enemy state should be able to represent:
- alive/active state
- defeated/removed state
- targetable state if applicable
- combat stats or stat reference
- behavior state or behavior controller reference
- reward/progression contribution on defeat

Optional later state:
- elite flag
- boss flag
- special behavior phase
- summon relationship
- status-effect state
- loot modifier state

---

## Enemy spawn/context model
Enemies appear through node combat context.

Minimum rule:
- node/run context determines what enemies can appear
- enemies must become active in a way compatible with automated combat flow
- enemy presence must contribute to the run’s progress and pacing

Exact spawn logic belongs to implementation and later combat content specs.

---

## Enemy defeat model
When an enemy is defeated:
- it stops being an active hostile participant
- it contributes to kill-related outputs
- it may contribute to rewards
- it may contribute to node/map progress

Enemy defeat must therefore always be structurally meaningful.

---

## MVP requirements
The first playable version must support:
- at least one standard enemy type/profile
- hostile combat behavior against the player side
- enemy defeat through normal combat rules
- enemy defeat contributing to node progress
- enemy defeat contributing to run rewards or reward generation
- at least some visible enemy variation by stats, threat, or durability

MVP may omit:
- elites
- bosses
- complex multi-phase behaviors
- advanced support/summoner enemy roles
- large enemy roster
- region-specific enemy ecosystems

---

## MVP priorities
Focus on:
- one readable baseline enemy role
- stable kill flow
- clear hostile pressure
- visible difference between weaker and stronger enemy profiles if more than one exists
- compatibility with autobattle readability
- support for both push and farm contexts

Avoid in MVP:
- many enemy types before baseline combat is proven
- high AI complexity before basic role clarity is proven
- enemy gimmicks that require manual action skill
- large visual/theme taxonomies without mechanical distinction
- enemy complexity that weakens repeated farming readability

---

## Data model requirements
Minimum required enemy-related data:
- enemy id/type
- stat reference or values
- behavior/controller reference
- allegiance/targeting category
- defeat output definition or reward/progression references
- node/context source rules

Optional later data:
- elite/boss flags
- rarity tags
- region tags
- loot tables
- phase data
- summon/support role tags
- special mechanic tags

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make ordinary enemies mechanically unreadable under autobattle.
- Do not make enemy kills economically or progression-wise unimportant.
- Do not require manual reaction skill to handle standard enemies in the core loop.
- Do not make all enemies functionally identical in practice.
- Do not overcomplicate ordinary enemies before baseline combat is proven.
- Do not make scaling so extreme that old-content farming becomes pointless.
- Do not make enemy identity purely cosmetic.

---

## Extension points
The enemy system must support later addition of:
- elites
- bosses
- support/summoner enemies
- hazard-like hostile entities
- region-specific enemy families
- phase-based enemy behavior
- special drop rules
- context-specific enemy modifiers

These additions must extend the readable hostile-entity foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact enemy list
- exact enemy names/themes
- exact stat numbers
- exact AI trees/state machines
- exact loot rates
- exact regional distribution tables
- exact presentation/VFX rules

---

## Validation checklist
- [ ] The game has at least one defined standard enemy profile.
- [ ] Enemies act as hostile combat participants.
- [ ] Enemies can be defeated through normal combat rules.
- [ ] Enemy defeats contribute to node progress.
- [ ] Enemy defeats contribute to rewards or reward generation.
- [ ] Enemy variation can affect build/combat outcomes in a visible way.
- [ ] Enemy design remains readable under autobattle.
- [ ] The system can later support elites, bosses, and