# Automation / AI Behavior

## Purpose
Define how automated gameplay works, what decisions are handled by game-controlled behavior, what remains player-controlled, and what rules later combat, progression, farming, UX, and save specs must follow.

---

## Scope
This spec defines:
- automation role in the game
- AI-controlled actions during runs
- player-controlled decisions that remain outside automation
- automation tiers/states
- relationship between automation and progression
- minimal MVP automation requirements
- constraints for later specs

This spec does not define:
- exact combat formulas
- exact stat scaling
- exact enemy AI details
- exact power-up pool
- exact UI controls for automation settings
- exact save serialization

---

## Core statement
Automation is a core pillar of the game.

The game is not built around direct manual execution of combat actions.
The player primarily expresses intent through:
- node choice
- push vs farm choice
- build/loadout choice
- upgrade priorities
- automation settings
- long-term progression decisions

During runs, the game-controlled behavior system executes most moment-to-moment combat actions.

Automation must:
- reduce friction,
- enable lazy grinding,
- preserve strategic decision-making,
- become more effective on mastered content over time,
- not fully remove progression tension too early.

---

## Definition of automation
Automation means game-controlled handling of repetitive or low-level actions that would otherwise require manual input.

In this project, automation may control:
- movement during combat
- targeting during combat
- attack execution during combat
- skill execution during combat if such skills are auto-usable
- repeated run flow in farming contexts
- selection of upgrades if auto-pick is enabled

Automation does not replace player intent.
It executes within boundaries chosen by the player or implied by current mode/state.

---

## Core automation rule
Default rule:

`player sets goal + build + route context -> automation executes combat behavior -> player receives results and adjusts strategy between runs`

This is the core control model of the game.

---

## Control split
### Player-controlled decisions
The player remains responsible for:
- choosing nodes
- choosing push vs farm intent
- choosing build/loadout
- choosing persistent upgrades
- choosing route direction when multiple options exist
- choosing whether to use manual upgrade selection or automation where available
- adjusting automation preferences if such settings are unlocked/available

### AI-controlled decisions
Automation is responsible for:
- movement during runs
- target selection during runs
- basic attack timing/execution during runs
- low-level combat positioning behavior within defined heuristics
- repeated execution of stable combat behavior
- optional auto-pick of upgrades when enabled

The core loop must never require manual movement or manual attack execution.

---

## Automation layers
Automation should be understood in layers.

### Layer 1: base combat automation
Always or nearly always active.

Controls:
- movement
- targeting
- attack execution

This is required for the core identity of the game.

### Layer 2: decision assistance
Controls some run-time choices when enabled or unlocked.

Examples:
- auto-pick upgrades
- auto-continue repeated runs
- auto-replay known farming nodes

This may start limited in MVP.

### Layer 3: low-friction farming automation
Used mainly on well-known content.

Examples:
- repeated farming with reduced player attention
- lower-friction farming loops
- stable automation presets for known content

This is a key long-term direction even if implemented minimally in MVP.

---

## Automation states by content familiarity
Automation behavior should relate to content state.

### New / uncertain content
Expected behavior:
- base combat automation still active
- player may manually choose upgrades during run
- player pays more attention to build performance
- automation may be less reliable due to content difficulty or lack of tuning

### Cleared content
Expected behavior:
- player can reuse known build patterns
- automation becomes more stable and predictable
- repeated runs become lower-friction

### Mastered / established farming content
Expected behavior:
- automation can carry most or all ordinary run decisions
- player intervention becomes minimal
- efficiency is stable but not necessarily fully optimal

This supports the intended transition from active decision mode to comfortable farming mode.

---

## Required combat automation behaviors
The automation system must support these core behavior domains.

### 1. Movement behavior
Automation must move the player-controlled combat entity without direct player steering.

Minimum goals:
- remain functional during combat
- avoid obviously nonfunctional behavior
- support target engagement
- support repeated farming runs

The movement model may be simple in MVP.
It only needs to be reliable enough to prove the lazy-combat loop.

### 2. Targeting behavior
Automation must choose targets.

Minimum goals:
- identify a valid target
- avoid idle or non-targeting behavior
- support progression through enemy-kill loops

Targeting logic may later become more sophisticated, but MVP targeting must be readable and stable.

### 3. Attack execution behavior
Automation must perform attacks or attack-like actions when allowed.

Minimum goals:
- attack when valid target conditions are met
- do so consistently enough for progression/farming loops
- interact cleanly with build/loadout definitions

### 4. Upgrade decision behavior
Automation may optionally choose upgrades during runs.

Minimum goals for MVP:
- manual upgrade choice must remain supported
- auto-pick may be omitted or implemented simply

Future expectation:
- auto-pick becomes a major convenience layer for farm content

---

## Behavior priority model
Automation should follow a stable priority order.

Baseline priority order:
1. remain in active combat state
2. identify reachable/valid threat or goal target
3. move into useful engagement position if needed
4. execute available attack/action behavior
5. repeat

If no valid combat target exists, behavior may:
- idle briefly,
- reposition according to simple rules,
- transition according to node/run logic if combat wave/state has advanced.

