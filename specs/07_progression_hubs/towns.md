# Towns

## Purpose
Define the town system for the first playable version and establish the structural rules that later progression, powerup mechanics, resources, gear, loot, UX, menu, and save specs must follow.

---

## Scope
This spec defines:
- what a town is in this project
- the role of towns in the core loop
- town function categories
- relationship between towns and world structure/progression/powerup systems
- minimal MVP town requirements
- constraints for later specs

This spec does not define:
- exact town visual theme
- exact building list
- exact shop inventories
- exact recipe lists
- exact UI layouts
- exact save serialization
- exact NPC/lore content

---

## Core statement
Towns are persistent non-combat service hubs that let the player convert resources, manage progression, prepare builds, and reduce friction between runs.

Towns must:
- reinforce the push/farm-upgrade-repeat loop,
- provide access points to persistent systems,
- create natural session pauses and planning moments,
- remain low-friction and readable,
- avoid becoming a detached minigame layer.

Towns are service and progression interfaces, not separate gameplay pillars that compete with the world-node loop.

---

## Definition of a town
A town is a persistent service-oriented world location or service-layer context that provides access to one or more non-combat progression functions.

A town may provide:
- upgrade access
- crafting/conversion access
- gear/build management
- progression spending
- unlock/service utility

A town is not the same thing as:
- a combat node
- a boss gate
- a single reward popup
- a temporary event room only
- a pure cosmetic/social space with no gameplay role

---

## Core town rule
Default town flow:

`player reaches or enters town/service context -> player spends resources / changes build / accesses persistent systems -> persistent state improves or changes -> player returns to world-level progression`

Towns must shorten the path between reward acquisition and meaningful use of those rewards.

---

## Town design principles
- Towns must have clear mechanical purpose.
- Town interactions must remain short and readable.
- Towns must improve the quality of the progression loop, not interrupt it excessively.
- Towns must centralize or expose persistent systems cleanly.
- The first version should keep town structure simple.
- Towns should support both push preparation and farm conversion.
- Towns should create natural stopping points without requiring long management sessions.
- Town systems must be extensible without requiring a major redesign.

---

## Town system role in the game
Towns are one of the main places where the player:
- converts rewards into persistent progress,
- manages build state,
- chooses what kind of strength to pursue next,
- accesses non-combat progression systems,
- pauses between push and farm phases.

Without towns or equivalent service contexts, the game risks making progression feel too abstract or too hidden.

---

## Town functional categories
The system must support a small set of structural town functions.

### 1. Progression hub function
Purpose:
- provide access to permanent upgrade systems
- expose one or more persistent sinks

Examples structurally:
- account upgrade board
- project board
- persistent powerup access

This function is highly important for MVP.

### 2. Build management function
Purpose:
- let the player prepare or revise future runs

Examples structurally:
- character selection later
- gear assignment
- loadout adjustment
- skill/build preparation

This function may begin simple but is structurally required.

### 3. Resource conversion function
Purpose:
- transform accumulated rewards into more useful forms

Examples structurally:
- crafting
- refinement/alchemy
- exchange/conversion
- project material spending

This may begin as a simple upgrade/conversion layer in MVP.

### 4. Utility/service function
Purpose:
- provide convenience and support systems

Examples structurally:
- route prep tools later
- automation convenience access later
- reward claim/service layers

This is future-compatible and may remain minimal in MVP.

---

## Town structural models
Towns may be represented in more than one way.

### Model A: explicit town node
A town exists as a node on the world map.

Benefits:
- town becomes part of route structure
- makes service access part of world progression

### Model B: town/service layer accessible from world state
A town exists as a persistent interface layer rather than a required world node.

Benefits:
- simpler MVP flow
- lower route friction
- easier implementation of persistent systems

### MVP interpretation rule
Either model is valid for MVP.
The first version should choose the simpler implementation that still preserves the structural role of towns.

---

## Relationship between towns and world structure
Towns must be compatible with node-based world progression.

Required behavior:
- towns should fit naturally into world progression as nodes or service layers
- towns should not feel detached from the world identity
- towns should support route/session flow rather than replace it

If explicit town nodes are used later, they must behave as meaningful route/service nodes.

---

## Relationship between towns and progression
Towns are major access points to persistent progression systems.

Required behavior:
- towns must expose at least one persistent progression sink
- towns should make it easier to understand what rewards can be used for
- towns should support medium- and long-term goals
- towns should reinforce account growth rather than purely short-term effects

Towns are interfaces to progression, not substitutes for it.

---

## Relationship between towns and powerup mechanics
Towns are likely the main presentation layer for powerup mechanics.

Required principle:
- powerup mechanics may structurally exist independently,
- but towns may be their main access point,
- towns should organize these systems in a readable way

This means towns and powerup mechanics are related but not identical.

---

## Relationship between towns and resources/currencies
Towns are one of the main sink access layers for economic outputs.

