# Game Modes

## Purpose
Define the playable mode categories of the game, their structural roles, and the rules that later node, progression, automation, reward, and UX specs must follow.

---

## Scope
This spec defines:
- what a game mode is in this project
- which mode categories exist in MVP
- the role of each mode category
- relationship between modes and nodes
- relationship between modes and progression
- minimal MVP mode requirements
- constraints for later specs

This spec does not define:
- exact combat formulas
- exact AI rules
- exact reward values
- exact node graph layout
- exact town mechanics
- exact UI layout
- exact save behavior

---

## Core statement
The game is built around a small set of mode categories that serve different progression functions.

Modes exist to structure player intent and content purpose, not to fragment the game into unrelated subsystems.

All modes must reinforce the same core loop:
`choose destination -> resolve content -> gain progress/rewards -> improve -> choose next destination`

A mode is valid only if it strengthens the push/farm-upgrade-repeat structure.

---

## Definition of a mode
A game mode is a repeatable gameplay context with:
- a defined purpose
- a defined input/output relationship
- a defined relationship to progression
- a defined place in the run/session structure

A mode is not the same thing as:
- a biome
- a region
- a specific node
- a visual theme
- a single event variant

A mode describes the type of gameplay function being executed.

---

## Mode hierarchy
Modes should be interpreted at these levels:

1. **Primary mode category**
   - high-level gameplay function
   - example: progression combat, farming, service interaction

2. **Node role implementation**
   - how that mode appears on the world map
   - example: combat node, elite node, town node

3. **Local variation**
   - subtype, modifiers, region flavor, or special rules

The first playable version should keep this hierarchy simple.

---

## Required mode categories
The design must support these categories.

### 1. Progression combat mode
Primary role:
- advance node progress
- unlock forward route content
- test current build strength

Core properties:
- usually centered on combat
- uses kill-driven progress as default backbone
- is the main push mode
- may still grant farm value

This is the core playable mode of the game.

### 2. Farming mode
Primary role:
- generate stable resources
- reinforce persistent progression
- convert cleared content into repeatable value

Core properties:
- lower risk than forward push
- higher consistency
- better suited to automation
- usually reuses cleared or well-known nodes

This is not a separate disconnected game.
It is a lower-friction usage state of world content.

### 3. Service / progression mode
Primary role:
- convert rewards into upgrades, route decisions, or utility actions
- provide non-combat progression moments

Core properties:
- lower or zero combat focus
- typically short interaction flow
- exists between or around runs
- may be represented by special nodes

This category includes town/service/progression interactions at a structural level.

---

## Optional early mode categories
These are allowed in MVP if scope supports them, but are not required as fully distinct systems.

### 4. Elite challenge mode
Primary role:
- provide higher-risk combat for better rewards or faster progression

### 5. Boss / gate mode
Primary role:
- serve as an important progression checkpoint or region gate

### 6. Loot / reward node mode
Primary role:
- resolve into reward-focused interaction with low combat emphasis

### 7. Special event mode
Primary role:
- provide uncommon variation without replacing the main loop

These may exist as distinct node roles before they become large independent systems.

---

## MVP mode model
The first playable version should treat modes as a small, readable set.

### Required MVP mode set
- progression combat mode
- farming mode
- service/progression mode

### MVP interpretation rule
In MVP, farming mode may be implemented as repeated use of cleared combat nodes rather than a separate node family.

In MVP, service/progression mode may be implemented through a simple post-run or special-node interaction layer.

This keeps the mode structure small while preserving future extensibility.

---

## Relationship between modes and nodes
Modes are not detached from the world map.

Required relationship:
- nodes express mode categories through node role
- the world map remains the main entry point into modes
- modes should feel like different uses of the same progression world, not separate games selected from a menu

Examples:
- combat node -> progression combat mode
- cleared combat node replay -> farming mode
- town/progression node -> service/progression mode
- boss/gate node -> boss/gate mode

---

## Relationship between modes and progression
Each mode category must have a clear progression role.

### Progression combat mode contributes mainly to:
- forward unlocks
- node clear states
- route advancement
- exposure of build weaknesses

