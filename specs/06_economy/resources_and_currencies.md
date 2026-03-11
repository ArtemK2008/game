# Resources and Currencies

## Purpose
Define the resource and currency structure for the first playable version and establish the economic contract that later progression, loot, towns, gear, crafting, automation, and UX specs must follow.

---

## Scope
This spec defines:
- resource and currency categories
- what each category is used for
- where each category comes from at a structural level
- how resources connect combat, farming, and progression
- minimal MVP economic requirements
- constraints for later specs

This spec does not define:
- exact drop numbers
- exact balance values
- exact shop prices
- exact crafting recipes
- exact upgrade costs
- exact UI layout
- exact save serialization

---

## Core statement
Resources and currencies are the economic outputs of gameplay.

They must:
- make runs feel productive,
- connect farming to long-term growth,
- reinforce push vs farm decisions,
- preserve value in both new and old content,
- remain readable enough for balancing and implementation.

The economic model must support the core loop:
`run content -> gain rewards -> convert rewards into progress/efficiency -> choose next goal`

---

## Core economic rule
Default resource flow:

`combat/run completion -> rewards generated -> resources/currencies added to persistent inventory -> resources spent on upgrades/unlocks/conversions -> player power or efficiency improves`

The economy exists to convert repeated runs into meaningful long-term state change.

---

## Definitions
### Resource
A stored gameplay output with a specific thematic or progression use.

Examples:
- region materials
- crafting materials
- upgrade materials
- loot fragments

### Currency
A stored gameplay output primarily used as a generic spending medium.

Examples:
- gold-like soft currency
- premium-like rare progression currency

### Sink
Any system that permanently consumes a resource or currency in exchange for progression, convenience, or utility.

### Source
Any gameplay action or system that grants a resource or currency.

---

## Economic design principles
- Every important resource/currency must have a clear purpose.
- Every important sink must have a believable source path.
- Farming must create useful economic output.
- Pushing must create useful economic output.
- The first version should keep the number of currencies low.
- Region/material identity should preserve value of old content.
- Economic outputs should reinforce progression, not only maintenance.
- The economy must remain extensible without forcing a redesign.

---

## Required economic categories
The first playable version should support a small set of distinct categories.

### 1. Soft currency
Primary generic spend currency.

Use cases:
- basic upgrades
- simple service costs
- general progression spending
- repeatable utility spending

Expected source pattern:
- common run rewards
- frequent combat/farming output

Design role:
- universal low-friction conversion layer
- keeps ordinary runs feeling productive

### 2. Region/resource materials
Semi-specific or specific materials tied to world content.

Use cases:
- region-linked upgrades
- future crafting/progression systems
- gating of certain progression paths
- preserving relevance of old regions

Expected source pattern:
- region-specific node rewards
- repeated farming of known content

Design role:
- give old content lasting value
- create farming identity by region/node

### 3. Persistent progression material
A higher-value material or currency used mainly for long-term account power.

Use cases:
- account-wide upgrades
- major unlocks
- automation improvements
- progression sink feeding long-term growth

Expected source pattern:
- harder content
- milestone rewards
- efficient farming over time
- possibly first-clear or gate-related outputs later

Design role:
- convert gameplay time into durable progression
- separate ordinary rewards from important long-term growth

---

## Optional early economic categories
These may be added early if needed, but are not required for the first proof of the loop.

### 4. Service-only currency
Use case:
- node-specific service interactions
- town-only functions
- temporary utility conversion

### 5. Crafting-only materials
Use case:
- recipes
- equipment creation
- alchemy/forge systems

### 6. Automation-related resource
Use case:
- unlock or improve automation convenience
- fuel progression of farming comfort

These should only be added if they solve a real progression need.

---

## Core source categories
Resources and currencies must come from a small number of readable source types.

### 1. Run rewards
Awarded at or after run resolution.

Examples:
- soft currency
- materials
- progression currency
- reward bundles

This is the primary source category.

### 2. Enemy-derived rewards
Generated from combat outcomes.

Examples:
- kill-linked resources
- loot drops
- region materials

This keeps combat economically meaningful.

### 3. Clear/unlock rewards
Awarded when maps/nodes are cleared or progression thresholds are reached.

Examples:
- first-clear reward
- unlock bonus
- gate milestone reward

MVP may keep this minimal.

### 4. Conversion/system rewards
Generated through non-combat progression systems.

Examples:
- processing materials
- converting common resources into rarer value
- town/service utility outputs

MVP may omit most of these if needed.

---

## Core sink categories
Resources and currencies must be spendable into progression or utility.

### 1. Persistent upgrade sinks
Use cases:
- account growth
- character growth
- permanent efficiency
- unlocking stronger future runs

This is required in MVP.

### 2. Utility / service sinks
Use cases:
- rerolls
- repairs if such a system ever exists
- service fees
- convenience actions

Not all utility sinks are needed in MVP.

### 3. Content-gating sinks
Use cases:
- unlocking specific paths
- enabling progression systems
- paying for special access

This should be used carefully and readably.

### 4. Craft / construction / system sinks
Use cases:
- building features
- recipes
- meta systems
- long-term projects

These are future-compatible but not required in full for MVP.

---

## Source-to-sink relationship rule
Every important economic category must satisfy:
- clear primary source
- clear primary sink
- understandable reason to keep farming it