Required behavior:
- towns should let the player spend meaningful resources/currencies
- towns should make resource purpose easier to understand
- repeated farming should feel useful because towns provide sinks/conversion options

Towns help turn inventory into action.

---

## Relationship between towns and gear/build systems
Towns should support build planning.

Required behavior:
- towns may expose gear assignment and build preparation
- towns may expose character/build management later
- towns should help the player move from reward gain to next-run preparation

Towns should reduce friction between “I got something useful” and “I will use it next run.”

---

## Relationship between towns and push/farm rhythm
Towns should support both sides of the progression rhythm.

### Push-side town role
Examples:
- prepare for harder content
- spend upgrades before a gate/boss push
- adjust build toward survivability/power

### Farm-side town role
Examples:
- convert repeated farming rewards into progress
- refine materials
- improve efficiency/automation later
- prepare new farming loadout later

Towns should help connect both states, not favor only one.

---

## Relationship between towns and sessions
Towns should act as clean between-run planning spaces.

Required behavior:
- towns should provide natural session stop points
- towns should support short interactions in short sessions
- towns should also support deeper planning in longer sessions
- towns should not require long mandatory management before every run

Towns should improve session flow, not slow it down.

---

## Town complexity rule
Towns must remain structurally compact in MVP.

### MVP rule
Town functionality may be represented through:
- one progression hub screen,
- one simple gear/build management area,
- one conversion/upgrade access point.

Do not require:
- many separate buildings,
- many NPC roles,
- complex town navigation,
- large town-only subloops.

The first version should prove that towns are useful, not that towns are large.

---

## Town output categories
Town interactions may produce one or more of these outputs.

### 1. Direct power
Examples:
- persistent stat increase
- gear improvement
- character improvement

### 2. Access
Examples:
- unlock new upgrade options
- unlock new system layer
- unlock better conversion path later

### 3. Efficiency
Examples:
- better farming comfort
- stronger automation support later
- better resource conversion

### 4. Build revision
Examples:
- new loadout
- changed equipment
- changed character selection later

These are the main reasons towns exist.

---

## Town interaction model
Town interaction should be discrete and low-friction.

Preferred flow:
1. enter town/service context
2. review available systems
3. spend/convert/adjust
4. apply persistent changes
5. return to world/session flow

Town interaction should not require extended navigation overhead to realize basic value.

---

## Town state model
Minimum persistent town-related state may include:
- town/service access unlock state
- available service functions
- persistent progression options unlocked through town systems
- town-linked project/construction state later

MVP may keep town state minimal if towns are mainly a service interface.

---

## MVP requirements
The first playable version must support:
- at least one town-equivalent service context
- at least one persistent progression sink accessible through it
- at least one build/loadout or run-preparation function accessible through it
- clear return from town/service context back to world/session flow
- low-friction use during both short and long sessions

MVP may omit:
- explicit town node on map
- multiple towns
- building-by-building town expansion
- deep construction system
- NPC-specific service differentiation
- decorative social functions

---

## MVP priorities
Focus on:
- one clear service/progression hub
- readable access to important sinks
- support for build preparation
- low interaction friction
- strong connection to rewards/progression systems
- natural session pause/support role

Avoid in MVP:
- many buildings or service tabs with overlapping roles
- town gameplay that competes with the main world loop
- excessive management time between runs
- cosmetic town complexity before functional value is proven
- deep simulation of town life/economy

---

## Data model requirements
Minimum required town-related data:
- town/service context id or equivalent
- available function list
- unlock state for functions if relevant
- relation to progression sinks/conversion systems/build systems

Optional later data:
- building definitions
- service NPC mappings
- town upgrade state
- construction queue/state
- multiple town identities
- region-linked town access

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make towns a detached game separate from the world/run loop.
- Do not require long town management before ordinary runs.
- Do not make town interactions the only meaningful way to progress.
- Do not create many overlapping service layers with unclear roles.
- Do not hide important sinks behind excessive town complexity.
- Do not force explicit town-node routing in MVP if a simpler service-layer model works better.
- Do not make towns mostly cosmetic when they are supposed to be functional hubs.

---

## Extension points
The town system must support later addition of:
- explicit town nodes on the world map
- multiple towns or hub types
- crafting workshop
- alchemy lab
- construction/infrastructure layer
- automation/convenience service access
- region-specific town functions
- town growth/upgrade layers

These additions must extend the service-hub foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact town visual design
- exact building list
- exact NPCs
- exact UI screens
- exact construction recipes
- exact economic values

---

## Validation checklist
- [ ] The game has at least one town-equivalent service context.
- [ ] Towns provide access to at least one persistent progression sink.
- [ ] Towns provide access to at least one build/loadout preparation function.
- [ ] Town interactions return cleanly to world/session flow.
- [ ] Towns support both short-session and long-session use.
- [ ] The system can later expand into explicit town nodes, construction, and specialized services without redesign.