### Farming mode contributes mainly to:
- resource generation
- account growth
- efficiency improvement
- automation suitability
- preparation for later push attempts

### Service / progression mode contributes mainly to:
- converting rewards into long-term growth
- enabling route or build decisions
- reducing friction between runs

A mode should never exist only for content variety if it has no progression role.

---

## Relationship between modes and run/session structure
Each mode must fit into the run/session structure already defined.

Minimum compatibility rules:
- progression combat mode uses full run flow
- farming mode uses repeatable low-friction run flow
- service/progression mode returns player to a world-level decision state after interaction

All modes must preserve:
- meaningful post-resolution state
- ability to continue or stop cleanly
- readable relation to the current session goal

---

## Relationship between modes and automation
Modes differ in automation suitability.

### Progression combat mode
- may require more attention on new content
- should still keep combat primarily automatic
- may rely more on manual decision-making during runs

### Farming mode
- should be the most automation-friendly mode category
- should support low-attention repetition
- should provide predictable value

### Service / progression mode
- may involve no autobattle at all
- should remain low-friction and short

The mode system must allow automation level to vary by mode without changing the core game identity.

---

## Mode transition rules
The player must be able to transition between mode categories naturally.

Expected common transitions:
- progression combat -> farming
- farming -> service/progression
- service/progression -> progression combat
- farming -> back to current push target

Mode transitions should happen through normal world/session flow, not through isolated modal design that breaks continuity.

---

## Mode design rules
- Keep the number of primary mode categories small.
- Each mode must have a clear reason to exist.
- Modes should differ mainly by progression function, not only presentation.
- Modes should be compatible with the same persistent growth structure.
- Modes should reinforce the same world identity.
- Modes should not split the player into unrelated gameplay loops too early.

---

## MVP requirements
The first playable version must support:
- one primary combat mode used for progression
- one farming usage pattern for previously cleared content
- one service/progression interaction layer
- mode transitions through normal world/session flow
- the ability to understand what the current run or interaction is for

If scope is limited, farming and progression combat may share the same node type, provided their gameplay role still differs by context.

---

## MVP priorities
Focus on:
- small number of mode categories
- clear role for each mode
- strong connection between modes and progression
- strong connection between modes and world nodes
- low friction when switching between push, farm, and upgrade actions

Avoid in MVP:
- too many primary modes
- mode categories that exist only for novelty
- side activities that compete with the main progression loop
- detached menu-selected minigame-like systems
- over-specialized mode rules before the core loop is proven

---

## Data model requirements
Minimum required mode-related data:
- node role/type
- mode category or resolvable mode mapping
- mode-specific entry behavior
- mode-specific result behavior
- relation to progression outputs

Exact schema belongs to implementation.

Mode identity may be stored directly or derived from node type and state.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not create many unrelated primary modes.
- Do not make modes feel like separate games with separate identities.
- Do not create a mode that has no clear progression role.
- Do not detach most content from the world-map progression structure.
- Do not make farming a meaningless filler state.
- Do not make service/progression interactions so heavy that they disrupt session flow.
- Do not make new modes bypass the core push/farm-upgrade-repeat loop.

---

## Extension points
The mode system must support later addition of:
- elite challenge mode
- boss/gate mode
- loot node mode
- special event mode
- offline/AFK farming layer
- temporary challenge modes
- region-specific node-role variants

These additions must extend the small core mode set, not replace it.

---

## Out of scope
This spec does not define:
- exact node contents
- exact combat pacing
- exact town feature list
- exact reward scaling by mode
- exact UI grouping of modes
- exact save/load behavior per mode

---

## Validation checklist
- [ ] There is a defined primary combat mode for progression.
- [ ] There is a defined farming mode or farming usage state.
- [ ] There is a defined service/progression interaction mode.
- [ ] Each mode has a clear progression role.
- [ ] Modes are tied to world-map node structure rather than detached systems.
- [ ] The player can transition between push, farm, and upgrade behavior without breaking session flow.
- [ ] The mode system can later expand without re