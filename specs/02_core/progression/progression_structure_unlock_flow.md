# Progression Structure / Unlock Flow

## Purpose
Define how progression is structured and how content is unlocked in the first playable version.

---

## Scope
This spec defines:
- progression layers
- what counts as unlocking content
- the order of progression decisions
- the relationship between node progression, world progression, and persistent progression
- minimal MVP unlock flow requirements
- constraints for later specs

This spec does not define:
- exact node layouts
- exact combat formulas
- exact AI rules
- exact resource numbers
- exact UI representation
- exact town features
- exact power-up pools

---

## Core progression statement
Progression is built around repeated completion of combat runs that:
- increase node progress,
- unlock new nodes and routes,
- generate resources,
- improve persistent systems,
- make future pushing easier and farming more efficient.

Progression must feel like expanding access, increasing efficiency, and reducing friction over time.

---

## Progression layers
The game has 4 progression layers.

### Layer 1: run progression
Short-lived growth during a single run.

Examples:
- level-up choices
- temporary power-ups
- temporary build shaping

Properties:
- usually resets after run ends
- affects immediate run performance
- helps clear current node more efficiently

### Layer 2: node progression
Progress tied to a specific node.

Examples:
- kill-based node unlock meter
- node completion state
- node mastery state
- automation suitability state

Properties:
- persists across runs
- determines when the node is considered cleared or sufficiently progressed
- controls access to new route options

### Layer 3: world/route progression
Progress tied to region and map connectivity.

Examples:
- newly unlocked node
- newly unlocked branch
- access to new region
- access to service or progression node

Properties:
- expands available choices on the world map
- is the main source of forward exploration
- depends primarily on node progression

### Layer 4: persistent/account progression
Long-term progression that outlives individual runs and local route progress.

Examples:
- account-wide upgrades
- character growth
- stronger automation
- more efficient farming
- permanent system unlocks

Properties:
- never resets during normal play
- increases future pushing and farming efficiency
- keeps old runs meaningful

---

## Unlock categories
Unlocks are divided into categories.

### Category 1: route unlocks
Unlock access to additional nodes.

Examples:
- next node on current route
- optional branch node
- side farming node

Main trigger:
- sufficient node progress on prerequisite node

### Category 2: region unlocks
Unlock access to deeper world areas.

Examples:
- next region
- next major world segment
- region gate opened

Main trigger:
- completion of required gate path, boss path, or progression threshold

### Category 3: system unlocks
Unlock mechanics not previously available.

Examples:
- automation features
- new persistent upgrade categories
- new node roles
- new progression sinks

Main trigger:
- persistent progression threshold, region milestone, or specific gate completion

### Category 4: efficiency unlocks
Unlock convenience or better farming quality.

Examples:
- auto-pick
- improved repeat farming
- improved node mastery rewards
- better resource conversion

Main trigger:
- mastery or progression threshold on known content

---

## Core unlock rule
The default unlock rule is:

`kill enemies on current combat node -> increase node progress -> reach unlock threshold -> unlock next available route content`

This is the backbone of progression.
All other unlock rules are extensions of this backbone, not replacements.

---

## Primary progression flow
The intended flow is:
1. enter available combat node,
2. kill enemies,
3. increase node progress,
4. gain rewards,
5. if threshold reached, unlock new route content,
6. if current efficiency is insufficient, return to farm,
7. improve persistent systems,
8. retry forward progression,
9. repeat.

---

## Forward progression definition
Forward progression means any state change that expands future capability.

Forward progression may include:
- unlocking a new node
- unlocking a new branch
- unlocking a new region
- unlocking a new system
- increasing node mastery or farming efficiency
- purchasing a persistent upgrade that enables future advancement

Forward progression is broader than only “reaching the next node.”
However, world expansion should remain the most visible form of progression.

---

## Node progression model
Each combat-oriented node must have a persistent progression state.

Minimum node progression state:
- `locked`
- `available`
- `in_progress`
- `cleared`
- `mastered` or equivalent later-state flag

Minimum node progression values:
- node unlock progress meter
- unlock threshold
- completion state
- replay availability
- optional mastery/automation state

### Core node rule
Enemy kills are the main input into node progress.

Optional future secondary inputs may exist, but the first implementation should keep node progression kill-driven.

---

## Unlock threshold model
A node unlocks subsequent content when its unlock progress reaches a threshold.

### MVP rule
- each combat node has one main progress meter
- meter increases mainly through enemy kills
- reaching threshold unlocks next route step

