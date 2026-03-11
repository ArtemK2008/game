# What Does It Mean to Beat a Map?

## Purpose
Define what counts as beating/completing a node/map in the first playable version and establish the completion model that later progression, automation, reward, and mastery specs must follow.

---

## Scope
This spec defines:
- what completion means
- map completion states
- difference between partial progress, clear, and mastery-ready states
- relationship between map completion and unlock flow
- minimal MVP completion requirements
- constraints for later specs

This spec does not define:
- exact combat formulas
- exact enemy compositions
- exact reward numbers
- exact mastery systems
- exact UI presentation
- exact save serialization

---

## Core completion statement
A map is considered beaten when the player satisfies that map’s primary completion condition and the map produces its intended progression result.

In the first playable version, the default meaning of beating a combat map is:
- complete a run on that node,
- accumulate enough map progress through enemy kills,
- reach the node’s clear threshold,
- update the node to a cleared state,
- unlock the next progression result if applicable.

Beating a map is therefore a progression state change, not only a momentary run result.

---

## Core rule
Default rule:

`enemy kills during runs -> map progress increases -> progress reaches clear threshold -> map becomes cleared -> next progression result becomes available`

This is the baseline definition of beating a standard combat map.

---

## Completion layers
Map completion has multiple layers.

### Layer 1: run completion
A single run ends and produces output.

Examples:
- run survived long enough
- run ended normally
- run ended by failure but still produced progress
- run granted rewards

Run completion does not automatically mean map completion.

### Layer 2: map progress completion
The map’s persistent progress meter reaches its clear threshold.

This is the default MVP meaning of “beat the map.”

### Layer 3: map clear state
The map enters a persistent `cleared` state.

Effects may include:
- unlock next node or branch
- remain replayable
- become eligible for lower-friction farming
- become a stable backtracking point

### Layer 4: post-clear optimization state
The map is still replayed after being cleared.

Examples:
- resource farming
- efficiency farming
- automation suitability
- mastery progress

This layer is not required to exist in full in MVP, but the completion model must allow it.

---

## Map completion states
Minimum required states:

### `locked`
- not yet reachable
- cannot be entered
- cannot be beaten

### `available`
- reachable and enterable
- not yet cleared
- progress meter may be at zero or nonzero

### `in_progress`
- entered at least once or has nonzero persistent map progress
- not yet cleared

### `cleared`
- main clear threshold reached
- intended unlock result already granted
- map remains replayable if replay is allowed

### `mastered` or equivalent later-state flag
- optional later state
- indicates deeper post-clear value
- not required for MVP

---

## Definition of beating a map in MVP
For MVP, a combat map is beaten when all of the following are true:
- the map is reachable,
- the player completes one or more runs on the map,
- enemy kills on those runs increase the map’s persistent progress,
- persistent progress reaches the clear threshold,
- the map state becomes `cleared`,
- the map’s defined unlock result is granted.

This definition allows a map to be beaten across multiple runs.
It does not require a one-run perfect clear.

---

## Partial completion
A run may contribute to beating a map without beating it fully.

Partial completion means:
- the run granted persistent map progress,
- the map did not yet reach clear threshold,
- the player is closer to clearing the map than before the run.

Partial completion is valid and important for the game’s lazy grind structure.
A failed or incomplete run may still count as meaningful progress if it advances map progress or account growth.

---

## Clear threshold model
Each combat map must have a persistent clear threshold.

Minimum MVP rule:
- threshold is primarily driven by enemy kills
- kill progress accumulates across runs
- once threshold is reached, the map is considered beaten

Future-compatible rule:
Later map types may add different or extra completion logic, but standard combat maps should keep the kill-progress backbone.

---

## Relationship between beating a map and unlocking content
Beating a map must usually cause one or more progression outputs.

Minimum expected outputs:
- unlock next node
- unlock next route step
- mark map as cleared
- preserve replay access

Optional later outputs:
- unlock branch choice
- unlock service/town node
- unlock region gate
- unlock automation convenience
- unlock mastery-related value

A map clear must matter.
It should not be only cosmetic.

---

## Relationship between beating a map and replaying it
Beating a map does not mean the map is finished forever.

