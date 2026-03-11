# Progression

## Purpose
Define the persistent progression model for the first playable version and establish the structural rules that later characters, gear, skills, towns, powerup mechanics, resources, world, and UX specs must follow.

---

## Scope
This spec defines:
- what progression means in this project beyond direct map unlock flow
- persistent progression layers
- what can become stronger over time
- how progression is gained and spent at a structural level
- relationship between progression and the core loop
- minimal MVP progression requirements
- constraints for later specs

This spec does not define:
- exact balance numbers
- exact upgrade trees
- exact costs
- exact UI layout
- exact save serialization
- exact node unlock thresholds

---

## Core statement
Progression is the set of persistent systems that make the account, characters, builds, and support layers stronger over time.

Progression must:
- make repeated play feel meaningful,
- convert farming and milestones into lasting improvement,
- support both push and farm goals,
- preserve the value of world progression without replacing it,
- remain readable enough to plan around in a grind-heavy autobattler.

Progression is broader than unlock flow.
Unlock flow opens content. Progression makes the player better at handling content.

---

## Core progression rule
Default progression flow:

`runs and milestones generate rewards/resources/progression materials -> player invests them into persistent progression layers -> future runs become stronger, safer, faster, or more efficient`

Progression must convert gameplay time into durable capability.

---

## Progression design principles
- Progression must be persistent.
- Progression must have visible gameplay impact.
- Progression must support multiple improvement paths over time.
- Progression must reinforce the main loop rather than distract from it.
- Progression must not depend only on one system.
- The first version should use a small number of strong progression layers.
- Progression should support both push power and farm efficiency.
- Progression must remain extensible without forcing redesign.

---

## Progression system role in the game
Progression is one of the main reasons the game remains satisfying over many sessions.

It should help express:
- stronger combat performance
- stronger farming efficiency
- better automation comfort
- stronger build flexibility
- long-term goals beyond immediate node clearing
- a sense that the account is evolving, not only the current run

If progression is weak, the game risks feeling too temporary or too dependent on only map unlocks.

---

## Progression layers
The game should support multiple persistent progression layers.

### 1. Account-wide progression
Progress affecting the whole account or broad gameplay baseline.

Possible outputs:
- broad stat improvements
- efficiency improvements
- automation improvements
- access to new systems or upgrade tiers

Design role:
- create strong long-term goals
- ensure every session can improve the broader account

This layer is highly important for MVP.

### 2. Character progression
Progress affecting specific playable characters.

Possible outputs:
- stat growth
- skill growth/access
- specialization later
- better push/farm suitability

Design role:
- make characters persistent investment targets
- deepen build identity over time

MVP may keep this simple but must remain compatible with it.

### 3. Build/equipment progression
Progress affecting build components such as gear or build-defining choices.

Possible outputs:
- stronger gear
- better synergies
- access to stronger configurations
- greater flexibility in how a build is assembled

Design role:
- make loot and equipment matter over time
- support distinct build planning

### 4. System/infrastructure progression
Progress affecting support systems around combat.

Possible outputs:
- stronger powerup mechanics
- better conversion systems
- better farming comfort
- stronger service/town utility
- access to advanced progression mechanics later

Design role:
- make the game feel like building a larger machine around combat

---

## Relationship between progression and unlock flow
Progression and unlock flow are related but distinct.

### Unlock flow
Answers:
- what content is reachable now?
- what node/route/region is unlocked?

### Progression
Answers:
- how strong is the account now?
- how strong is the character/build now?
- how efficient is farming now?
- how much support/automation/conversion power does the player have?

Required principle:
- progression should help the player handle future unlocks more easily
- progression should not fully replace the need to engage with world unlock flow too early

---

## Relationship between progression and the core loop
Progression must reinforce:

`run -> gain value -> invest -> become stronger -> run again`

Required behavior:
- ordinary farming contributes to progression
- push milestones contribute to progression
- progression improves future run outcomes
- progression gives medium- and long-term goals between immediate node clears

Progression is one of the main reasons the loop stays meaningful over time.

---

## Relationship between progression and resources/currencies
Progression must consume meaningful economic inputs.

Required behavior:
- progression layers need clear sinks
- resources/currencies/materials must have clear progression use paths
- repeated farming should stay meaningful because it feeds progression
- progression should help justify region-specific materials and other economic differentiation

Progression is one of the main sink destinations for the economy.

---

## Relationship between progression and powerup mechanics
Powerup mechanics are structural access points or themed subsystems for progression.

Required principle:
- progression defines what kinds of lasting growth exist
- powerup mechanics define how some of that growth is accessed, themed, or processed

These concepts overlap but are not identical.

Example:
- progression may include account-wide efficiency growth
- powerup mechanics may present that through projects, crafting, construction, or alchemy systems

---

## Relationship between progression and towns
Towns are often the service access layer for progression systems.

Required principle:
- progression should exist structurally even if town presentation is simple
- towns may centralize how progression is spent or managed
- towns should make progression easier to understand and use

Towns expose progression. They are not the only definition of progression.

---

## Relationship between progression and characters
Characters are one persistent progression target.

Required behavior:
- character progression should exist alongside account-wide progression
- character progression should matter without becoming the only meaningful growth path
- character growth should affect future run performance visibly

Characters are one layer of progression, not the whole model.

---

## Relationship between progression and gear/build systems
Progression should support build development.

