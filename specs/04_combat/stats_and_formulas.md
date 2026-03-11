# Stats and Formulas

## Purpose
Define the stat model and formula responsibilities for the first playable version and establish the numeric contract that later combat, gear, characters, enemies, bosses, skills, loot, and progression specs must follow.

---

## Scope
This spec defines:
- core stat categories
- what each stat category affects
- formula responsibilities
- how player-side and enemy-side stats relate to combat
- MVP stat requirements
- constraints for later specs

This spec does not define:
- final balance numbers
- exact enemy stat tables
- exact item stat rolls
- exact skill coefficients
- exact progression cost values
- exact UI presentation of stats

---

## Core statement
Stats are the numeric layer that converts build choices, enemy scaling, and progression into combat outcomes.

Formulas must:
- be readable enough to reason about,
- preserve build differences,
- support both pushing and farming,
- scale without breaking automation-based gameplay,
- remain extensible as new systems are added.

The stat system must reward planning and progression, not manual execution skill.

---

## Core formula rule
Default numeric flow:

`base stats + persistent modifiers + build modifiers + temporary run modifiers -> effective combat stats -> combat formulas -> combat outcomes`

All later systems that affect combat should integrate through this flow unless explicitly justified otherwise.

---

## Stat model principles
- Stats must have clear ownership.
- Stats must have clear impact.
- Stats must not overlap excessively without purpose.
- The first version should use a small readable stat set.
- Formulas should prefer predictable scaling over hidden complexity.
- Temporary run power and persistent account power must remain separable.
- The system must support both player-side and enemy-side stat definitions.

---

## Stat ownership layers
Stats may come from multiple layers.

### 1. Base entity stats
Intrinsic stats of a character/entity/unit.

Examples:
- base health
- base damage
- base attack rate
- base movement speed

### 2. Persistent modifiers
Long-term account/build modifiers.

Examples:
- permanent upgrades
- character growth
- gear-derived bonuses
- system unlock modifiers

### 3. Temporary run modifiers
Short-lived modifiers active only during the current run.

Examples:
- level-up bonuses
- temporary power-ups
- run-specific effects

### 4. Contextual modifiers
Modifiers from node, mode, or encounter context.

Examples:
- node difficulty adjustments
- elite/boss modifiers
- special event modifiers

The implementation must keep these layers conceptually separable even if some are combined in code.

---

## Required core stat categories
The first playable version should support a minimal but sufficient set of combat stats.

### 1. Max Health
Purpose:
- determines how much damage an entity can take before defeat

Applies to:
- player-side combat entity
- enemies

### 2. Damage / Attack Power
Purpose:
- determines offensive output before mitigation and modifiers

Applies to:
- player-side combat entity
- enemies

### 3. Attack Rate / Attack Speed
Purpose:
- determines how often offensive actions can occur

Applies to:
- player-side combat entity
- enemies if relevant

### 4. Defense / Damage Reduction stat
Purpose:
- reduces incoming damage or otherwise affects survivability

Applies to:
- player-side combat entity
- enemies if relevant

### 5. Movement stat
Purpose:
- affects repositioning or combat-space traversal if movement exists in MVP

Applies to:
- player-side combat entity
- enemies if relevant

This is the minimum core set.

---

## Optional early stat categories
These may be added early if needed, but are not required for the first combat proof.

### 6. Crit-related stats
Examples:
- crit chance
- crit multiplier

### 7. Range-related stats
Examples:
- attack range
- detection range
- area size

### 8. Penetration / armor-bypass stat
Purpose:
- improve damage against defended targets

### 9. Cooldown-related stat
Purpose:
- affects non-basic action timing

### 10. Resource-find / loot-efficiency stat
Purpose:
- improve non-combat output from runs

These should only be added when they solve a real design need.

---

## Derived stat concept
Some stats may be derived rather than stored directly.

Examples:
- effective DPS
- effective survivability
- attack interval derived from attack speed
- effective mitigation derived from defense

Rule:
- later systems may compute derived values for combat resolution or display,
- but the underlying stat inputs must remain identifiable.

---

## Player-side stat requirements
Player-side stats must be able to express:
- build strength
- build weakness
- progression improvement
- difference between stable farm performance and weaker push performance
- meaningful change from upgrades

If a player build can barely be distinguished numerically from another, the stat system is insufficient.

---

## Enemy-side stat requirements
Enemy stats must be able to express:
- difficulty progression
- regional/node scaling
- elite/boss distinction later
- meaningful resistance to underprepared builds
- meaningful vulnerability to stronger builds

Enemy scaling must challenge progression without making old content useless.

---

## Formula responsibilities
The formula layer must at minimum be able to resolve:
- outgoing damage
- incoming damage
- survival/defeat thresholds
- attack timing/frequency
- movement-related timing if movement exists
- kill speed differences between builds and content levels

Later systems may add:
- crit resolution
- status resolution
- AoE resolution
- penetration/armor interaction
- summon scaling
- loot-affecting formulas

---

## Damage formula principles
Exact formulas are implementation-defined, but must follow these principles.

- Damage must scale with offensive investment.
- Survivability must scale with defensive investment.
- Enemy difficulty must meaningfully affect time-to-kill and survival pressure.
- Damage formulas must not flatten build differences too strongly.
- Damage formulas must not produce unreadable results for ordinary progression.
- A stronger build should usually kill normal content faster and/or more safely.

---

## Defense / mitigation principles
If a defense or mitigation stat exists, it must:
- improve survivability in a predictable way
- not make entities effectively invulnerable too easily
- not become useless too early in scaling
- remain compatible with future penetration or specialized damage rules if such systems are added later

