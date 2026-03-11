# Visual Style

## Purpose
Define the visual-style direction for the first playable version and establish the structural rules that later locations, UX, combat presentation, enemies, loot, towns, and marketing-facing materials must follow.

---

## Scope
This spec defines:
- what visual style must achieve in this project
- visual priorities for a lazy/autobattle progression game
- relationship between visual style and readability
- relationship between visual style and world/location identity
- minimal MVP visual-style requirements
- constraints for later specs

This spec does not define:
- final concept art
- exact color palettes
- exact shaders
- exact VFX implementations
- exact animation style guide
- exact asset pipeline details
- exact marketing key art

---

## Core statement
The visual style must make the game feel like a readable, grind-heavy world-progression autobattler where places matter, builds are legible, and repeated play remains comfortable over long sessions.

The visual style must:
- support gameplay readability,
- reinforce world/location identity,
- make progression feel spatial and tangible,
- remain comfortable under repeated farming,
- work well with automated combat and low-friction sessions,
- stay extensible for later content growth.

Visual style is not only aesthetic flavor.
It is a functional layer that supports player understanding and long-term comfort.

---

## Core visual rule
Default visual rule:

`the player must be able to read world state, combat state, rewards, and progression state without the visual presentation obscuring important information`

If visuals conflict with readability, readability wins.

---

## Visual design principles
- Readability is the first priority.
- Strong location identity is more important than raw visual complexity.
- Repeated content must remain comfortable to look at for long sessions.
- Visuals must support autobattle legibility.
- The first version should prefer strong simple shapes over high detail density.
- Important gameplay states must be visually distinguishable.
- Presentation should reinforce progression, not distract from it.
- The style must remain scalable as more regions/nodes/enemies/systems are added.

---

## Visual system role in the game
Visual style is one of the main ways the game expresses:
- world progression depth
- location identity
- combat clarity
- build activity and power growth
- milestone importance
- farming comfort and long-session usability

If visual style is weak or unfocused:
- autobattle becomes harder to trust,
- places feel forgettable,
- progression feels abstract,
- repeated farming becomes tiring faster.

---

## Primary visual goals
The visual style must support these goals.

### 1. World readability
The player should understand where they are in the world and what type of place they are in.

### 2. Combat readability
The player should understand what is happening during autobattle without needing to parse visual chaos.

### 3. Progression readability
The player should feel when content becomes deeper, harder, or more important.

### 4. Long-session comfort
The game should remain visually comfortable during repeated farming and long play sessions.

### 5. Identity and memory
Locations, enemies, and important progression moments should be memorable enough that the world feels like places, not anonymous stages.

---

## Relationship between visual style and core loop
The visual style must support the loop:

`choose node -> run autobattle -> gain rewards/progress -> upgrade -> continue`

Required behavior:
- world map visuals should help route understanding
- combat visuals should make success/failure legible
- post-run visuals should make rewards/progression easy to scan
- service/town visuals should feel like calmer planning spaces

Visual style should reinforce the emotional rhythm between:
- push
- stabilize
- upgrade
- repeat

---

## Relationship between visual style and world structure
Visual style must support a node/region-based world.

Required behavior:
- different regions/locations should feel meaningfully different
- world progression should visually imply movement into new spaces
- world map or node-selection presentation should make connected progression feel real
- earlier areas should remain recognizable and revisitable

The world should not feel like a flat UI list with random backgrounds.

---

## Relationship between visual style and locations
Locations are one of the main anchors of visual identity.

Required behavior:
- each location identity should have a distinct visual mood
- visual differences should support location purpose, not just scenery variation
- location visuals should help the player remember what a place is good for

Examples of identity anchors:
- terrain/biome feel
- silhouette language
- color-family bias
- environmental motifs
- landmark shapes

Exact art choices belong to later content production.

---

## Relationship between visual style and combat readability
Because combat is automated, the player must be able to visually understand combat at a glance.

Required behavior:
- player-side entity should be readable from enemies
- important attacks/effects should be readable enough to explain outcomes
- hostile pressure should be visible without overwhelming the screen
- strong builds should feel visually stronger in a readable way
- enemy deaths/kills should be visually recognizable

The game should avoid visual noise that makes autobattle feel random.

---

## Relationship between visual style and automation
Visual style must make automated systems feel intentional rather than chaotic.

Required behavior:
- automated combat actions should look understandable
- repeated farming should remain comfortable rather than exhausting
- effect density should not make the game unreadable at higher efficiency levels
- the player should be able to trust what the system is doing

Autobattle-friendly visual clarity is a core requirement, not a secondary polish goal.

---

## Relationship between visual style and UX/HUD
Visual style must support information hierarchy rather than compete with it.

Required behavior:
- backgrounds and effects should not overpower HUD/readability
- important state changes should have visual emphasis
- routine states should remain calm enough to scan quickly
- world, combat, and service screens should feel distinct in purpose

The UI and the world visuals must cooperate.

---

## Relationship between visual style and progression
Visual style should help progression feel tangible.

Required behavior:
- deeper content should feel visually more significant or different
- milestone encounters should look more important than ordinary nodes
- upgrades and stronger builds should produce some visible feedback
- the player should feel that the account is growing into stronger content over time