Required behavior:
- the player should be able to improve build quality over time
- build progression should create visible changes in combat/farming behavior
- build progression should interact with gear/skills without collapsing into one generic stat layer

This helps keep long-term growth varied.

---

## Relationship between progression and automation
Progression may improve automation comfort and reliability.

Possible outputs later:
- auto-pick access or quality
- stronger low-friction farming
- better repeat-run support
- reduced friction in service/progression flows

Required principle:
- automation progression should reduce friction without fully erasing challenge too early

Automation can be both a baseline feature and a progression dimension.

---

## Relationship between progression and push/farm rhythm
Progression must support both push and farm-oriented improvement.

### Push-oriented progression outputs
Examples:
- more survivability
- more damage
- better boss/gate readiness
- better ability to handle deeper nodes

### Farm-oriented progression outputs
Examples:
- better clear speed on known content
- stronger automation comfort
- better resource efficiency
- improved conversion value

Good progression lets the player choose what type of strength matters next.

---

## Progression output categories
Persistent progression may produce one or more of these outputs.

### 1. Direct power
Examples:
- offensive growth
- defensive growth
- stronger combat behavior through build/skill layers

### 2. Efficiency
Examples:
- faster farming
- stronger reward conversion
- reduced friction
- better resource usage

### 3. Access
Examples:
- unlock new systems
- unlock stronger progression tiers
- unlock more build options

### 4. Flexibility
Examples:
- more viable builds
- more character options later
- more routes to power

The progression model should remain open to all four output types.

---

## Progression identity rule
Every implemented progression layer should answer:
- what gets stronger?
- what resource/input drives it?
- why is it separate from other growth layers?
- what type of advantage does it create?

If those answers are unclear, the layer should probably be merged into a simpler one.

---

## Complexity control rule
Do not split progression into too many overlapping layers too early.

### MVP rule
Use a small set of progression layers with distinct roles.

Good MVP examples:
- one account-wide upgrade layer
- one character/build growth layer
- one support/project/system layer

Bad MVP examples:
- many overlapping trees all giving generic permanent bonuses with unclear differences

The first version should prove that progression is meaningful before it becomes broad.

---

## Progression pacing principles
Progression must support both short-term and long-term satisfaction.

Required principles:
- short sessions should still move at least one progression needle
- long sessions should allow larger progression investments or unlocks
- expensive progression goals should exist, but not every goal should be expensive
- progression should not stall so hard that only node unlocks feel meaningful

This is critical for a grind-heavy game.

---

## Relevance preservation rule
Progression must help preserve the relevance of old content.

Required behavior:
- older content can still provide inputs needed for progression
- progression should create reasons to revisit earlier nodes/regions
- the game should not move into a state where only the newest content matters economically and progression-wise

This is one of the key strengths of the overall concept.

---

## MVP requirements
The first playable version must support:
- at least one account-wide or broad persistent progression layer
- at least one additional persistent strengthening path affecting character/build effectiveness
- clear conversion of gameplay rewards into persistent growth
- visible gameplay impact from progression investment
- progression relevance across multiple sessions
- compatibility with future expansion into more specialized progression layers

MVP may omit:
- deep specialization trees
- many parallel progression tracks
- separate town-bound progression screens for every mechanic
- large project chains
- progression reset/prestige systems

---

## MVP priorities
Focus on:
- one or two strong persistent growth layers
- visible effect of upgrades on runs
- strong source-to-sink clarity
- support for both push power and farm efficiency
- long-term goals without too much structure overhead
- future-ready support for more progression systems later

Avoid in MVP:
- many overlapping progression layers
- too many abstract currencies with unclear purpose
- progression systems that differ only in theme and not mechanics
- high UI complexity before the value of the progression is proven
- prestige/reset structures before the core progression model is proven

---

## Data model requirements
Minimum required progression-related data:
- progression layer id/type
- current progression state/value
- unlock state if relevant
- input requirements/cost rules
- output/effect definition
- relation to affected system (account/character/build/automation/etc.)

Optional later data:
- tier/rank state
- branching specialization state
- per-character progression trees
- automation progression state
- region-linked progression dependencies
- project-chain dependencies

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make progression only equal to content unlocks.
- Do not make progression only equal to raw stat inflation.
- Do not add many overlapping progression layers before one useful structure is proven.
- Do not make progression detached from farming and world content.
- Do not make only the newest content relevant for progression inputs.
- Do not let one progression layer erase the need for all others too early.
- Do not make automation progression completely remove the push/farm distinction early.
- Do not add prestige/reset systems before the core persistent model is proven.

---

## Extension points
The progression system must support later addition of:
- deeper account-wide upgrade boards
- character specialization paths
- gear enhancement progression
- automation progression layers
- town-linked project chains
- region-specific progression systems
- construction/infrastructure progression
- long-term milestone projects
- optional reset/prestige-like layers later if ever desired

These additions must extend the persistent progression foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact upgrade names
- exact costs
- exact progression tree layouts
- exact UI screens
- exact timing curves
- exact prestige/reset design

---

## Validation checklist
- [ ] The game has at least one account-wide persistent progression layer.
- [ ] The game has at least one additional persistent strengthening path for character/build effectiveness.
- [ ] Gameplay rewards can be converted into lasting growth.
- [ ] Progression has visible effect on future runs.
- [ ] Progression supports both push and farm improvement.
- [ ] Older content can remain relevant to progression inputs.
- [ ] The system can later expand into more spec