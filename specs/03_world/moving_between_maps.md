# Moving Between Maps

## Purpose
Define how the player moves between nodes/maps in the world structure and establish the navigation rules that later map, progression, UX, and save specs must follow.

---

## Scope
This spec defines:
- when movement between nodes is allowed
- what movement actions the player can take
- forward movement rules
- backtracking rules
- movement availability states
- minimal MVP movement requirements
- constraints for later specs

This spec does not define:
- exact unlock thresholds
- exact node completion rules
- exact world graph layout
- exact UI layout of the world map
- exact animation or visual transition style
- exact save serialization

---

## Core movement statement
Movement between maps is world-map navigation between connected nodes.

Movement must support:
- forward progression into newly unlocked content
- return to previously reachable content
- readable route choice
- low-friction repeated farming

Movement is a progression action, not only a presentation detail.

---

## Movement unit
The smallest movement unit is:
- move from one world node to another connected world node

The player does not move freely through continuous world space in the core design.
The player selects connected progression nodes.

---

## Movement prerequisites
Movement from node A to node B is allowed only if all of the following are true:
- node B exists as a connection target from node A or is otherwise globally reachable by allowed backtracking rules
- node B is in a reachable state
- any required unlock conditions for node B have been satisfied
- the player is not blocked by an unresolved higher-priority transition state if such a system exists later

In MVP, movement should remain simple and readable.

---

## Reachability states
Each node must be in one of these movement-relevant reachability states.

### `locked`
- cannot be entered
- may be visible or hidden depending on world presentation rules
- cannot be selected as a destination

### `available`
- can be selected and entered
- is part of the current reachable world space

### `cleared`
- remains selectable if replay is allowed
- may function as a stable movement anchor for forward progression or backtracking

### `mastered` or equivalent later-state flag
- still selectable
- usually better suited for low-friction farming
- may unlock additional automation or convenience behavior later

Movement rules depend on these states but do not redefine them.

---

## Allowed movement actions
The movement system must support these action categories.

### 1. Forward move
Move from the current node to a newly reachable deeper node.

Used for:
- pushing progression
- accessing new route segments
- reaching new region content

### 2. Lateral move
Move from one reachable node to another reachable node in the same local world area when allowed by route structure.

Used for:
- route choice
- side farming
- service node access
- optional content access

### 3. Backtrack move
Move from the current node or current region back to previously reachable content.

Used for:
- farming
- safer progress
- targeted resource runs
- preparation for harder content

### 4. Resume move
Return to the currently relevant progression location after farming or interruption.

Used for:
- session continuity
- quick re-entry into current push target
- reducing friction between farming and pushing

---

## Forward movement rules
Forward movement must follow the world structure and unlock flow.

Minimum rules:
- forward movement can only target nodes that are connected and unlocked
- forward movement should represent progression into deeper or alternative content
- forward movement should remain readable to the player
- at least one clear forward option should generally exist when progression is available

Forward movement should not require hidden path logic in MVP.

---

## Backtracking rules
Backtracking is required.

Minimum rules:
- previously reachable nodes remain revisitable unless explicitly restricted by a future special system
- backtracking must be low friction
- backtracking must not erase or reset world progression
- backtracking must support farming and recovery loops

Backtracking is a core part of the intended gameplay rhythm.
It is not a failure-only behavior.

---

## Route choice rules
When multiple reachable destinations exist, movement must support route choice.

Minimum route choice goals:
- choose between pushing and farming
- choose between safer and riskier available content
- choose between progression-oriented and utility-oriented routes when such routes exist

In MVP, route choice may remain limited.
The system only needs enough route choice to prove that the world is not a flat linear list.

---

## Entry model
Selecting a destination node must transition the player into that node’s playable or interactive state.

Minimum rule:
- selecting an available destination node and confirming entry should start that node’s content flow

Depending on node role, this may mean:
- start combat run
- open town/service interaction
- enter progression interaction
- enter special node flow

