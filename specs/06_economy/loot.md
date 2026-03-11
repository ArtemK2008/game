# Loot

## Purpose
Define the loot system for the first playable version and establish the structural rules that later rewards, gear, resources, progression, enemies, bosses, towns, and UX specs must follow.

---

## Scope
This spec defines:
- what loot is in this project
- loot categories
- loot sources at a structural level
- relationship between loot and progression/economy/build growth
- minimal MVP loot requirements
- constraints for later specs

This spec does not define:
- exact drop rates
- exact item tables
- exact rarity percentages
- exact reward UI layout
- exact save serialization
- exact shop inventories

---

## Core statement
Loot is the persistent reward output of runs, kills, clears, and milestone encounters.

Loot must:
- make combat and farming feel productive,
- connect enemy kills and node clears to long-term growth,
- support gear progression and resource progression,
- preserve value in both push and farm content,
- remain readable enough for balancing and implementation.

Loot is a reward delivery layer, not a separate progression system.

---

## Definition of loot
Loot is any reward object or reward payload granted from gameplay and stored or consumed as part of the player’s persistent progression.

Loot may include:
- currencies
- materials
- gear
- gear-improvement items
- milestone reward bundles
- special unlock-related reward items later

Loot is not the same thing as:
- temporary in-run power-ups
- one-time visual feedback with no persistent value
- node unlock state itself
- a service interaction with no reward output

---

## Core loot rule
Default loot flow:

`combat or progression event happens -> loot payload is generated -> loot is granted to persistent player state -> loot is spent, equipped, converted, or stockpiled for future progression`

Loot must convert gameplay into future usefulness.

---

## Loot design principles
- Loot must have a clear gameplay purpose.
- Loot must support both immediate satisfaction and long-term value.
- Loot must remain readable in an autobattle/grind-heavy game.
- The first version should use a small readable loot structure.
- Loot must reinforce push/farm rhythm.
- Loot should preserve world and node identity over time.
- Loot should not require a deep ARPG itemization model to matter in MVP.
- Important loot categories must have clear sinks or uses.

---

## Loot system role in the game
Loot is one of the main bridges between:
- combat and progression,
- farming and upgrades,
- world content and long-term power,
- push milestones and reward spikes.

If loot is weak or unclear:
- repeated runs feel hollow,
- farming feels pointless,
- build improvement feels disconnected from gameplay,
- node/world identity becomes weaker.

---

## Loot categories
The loot system must support a small set of readable categories.

### 1. Currency loot
Loot that grants generic spendable value.

Examples structurally:
- soft currency
- progression currency

Purpose:
- fuel persistent sinks
- make ordinary runs always feel productive

### 2. Material loot
Loot that grants specific or semi-specific crafting/progression inputs.

Examples structurally:
- region materials
- upgrade materials
- project materials later

Purpose:
- preserve value of different regions/nodes
- support progression and future system depth

### 3. Gear loot
Loot that grants equippable build options.

Purpose:
- create new build possibilities
- improve or diversify combat performance

MVP may keep this limited or deterministic, but the loot system must support it.

### 4. Milestone loot
Loot granted mainly from node clears, boss clears, or unlock milestones.

Purpose:
- create stronger reward spikes
- reinforce important progression moments

This is structurally distinct even if implemented through ordinary reward bundles in MVP.

---

## Optional early loot categories
These may be added early if needed, but are not required for the first proof of the loop.

### 5. Gear-improvement loot
Purpose:
- upgrade or reroll equipment later

### 6. Automation/convenience loot
Purpose:
- unlock or improve low-friction farming systems later

### 7. Special unlock loot
Purpose:
- gate specific systems, branches, or unique content later

These should be introduced only when their sink/use is already meaningful.

---

## Loot source categories
Loot must come from a small set of structurally clear sources.

### 1. Enemy kill loot
Loot tied directly or indirectly to enemy defeat.

Purpose:
- make combat kills economically meaningful
- reinforce kill-based progression with reward output

MVP note:
- may be granted as aggregated post-run reward instead of per-kill pickup

### 2. Run completion loot
Loot granted when a run resolves.

Purpose:
- keep run completion meaningful even when map clear is not achieved
- simplify reward delivery

### 3. Map/node clear loot
Loot granted when a map reaches clear threshold.

Purpose:
- reinforce map completion as an important progression event

### 4. Boss / milestone loot
Loot granted for major progression checkpoints.

Purpose:
- create stronger reward spikes
- justify encounter importance

### 5. System/service conversion loot later
Loot generated through towns/services/crafting/conversion systems.

Purpose:
- support deeper economy loops later

---

## Loot delivery model
Loot may be delivered in different ways.

### MVP-preferred delivery
- aggregated at run end and/or clear resolution

Reasons:
- simpler to implement
- cleaner for autobattle
- easier to debug
- avoids requiring manual pickup behavior

### Future-compatible delivery
Later systems may add:
- on-kill visible drops
- chest/reward-node resolution
- loot bundles by source type
- auto-pick/floor drop behaviors

The architecture must support richer delivery later, but MVP should stay simple.

---

## Relationship between loot and resources/currencies
Loot is the main delivery mechanism for resource and currency gain.

Required behavior:
- resources/currencies are often granted as loot payloads
- loot must map clearly onto the economy categories defined in `resources_and_currencies`
- loot design must not create an economy category with no meaningful structural use

Loot and economy are related but not identical concepts.

---

## Relationship between loot and gear
Loot is one of the main acquisition paths for gear.

