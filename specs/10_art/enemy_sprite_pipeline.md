# Enemy Sprite Pipeline

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
Define the minimal gameplay-facing asset pipeline for enemy combat sprites used by the current prototype and the upcoming visual-readability milestones.

This spec keeps enemy art:
- easy to find
- easy to hook up
- easy to replace later
- consistent with the existing player character sprite pipeline

## Scope
This spec defines:
- folder structure for enemy combat sprites
- canonical per-state file outputs
- the role of combined source sheets
- current shipped canonical enemy asset paths
- simple import and slicing expectations

This spec does not define:
- final animation timing
- enemy concept-art quality
- multi-frame animation clips
- boss-exclusive VFX pipelines
- runtime hookup behavior

## Core rule
Enemy combat art should expose one clean gameplay-facing sprite per current MVP combat state while keeping any combined source sheet available for future replacement or re-slicing.

## Folder structure
Use:

`Assets/Art/Enemies/<EnemyName>/Sprites/`

Current shipped folders:
- `Assets/Art/Enemies/EnemyUnit/Sprites/`
- `Assets/Art/Enemies/BulwarkRaider/Sprites/`
- `Assets/Art/Enemies/GateBoss/Sprites/`
- `Assets/Art/Enemies/RuinSentinel/Sprites/`

## Canonical gameplay-facing files
Preferred per enemy:
- `idle.png`
- `attack.png`
- `hit.png`
- `defeat.png`

Allowed source/reference file:
- `combat_states_sheet.png`

The separate state files are the canonical gameplay-facing assets for later hookup.
The combined sheet remains the source/reference asset when it exists.

## Current shipped canonical files

### Enemy Unit
- `Assets/Art/Enemies/EnemyUnit/Sprites/idle.png`
- `Assets/Art/Enemies/EnemyUnit/Sprites/attack.png`
- `Assets/Art/Enemies/EnemyUnit/Sprites/hit.png`
- `Assets/Art/Enemies/EnemyUnit/Sprites/defeat.png`
- source sheet: `Assets/Art/Enemies/EnemyUnit/Sprites/combat_states_sheet.png`

### Bulwark Raider
- `Assets/Art/Enemies/BulwarkRaider/Sprites/idle.png`
- `Assets/Art/Enemies/BulwarkRaider/Sprites/attack.png`
- `Assets/Art/Enemies/BulwarkRaider/Sprites/hit.png`
- `Assets/Art/Enemies/BulwarkRaider/Sprites/defeat.png`
- source sheet: `Assets/Art/Enemies/BulwarkRaider/Sprites/combat_states_sheet.png`

### Gate Boss
- `Assets/Art/Enemies/GateBoss/Sprites/idle.png`
- `Assets/Art/Enemies/GateBoss/Sprites/attack.png`
- `Assets/Art/Enemies/GateBoss/Sprites/hit.png`
- `Assets/Art/Enemies/GateBoss/Sprites/defeat.png`
- source sheet: `Assets/Art/Enemies/GateBoss/Sprites/combat_states_sheet.png`

### Ruin Sentinel
- `Assets/Art/Enemies/RuinSentinel/Sprites/idle.png`
- `Assets/Art/Enemies/RuinSentinel/Sprites/attack.png`
- `Assets/Art/Enemies/RuinSentinel/Sprites/hit.png`
- `Assets/Art/Enemies/RuinSentinel/Sprites/defeat.png`
- source sheet: `Assets/Art/Enemies/RuinSentinel/Sprites/combat_states_sheet.png`

## Slicing assumption used for current repo prep
The current enemy source sheets were normalized using this assumption:
- one horizontal strip
- four equal-width states
- left-to-right order:
  1. `idle`
  2. `attack`
  3. `hit`
  4. `defeat`

Current shipped source-sheet size:
- `1536 x 1024`

Current exported per-state size:
- `384 x 1024`

If a later source sheet does not follow this layout, do not reuse this slicing assumption blindly.

## Naming rule
Use lowercase stable names with underscores only.
Do not commit gameplay-facing enemy sprite files with exploratory names.

## Import guidance
For current prototype usage, enemy sprite files should be imported as:
- `Sprite (2D and UI)`
- single sprite per file
- transparent background preserved when present

The source sheet may also remain imported as a single sprite asset because the canonical runtime-facing files are the split state outputs.

## Relationship to future milestones
This prep supports later hookup work without claiming that those milestones are already implemented.
Future runtime work should consume the canonical split state files first and only fall back to the source sheets when a specific milestone explicitly needs sheet-based slicing logic.

## Current gaps
- No authored multi-frame enemy animation clips exist yet.
- No dedicated elite-only or boss-only alternate state sets exist yet beyond the current `GateBoss` state strip.
- `RuinSentinel` is currently prepared-only art and is not wired into runtime enemy content yet.
