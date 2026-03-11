# Run / Session Structure

## Purpose
Define what a run is, what a session is, how runs begin and end, how sessions are composed of runs, and what structural rules later combat, save, automation, UX, and progression specs must follow.

---

## Scope
This spec defines:
- run boundaries
- session boundaries
- relationship between runs and node progression
- relationship between runs and world-map flow
- expected short-session and long-session structure
- stopping points
- minimal MVP run/session requirements
- constraints for later specs

This spec does not define:
- exact combat formulas
- exact node unlock thresholds
- exact reward numbers
- exact save serialization
- exact offline progress rules
- exact UI layout

---

## Core statement
The game is played as a sequence of runs launched from world-map nodes.

A run is one execution of a node’s content.
A session is one play period containing one or more runs plus the between-run decisions that connect them.

The structure must support:
- short sessions with meaningful progress,
- long sessions with push/farm rhythm,
- clear stopping points,
- low-friction return to world-level decisions after each run.

---

## Definitions
### Run
One discrete execution of a node’s content.

A run:
- starts from entering a selected node,
- produces combat and/or node-role-specific output,
- ends in a resolved state,
- returns the player to a post-run world-level decision point.

### Session
One player play period that may include:
- world-map navigation,
- one or more runs,
- reward review,
- build adjustment,
- progression spending,
- route decisions,
- stopping and later resuming.

### Post-run state
The state after a run is resolved and before the next destination is chosen.

This is a critical structural state and must remain clear and low-friction.

---

## Run structure
A run must follow this high-level structure:
1. node selected
2. run initialized
3. node content executed
4. run end condition reached
5. run results computed
6. persistent state updated
7. player returned to post-run state

This sequence must remain stable even if later node types add variant behavior.

---

## Session structure
A session must follow this high-level structure:
1. player enters the game
2. current world/progression state is loaded
3. player chooses a goal
4. player performs one or more runs
5. between runs, player may:
   - review rewards
   - spend resources
   - adjust build
   - change route goal
   - stop playing
6. session ends at a stable stopping point

---

## Run start rules
A run begins when the player confirms entry into a selected available node.

Minimum run start requirements:
- node destination is known
- relevant node state is loaded
- player/build state is loaded
- run-specific temporary state is initialized
- run control is handed to the node content flow

In MVP, run start must be fast and readable.
It should not require excessive setup steps.

---

## Run end rules
A run ends when that node’s current run-end condition is satisfied.

Allowed MVP end conditions:
- node content resolved successfully
- player/build fails but run still resolves
- player exits through an allowed early termination rule if such a rule exists in MVP

Minimum run end requirements:
- rewards are computed
- persistent node progress is computed
- persistent account changes are computed
- unlock changes are applied if applicable
- run temporary state is finalized
- player is returned to a post-run decision state

A run must always end in a resolved state.

---

## Run result model
Every run must produce a result packet.

Minimum run result contents:
- node id
- run resolution state
- reward payload
- node progress delta
- persistent progression delta if applicable
- unlock result if applicable
- next available actions context

Run results must be sufficient to support:
- reward display
- progression updates
- replay decision
- push/farm decision
- save/resume behavior

---

## Temporary vs persistent state
### Temporary run state
Resets between runs.

Examples:
- temporary power-ups
- in-run level-ups
- temporary combat-state variables
- run timer/state if used

### Persistent state
Survives after run end.

Examples:
- node progress
- node clear state
- unlocked nodes
- resources
- permanent upgrades
- automation unlocks
- build/loadout selections if persistent by design

The run/session structure must keep these two state categories clearly separated.

---

## Relationship between runs and map completion
A run is not the same thing as beating a map.

Required behavior:
- one run may partially progress a map
- one run may clear a map
- multiple runs may be required to clear one map
- cleared maps may still be replayed in later runs

This relationship must remain explicit in implementation.

---

## Relationship between runs and sessions
Runs are the building blocks of sessions.

Session expectations:
- a short session may contain only one or a few runs
- a long session may contain many runs and several push/farm transitions
- a session should still feel meaningful even without a major unlock

The structure must support low-friction repetition without becoming unreadable.

---

## Short session model
A short session should support:
- opening the game,
- understanding current progression state quickly,
- starting a run quickly,
- gaining some useful value,
- returning to a stable stopping point,
- leaving without losing meaningfully resolved progress.

A short session may consist of:
- one push attempt,
- one or a few farm runs,
- one reward/spend cycle,
- stop.

---

## Long session model
A long session should support:
- several consecutive runs,
- at least one push/farm transition,
- route choice,
- resource spending,
- deeper progression decisions,
- repeated use of cleared nodes for farming,
- continued forward progression when available.

