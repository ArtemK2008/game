# Locations

## Purpose
Define the location model for the first playable version and establish the structural rules that later world, enemies, loot, resources, visual style, music, progression, and UX specs must follow.

---

## Scope
This spec defines:
- what a location is in this project
- the relationship between locations, regions, and nodes
- location identity dimensions
- how locations support progression, farming, and world readability
- minimal MVP location requirements
- constraints for later specs

This spec does not define:
- exact world graph layout
- exact node unlock logic
- exact enemy stat values
- exact loot tables
- exact biome art implementation
- exact soundtrack list
- exact UI layout

---

## Core statement
Locations are the world-facing content contexts through which the player experiences progression.

Locations must:
- make the world feel like places rather than abstract stages,
- support both push and farm value,
- help distinguish regions and nodes mechanically and thematically,
- preserve the relevance of earlier content,
- remain readable enough for a grind-heavy autobattler.

Locations are not only visual wrappers.
They are content containers with progression, resource, and gameplay identity.

---

## Definition of a location
A location is a recognizable world content context associated with one or more nodes and expressed through a combination of mechanical identity and presentation identity.

A location may define or influence:
- region/area identity
- node environment/theme
- expected enemy profile emphasis
- expected resource/material identity
- progression role
- farming role
- atmosphere and presentation cues

A location is not the same thing as:
- the entire world graph
- a single run only
- a pure background image with no gameplay meaning
- a character/build state

---

## Core location rule
Default location flow:

`player reaches location through world progression -> location provides node content with recognizable identity -> location contributes to farming/progression/reward meaning -> location remains relevant according to its world role`

Locations must reinforce both forward movement and meaningful revisits.

---

## Location design principles
- Locations must have clear identity.
- Identity must be more than visuals alone.
- Locations must support progression and/or farming value.
- Different locations should create different expectations.
- Earlier locations must remain useful when possible.
- The first version should use a small number of strong location identities.
- Location complexity should remain readable under autobattle.
- Locations must remain extensible for future regions and node types.

---

## Relationship between locations, regions, and nodes
Locations must fit cleanly into the world hierarchy.

### Region
A region is a major progression container in the world structure.

### Node
A node is the smallest route/progression unit on the world map.

### Location
A location is the content identity layer applied to regions and/or nodes.

Required interpretation:
- a region may represent one major location identity,
- a region may later contain multiple sub-locations,
- nodes are the playable progression units that exist within a location context.

Locations help answer:
- where is the player?
- what kind of content should be expected here?
- why should this place matter now or later?

---

## Location system role in the game
Locations are one of the main ways the game expresses:
- world progression depth
- region identity
- resource identity
- enemy identity
- farming identity
- atmosphere and player memory of places

If locations are weak, the game risks feeling like a generic chain of functionally anonymous nodes.

---

## Required location identity dimensions
A location should be able to differ through one or more of these dimensions.

### 1. Progression role
Examples:
- early-game onboarding zone
- mid-route farming zone
- gate/boss approach zone
- resource-rich revisit zone

### 2. Resource/material identity
Examples:
- wood/fiber emphasis
- ore/stone emphasis
- poison/herb emphasis
- relic/ancient-material emphasis

### 3. Enemy profile emphasis
Examples:
- fast fragile enemies
- durable enemies
- pressure-heavy enemies
- later special enemy family bias

### 4. Farming role
Examples:
- stable soft-currency farm
- material-target farm
- efficient low-risk auto-farm zone
- milestone/rare-reward location later

### 5. Presentation identity
Examples:
- forest frontier
- cave network
- swamp basin
- ashland field
- ruins/ancient zone

The first version does not need all dimensions equally deep, but each location must differ by at least one meaningful gameplay-related axis.

---

## Location categories
The system should support a small number of recognizable location categories in the first version.

### 1. Core progression locations
Purpose:
- carry the main region-to-region advancement
- represent the backbone of the world path

### 2. Farm-support locations
Purpose:
- remain useful after initial progression
- provide resource/material or efficiency value

### 3. Milestone/gate-adjacent locations
Purpose:
- frame boss/gate progression
- signal increased importance or difficulty

These categories are structural. A single location may fulfill more than one role.

---

## MVP location model
The first playable version should use a small set of strong location identities.

### MVP-compatible interpretation
- each region may correspond to one main location identity
- each location may contain multiple nodes
- node variation can exist within a location without requiring many sub-location systems

The first version does not need a deep biome taxonomy.
It needs readable location distinctions that support progression and farming.

---

## Relationship between locations and progression
Locations must support the progression structure.

Required behavior:
- deeper progression should move the player through distinct locations over time
- locations should help communicate progression depth
- earlier locations should remain useful through farming or progression inputs
- locations should create reasons to revisit earlier world content

Locations help make progression feel spatial and world-based, not purely numeric.

---

