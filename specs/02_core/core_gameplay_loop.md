# Core Gameplay Loop

## Purpose
Define the gameplay loop for the first playable version and establish the rules that later systems must follow.

---

## Scope
This spec defines:
- repeated player flow
- push vs farm rhythm
- active vs auto interaction level
- what counts as forward progress
- minimal MVP loop requirements
- constraints for later specs

This spec does not define:
- combat formulas
- exact stats
- exact AI behavior
- exact node type balance
- exact reward tables
- exact save behavior
- exact UI layout

---

## Core loop
`choose node -> run autobattle -> gain kill progress + rewards -> unlock/spend/improve -> choose push or farm -> repeat`

---

## Core rules
- Combat is primarily automatic.
- The player does not manually control movement or attacks in the core loop.
- Enemy kills are the main source of node unlock progress.
- Every run must produce a state change: rewards, progress, unlocks, or account growth.
- Every session should produce meaningful forward value, even if short.
- Old nodes must remain useful after new nodes are unlocked.
- Permanent progression matters more than one-run success.
- New content should require more attention than mastered content.
- Mastered content should become lower-friction and more automation-friendly.
- The world must feel like connected progression, not isolated stages.
- Both pushing and farming are intended play states.

---

## Repeated player flow
### Before run
- choose node
- choose goal: push or farm
- configure build/loadout
- configure automation options if available
- start run

### During run
- autobattle resolves most combat actions
- player may choose power-ups / level-up options
- player observes build stability and efficiency
- kills accumulate node progress and rewards

### After run
- rewards are granted
- node state is updated
- unlock progress is updated
- player may spend resources on persistent growth
- player chooses next action: replay / push / backtrack / visit service node / stop

---

## Forward progress definition
Forward progress may include one or more of the following:
- node unlock progress increased
- new node unlocked
- route advanced
- resources gained
- persistent upgrade purchased
- build power improved
- automation capability improved
- old content efficiency improved

A run does not need to advance all categories, but should usually advance at least one.

---

## Push state
Use push state when attempting to advance into newer or harder content.

Push state properties:
- lower efficiency
- higher uncertainty
- more manual decision-making
- higher progression value
- more likely to reveal build weaknesses

Push state goals:
- unlock next node
- unlock route branch
- reach better rewards or progression nodes
- test whether current build is sufficient

---

## Farm state
Use farm state when stable gains are needed.

Farm state properties:
- higher consistency
- lower risk
- lower attention requirement
- higher automation suitability
- more reliable resource generation

Farm state goals:
- gather resources
- improve permanent systems
- improve build efficiency
- stabilize automation
- prepare for the next push attempt

---

## Attention model
### Active decision mode
Used mainly for new or uncertain content.

Requirements:
- player can manually choose upgrades during runs
- player can observe build performance closely
- player can adjust setup between runs

### Auto mode
Used after some mastery/progression threshold.

Requirements:
- more run decisions can be automated
- repeated runs become lower-friction
- content becomes suitable for comfortable farming

### AFK-friendly farming state
Used for well-known and sufficiently safe content.

Requirements:
- low interaction demand
- predictable returns
- meaningful but not optimal efficiency
- supports long-term resource generation

---

## MVP requirements
The first playable version must support:
- world map with selectable available nodes
- at least one combat node type
- autobattle as default combat resolution
- kill-based node progress
- run rewards
- persistent post-run upgrade spending
- replaying old nodes
- unlocking new nodes
- basic distinction between pushing and farming
- clear post-run decision step

---

## MVP priorities
Focus on:
- one stable repeatable loop
- visible node progress
- repeatable farming value
- one persistent upgrade sink
- basic automation support
- clear post-run rewards
- readable route progression

Avoid in MVP:
- many currencies
- many side systems
- complex route structures
- complex run modifiers
- high content quantity before loop quality is proven

---

## Run output requirements
Each run must output at least:
- reward payload
- node progress delta
- node completion or partial completion state
- persistent progression delta if applicable
- unlock result if applicable

Even incomplete or failed runs should usually provide some useful value.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make manual control mandatory for core progression.
- Do not make node unlock progression depend mainly on rare scripted objectives.
- Do not make old nodes obsolete.
- Do not make farming produce zero meaningful account progress.
- Do not make automation equal to fully optimal play too early.
- Do not make all value come only from forward pushing.
- Do not make the loop depend on long uninterrupted sessions.
- Do not make route choice so complex that lazy play becomes unreadable.
- Do not make post-run flow long or unclear.

---

## Extension points
The loop must be extensible to support later addition of:
- more node types
- branch paths
- service/town/progression nodes
- elite and boss gates
- more automation layers
- map mastery systems
- character roster expansion
- additional persistent progression systems
- offline/AFK improvements

These additions must not require replacing the core loop formula.

---

## Out of scope
This spec does not define:
- damage formulas
- stat list
- drop formulas
- enemy AI details
- boss mechanics
- town feature list
- exact power-up pool
- exact save format
- UI screen structure

---

## Validation checklist
- [ ] Player can choose a node and start a run.
- [ ] Combat mostly resolves automatically.
- [ ] Player can optionally choose upgrades during runs.
- [ ] Killing enemies increases node progress.
- [ ] Runs grant rewards.
- [ ] Rewards can be converted into persistent progress.
- [ ] New nodes can be unlocked.
- [ ] Old nodes remain replayable and useful.
- [ ] Player can choose between pushing and farming.
- [ ] There is a clear post-run decision step.
- [ ] The loop supports both active and low-attention play.
- [ ] Later systems can extend the loop without replacing it.
