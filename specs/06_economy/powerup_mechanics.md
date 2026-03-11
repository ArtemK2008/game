# Powerup Mechanics

## Purpose
Define the persistent power-conversion systems that turn resources, loot, and progression outputs into stronger future runs, and establish the structural rules that later towns, progression, crafting, gear, resources, automation, and UX specs must follow.

---

## Scope
This spec defines:
- what powerup mechanics are in this project
- the role of non-combat strengthening systems
- structural categories such as crafting, mining, alchemy, construction, and related upgrade systems
- relationship between powerup mechanics and the core loop
- minimal MVP powerup-mechanics requirements
- constraints for later specs

This spec does not define:
- exact recipes
- exact building lists
- exact upgrade values
- exact UI layout
- exact timing numbers
- exact save serialization
- exact town screen structure

---

## Core statement
Powerup mechanics are persistent systems that convert gameplay outputs into stronger future performance.

They must:
- make farming and loot accumulation meaningful,
- provide goals beyond just unlocking the next node,
- create multiple ways to become stronger over time,
- reinforce the push/farm-upgrade-repeat loop,
- remain readable enough for an autobattle-first game.

Powerup mechanics are not separate games.
They are long-term strengthening layers attached to the main progression loop.

---

## Definition of a powerup mechanic
A powerup mechanic is a persistent, repeat-usable system that consumes player-earned resources and produces one or more forms of lasting value.

A powerup mechanic may produce:
- stat growth
- build access
- gear improvement
- efficiency improvement
- automation improvement
- unlock access to other systems
- economic conversion improvement

A powerup mechanic is not the same thing as:
- a run-only level-up choice
- a direct node unlock condition only
- a temporary buff with no persistence
- a cosmetic system

---

## Core powerup rule
Default flow:

`runs generate resources/loot -> player invests them into a powerup mechanic -> persistent state improves -> future runs become stronger, safer, faster, or more efficient`

This is one of the main bridges between grinding and long-term satisfaction.

---

## Design principles
- Every powerup mechanic must have a clear long-term purpose.
- Powerup mechanics must strengthen the core loop, not distract from it.
- They must convert repeated farming into persistent value.
- They must be readable enough to plan around.
- The first version should use a small number of strong mechanics.
- Different mechanics should have different strengthening roles.
- They should support both pushing and farming improvements.
- They should remain extensible for future systems.

---

## System role in the game
Powerup mechanics exist to provide more than one path to becoming stronger.

They should help express:
- broader account growth
- reasons to revisit older content
- reasons to collect region-specific materials
- different long-term goals beyond immediate node unlocks
- progression layers that make the world feel richer over time

If these systems are weak or absent, the game risks feeling too flat and too dependent on only direct node unlocks.

---

## Powerup mechanic categories
The system must support a small set of structural categories.

### 1. Crafting-like mechanics
Purpose:
- convert materials into usable power or utility
- create specific outputs from gathered resources

Possible outputs:
- gear
- gear upgrades
- consumable-like persistent components later
- stat-affecting build parts

Design role:
- turn region/material identity into build value
- provide deterministic progress from farming

### 2. Mining / gathering-linked mechanics
Purpose:
- create a progression layer around resource extraction or access to resource types

Possible outputs:
- raw materials
- material unlock tiers
- improved farming efficiency for certain resources later

Design role:
- reinforce old-region usefulness
- create long-term gathering goals

Important note:
- mining/gathering does not need to be a separate active minigame
- it may exist as a world/resource progression layer tied to nodes and rewards

### 3. Alchemy / conversion mechanics
Purpose:
- transform lower-tier or broad resources into more focused value
- support build refinement and resource smoothing

Possible outputs:
- refined materials
- upgrade catalysts
- stat-enhancing substances later
- conversion of common resources into specialized progression inputs

Design role:
- reduce economic dead ends
- make repeated loot more useful
- create strategic conversion choices

### 4. Construction / infrastructure mechanics
Purpose:
- create persistent structural improvements to the account/world support layer

Possible outputs:
- new system access
- stronger persistent modifiers
- better farming efficiency
- better automation convenience
- stronger town/service functionality later

Design role:
- make progression feel like building something larger than one character or one item
- provide long-term expensive goals

