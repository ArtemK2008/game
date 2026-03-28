# Combat VFX Pipeline

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
Define the minimal gameplay-facing pipeline for current combat VFX art assets so later hookup work can identify what should remain sheet-based versus what already has a stable canonical source file.

## Scope
This spec defines:
- where current combat VFX assets live
- which files are the current canonical gameplay-facing sources
- when to keep a sheet instead of splitting it

This spec does not define:
- animation controllers
- particle systems
- shader graphs
- timing curves
- combat VFX balance

## Core rule
Keep current combat VFX as stable, clearly named sheet assets unless a later gameplay milestone needs an unambiguous per-frame export.

For the currently shipped VFX set, the sheet files themselves are the canonical gameplay-facing assets, and milestone `094` uses those whole-sheet assets directly for the current restrained combat-readability cues.

## Folder structure
Use:

`Assets/Art/VFX/Combat/`

## Current shipped canonical files
- `Assets/Art/VFX/Combat/basic_combat_cues_sheet.png`
- `Assets/Art/VFX/Combat/burst_strike_effect_sheet.png`
- `Assets/Art/VFX/Combat/danger_pulse_sheet.png`
- `Assets/Art/VFX/Combat/defeat_effect_sheet.png`

## Why these remain sheets
The current sheets are better treated as grouped cue sources because later hookup still needs to decide:
- which regions of the sheet map to which richer runtime cue set
- whether future runtime should keep using the full sheet, one crop, or a sequence
- how timing should work for any later expanded effect treatment

The current milestone-094 hookup uses the full-sheet assets directly, and no broader per-frame split contract exists yet.

## Naming rule
Use lowercase names with underscores.
Keep the cue family readable in the file name.

## Import guidance
For current prototype prep, these VFX sheets should be import-ready for 2D sprite usage:
- `Sprite (2D and UI)`
- single sprite asset
- transparent background preserved when present

## Current gaps
- No finalized per-frame sequence contract exists yet for combat VFX.
- No separate boss-only VFX sheet is shipped yet.
- No environment-specific VFX variants are shipped yet.
