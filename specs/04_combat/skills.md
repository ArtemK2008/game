# Skills

## Purpose
Define the skill system for the first playable version and establish the rules that later combat, automation, stats, characters, gear, loot, and progression specs must follow.

---

## Scope
This spec defines:
- what a skill is in this project
- skill categories
- how skills are acquired and used at a structural level
- relationship between skills and automated combat
- relationship between skills and builds
- minimal MVP skill requirements
- constraints for later specs

This spec does not define:
- exact skill list
- exact numeric coefficients
- exact animation rules
- exact UI presentation
- exact drop tables
- exact save serialization

---

## Core statement
Skills are one of the main ways a build expresses combat identity.

In this project, skills must:
- work within automated combat,
- create meaningful differences between builds,
- interact with stats and progression,
- support both push and farm contexts,
- remain readable enough to debug and expand.

Skills are not primarily manual action buttons.
They are automated or automation-compatible combat behaviors/effects shaped by player planning.

---

## Definition of a skill
A skill is a reusable combat effect or combat action source attached to the player side or another valid combat entity.

A skill must have:
- a combat purpose
- an ownership/source
- activation behavior
- one or more combat effects
- interaction with stats or scaling rules

A skill is not the same thing as:
- a base stat
- a basic attack definition only
- a permanent upgrade sink
- a node modifier
- a pure UI choice with no combat effect

---

## Core skill rule
Default skill flow:

`build/loadout/progression provides skill access -> combat/run context activates or enables skill usage -> skill resolves through automation-compatible rules -> skill contributes to combat outcome`

Skills must reinforce the automated combat loop, not bypass it.

---

## Skill design principles
- Skills must have a clear combat purpose.
- Skills must be compatible with automation.
- Skills must create meaningful build differences.
- Skills must not require direct manual execution for core progression.
- The first version should use a small readable skill model.
- Skill behavior should be understandable enough to debug.
- Skills should scale through existing stat/formula channels where reasonable.
- Temporary skill gains and permanent skill access must remain separable.

---

## Skill ownership layers
Skills may come from multiple ownership layers.

### 1. Base skill access
Skill access attached to a character, class/archetype, or default combat entity definition.

### 2. Build/loadout skill access
Skill access attached to equipped/selected build components.

### 3. Temporary run skill access/modification
Skill access or enhancement granted only during the current run.

### 4. Contextual skill modification
Skill behavior affected by node/mode/encounter context.

The system must support at least the first two layers in a clear way over time.

---

## Required skill categories
The design must support at least these structural categories.

### 1. Basic combat skill / basic attack pattern
Primary repeated offensive behavior.

Purpose:
- provide consistent default damage output
- ensure the combat entity can always engage in combat

MVP note:
- this may be implemented as the default attack behavior rather than an exposed skill list item
- structurally it should still be compatible with the skill system

### 2. Passive skill
Always-on or conditionally always-active modifier/effect.

Purpose:
- shape build identity
- provide reliable combat value
- work cleanly with automation

Examples at structural level:
- stat bonus with combat meaning
- triggered enhancement modifier
- persistent attack behavior modifier

### 3. Triggered/automatic active skill
An active combat effect that triggers automatically when its activation condition is met.

Purpose:
- add burst, utility, control, or stronger combat identity without manual button use

Examples at structural level:
- periodic attack effect
- on-hit/on-kill effect
- timed burst effect
- conditional area effect

This category is highly compatible with the project’s combat model.

---

## Optional early skill categories
These may be added early if needed, but are not required for the first combat proof.

### 4. Summon skill
Creates or controls a secondary combat entity.

### 5. Mobility/positioning skill
Changes movement or positioning behavior.

### 6. Defensive/reactive skill
Improves survival, mitigation, or emergency response.

### 7. Utility skill
Improves farming efficiency, control, conversion, or non-damage combat value.

These should be introduced only when their design role is clear.

---

## Skill activation model
Skills must activate through automation-compatible logic.

Allowed activation patterns:
- always active (passive)
- periodic / interval-based
- triggered by combat event
- triggered by internal state threshold
- triggered by valid target availability

Disallowed baseline assumption:
- core progression requiring real-time manual button presses

Manual activation may exist later only if it remains optional or secondary.

---

## Skill execution model
A skill must resolve through one or more combat effects.

Possible structural effects:
- direct damage
- multi-hit or repeated damage
- area effect
- stat modification
- defensive effect
- summon creation
- targeting/behavior modification
- resource or reward-related combat effect if later supported

The skill system must remain general enough to support these without rewriting the base model.

---

## Relationship between skills and automation
Skills must be compatible with automated combat.

Required behavior:
- automation can execute skill usage logic where needed
- skill timing/triggering does not depend on manual precision
- skills should reinforce low-friction repeated runs
- stronger/more complex skills should still remain understandable under autobattle

Skills must not undermine the game’s lazy-combat identity.

---

## Relationship between skills and builds
Skills are one of the main levers of build identity.

Required behavior:
- different skills should produce meaningfully different combat patterns or strengths
- skill choice should matter for push vs farm suitability
- skills should interact with loadout/build decisions
- a build should be recognizable in part by its skill package or skill behavior profile

If different skill setups feel too similar, the system is failing its build role.

---

## Relationship between skills and stats/formulas
Skills should use the stat/formula system whenever possible.