Required behavior:
- gear can be granted as loot or via loot-conversion systems later
- gear loot must support build progression
- gear loot should not require deep randomization in MVP to be valuable

MVP may keep gear acquisition simple while preserving future compatibility with richer loot tables.

---

## Relationship between loot and progression
Loot must support progression rather than exist only as reward flavor.

Required behavior:
- ordinary content should provide ordinary loot value
- important progression moments should provide stronger loot value
- loot should convert into upgrades, build improvement, or future access
- farming should remain worthwhile because of loot output

Loot is one of the main ways runs become meaningful across many sessions.

---

## Relationship between loot and push/farm rhythm
Loot must reinforce both push and farm states.

### Push-oriented loot role
Push content should be better at one or more of:
- first-clear rewards
- milestone rewards
- unlocking access to better loot sources
- granting higher-value or rarer loot categories

### Farm-oriented loot role
Farm content should be better at one or more of:
- stable repeatable reward output
- material accumulation
- soft currency generation
- supplying inputs for persistent sinks

Both states must produce meaningful loot, but not always the same type of loot value.

---

## Relationship between loot and world structure
Loot must help preserve world identity.

Required behavior:
- different regions/nodes may later emphasize different loot categories or material identities
- older regions should remain able to produce useful loot
- deeper world progression should expand loot opportunities without fully invalidating earlier ones

The loot system must support world-based farming relevance.

---

## Relationship between loot and node roles
Different node roles may emphasize different loot outputs.

Examples:
- standard combat node -> baseline repeatable loot
- cleared farming node -> stable low-friction loot income
- loot node -> more concentrated reward output
- boss/gate node -> milestone loot
- service/progression node -> spending or conversion rather than raw loot generation

This relationship may begin simple in MVP.

---

## Relationship between loot and automation
Loot must be compatible with autobattle and low-attention play.

Required behavior:
- ordinary loot acquisition must not depend on manual pickup skill
- farming under automation must still generate meaningful loot
- loot delivery should remain readable even when many runs are repeated

The game should not require constant input to secure basic rewards.

---

## Loot rarity / value model
Rarity is optional in MVP but the system must be extensible to support it.

Possible later rarity dimensions:
- common
- uncommon
- rare
- milestone-unique

### MVP rule
The first version does not require a deep rarity taxonomy.
It only needs visible differences in reward importance where necessary.

---

## Loot readability rules
The player and developer should be able to understand:
- what loot was gained
- why it was gained
- what it is used for
- whether it is ordinary, special, or milestone-level
- where similar loot can be obtained again

This is especially important because the game expects repeated farming.

---

## Duplicate / repeat loot principles
Repeated loot gain is expected and intended.

Required principles:
- repeat loot must still have value through sinks, upgrades, or stockpiling
- duplicate gear or material gain later should have a conversion path if duplication becomes common
- the first version may keep duplicates simple as long as repeated rewards remain useful

Loot repetition is not a problem by itself in a grind-heavy game.
It only becomes a problem when repeated loot has no meaning.

---

## MVP requirements
The first playable version must support:
- persistent reward payloads from runs
- loot that includes at least one currency/resource category
- loot that contributes to persistent progression
- map clear or milestone reward differentiation in some visible form
- farming runs producing useful loot repeatedly
- compatibility with future gear-as-loot support

If MVP scope is limited, gear loot may be absent initially, provided the loot model still supports adding it later.

---

## MVP priorities
Focus on:
- small readable loot category set
- run-end reward clarity
- useful repeatable loot
- visible distinction between ordinary and milestone rewards
- clean integration with economy/progression systems
- no manual pickup dependency

Avoid in MVP:
- large randomized drop tables
- many rarity layers
- many niche reward items with no sink
- pickup-heavy systems that conflict with lazy/autobattle play
- deep salvage/conversion systems before baseline loot value is proven

---

## Data model requirements
Minimum required loot-related data:
- loot category/type
- source category
- reward payload structure
- persistent destination mapping (inventory/currency/gear/etc.)
- optional rarity/value tier field or equivalent later-compatible hook

Optional later data:
- drop table references
- rarity weights
- gear generation templates
- region affinity tags
- duplicate conversion metadata
- boss-specific reward tags

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make ordinary loot acquisition depend on manual pickup skill.
- Do not make loot categories exist without meaningful sinks.
- Do not make farming produce weak or meaningless loot by default.
- Do not make all loot so generic that world/node identity disappears.
- Do not require deep rarity/affix systems in order for loot to matter.
- Do not make milestone encounters reward the same way as ordinary content with no visible distinction.
- Do not let reward delivery become unreadable during repeated farming.

---

## Extension points
The loot system must support later addition of:
- gear drops
- rarity tiers
- boss-specific loot
- loot nodes/chests
- duplicate conversion/salvage
- auto-pick or floor-drop presentation layers
- region-specific reward pools
- special unlock items
- crafting and construction materials

These additions must extend the readable reward foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact drop rates
- exact reward numbers
- exact rarity percentages
- exact reward screen layout
- exact inventory UI behavior
- exact salvage formulas

---

## Validation checklist
- [ ] Runs generate persistent loot/reward output.
- [ ] Loot includes at least one meaningful resource/currency category.
- [ ] Loot supports persistent progression.
- [ ] Farming generates useful repeated loot.
- [ ] Important clears/milestones have visibly stronger or distinct reward meaning.
- [ ] Loot is compatible with autobattle and no-manual-pi