Required behavior:
- a cleared map remains replayable unless a later special rule explicitly restricts it
- cleared maps continue to provide value through farming, resources, efficiency, or progression support
- clearing a map converts it from a push target into a potential farm target

This is a core part of the design.

---

## Relationship between beating a map and farming
After a map is beaten, it should usually become easier to treat it as farm content.

Expected post-clear behavior:
- map remains available
- player can revisit it intentionally
- repeated runs can still generate meaningful value
- friction to reuse it should not increase after clear

The game loop depends on maps remaining useful after being beaten.

---

## Relationship between beating a map and automation
Beating a map does not automatically mean the map is fully automated.

Required separation:
- `cleared` means main progression goal was achieved
- automation suitability may improve after clear
- full low-friction automation may depend on extra conditions later

This separation is important so that completion, mastery, and automation do not collapse into one state too early.

---

## Relationship between beating a map and mastery
Mastery is optional in MVP, but the completion model must allow it later.

Expected future relationship:
- `cleared` = primary map progression result achieved
- `mastered` = map has become a stronger farming/automation/resource target

Mastery should extend the meaning of post-clear repetition, not replace the clear state.

---

## Beating non-combat maps
Not all node roles need to use the same completion logic.

### MVP rule
The first playable version may define beating only for combat-oriented maps.

### Future-compatible rule
Later non-combat nodes may define completion differently.
Examples:
- town/service node visited
- loot node resolved
- progression node used
- boss/gate node cleared through its own rule

This spec defines the default combat-map completion model.
Other node roles may extend it later.

---

## Failure and incomplete runs
Failure does not automatically mean zero value.

Allowed behavior:
- incomplete run may still grant rewards
- incomplete run may still increase persistent map progress
- incomplete run may still contribute to account growth

Disallowed baseline behavior:
- most failed runs should not result in total wasted time unless explicitly designed otherwise for a special mode

This supports the intended lazy grind loop.

---

## MVP requirements
The first playable version must support:
- a persistent map progress meter for combat maps
- a clear threshold per combat map
- accumulation of map progress across runs
- transition from `available/in_progress` to `cleared`
- a clear unlock output when a map is beaten
- replay of cleared maps
- partial completion value from runs that do not yet clear the map

---

## MVP priorities
Focus on:
- one readable definition of map clear
- persistent progress across runs
- clear distinction between run result and map result
- visible transition into cleared state
- preserving value of cleared maps

Avoid in MVP:
- multiple competing completion definitions for standard combat maps
- one-run-only perfect clear requirements
- hidden clear conditions
- systems that make cleared maps disposable
- collapsing clear/mastery/automation into one state immediately

---

## Data model requirements
Minimum required map completion data:
- node id
- map state
- persistent map progress value
- clear threshold
- clear result reference
- replay availability flag or equivalent rule

Optional later data:
- mastery progress
- automation suitability level
- best efficiency metrics
- first-clear timestamp / flags

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not redefine standard combat map clear away from kill-driven progress without explicit reason.
- Do not make map clear depend mainly on rare scripted objectives for ordinary combat maps.
- Do not make maps one-and-done disposable content.
- Do not make failed runs usually worthless.
- Do not make clear state, mastery state, and automation state identical by default.
- Do not hide whether a map is cleared or not.
- Do not make replay of cleared maps feel like a design mistake.

---

## Extension points
The completion model must support later addition of:
- first-clear rewards
- mastery state
- post-clear farming bonuses
- automation unlock conditions
- alternate completion logic for special node roles
- boss/gate-specific clear rules
- branch unlock choices on clear

These additions must extend the clear model, not replace the basic combat-map meaning of “beat the map.”

---

## Out of scope
This spec does not define:
- exact kill numbers
- exact reward tables
- exact automation rules
- exact mastery mechanics
- exact UI widgets
- exact save format

---

## Validation checklist
- [ ] Combat maps have persistent progress.
- [ ] Progress accumulates across runs.
- [ ] Reaching clear threshold marks the map as cleared.
- [ ] Clearing a map grants a progression result.
- [ ] Cleared maps remain replayable.
- [ ] Failed or incomplete runs can still provide useful value.
- [ ] Clear state is distinct from later mastery/automation states.
- [ ] The completion model can support future