## Relationship between locations and world structure
Locations must be compatible with node-based world structure.

Required behavior:
- nodes belong to locations/regions in a readable way
- location identity should help organize node expectations
- location changes should feel meaningful, not arbitrary

Locations do not replace nodes or regions.
They enrich them.

---

## Relationship between locations and resources/loot
Locations should help preserve economic identity.

Required behavior:
- different locations may emphasize different resource/material outputs
- location identity should help explain why the player farms one place over another
- older locations can stay relevant because of distinct outputs

Locations are one of the main ways the economy becomes spatially meaningful.

---

## Relationship between locations and enemies
Locations should help shape enemy expectations.

Required behavior:
- different locations may later emphasize different enemy profiles or families
- enemy identity should help locations feel mechanically distinct
- location and enemy identity should reinforce each other

The first version may keep this simple, but the structure must support it.

---

## Relationship between locations and push/farm rhythm
Locations should support both push and farm uses.

### Push-side location role
Examples:
- new place to unlock and stabilize
- harder zone with stronger progression value
- gateway into deeper content

### Farm-side location role
Examples:
- known low-risk source of useful materials
- efficient auto-farm location
- specific place to revisit for targeted value

A location should not become meaningless just because it is no longer the main push target.

---

## Relationship between locations and automation
Locations must remain readable in low-attention play.

Required behavior:
- location identity should still matter even when combat is automated
- location differences should show up through enemy mix, reward profile, or progression role
- location design must not rely on precise manual movement or environmental execution skill

Locations must fit the autobattle-first philosophy of the project.

---

## Relationship between locations and towns/service layers
Locations may contain or connect to town/service access later.

Required principle:
- service/town access should feel grounded in the world if explicit locations/nodes are used
- town/service presentation can remain simple in MVP even if location identity exists

Locations can strengthen the feeling that services belong to places in the world.

---

## Relationship between locations and presentation systems
Locations are one of the main anchors for visual style and music.

Required behavior:
- each location should be able to support a recognizable visual mood
- each location should be able to support recognizable sound/music mood later
- presentation should reinforce mechanical identity when possible

Presentation specs may expand this later, but the location system must support it.

---

## Relevance preservation rule
Locations must support continued relevance over time.

Required behavior:
- earlier locations should remain worth revisiting
- deeper progression should expand location choices, not erase all earlier value
- location identity should help explain why old content is still useful

This is one of the main strengths of the broader concept.

---

## Location state model
Minimum location-related state may include:
- location id
- associated region id or grouping reference
- unlocked/known state if relevant
- associated node set or node references
- location identity tags
- resource/enemy emphasis tags later

Optional later state:
- sub-location states
- mastery/relevance markers
- location event state
- region-wide service/town access state

Exact schema belongs to implementation.

---

## MVP requirements
The first playable version must support:
- at least 2 distinct location identities or region-as-location identities
- each location containing or representing multiple nodes
- visible location difference beyond only node order
- location relevance for either progression, farming, or both
- compatibility with future location-specific resources/enemies/presentation layers

If MVP scope is limited, location identity may be carried mainly by regions rather than sub-regional variation.

---

## MVP priorities
Focus on:
- a small number of strong location identities
- clear location role in progression and/or farming
- visible difference between places
- support for old-location relevance
- simple mapping from world structure to location identity

Avoid in MVP:
- many minor biome variants with no gameplay meaning
- deep sub-location hierarchies
- location complexity that adds little beyond presentation
- lots of unique assets before location role is proven
- turning locations into decorative labels only

---

## Data model requirements
Minimum required location-related data:
- location id
- parent region reference or equivalent grouping
- node membership or node association
- identity tags
- role tags (progression/farming/gate/etc.)

Optional later data:
- enemy family tags
- resource profile tags
- music/visual profile references
- sub-location definitions
- event pool references
- town/service association

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make locations purely decorative.
- Do not make all locations functionally identical in practice.
- Do not erase old-location value too quickly.
- Do not over-fragment the world into many tiny location types before strong identities are proven.
- Do not require location-specific manual execution mechanics for core value.
- Do not let presentation identity completely replace gameplay identity.

---

## Extension points
The location system must support later addition of:
- more distinct region identities
- sub-locations inside a region
- location-specific enemy families
- location-specific resource profiles
- location-linked town/service hubs
- special event locations
- location mastery systems
- stronger visual/audio differentiation

These additions must extend the readable location foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact map layouts
- exact enemy assignments
- exact resource tables
- exact biome art pipelines
- exact soundtrack mapping
- exact UI widgets for location display

---

## Validation checklist
- [ ] The game has at least 2 distinct location identities or region-as-location identities.
- [ ] Locations are more than visual wrappers.
- [ ] Locations support progression and/or farming meaning.
- [ ] Locations map cleanly onto the world structure.
- [ ] Old locations can remain relevant after deeper progression.
- [ ] The system can later support location