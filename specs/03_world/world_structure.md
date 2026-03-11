# World Structure

## Purpose
Define how the game world is organized in the first playable version and establish the structural rules that later map, node, route, and progression specs must follow.

---

## Scope
This spec defines:
- world hierarchy
- region structure
- node-based world organization
- route connectivity rules
- allowed branching complexity
- relationship between world structure and progression
- minimal MVP world structure requirements
- constraints for later specs

This spec does not define:
- exact node unlock logic
- exact node completion rules
- exact combat behavior
- exact reward tables
- exact town mechanics
- exact visual representation of the map
- exact save behavior

---

## Core world statement
The world is a persistent progression structure made of regions, and each region contains a connected set of nodes.

The world must feel like:
- forward movement into deeper content,
- meaningful route choice,
- repeated return to earlier locations,
- persistent expansion of accessible space.

The world must not feel like a flat stage list or a disconnected menu of isolated battles.

---

## World hierarchy
The world hierarchy is:

1. **World**
   - the full progression space available to the player over time

2. **Region**
   - a major world segment with its own progression role, difficulty band, and resource identity

3. **Node**
   - the smallest world progression unit shown on the world map
   - can represent combat, elite, boss, loot, town, progression, or other route functions

4. **Run instance**
   - one execution of playable content inside a node

This hierarchy must remain stable across later systems.

---

## Structural roles
### World role
The world provides long-term progression shape.

World-level responsibilities:
- organize the overall order of progression
- define how the player moves into deeper content
- preserve relevance of earlier content
- support future expansion by adding regions and route variations

### Region role
A region is a medium-scale progression container.

Region-level responsibilities:
- group nodes into a recognizable progression area
- define a difficulty band
- define resource identity
- define progression role within the world
- provide local route structure

### Node role
A node is the basic route element.

Node-level responsibilities:
- provide one meaningful progression step
- connect to other nodes through explicit route links
- represent a clear gameplay purpose
- participate in push/farm decisions

---

## Region model
Each region must function as a persistent area of the world.

Minimum region properties:
- unique identifier
- progression order index or equivalent world-depth marker
- set of nodes
- entry node or entry condition
- local route connectivity
- intended difficulty band
- intended resource identity
- at least one forward progression output

Region properties may later expand, but these are the minimum structure requirements.

---

## Region design rules
- Each region must have a clear progression role.
- Each region must contain more than one node.
- Each region must support both pushing and later revisiting.
- Each region should provide some unique or relatively distinct value.
- A region should not exist only as a one-time corridor if the broader design wants old content to stay relevant.
- Later regions should generally represent deeper progression, but older regions must still keep farming value.

---

## Node-based world model
The world is represented as a graph of connected nodes.

Each node must:
- belong to one region
- have a node role/type
- have explicit inbound and/or outbound connections
- have a state such as locked / available / cleared / mastered-equivalent
- be visible or discoverable through progression

Node connectivity is part of gameplay structure, not decoration.

---

## Node roles in world structure
World structure must allow multiple node roles.

Minimum world-compatible node roles:
- `combat`
- `boss_or_gate`
- `service_or_progression`

Optional early roles:
- `elite`
- `loot`
- `town`
- `special`

The first playable version may implement only a subset, but world structure must not assume all nodes are identical combat stages.

---

## Connectivity model
Nodes are connected through explicit route links.

Each route link means:
- the player can move from source node to target node if unlock conditions are satisfied
- the target node becomes part of the reachable progression space

Connectivity must support:
- forward progression
- backtracking to older nodes
- limited route choice over time

Connectivity must not assume one-way disposable progression only.

---

## Branching model
The world should use limited branching, not uncontrolled complexity.

### MVP branching rule
The first playable version should support one of the following:
- mostly linear progression with occasional branches
- very small local branching per region

### Branching design goal
Branching should add:
- route choice
- risk/reward variation
- resource-path variation
- access to useful service or progression nodes

Branching should not:
- overwhelm readability
- require complex planning to make basic progress
- make lazy/autobattle play hard to understand

---

## World readability rules
The player must be able to understand:
- current region
- currently available nodes
- locked versus unlocked paths
- rough direction of forward progression
- available backtracking options

The world structure must remain readable even after more nodes and regions are added.

