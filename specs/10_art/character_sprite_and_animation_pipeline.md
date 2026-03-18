# Character Sprite And Animation Pipeline

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
Define a minimal, practical asset pipeline for playable-character 2D art used in the first playable version.

This spec exists to keep character art:
- organized
- readable
- easy to import
- easy to replace later
- consistent enough for MVP and near-MVP work

This spec turns high-level visual direction into simple delivery rules for:
- portraits
- world icons
- combat sprites
- early state-based animation assets
- file naming
- folder structure
- basic Unity import expectations

This spec must be used together with:
- `specs/09_presentation/visual_style.md`
- `specs/05_player/characters.md`

If those specs conflict with this file, do not guess.
Call out the conflict explicitly and resolve it before implementation.

---

## Scope
This spec defines:
- the minimum required character-asset outputs for MVP and near-MVP work
- the folder structure for character assets
- lightweight consistency rules across the roster
- the expected combat-state asset structure
- naming rules for gameplay-facing character art
- default Unity import guidance
- simple validation before hookup

This spec does not define:
- final concept art quality
- final exact color palettes
- final exact animation frame counts
- advanced VFX pipelines
- shader graphs
- rigged 3D characters
- marketing illustration rules
- a polished final art pipeline

---

## Core statement
Character art assets should support gameplay readability and consistent production flow without creating unnecessary art-pipeline overhead.

The main rule is:

`playable-character assets should be readable, organized, easy to import, and visually consistent enough to feel like one game`

For MVP, placeholder-quality assets are acceptable if they:
- are readable
- are correctly organized
- communicate class identity
- can be replaced cleanly later

---

## Dependency rule
This spec depends on these design truths.

From `characters.md`:
- characters are persistent player-owned combat identities
- character identity should affect gameplay visibly
- characters should support autobattle readability
- the system should remain extensible for a small roster

From `visual_style.md`:
- readability is more important than spectacle
- silhouettes matter more than small details
- repeated viewing comfort matters
- player-side identity should be distinguishable from enemies
- combat and world contexts should stay readable

This spec applies those principles in a lightweight, implementation-friendly way.

---

## Asset delivery model
Each playable character should have its own asset folder:

`Assets/Art/Characters/<CharacterName>/`

Example:
- `Assets/Art/Characters/Vanguard/`
- `Assets/Art/Characters/Striker/`
- `Assets/Art/Characters/Duelist/`
- `Assets/Art/Characters/Arcanist/`

Inside each character folder, use:

- `Portrait/`
- `WorldIcon/`
- `Sprites/`

Example:
- `Assets/Art/Characters/Vanguard/Portrait/`
- `Assets/Art/Characters/Vanguard/WorldIcon/`
- `Assets/Art/Characters/Vanguard/Sprites/`

---

## MVP required outputs per character
For each playable character, the minimum useful asset set is:

### A. Portrait
One gameplay-facing portrait image.

Purpose:
- character identity in selection or profile contexts
- readable roster differentiation
- future-compatible support for character UI

### B. World icon
One simplified gameplay-facing icon image.

Purpose:
- world map / selection / compact roster display
- quick recognition at smaller sizes

### C. Combat state sprites
One gameplay-facing sprite for each of these states:
- idle
- attack
- hit
- defeat

Purpose:
- placeholder or early-runtime combat-state presentation
- readable state switching without needing a full polished animation system first

---

## Canonical file format rule
For MVP and near-MVP work, the preferred gameplay-facing format is:

- PNG files
- separate files per output when practical
- transparent backgrounds preferred, but not mandatory for placeholder art
- a combined state sheet is allowed

### Preferred delivery
- `portrait.png`
- `world_icon.png`
- `idle.png`
- `attack.png`
- `hit.png`
- `defeat.png`

### Allowed alternative
- `combat_states_sheet.png`

If only a sheet exists, that is acceptable for MVP as long as:
- the poses are readable
- the order is clear
- the sheet can be split later without ambiguity

Separate files are preferred for runtime hookup, but sheet-first delivery is acceptable during early art production.

---

## Required file naming rule
Inside each character folder, use these lowercase file names when possible.

### Portrait
- `portrait.png`

### World icon
- `world_icon.png`

### Combat sprites
- `idle.png`
- `attack.png`
- `hit.png`
- `defeat.png`

### Optional
- `combat_states_sheet.png`

Avoid:
- spaces
- mixed naming styles
- unstable names like `_final_final2`
- inconsistent exploratory names in committed gameplay asset folders

If rough source exploration files must exist, keep them separate from the intended gameplay-facing files.

---

## Recommended output sizes
Use these as default targets unless a later spec or implementation need changes them.

### Portrait
Recommended:
- `512 x 512`

### World icon
Recommended:
- `256 x 256`

### Combat state sprite
Recommended:
- `256 x 256`

### Optional combat sheet
Recommended:
- `1024 x 256`
- four states in one horizontal row in this order:
    1. idle
    2. attack
    3. hit
    4. defeat

These are defaults, not hard blockers.
If assets differ slightly but remain usable and readable, they can still be accepted for MVP.

---

## Camera and framing rules
Perfect production-level consistency is not required yet, but assets should follow roughly the same framing logic across the roster.

### Portrait framing
Preferred:
- bust or upper-body
- similar crop philosophy across characters
- broadly similar zoom level

### World icon framing
Preferred:
- simplified readable character representation
- similar icon scale across characters

### Combat sprite framing
Preferred:
- similar ground line
- similar scale
- similar gameplay-facing camera/view angle

The goal is not perfect uniformity yet.
The goal is to avoid obviously incompatible framing.