Visual progression does not need to mean realism or bigger numbers only.
It needs to create a sense of growing reach and competence.

---

## Relationship between visual style and loot/rewards
Rewards should be visually understandable.

Required behavior:
- milestone rewards should feel more important than ordinary gains
- common reward presentation should remain easy to scan repeatedly
- reward visuals should not create unnecessary clutter during repeated play

The first version should prefer clarity over flashy loot spectacle.

---

## Relationship between visual style and towns/service spaces
Towns/service contexts should feel different from combat spaces.

Required behavior:
- towns/service areas should feel calmer, clearer, and less visually hostile
- they should support planning and readability
- they should feel like spaces for conversion/preparation rather than danger

This contrast helps session rhythm.

---

## Push vs farm visual rhythm
The visual style should help distinguish:

### Push context
Expected feel:
- more tension
- stronger sense of danger or pressure
- more visual weight around important content

### Farm context
Expected feel:
- more comfort
- more familiarity
- easier scanning and lower visual stress

This distinction should emerge through presentation emphasis, not only explicit labels.

---

## Style direction constraints
The style direction should favor:
- clear silhouettes
- readable color separation between important categories
- restrained effect clutter
- strong location/world identity
- scalable asset production
- a look that supports many repeated runs without fatigue

The style direction should avoid:
- overly realistic visual density that hurts readability
- excessive screen noise
- effect spam that hides combat state
- many tiny details that do not improve player understanding

---

## Visual identity pillars
The style should be built around a few stable identity pillars.

Recommended pillars:
1. **Readable autobattle clarity**
2. **Distinct world/location identity**
3. **Comfortable long-session presentation**
4. **Clear contrast between combat and service/progression spaces**
5. **Scalable stylization over excessive realism**

These pillars are more important than any specific rendering technique.

---

## Camera / scene readability direction
The visual style must remain compatible with readable scene composition.

Required behavior:
- player-side focus should remain easy to track
- enemy density should remain readable enough for automated combat viewing
- important effects should stand out from ambient visuals
- scene framing should support fast understanding

Exact camera implementation belongs elsewhere, but the style must not assume unreadable scene density.

---

## Animation/VFX direction principles
Animation and VFX must reinforce clarity.

Required principles:
- attack and hit feedback should be understandable
- kill/defeat states should be recognizable
- milestone events should feel distinct
- routine VFX should not overwhelm repeated sessions
- stronger effects should be reserved for stronger moments where possible

MVP should prefer simple readable feedback over large amounts of VFX.

---

## Asset complexity rule
Do not overbuild asset complexity before gameplay readability is proven.

### MVP rule
Prefer:
- strong shape language
- consistent iconography
- clear environment themes
- simple readable effects
- limited but distinct location/enemy variations

Avoid in MVP:
- large numbers of unique high-detail assets
- many subtle style distinctions with little gameplay value
- heavy polish passes that arrive before readability decisions are settled

---

## MVP requirements
The first playable version must support:
- clear visual distinction between world-map/service/combat contexts
- at least 2 distinct location/region visual identities
- readable distinction between player-side and hostile entities
- readable combat feedback under autobattle
- readable reward/progression presentation
- visual style compatible with repeated farming sessions

MVP may omit:
- advanced shaders
- high-end cinematic VFX
- dense environmental storytelling layers
- highly differentiated biome micro-variants
- expensive presentation-only polish systems

---

## MVP priorities
Focus on:
- combat readability
- location identity
- world-versus-combat-versus-service contrast
- low visual fatigue during repeated play
- simple, strong, scalable style rules
- visual support for progression readability

Avoid in MVP:
- detail-first art direction
- over-rendered scenes
- excessive effect density
- many style experiments with no gameplay readability gain
- visual polish that slows iteration on core systems

---

## Data/pipeline direction
The style system should remain compatible with structured content growth.

Required principle:
- locations, enemies, UI states, and rewards should be able to reuse a coherent visual language
- the art direction should support adding new regions/nodes/enemies without redefining the entire style each time

Exact asset pipeline belongs to implementation/production planning.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not prioritize spectacle over readability.
- Do not make autobattle visually opaque.
- Do not make locations visually interchangeable in practice.
- Do not make repeated farming visually exhausting by default.
- Do not rely on heavy visual density to communicate importance.
- Do not make UI readability secondary to background art/VFX.
- Do not overcommit to realism if it weakens clarity and scalability.

---

## Extension points
The visual-style layer must support later addition of:
- stronger region differentiation
- richer environmental storytelling
- expanded enemy-family visual identities
- more distinctive milestone/boss presentation
- stronger build-effect visual differentiation
- accessibility/readability visual options
- polished reward/milestone presentation

These additions must extend the readable style foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact color palettes
- exact model/sprite style pipeline
- exact animation frame counts
- exact VFX specs
- exact art team production workflow
- exact marketing illustration direction

---

## Validation checklist
- [ ] The game has a readable visual direction for autobattle combat.
- [ ] World, combat, and service contexts are visually distinguishable.
- [ ] At least 2 locations/regions can feel visually distinct.
- [ ] Player-side and hostile entities are readable in combat.
- [ ] Important progression/reward moments can be visually emphasized.
- [ ] Repeated farming remains visually comfortable.
- [ ] The style can later expand to m