---

## Backtracking rule
Backtracking is required.

The world must allow the player to revisit older nodes and older regions for:
- farming
- safer progression
- resource targeting
- mastery
- efficiency gains
- preparation for harder content

Backtracking is not a fallback failure state.
It is a normal part of the intended progression rhythm.

---

## World progression relationship
World structure must support the progression hierarchy defined in `progression_structure_unlock_flow`.

Required relationship:
- nodes are the immediate progression steps
- regions are the medium-scale progression containers
- unlock flow expands reachable nodes first
- region progression expands reachable world space over time
- backtracking remains valid after forward expansion

World structure provides the space in which unlock flow operates.
It does not replace unlock flow logic.

---

## Region progression relationship
A region should generally contain:
- entry content
- internal progression nodes
- at least one important forward gate or output

A region may later contain:
- optional branches
- side farming nodes
- elite routes
- service/town nodes
- boss gate paths

The region structure must support both simple MVP progression and later expansion.

---

## Resource identity relationship
World structure should allow regions to differ in resource value.

Minimum rule:
- different regions should be allowed to provide different farming value or resource identity

This is required so that old regions can stay useful instead of being automatically replaced by newer ones.

Exact resource design belongs to later specs.

---

## Difficulty relationship
World structure should imply broad progression depth.

Minimum rule:
- later regions usually represent higher difficulty or higher progression expectation than earlier regions

Important exception:
- earlier regions must still remain useful for farming and progression support even if they are no longer the main push target

Exact scaling rules belong to later specs.

---

## MVP world structure requirements
The first playable version must support:
- at least 2 regions or an equivalent structure that proves region-to-region progression
- each region containing multiple nodes
- at least one forward route from one region into another
- node connectivity visible on a world map or equivalent progression screen
- backtracking to earlier reachable nodes
- at least one non-combat route role somewhere in the reachable world structure
- limited branching or a clear placeholder structure that supports branching later

If MVP scope must be reduced, region count may be reduced only if the implementation still proves:
- node connectivity
- forward progression
- backtracking
- future region extensibility

---

## MVP priorities
Focus on:
- one readable world graph
- low branching complexity
- clear region boundaries
- explicit node connections
- easy understanding of where to go next
- easy understanding of where to farm
- stable support for backtracking

Avoid in MVP:
- large world size
- many region types
- many node roles before route readability is proven
- highly tangled graph structures
- procedural world layouts if they reduce clarity

---

## Data model requirements
The world structure must be representable in data.

Minimum required data entities:
- `World`
- `Region`
- `Node`
- `NodeConnection`

Minimum required node data:
- node id
- region id
- node role/type
- node state
- connection list
- unlock prerequisite reference(s)

Minimum required region data:
- region id
- region order/depth marker
- list of node ids
- entry reference

Exact schema belongs to implementation, but the world must be data-driven enough to allow expansion without hardcoding all routes manually in logic code.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not reduce the world to a flat list of isolated stages.
- Do not make all nodes identical in world role.
- Do not remove backtracking from the intended progression rhythm.
- Do not make branching so complex that route readability breaks.
- Do not make regions disposable one-time content only.
- Do not make world progression independent from node progression.
- Do not force exact visual complexity in order to express the world structure.
- Do not couple world structure too tightly to one specific temporary system.

---

## Extension points
The world structure must support later addition of:
- more node roles
- more regions
- optional side routes
- elite paths
- boss gate paths
- town hubs
- service clusters
- region-specific mechanics
- special event nodes
- alternate unlock routes

These additions must extend the structure, not replace the region -> node -> route foundation.

---

## Out of scope
This spec does not define:
- exact node completion rules
- exact unlock thresholds
- exact combat contents of nodes
- exact town feature list
- exact map art style
- exact UI layout for the world map
- exact save representation

---

## Validation checklist
- [ ] The world is organized into regions.
- [ ] Regions contain multiple connected nodes.
- [ ] Nodes have explicit roles/types.
- [ ] Nodes have explicit route connections.
- [ ] The player can progress forward through the world.
- [ ] The player can backtrack to older reachable nodes.
- [ ] The world is readable as connected progression, not isolated stages.
- [ ] The structure supports limited branching.
- [ ] The structure supports future addition of more node roles and regions without