Disallowed pattern:
- resource exists with no meaningful sink
- sink exists with no readable source path
- resource is so generic that all region identity disappears

---

## Relationship between economy and push/farm rhythm
The economy must reinforce both push and farm.

### Push contribution
Push content should be better at one or more of:
- unlocking new economic opportunities
- granting higher-value progression materials
- granting first-clear or milestone rewards
- opening better farming locations

### Farm contribution
Farm content should be better at one or more of:
- stable soft currency income
- repeatable material gain
- supplying required inputs for upgrades
- enabling efficient preparation for later push attempts

Farm content must not feel economically irrelevant.
Push content must not be the only meaningful source of value.

---

## Relationship between economy and world structure
The economic model must preserve world relevance.

Required behavior:
- different regions/nodes should be allowed to produce different material identities
- older regions should stay economically useful after new regions unlock
- world progression should expand economic options, not delete older ones immediately

Exact regional resource design belongs to later specs.

---

## Relationship between economy and node roles
Different node roles may have different economic emphasis.

Examples:
- combat nodes -> baseline combat rewards
- farming usage of cleared nodes -> stable income
- elite/boss nodes -> better concentrated rewards later
- service/progression nodes -> convert or spend value rather than generate raw value
- loot nodes -> reward-focused output

This relationship may start simple in MVP.

---

## Relationship between economy and progression
Resources/currencies must convert into long-term progress.

Required behavior:
- ordinary runs should grant something useful
- persistent upgrade systems must consume economic outputs
- stronger progression should improve future economic efficiency or content access
- the economy must be one of the main bridges between combat and growth

---

## Relationship between economy and automation
Automation must improve economic comfort without invalidating the economy.

Required behavior:
- automated farming should still produce meaningful value
- more comfortable farming may be less than fully optimal but must remain worthwhile
- automation may later improve economic efficiency indirectly through mastery or convenience

The economy must remain compatible with low-attention play.

---

## Economic readability rules
The player and developer should be able to understand:
- what was gained,
- why it was gained,
- what it is used for,
- whether it is common or rare,
- where to get more of it.

This is especially important because the game is grind-heavy and repeated farming is expected.

---

## Economic inflation control principles
The system should avoid meaningless inflation.

Required principles:
- adding a new currency should require a clear reason
- soft currency should not replace all specialized materials
- specialized materials should not be so fragmented that the economy becomes unreadable
- rare currencies should remain meaningfully rarer than common ones
- sinks should scale with progression without making ordinary gains feel pointless

Exact balancing belongs to later work.

---

## Inventory/storage principles
All persistent resources and currencies must be storable between runs and sessions.

Minimum rule:
- rewards granted from resolved runs become part of persistent player-owned state

Future-compatible expectation:
- resources can later be grouped, filtered, or categorized without changing their structural role

Exact inventory UX belongs to later specs.

---

## MVP requirements
The first playable version must support:
- one soft currency
- one region/material category or equivalent non-generic material layer
- one persistent progression currency/material or equivalent upgrade-driving resource
- rewards granted from runs
- at least one persistent upgrade sink consuming economic output
- replayable farming producing useful resources
- cleared or deeper content creating different economic opportunity from early content

If MVP scope must be reduced, soft currency and persistent progression material may temporarily overlap, but the design should still preserve a path toward later separation.

---

## MVP priorities
Focus on:
- small number of economic categories
- clear source/sink relationship
- visible usefulness of rewards
- stable farm value
- preserving old-region value
- one strong persistent sink
- easy debugging of reward flow

Avoid in MVP:
- many currencies
- many near-duplicate materials
- sinks with no meaningful progression outcome
- highly fragmented crafting inputs
- economy layers that exist before their related systems are implemented

---

## Data model requirements
Minimum required data categories:
- resource/currency type definitions
- persistent inventory balances
- reward payload definitions
- sink definitions or resolvable spending rules
- source attribution or source mapping rules

Optional later data:
- rarity classification
- region affinity
- stack limits if needed
- conversion recipes
- service-only currencies
- automation-related economic values

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not add many currencies before the main loop economy is proven.
- Do not make farming economically pointless.
- Do not make all progression depend on only one rare currency source.
- Do not remove economic relevance from older regions entirely.
- Do not create major sinks without clear sources.
- Do not create major sources without clear sinks.
- Do not make rewards so generic that route/world identity disappears.
- Do not make automation economically useless for ordinary farming.

---

## Extension points
The economy must support later addition of:
- first-clear rewards
- boss/elite reward tiers
- crafting materials
- alchemy/construction/project materials
- automation upgrade resources
- service-node conversions
- rare unlock currencies
- region-specific processing chains

These additions must extend the core readable economy, not replace it.

---

## Out of scope
This spec does not define:
- exact drop rates
- exact cost values
- exact recipe formulas
- exact shop inventories
- exact reward UI layout
- exact save format

---

## Validation checklist
- [ ] The game has a small defined set of resource/currency categories.
- [ ] Each important category has a clear source.
- [ ] Each important category has a clear sink.
- [ ] Runs generate persistent economic output.
- [ ] Farming generates useful economic output.
- [ ] Progression systems consume economic output.
- [ ] Older content can remain economically useful.
- [ ] The economy can later expand into crafting/towns/automation s