Exact behavior trees/state machines belong to implementation, but the behavior must remain goal-driven and stable.

---

## Farming vs pushing behavior expectations
Automation may behave differently depending on player intent.

### Push-oriented expectation
- acceptable to be less efficient or less stable than in established farm content
- should still function well enough to test the build
- may require more player attention to upgrades and between-run adjustment

### Farm-oriented expectation
- should prioritize stability and repeatability
- should reduce player attention requirements
- should make repeated runs comfortable
- should not require constant intervention to remain useful

Intent-based automation tuning may be explicit later, but the system must be compatible with that separation.

---

## Relationship between automation and build design
Automation operates on top of build choices.

Required relationship:
- automation should not fully ignore build identity
- builds should influence how automated combat performs
- bad or mismatched builds should still perform worse than good builds
- automation should express player planning, not erase it

The game should reward good setup and route choice, not manual execution skill.

---

## Relationship between automation and progression
Automation must be progression-aware.

Required behavior:
- automation should feel stronger or more reliable on familiar content over time
- progression may unlock additional automation convenience
- automation should help convert cleared content into comfortable farming content
- automation must not completely erase the push/farm distinction too early

Automation is both a baseline feature and a progression vector.

---

## Relationship between automation and failure
Automation is allowed to be imperfect.

Required principles:
- early automation does not need to be optimal
- imperfect automation should create reasons to improve builds and progression
- automation failure should usually still produce some value through partial progress or rewards
- automation should not feel random or uncontrollable when it fails

Failure should reveal weakness in build/progression/context, not feel like pure chaos.

---

## Relationship between automation and node roles
Automation expectations vary by node role.

### Combat nodes
- full combat automation required

### Farming on cleared combat nodes
- highest automation suitability expected

### Service / progression nodes
- little or no combat automation required
- may use only interaction automation or no automation at all

### Elite / boss / gate nodes
- automation still drives combat
- player may be expected to pay more attention to decisions around the run

This allows mode-specific variation without breaking the core identity.

---

## Relationship between automation and sessions
Automation must support the intended session rhythm.

Required behavior:
- enable low-friction repeat runs
- reduce attention requirements for farm sessions
- keep new-content sessions readable
- support stopping after resolved runs without ambiguity

Future additions like auto-repeat or resume helpers must extend this session rhythm, not replace it.

---

## MVP requirements
The first playable version must support:
- automatic movement during combat
- automatic targeting during combat
- automatic attack execution during combat
- manual build/setup decisions before runs
- manual upgrade choice during runs
- automation stable enough to clear early content with an appropriate build
- replay of content without requiring direct combat control

MVP may omit:
- complex behavior customization
- advanced auto-pick logic
- auto-repeat farming
- detailed automation presets

---

## MVP priorities
Focus on:
- stable baseline autobattle
- readable AI behavior
- consistent targeting and attack execution
- simple but functional movement logic
- preserving player control at the planning layer
- proving that lazy combat is satisfying

Avoid in MVP:
- overly complex behavior scripting
- high number of automation options before baseline behavior is proven
- hiding core AI behavior behind too many unlocks
- making early automation either useless or perfect
- coupling automation too tightly to UI complexity

---

## Data model requirements
Minimum required automation-related data:
- current automation-enabled behavior state
- player/build combat entity reference
- target selection context
- current node/run context
- upgrade choice mode: manual or automated if implemented

Optional later data:
- automation profile/preset
- push vs farm preference profile
- recent successful farming setup
- auto-repeat preferences
- per-node automation suitability state

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not require manual movement or manual attacks for core progression.
- Do not make automation irrelevant on ordinary content.
- Do not make automation fully optimal too early.
- Do not make build choice meaningless by overcorrecting AI behavior.
- Do not make automation so opaque that players cannot understand why runs succeed or fail.
- Do not make farming content require constant supervision.
- Do not split automation behavior into too many systems before baseline autobattle is proven.
- Do not make progression rely on difficult mechanical execution hidden behind an autobattle presentation.

---

## Extension points
The automation system must support later addition of:
- auto-pick upgrades
- automation profiles/presets
- push vs farm behavior presets
- auto-repeat runs
- node-specific farming preferences
- mastery-based automation improvements
- better pathing/positioning logic
- priority tuning systems

These additions must extend baseline autobattle, not replace the core control split.

---

## Out of scope
This spec does not define:
- exact movement algorithm
- exact pathfinding implementation
- exact targeting formula
- exact skill AI rules
- exact UI layout for automation settings
- exact save format for automation state

---

## Validation checklist
- [ ] Combat can resolve without manual movement.
- [ ] Combat can resolve without manual attack execution.
- [ ] Automation chooses targets.
- [ ] Automation moves in a functional way.
- [ ] Automation executes attacks in a functional way.
- [ ] Player retains control over planning/build/route decisions.
- [ ] Early content is playable through autobattle.
- [ ] Cleared content can become lower-friction to farm.
- [ ] Automation does not remove the need for progression and build improvement.
- [ ] The system can later support auto-pick, auto-repeat, and presets without re