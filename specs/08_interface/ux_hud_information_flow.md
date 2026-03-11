# UX / HUD / Information Flow

## Purpose
Define the player-facing information model for the first playable version and establish the structural rules that later combat, world, progression, loot, towns, automation, menu, and save specs must follow.

---

## Scope
This spec defines:
- what information the player must be able to understand at each stage of play
- core UX principles for a lazy/autobattle progression game
- HUD responsibilities during runs
- information flow before, during, and after runs
- relationship between UX and core loop readability
- minimal MVP UX/HUD requirements
- constraints for later specs

This spec does not define:
- exact visual art style
- final UI mockups
- exact button placement
- final typography/colors
- exact platform-specific controls
- exact animation details

---

## Core statement
The game’s UX must make a grind-heavy autobattle loop readable, low-friction, and goal-oriented.

The player must always be able to answer:
- where am I,
- what am I doing,
- what is happening now,
- what did I gain,
- what can I do next,
- why should I keep playing this node/route/session.

UX is not decoration.
It is the readability layer that makes automated combat and long-term progression understandable.

---

## Core UX rule
Default information flow:

`world context shown -> player chooses next action -> run begins with readable combat HUD -> run resolves with readable results -> post-run state clearly shows gains and next options -> player continues or stops`

At no point should the player lose track of current goal, current progress, or current available next step.

---

## UX design principles
- Readability is more important than visual spectacle.
- The player must always know the current goal.
- Important progression information must be visible without deep menu digging.
- Repeated farming must stay low-friction.
- New content must be understandable without excessive explanation.
- Important state changes must be clearly surfaced.
- The first version should prefer simple screens with clear hierarchy.
- Information density must scale carefully to avoid overwhelming a lazy-play game.

---

## Information hierarchy
The UI must prioritize information in the following order.

### Tier 1: immediate action context
What the player needs right now.

Examples:
- current node
- current run state
- current available actions
- current danger/progress state
- current upgrade choice if a choice is active

### Tier 2: short-term goal context
What the player is currently trying to achieve.

Examples:
- clear this node
- reach next unlock threshold
- farm this resource
- prepare for a boss/gate

### Tier 3: medium-term progression context
What the current session contributes to.

Examples:
- unlock progress
- reward accumulation
- current upgrade target
- build improvement target

### Tier 4: long-term account context
What the player is building over many sessions.

Examples:
- account-wide progression status
- unlocked systems
- character/build growth direction
- major future goals

The first playable version should keep Tier 1 and Tier 2 especially clear.

---

## Core information questions by game stage
The UX must answer different questions at different stages.

### On world map / between runs
The player must be able to understand:
- what nodes are available
- which direction is forward progression
- which places are useful for farming
- what the current session goal could be
- where towns/services/upgrades can be accessed

### During run
The player must be able to understand:
- whether the run is going well or badly
- current combat pressure level
- current map/node progress contribution
- available upgrade choice when relevant
- approximate reason for success/failure

### After run
The player must be able to understand:
- what rewards were gained
- what progression changed
- whether anything was unlocked
- whether the current node is beaten/partially progressed
- what the next reasonable action is

### In progression/service screens
The player must be able to understand:
- what can be upgraded now
- what each upgrade changes
- what it costs
- why it matters
- what goal it supports next

---

## World map UX responsibilities
The world-level UI must communicate:
- current region/location context
- reachable vs locked nodes
- current node state (available / in progress / cleared / mastered-equivalent later)
- forward route direction
- available branch choices if any
- return paths for farming
- access to town/service/progression contexts

Minimum rule:
The player must not need to memorize world state outside the interface.

---

## Run HUD responsibilities
The run HUD must communicate enough information to make autobattle readable.

Minimum required HUD responsibilities:
- show current run state
- show player-side survivability state
- show current node or run progress contribution
- show active upgrade choice when a decision is required
- show reward/progression-relevant context when helpful

The run HUD does not need to expose every combat formula.
It must expose enough to understand whether the build is working.

---

## Required run HUD information
The first playable version should expose at least:
- player health or equivalent survival indicator
- current node/run identifier
- visible indication of map/node progress contribution or progress context
- active upgrade choice panel when relevant
- pause/exit access if supported

Optional early additions:
- enemy count or kill count
- current reward summary preview
- current automation mode indicator
- simple danger or wave/stage indicator if used

---

## Post-run information flow
After a run resolves, the game must clearly surface:
- reward payload
- node/map progress delta
- node clear/unlock result if any
- persistent progression change if relevant
- next available actions

Post-run information should answer:
- was this run useful?
- did I get stronger?
- should I replay, push, upgrade, or stop?

This is one of the most important UX moments in the whole game.

---

## Reward presentation rules
Rewards must be shown clearly enough to support repeated farming.

Required behavior:
- common rewards should be easy to scan quickly
- milestone rewards should feel distinct from ordinary rewards
- resource purpose should be understandable
- repeated farming should not flood the player with unreadable reward noise

The first version should prefer readable aggregated reward summaries over noisy detailed breakdowns unless detail is actually useful.

---

## Upgrade choice UX rules
If runs include player-made upgrade choices, the UI must make them fast to parse.

Required behavior:
- available choices are clearly separated
- each choice clearly communicates its effect category
- choice flow does not become a heavy interruption
- the player can understand why a choice is useful for the current build

If auto-pick exists later, manual choice readability must still remain strong.

---

## Build/preparation UX responsibilities
Before a run, the player must be able to understand:
- which character/build/loadout is active
- what can be changed before entering the node
- whether the player is preparing for push or farm
- what major build identity the current setup has