### 5. General project / upgrade mechanics
Purpose:
- provide broad power sinks that do not need to belong to one narrow fantasy like alchemy or crafting

Possible outputs:
- account-wide upgrades
- push/farm efficiency upgrades
- unlocks of new utility layers
- structural improvements to progression comfort

Design role:
- flexible upgrade layer for MVP and future systems

---

## MVP interpretation rule
The first playable version does not need all categories as fully separate systems.

MVP may implement powerup mechanics through:
- one general persistent upgrade/project system
- one simple crafting or conversion layer
- one material-driven upgrade path

The architecture must still allow later splitting into:
- crafting
- alchemy
- construction
- region/material specialization
- automation-focused power layers

---

## Relationship to the core gameplay loop
Powerup mechanics must reinforce the main loop:

`run -> gain rewards -> invest in powerup mechanic -> become stronger or more efficient -> run again`

They should not introduce separate mandatory gameplay loops that compete with the main node/world loop.

Required behavior:
- ordinary farming should feed powerup mechanics
- powerup mechanics should improve future combat/progression
- investment decisions should create visible medium-term goals

---

## Relationship to progression structure
Powerup mechanics are persistent progression sinks.

Required behavior:
- they consume resources/currencies/materials
- they produce lasting improvement
- they may unlock systems, but should not replace basic world unlock flow
- they should help convert grind into account growth

Powerup mechanics are one of the main long-term progression layers beyond direct node clears.

---

## Relationship to resources and currencies
Powerup mechanics must have clear input requirements.

Required behavior:
- each mechanic must consume one or more meaningful resource/currency categories
- source-to-sink relationships must stay understandable
- region/material identity should remain useful through these mechanics
- repeated loot/material gain should become meaningful through powerup sinks

Powerup mechanics are one of the main justifications for a richer economy.

---

## Relationship to loot
Loot must be usable by powerup mechanics.

Required behavior:
- material/resource loot should feed one or more powerup systems
- milestone loot may unlock stronger powerup opportunities later
- repeated loot gain should remain useful because it can be converted into growth

Powerup mechanics help prevent repeated loot from becoming dead value.

---

## Relationship to towns and service layers
Powerup mechanics may later be presented through towns, forges, workshops, labs, construction screens, or similar service structures.

Required principle:
- the structural mechanic must exist independently of its presentation layer
- towns/services are often access points to these mechanics, not the mechanics themselves

This separation allows implementation flexibility.

---

## Relationship to characters, gear, and builds
Powerup mechanics may affect:
- character growth
- gear creation or improvement
- skill access or refinement
- build efficiency
- specialization tendencies later

Required principle:
- different powerup systems should improve different parts of the account/build stack
- they should not all collapse into one generic “+power” button with no identity

---

## Relationship to automation
Powerup mechanics may improve automation indirectly or directly.

Possible outputs later:
- better auto-farm efficiency
- stronger automation reliability
- access to convenience systems
- lower-friction repeated farming

Required principle:
- automation-related powerups should reduce friction without erasing the push/farm distinction too early

---

## Push vs farm relationship
Powerup mechanics should support both push and farm goals.

### Push-oriented outputs
Examples:
- stronger survivability
- stronger build power
- unlocks that help deeper content
- better boss/gate readiness

### Farm-oriented outputs
Examples:
- better reward efficiency
- faster or safer repeat clears
- automation convenience
- better resource conversion

A good powerup system should make the player choose what kind of strength they want next.

---

## Mechanic output categories
Powerup mechanics may produce one or more of these output types.

### 1. Direct power
Examples:
- offensive stat increase
- defensive stat increase
- skill improvement

### 2. Access
Examples:
- unlock new mechanic
- unlock new gear category later
- unlock new conversion path

### 3. Efficiency
Examples:
- improved farming return
- reduced cost
- better automation comfort
- faster progression conversion

### 4. Flexibility
Examples:
- more build options
- more loadout choices
- more ways to transform resources into useful outputs

The system should remain open to all 4 output types.

---

## Mechanic identity rule
Each implemented powerup mechanic should answer:
- what does it consume?
- what kind of strength does it create?
- why is it separate from other upgrade systems?