A long session should feel like a chain of meaningful state changes, not one uninterrupted combat block.

---

## Stopping point rules
The system must provide natural stopping points.

Minimum stopping point rule:
- after each run resolves and its results are applied, the player must be in a stable state where they can stop safely

Desirable stopping point properties:
- world-level state is known
- rewards are already granted
- current node/route context is known
- no unresolved temporary state remains

The game must not require long uninterrupted sessions for ordinary progress.

---

## Session goal model
At the start of a session, the player should usually be able to pursue one of a small number of goal types.

Minimum goal types:
- push deeper content
- farm known content
- gather resources for a persistent upgrade
- retry a recently unlocked node

The run/session structure should make these goals easy to enter and easy to switch between.

---

## Push/farm rhythm inside sessions
Sessions must support alternating between two valid states.

### Push sequence example
- select deeper node
- attempt progression run
- gain some unlock progress or discover weakness
- decide whether to retry or retreat

### Farm sequence example
- select known node
- run low-friction repeat content
- gain resources/progression
- spend upgrades
- return to push target later

The session structure must make this alternation natural, not awkward.

---

## Post-run decision state
After every run, the player must return to a decision state.

Minimum available actions from this state:
- replay current node if allowed
- move to another reachable node
- spend or inspect rewards/progression
- stop session

Optional later actions:
- quick resume current push path
- visit town/service node immediately
- apply automation presets
- inspect detailed run summary

This post-run state is a core structural requirement.
It should remain short and readable.

---

## Session continuity expectations
The run/session structure must support continuity across interrupted play.

Minimum future-compatible expectations:
- player can stop after a resolved run without ambiguity
- the game can later resume from world-level state without needing to preserve unresolved temporary run state
- session continuity should center on post-run states, not mid-run dependence, unless a later save feature explicitly supports mid-run continuation

MVP may avoid mid-run persistence if needed.

---

## Node-role compatibility
The run/session structure must support more than one node role over time.

Minimum compatibility rule:
- combat nodes use full run structure
- non-combat nodes may use shorter interaction flows
- regardless of node role, the player must still return to a world-level post-node decision state

This allows service/town/progression nodes later without replacing session structure.

---

## MVP requirements
The first playable version must support:
- entering a run from a world node
- resolving a run
- applying rewards and progression after the run
- returning to a world-level post-run state
- replaying a node
- performing multiple runs in a session
- stopping safely after a resolved run
- meaningful short sessions
- meaningful long sessions

---

## MVP priorities
Focus on:
- fast run start
- clear run end
- visible result application
- stable post-run state
- low-friction repeat runs
- natural stopping points
- clear separation of temporary and persistent state

Avoid in MVP:
- overlong between-run sequences
- mandatory long sessions
- heavy setup before each run
- unclear run resolution states
- systems that block stopping after a resolved run
- many session-only mechanics before the base loop is proven

---

## Data model requirements
Minimum data categories required by run/session structure:
- current world-level position/context
- selected node reference
- temporary run state
- persistent state update payload
- resolved run result payload
- next action availability context

Optional later data:
- session statistics
- recent push target
- recent farming target
- recent route history
- resumable mid-run state if later supported

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make one run equal to permanent map completion by default.
- Do not make sessions require long uninterrupted play.
- Do not make post-run state unclear or overloaded.
- Do not blur temporary run state and persistent progression state.
- Do not make ordinary progression depend on rare one-run perfection.
- Do not remove replayability of cleared maps from the session rhythm.
- Do not create node role flows that bypass the world-level return state without explicit reason.

---

## Extension points
The run/session structure must support later addition of:
- richer run summaries
- auto-repeat farming
- quick resume to current push target
- service/town session detours
- session goals or reminders
- offline/AFK resolution layers
- optional mid-run suspend/resume
- more specialized node-role flows

These additions must extend the run -> post-run -> next decision structure, not replace it.

---

## Out of scope
This spec does not define:
- exact combat timing
- exact rewards
- exact UI screen flow
- exact save system behavior
- exact offline progress rules
- exact mastery rules

---

## Validation checklist
- [ ] Player can start a run from a selected node.
- [ ] Runs end in a resolved state.
- [ ] Run results apply persistent changes.
- [ ] Player returns to a world-level post-run decision state.
- [ ] Multiple runs can be chained in one session.
- [ ] Cleared nodes can be replayed in later runs.
- [ ] Short sessions provide useful progress.
- [ ] Long sessions support push/farm rhythm.
- [ ] Player can stop safely after a resolved run.
- [ ] The structure can support later non-combat node flows without redesign.