Required behavior:
- skill power should scale through defined stat channels and/or clearly defined coefficients
- skills may have their own parameters, but should not bypass the numeric model by default
- offensive skills should reflect offensive stat growth
- defensive skills should reflect survivability-related growth where reasonable

This keeps the system composable and debuggable.

---

## Relationship between skills and temporary run progression
Skills may be modified during a run.

Allowed forms:
- gain a new temporary skill
- enhance an existing skill
- alter activation frequency
- alter damage/area/utility profile
- add triggered effects

Required principle:
- temporary run skill changes should improve current-run performance without replacing the value of long-term skill access/build setup

---

## Relationship between skills and persistent progression
Persistent progression may affect skills through:
- unlocking skill access
- improving skill scaling
- improving skill consistency or usability
- unlocking new skill categories later

Required principle:
- persistent progression should make skill-based builds grow stronger over time without invalidating ordinary build choice too early

---

## Relationship between skills and node roles
Skills must be compatible with different node contexts.

### Push-oriented content
Expected skill value:
- survivability
- stronger targeted damage
- burst or consistency under pressure
- utility that helps difficult content

### Farm-oriented content
Expected skill value:
- stable clear speed
- low-friction repeated damage
- efficient area coverage
- reliable automation performance

### Special/elite/boss contexts later
Expected skill value:
- build specialization
- context-specific strengths
- clearer payoff from deliberate skill setup

The same skill system should support all of these without splitting into unrelated sub-systems.

---

## Relationship between skills and characters
The skill system must be compatible with characters, but must not over-commit to one ownership model too early.

Allowed future models:
- character defines core skill package
- character influences skill access pool
- character modifies skill scaling or behavior
- skills mainly come from build/loadout and characters only bias them

MVP does not need to finalize the long-term character-skill ownership model, but the skill system must not block it.

---

## Relationship between skills and loot/gear
Skills may be modified by loot/gear.

Allowed relationships:
- grant skill access
- modify skill stats
- modify skill trigger conditions
- add secondary effects
- improve specific skill archetypes

Required principle:
- item/gear interaction with skills should be explicit enough to support build planning

---

## Skill slot / access model
The first playable version should keep skill access simple.

Possible MVP-compatible models:
- one basic attack pattern + one or more passive/triggered skill slots
- small loadout of predefined skill entries
- character-based default skill set with limited modification

MVP does not require a large slot system.
It requires only enough structure to prove build differentiation and automated combat variety.

---

## Skill progression model
Skills may improve through:
- permanent unlock
- permanent strengthening
- loadout choice
- temporary run upgrades

Required principle:
- skill progression must create visible combat change
- skill progression must not be purely numeric if it makes all builds feel identical

At least some skill progression should affect combat pattern, not only numbers.

---

## MVP requirements
The first playable version must support:
- a basic attack or equivalent baseline combat action
- at least one passive or auto-triggered skill layer beyond the baseline attack
- skill effects that influence combat outcomes visibly
- skills functioning under full autobattle
- skill interaction with the stat/formula system
- at least some run-time or build-time skill differentiation

MVP may omit:
- manual active skills
- summon-heavy skill systems
- large skill trees
- complex combo logic
- large slot counts
- advanced cooldown management UI

---

## MVP priorities
Focus on:
- small readable skill set
- automation-compatible activation rules
- visible combat differentiation from skill choices
- clean interaction with stats/formulas
- support for both push and farm contexts
- simple debugging of why a skill build works or fails

Avoid in MVP:
- many niche skill categories
- manual-skill-centric design
- too many special-case skill rules
- complex conditional logic before baseline skills are proven
- many skills that differ only cosmetically

---

## Data model requirements
Minimum required skill-related data:
- skill id
- skill category/type
- owner/source reference
- activation type
- effect definition or effect references
- scaling/stat interaction references
- temporary/permanent ownership flag or equivalent state

Optional later data:
- cooldown data
- trigger condition data
- target rule data
- area shape data
- summon templates
- upgrade tiers
- rarity/source metadata

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not require manual real-time skill activation for core progression.
- Do not make most skills bypass the stat/formula system without clear reason.
- Do not make skill identity so weak that build differences become unclear.
- Do not add many niche skill types before the basic system is proven.
- Do not make skill effects so opaque that autobattle outcomes become hard to debug.
- Do not make skill progression purely cosmetic.
- Do not overload the first implementation with many slots, trees, or combo rules.

---

## Extension points
The skill system must support later addition of:
- cooldown-governed active skills
- summon skills
- defensive/reactive skills
- utility/farming-oriented skills
- more complex trigger conditions
- area/range variation
- skill rarity tiers
- skill upgrade trees
- character-specific skill packages
- gear-driven skill modifications

These additions must extend the automation-compatible core skill model, not replace it.

---

## Out of scope
This spec does not define:
- exact skill names
- exact coefficients
- exact visual effects
- exact cooldown numbers
- exact unlock sources
- exact UI for choosing skills

---

## Validation checklist
- [ ] The game has a defined skill model beyond only raw stats.
- [ ] Skills are compatible with automated combat.
- [ ] A baseline combat action exists.
- [ ] At least one additional passive or auto-triggered skill layer exists.
- [ ] Skill choices can change combat outcomes in a visible way.
- [ ] Skills interact with the stat/formula system.
- [ ] The system can later