If those answers are unclear, the mechanic should probably be merged into a simpler system.

---

## Complexity control rule
Do not split powerup mechanics too early.

### MVP rule
Use a small number of mechanics with broad clear roles.

Good MVP examples:
- one project/upgrade board
- one simple forge/crafting path
- one refinement/conversion path

Bad MVP examples:
- separate deep mining, smithing, chemistry, engineering, and construction trees with overlapping purposes

The first version must prove usefulness before it proves quantity.

---

## Upgrade / project model
At least one powerup mechanic should support project-style progression.

Project-style properties:
- consumes persistent resources
- gives permanent outcome
- may be expensive enough to create medium-term goals
- clearly improves future play

This model is highly compatible with the game’s grind-heavy identity.

---

## Gathering-linked model
If gathering/mining-style mechanics exist, they should usually be tied to the world loop rather than isolated minigames.

Preferred behavior:
- resources come from nodes/regions/runs
- gathering progression improves access to or value of those resources
- the mechanic reinforces the world map and farming loop

Avoid making gathering a detached manual chore in the core design.

---

## Conversion/alchemy model
If conversion systems exist, they should reduce dead resources and smooth progression.

Preferred behavior:
- common resources can become more useful through conversion
- conversion choices create planning value
- the system prevents inventory bloat from becoming meaningless

Avoid making conversion chains so deep that the game becomes inventory management first.

---

## Construction/infrastructure model
If construction-like systems exist, they should represent long-term account growth.

Preferred behavior:
- expensive but clear persistent upgrades
- stronger progression comfort over time
- system-level unlocks or improvements

Construction should feel like building support structure around the core loop, not replacing it.

---

## MVP requirements
The first playable version must support:
- at least one persistent powerup mechanic beyond direct node unlocks
- resource-to-power conversion through that mechanic
- visible long-term benefit from investing in the mechanic
- compatibility with repeated farming as an input source
- compatibility with later splitting into more thematic mechanics

MVP may omit:
- separate mining screen/system
- separate alchemy screen/system
- separate construction grid/system
- many recipe categories
- deep processing chains

---

## MVP priorities
Focus on:
- one or two clear persistent strengthening systems
- visible benefit from investment
- strong link between farming and long-term goals
- readable source-to-sink relationships
- future-ready support for more thematic systems later

Avoid in MVP:
- many fragmented mechanics
- large recipe trees
- deep production chains
- mechanics that require large UI overhead before their value is proven
- manual busywork that distracts from the world/progression loop

---

## Data model requirements
Minimum required powerup-related data:
- mechanic id/type
- input requirements (resource/material/currency)
- output definition
- unlock state if relevant
- persistent progress/completion state if relevant
- relation to affected systems (stats, gear, automation, etc.)

Optional later data:
- recipe definitions
- building definitions
- upgrade tiers
- production chains
- conversion rates
- region affinity tags
- automation-related mechanic metadata

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not add many overlapping powerup systems before one useful system is proven.
- Do not make powerup mechanics detached from the main world/run loop.
- Do not make resources accumulate with no meaningful conversion path.
- Do not make every powerup mechanic only produce generic stat increases.
- Do not make construction/crafting/alchemy systems mandatory busywork for ordinary progression.
- Do not require manual micro-management to realize basic value from these systems.
- Do not let thematic labels replace clear mechanical roles.

---

## Extension points
The powerup-mechanics layer must support later addition of:
- dedicated crafting system
- dedicated alchemy/refinement system
- dedicated construction/infrastructure system
- mining/gathering specialization
- recipe discovery
- automation-focused upgrades
- project chains
- region-specific processing systems
- gear upgrade/enhancement crafting

These additions must extend the persistent resource-to-power foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact recipes
- exact buildings
- exact upgrade names
- exact costs
- exact processing times
- exact UI screens

---

## Validation checklist
- [ ] The game has at least one persistent powerup mechanic beyond direct node unlocks.
- [ ] Resources can be converted into lasting strength or efficiency.
- [ ] The system reinforces the main run/farm/progression loop.
- [ ] The mechanic has clear inputs and outputs.
- [ ] The mechanic creates medium-term or long-term goals.
- [ ] The architecture can later split into craft