The first version should keep preparation screens simple and avoid deep menu chains.

---

## Progression/town/service UX responsibilities
Progression-access screens must communicate:
- what systems are available now
- which upgrades or conversions are currently affordable
- what each action improves
- what the current medium-term goal might be

Required principle:
The player should not need to remember hidden sink locations or upgrade meanings across many screens.

---

## Information flow between screens
The system must maintain continuity of meaning across transitions.

Required transitions:
- world map -> run
- run -> post-run results
- post-run -> world map
- world map -> progression/service context
- progression/service context -> world map or next run prep

Each transition should preserve or restate enough context so the player does not feel lost.

---

## Goal visibility rules
The UX must help the player identify both immediate and medium-term goals.

Immediate goal examples:
- finish this run
- choose an upgrade
- beat or advance this map

Medium-term goal examples:
- unlock next node
- gather enough material for upgrade X
- prepare for boss/gate
- improve farm efficiency

The first version does not need a formal quest/task system, but goal visibility must still exist through information hierarchy.

---

## Push vs farm readability
The UI should help the player understand whether current content is being used for pushing or farming.

Possible indicators:
- node state and route position
- current reward emphasis
- difficulty or progression context
- automation/manual decision mode state

Required principle:
The player should usually know whether they are advancing, stabilizing, or preparing.

---

## Automation readability rules
Because combat is automated, the UI must help the player understand automation state.

Required behavior:
- player should know whether manual upgrade choice or auto-pick is active if relevant
- player should be able to tell when the build is succeeding or failing under automation
- UI should not make combat look random when it is actually system-driven

The interface must help players trust autobattle by making it legible.

---

## Error/friction reduction rules
The UX should reduce common friction points.

Required behavior:
- minimize unnecessary navigation between ordinary actions
- avoid hiding important progression actions behind deep menu chains
- avoid forcing the player to inspect too many details to make ordinary decisions
- provide clean stop points after resolved runs
- keep frequent actions faster than rare actions

The game should feel comfortable to return to repeatedly.

---

## Notification/feedback rules
The interface should surface important state changes clearly.

Important events include:
- node unlocked
- node cleared
- major reward gained
- boss defeated
- progression milestone reached
- new system unlocked
- upgrade affordability reached

The first version should keep feedback clear but not over-animated or noisy.

---

## MVP screen/function groups
The first playable version should support at least these UX groups.

### 1. World/progression selection screen
Needed for:
- node selection
- route understanding
- current progression state

### 2. Run HUD
Needed for:
- survival state
- run/node context
- upgrade choices when relevant
- readable autobattle state

### 3. Post-run summary/state screen or panel
Needed for:
- rewards
- progress changes
- next actions

### 4. Progression/service access screen
Needed for:
- spending rewards
- build/progression adjustments

These groups may be implemented as separate screens or compact panels depending on MVP scope.

---

## MVP requirements
The first playable version must support:
- readable world map / node-selection interface
- readable run HUD
- readable post-run reward/progress summary
- clear access to at least one progression/service context
- ability to understand current goal and next action without hidden information
- low-friction repeated farming flow

MVP may omit:
- deep analytics screens
- highly detailed combat breakdown panels
- large codex/tutorial systems
- heavy notification systems
- many layered tabs for the same function

---

## MVP priorities
Focus on:
- clarity of current state
- clarity of rewards and progress
- simple world navigation readability
- readable autobattle HUD
- low-friction loop transitions
- clear build/progression access
- natural stopping points

Avoid in MVP:
- excessive HUD clutter
- many overlapping menus
- deeply nested progression screens
- overly flashy feedback that hides useful information
- heavy explanation systems before core readability is proven

---

## Data model requirements
Minimum required UX-facing data categories:
- world node states
- current selected node/goal context
- run-state summary data
- player survival/progress summary data
- reward payload summaries
- progression affordability/availability state
- unlock/change notifications

Optional later data:
- advanced run metrics
- detailed drop breakdowns
- saved loadout summaries
- automation profile indicators
- goal pinning/tracking data

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not hide current progression state behind multiple screens by default.
- Do not make ordinary reward meaning hard to read.
- Do not make autobattle outcomes opaque.
- Do not require players to inspect deep stats for ordinary decisions.
- Do not add many overlapping UI flows for the same purpose.
- Do not make common actions slower than rare actions.
- Do not overload the run HUD with information that is not actionable or readable.
- Do not make push vs farm context hard to distinguish.

---

## Extension points
The UX/HUD layer must support later addition of:
- more detailed automation indicators
- more detailed build/loadout views
- advanced reward breakdowns
- pinned goals or task-tracking helpers
- town-specific service screens
- inventory/loot-management views
- progression-tree navigation
- analytics/history panels

These additions must extend the readable core loop UX, not replace it.

---

## Out of scope
This spec does not define:
- final art direction for UI
- exact HUD layout coordinates
- exact control bindings
- exact font/icon choices
- exact animation timings
- exact tutorial text

---

## Validation checklist
- [ ] The player can understand world state and reachable nodes.
- [ ] The player can understand current run state during combat.
- [ ] The player can understand what rewards were gained after a run.
- [ ] The player can understand what progression changed after a run.
- [ ] The player can identify a reasonable next action.
- [ ] Repeated farming remains low-friction from an information-flow perspective.
- [ ] Autobattle remains readable rather than opaque.
- [ ] The UX can later expand to deeper service, inventory, and progression screens without re