### Future-compatible rule
Later versions may add:
- bonus progress from elites
- gate conditions for special nodes
- multiple outputs from one node
- branch unlock choices

But the core rule must stay readable.

---

## Push and farm relationship to progression
Progression must support both push and farm states.

### Push contribution
Push runs contribute most strongly to:
- route unlocks
- region access
- new system access
- visibility of the next progression target

### Farm contribution
Farm runs contribute most strongly to:
- resource generation
- persistent upgrade progress
- efficiency unlocks
- mastery and automation readiness
- making later push attempts easier

Farm runs must still be part of the progression structure, not only resource maintenance.

---

## Persistent progression relationship
Persistent progression must reinforce the unlock flow instead of bypassing it entirely.

Required behavior:
- persistent upgrades should make unlocking easier, faster, safer, or more efficient
- persistent upgrades should not instantly invalidate the route structure too early
- persistent progression should reduce friction over time
- persistent progression should make old content more comfortable and new content more reachable

---

## Mastery relationship
Mastery is optional in MVP, but the progression structure must allow it later.

Expected mastery role:
- convert cleared nodes into better farming nodes
- unlock additional automation or convenience
- improve returns from old content
- create reasons to revisit cleared nodes

Mastery should deepen progression without replacing the main unlock flow.

---

## Unlock dependency model
Each unlockable element should depend on one or more prerequisite conditions.

Allowed dependency types:
- node progress threshold
- prerequisite node cleared
- prerequisite region reached
- persistent progression threshold
- required resource/payment
- system unlock prerequisite

Avoid hidden dependency chains in MVP.
Dependencies should be explicit and readable.

---

## World progression hierarchy
The progression hierarchy should be:
1. runs improve node progress and grant rewards
2. node progress unlocks route/world access
3. world access unlocks better rewards and systems
4. rewards improve persistent/account progression
5. persistent/account progression improves future runs

This hierarchy is the main progression engine.

---

## MVP requirements
The first playable version must support:
- locked and unlocked node states
- kill-based node progress
- unlock threshold per combat node
- unlocking at least one next node from a completed node
- replay of old nodes
- at least one persistent progression sink
- visible difference between push progression and farming for upgrades
- progression that remains useful across multiple sessions

---

## MVP priorities
Focus on:
- one readable unlock rule
- visible node progress
- stable unlock thresholds
- easy-to-understand prerequisites
- one persistent upgrade layer that feeds back into progression
- preserving value of old nodes

Avoid in MVP:
- many competing unlock conditions
- many progression layers with overlapping responsibilities
- hidden unlock rules
- excessive branching
- many different gate types before the basic loop is proven

---

## Run output requirements for progression
Each run must be able to change progression state through one or more of the following:
- node progress increase
- node state change
- route availability change
- persistent resource gain
- persistent upgrade progress
- mastery/automation progress if implemented

A run should rarely end with zero useful progression output.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not replace kill-based node progression as the default unlock backbone.
- Do not make progression depend mainly on rare scripted objectives.
- Do not make only forward pushing matter.
- Do not make farming disconnected from long-term progression.
- Do not make persistent upgrades completely skip the route structure too early.
- Do not make old nodes irrelevant after clearing them once.
- Do not make unlock dependencies hidden or hard to read.
- Do not create multiple progression systems that compete for the same role without clear separation.

---

## Extension points
The progression structure must support later addition of:
- node mastery
- branch unlock choices
- region gates
- boss gate logic
- special unlock conditions for rare nodes
- automation unlock tiers
- additional persistent progression sinks
- side progression loops attached to towns or service nodes

These additions must extend the existing structure, not replace the core unlock flow.

---

## Out of scope
This spec does not define:
- exact node graph design
- exact node content rules
- exact combat balance
- exact stats or scaling formulas
- exact AI behavior
- exact loot tables
- exact town mechanics
- exact UI widgets for progression display

---

## Validation checklist
- [ ] Combat nodes have persistent unlock progress.
- [ ] Enemy kills increase node progress.
- [ ] Reaching threshold unlocks new route content.
- [ ] Old nodes remain replayable after unlock.
- [ ] Persistent progression improves future advancement.
- [ ] Farming contributes to long-term progression.
- [ ] Unlock prerequisites are readable.
- [ ] There is a clear distinction between run progression and account progression.
- [ ] The structure can later support mastery, branches, and system unlocks without redesign.