---

## Roster consistency rules
All playable-character assets should feel like they belong to one game.

For MVP, this means:
- similar overall rendering family
- similar readability level
- similar silhouette-first philosophy
- similar production intent
- no extreme style clashes

It is acceptable if placeholder art is not perfectly matched yet, as long as:
- classes remain readable
- the roster does not feel completely fragmented
- the art can be iterated toward stronger consistency later

---

## Character differentiation rules
Classes should differ clearly while still feeling related.

### Vanguard
Should communicate:
- broad silhouette
- heavier armor
- durable frontline feel
- shield-bearing or defensive identity

### Duelist
Should communicate:
- narrower silhouette
- lighter armor / cloth accents
- fast lethal feel
- blade-focused identity
- no shield

### Striker
Should communicate:
- athletic mid-weight silhouette
- offensive melee pressure
- more force and momentum than Duelist
- less defensive feel than Vanguard

### Arcanist
Should communicate:
- readable caster silhouette
- robe or robe-armor hybrid
- arcane / mystical identity
- grounded fantasy consistency with the other classes

The goal is:
- visible class identity
- readable gameplay differentiation
- one shared game language

---

## Combat state definitions
Each combat-state sprite should communicate a specific readable state.

### Idle
Meaning:
- ready stance
- neutral combat presence

### Attack
Meaning:
- active offensive motion
- class identity visible through pose

### Hit
Meaning:
- temporary disruption
- recoil, stagger, or impact reaction

### Defeat
Meaning:
- downed or defeated state
- readable combat loss
- no gore required

For MVP, these states only need to be clearly distinguishable.
They do not need polished final animation quality.

---

## Background rules
### Portrait
Allowed:
- transparent background
- neutral clean background
- minimal shared backdrop treatment

### World icon
Preferred:
- transparent background
- otherwise a clean background with good readability

### Combat sprites
Preferred:
- transparent background

For MVP, non-transparent combat sprites are acceptable if:
- the background is clean enough
- the character remains readable
- the file can still be used or replaced cleanly later

Avoid:
- watermarks
- heavy UI frames
- cluttered scene backgrounds
- noisy baked presentation elements that block gameplay readability

Text labels inside placeholder sheets are not ideal, but may be tolerated temporarily if the asset is clearly marked placeholder-quality.

---

## Animation rule for MVP
The first playable version does not require full frame-by-frame animation for each character.

MVP-compatible interpretation:
- one clear sprite per state is enough
- runtime may switch between state sprites
- this is sufficient for placeholder combat presentation

Later milestones may replace this with:
- multi-frame strips
- clips
- more polished animation states

That future work should extend this structure rather than replace it unnecessarily.

---

## Unity import guidance
These are default import settings, not rigid blockers.

### Portrait import
Suggested:
- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: `Single`
- Mesh Type: `Full Rect`
- Pivot: `Center`

### World icon import
Suggested:
- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: `Single`
- Mesh Type: `Full Rect`
- Pivot: `Center`

### Combat sprite import
Suggested:
- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: `Single`
- Mesh Type: `Full Rect`
- Pivot: `Bottom Center`

Reason:
- bottom-center pivot is a practical default for character placement on a shared ground line

### Optional combat sheet import
Suggested:
- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: `Multiple`
- slicing order should match:
    1. idle
    2. attack
    3. hit
    4. defeat

If a specific asset needs a small import adjustment to work cleanly, that is acceptable.

---

## Pixels-per-unit rule
Use one consistent pixels-per-unit value across the roster when possible.

Default recommendation:
- `128 PPU`

If a different value is needed later, it should be changed intentionally across the relevant assets rather than drifting randomly.

---

## Asset readability rule at gameplay size
Each gameplay-facing character asset should still read reasonably well when reduced.

Check:
- portrait still identifies the class
- world icon still identifies the class
- combat sprite silhouette still identifies the class
- class identity does not rely entirely on tiny details

For MVP, the standard is “clearly usable,” not “final polished.”

---

## Art-production rule for future characters
New characters should extend the existing roster style rather than redefine it.

Future character asset packs should try to preserve:
- the same folder structure
- the same naming logic
- the same general framing logic
- the same readability priorities
- the same broad visual family

Perfect uniformity is not required in early production, but new additions should move the roster toward consistency, not away from it.

---

## Request-assets rule for implementation work
If a gameplay milestone needs new character art, icons, sprites, or state visuals, and the required assets do not exist in the expected folders, implementation should stop and request them clearly.

Do not:
- silently invent missing final assets
- assume a combat state exists when it does not
- pretend portrait-only delivery is full character delivery

Placeholder assets are acceptable, but they should be treated as placeholder assets.

---

## MVP validation checklist
- [ ] Each playable character has its own folder under `Assets/Art/Characters/`.
- [ ] Each playable character has `Portrait/`, `WorldIcon/`, and `Sprites/`.
- [ ] Each playable character has a portrait asset.
- [ ] Each playable character has a world icon asset.
- [ ] Each playable character has combat-state coverage through either separate files or a clear sheet.
- [ ] Combat states are readable as idle / attack / hit / defeat.
- [ ] Character classes remain distinguishable.
- [ ] Assets are organized well enough for Unity import and later replacement.
- [ ] The roster feels broadly consistent enough for MVP.
- [ ] The asset pipeline can support future cleanup and stronger consistency later.

---

## Out of scope
This spec does not define:
- final shader choices
- high-end VFX
- final polished animation pipelines
- 3D rigging
- cinematics
- marketing splash art
- enemy asset pipeline
- environment asset pipeline