The movement system hands off control to the node role system.

---

## Return model
After completing or leaving node content, the player must return to a world-level state where the next destination can be chosen.

Minimum rules:
- node result is applied before the player chooses the next move
- newly unlocked destinations become available at this point
- the player can choose to continue, backtrack, replay, or stop

The player should not become trapped in forced chain transitions unless intentionally defined by a later feature.

---

## Movement and progression relationship
Movement depends on progression state, but is not identical to progression.

Required relationship:
- unlock flow determines what destinations become reachable
- movement system determines where the player can go now
- world structure determines what nodes and links exist

This spec only defines the rules of navigation across that structure.

---

## Movement and farming relationship
Movement must support frequent repetition of known farming routes.

Required behavior:
- player can quickly revisit previously useful nodes
- moving to older farming content should not require excessive navigation friction
- returning from farming to current push path should also remain low friction

Future systems may add shortcuts or convenience features, but the base navigation model must already support this rhythm.

---

## Region transition relationship
Movement between maps may also include crossing region boundaries.

Minimum rule:
- if a node connection crosses into another region and the destination is unlocked, the player can move across that region boundary through the same node-selection model

Region transitions do not require a separate movement system in MVP.
They are just movement across nodes with region metadata.

---

## Fast return / convenience expectations
The movement system should support future low-friction convenience features.

Future-compatible expectations:
- resume current push path quickly
- reopen recently used farming nodes quickly
- reduce repeated navigation overhead for known routes

MVP does not need advanced shortcut systems, but must not block them architecturally.

---

## MVP requirements
The first playable version must support:
- selecting an available node from a world map or equivalent progression screen
- entering that node
- returning to a world-level selection state after node completion
- moving forward to a newly unlocked node
- backtracking to a previously reachable node
- at least one case of route choice between multiple reachable destinations
- movement across at least one region boundary or an equivalent extensible placeholder

---

## MVP priorities
Focus on:
- clear destination availability
- low-friction node selection
- readable forward progression
- easy backtracking
- clean return to world map after node resolution
- minimal navigation overhead for repeat farming

Avoid in MVP:
- complex travel simulations
- hidden navigation rules
- large numbers of movement states
- forced pathing sequences that reduce readability
- presentation-heavy travel systems that do not add progression value

---

## Data model requirements
The movement system must be data-driven enough to navigate the world graph.

Minimum required data:
- current node reference
- reachable node set or resolvable reachability logic
- node connection data
- node state data
- region membership data

Optional later convenience data:
- last visited push node
- recent farming nodes
- preferred route markers

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not require free manual movement through continuous world space for core navigation.
- Do not make backtracking hard or expensive by default.
- Do not hide basic destination availability behind obscure rules.
- Do not make farming routes cumbersome to revisit.
- Do not make node entry dependent on unrelated side systems in the core flow.
- Do not force the player through long transition chains for ordinary progression movement.
- Do not break readability of forward versus optional destinations.

---

## Extension points
The movement system must support later addition of:
- more branching paths
- route shortcuts
- favorite/recent node access
- region hubs
- town fast-travel behaviors
- special one-way or gated routes
- alternate unlock routes
- convenience navigation for mastered content

These additions must extend the node-to-node navigation model, not replace it.

---

## Out of scope
This spec does not define:
- exact unlock conditions
- exact node graph contents
- exact UI controls
- exact transition animations
- exact save/load resume rules
- exact town entry flow

---

## Validation checklist
- [ ] Player can select an available node as a destination.
- [ ] Player can enter the selected node.
- [ ] Player returns to world-level navigation after node resolution.
- [ ] Forward movement to newly unlocked content works.
- [ ] Backtracking to previously reachable content works.
- [ ] At least one route choice between multiple reachable nodes exists.
- [ ] Movement supports repeated farming without high friction.
- [ ] Region transitions can be represented through the same node-to-node model.
- [ ] The movement system can be extended later without replacing the core navigation model.