The first implementation should prefer a readable mitigation model over a highly complex one.

---

## Attack timing principles
Attack timing or attack speed must:
- produce visible differences in performance
- interact cleanly with damage output
- not create unstable automation behavior
- remain readable enough for balancing and debugging

The implementation may use attack interval, attack speed, or equivalent internal representation.

---

## Movement stat principles
If movement exists in combat, movement-related stats must:
- have a clear effect on combat behavior
- influence efficiency or survivability meaningfully
- not make automation unstable or chaotic

Movement stats are optional if the first implementation chooses a simpler combat-space model.

---

## Stat growth principles
The stat system must support growth from:
- characters/base entities
- gear
- persistent progression
- temporary run upgrades
- encounter/world context

Growth must be able to produce:
- smoother farming on known content
- better success rate on harder content
- visible difference between underprepared and prepared builds

---

## Push vs farm numeric relationship
The stat system must support the difference between:

### Push context
- narrower margins
- slower kills or lower survivability if underprepared
- more visible build weakness

### Farm context
- faster kills
- higher consistency
- more reliable automation
- lower failure pressure

This difference should emerge from the same stat model, not from completely separate combat rules.

---

## Temporary vs persistent power relationship
The formula system must keep temporary and persistent power distinct.

Required behavior:
- persistent upgrades should improve the baseline before run start
- temporary upgrades should improve current-run performance only
- the system should support runs feeling stronger over time without erasing long-term progression identity

This separation is critical for incremental progression.

---

## Scaling principles
Scaling must support long-term growth without collapsing the game loop.

Required principles:
- later content should require stronger effective stats than earlier content
- older content should become easier with progression
- scaling should preserve farming usefulness of older nodes
- scaling should not make every new step feel impossible without excessive grind
- scaling should not trivialize all combat too quickly after one upgrade source improves

Exact curves belong to balancing, but the system must be capable of these outcomes.

---

## Formula readability requirement
Formulas do not need to be shown in full to the player, but they must be understandable by developers and debuggable during implementation.

Required principles:
- avoid hidden formula interactions unless they provide clear value
- keep source of major stat changes traceable
- make it possible to explain why one build outperforms another
- make it possible to debug why a node is too easy or too hard

This is especially important for AI-driven combat where manual correction is absent.

---

## Relationship to automation
Stat formulas must remain compatible with automated gameplay.

Required behavior:
- formulas should reward better planning and setup
- formulas should not rely on manual timing windows to create value
- stat differences should be visible through AI-driven outcomes
- automated combat should still reveal whether the build is strong or weak

---

## Relationship to loot and gear
Stats are one of the main ways loot, gear, and progression systems influence gameplay.

Required rule:
- later item/gear/progression systems should affect combat primarily through defined stat channels or explicitly defined special-effect channels

This keeps content systems composable.

---

## Relationship to skills
Skills may later modify or bypass standard stat relationships in limited ways.

Required rule:
- default skill interaction should still be expressible through the stat/formula layer where reasonable
- highly special-case skill logic should be the exception, not the baseline

---

## MVP requirements
The first playable version must support:
- max health
- damage/offensive power
- attack speed or equivalent attack timing stat
- some survivability stat or direct survivability rule
- enemy and player-side stat separation
- formula resolution for damage and defeat
- visible build improvement from persistent and temporary modifiers

MVP may omit:
- crit
- penetration
- complex status formulas
- advanced loot-efficiency stats
- many derived display stats

---

## MVP priorities
Focus on:
- small readable stat set
- clear offensive vs defensive scaling
- visible impact of upgrades
- formulas that are easy to debug
- stable support for automated combat
- enough numeric depth to distinguish push vs farm performance

Avoid in MVP:
- many niche stats
- too many overlapping offensive modifiers
- too many survivability layers
- hidden scaling rules
- stat inflation without clear gameplay effect

---

## Data model requirements
Minimum required data categories:
- base stats per combat entity type
- modifier sources
- effective stat calculation result or resolvable formula inputs
- offensive and defensive combat parameters
- temporary modifier context
- persistent modifier context

Optional later data:
- crit-related values
- penetration values
- status-related values
- range-related values
- loot-efficiency values
- derived stat display values

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make stats so complex that build impact becomes unreadable.
- Do not make formulas flatten build differences excessively.
- Do not make persistent and temporary power indistinguishable.
- Do not make survivability or damage scale in a way that breaks repeated farming viability.
- Do not require manual execution skill to realize stat value.
- Do not add many niche stats before the core numeric model is proven.
- Do not bypass the stat system for most progression sources without clear reason.

---

## Extension points
The stat/formula system must support later addition of:
- crit systems
- penetration/armor interaction
- AoE/range modifiers
- cooldown systems
- summon scaling
- status effect scaling
- resource-find or reward-efficiency stats
- boss/elite-specific formula hooks

These additions must extend the core numeric model, not replace it.

---

## Out of scope
This spec does not define:
- exact balance numbers
- final coefficient values
- exact enemy stat curves
- exact item stat ranges
- exact skill multipliers
- exact UI labels or presentation

---

## Validation checklist
- [ ] The game has a small defined core stat set.
- [ ] Player-side and enemy-side entities both use the stat model.
- [ ] Offensive and defensive outcomes can be resolved numerically.
- [ ] Persistent and temporary modifiers can both affect effective stats.
- [ ] Stronger builds perform better in combat in a visible way.
- [ ] Weaker builds perform worse in a visible way.
- [ ] The numeric model supports both push and farm contexts.
- [ ] The model is extensible to later crit/penetration/status systems without redesign.

