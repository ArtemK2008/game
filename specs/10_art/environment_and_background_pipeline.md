# Environment And Background Pipeline

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
Define the minimal gameplay-facing pipeline for current location and service/town background art used by the prototype.

This spec exists so later background hookup work can find the intended files without guessing.

## Scope
This spec defines:
- where current combat and service backgrounds live
- which files are the canonical gameplay-facing assets
- the naming rule for current background categories
- the current shipped place coverage

This spec does not define:
- parallax systems
- camera behavior
- lighting pipelines
- world-map background art systems
- scene wiring

## Core rule
Each shipped place should expose one clearly named canonical background file in its own place folder.

Current background assets stay grouped by place under:

`Assets/Art/Locations/<PlaceName>/Backgrounds/`

This includes both combat locations and the current safe-space/service location.

## Canonical file naming
Use:
- `combat_background.png` for combat-facing places
- `service_background.png` for service/town safe-space places

## Current shipped canonical files

### Combat backgrounds
- `Assets/Art/Locations/VerdantFrontier/Backgrounds/combat_background.png`
- `Assets/Art/Locations/EchoCaverns/Backgrounds/combat_background.png`

### Service / town-safe background
- `Assets/Art/Locations/CavernServiceHub/Backgrounds/service_background.png`

## Folder rule
Keep the place folder readable and shallow:
- `Assets/Art/Locations/<PlaceName>/Backgrounds/`

Do not add extra nested source/canonical folders unless a later milestone truly needs them.

## Import guidance
For current prototype prep, these background files should be import-ready for 2D sprite usage:
- `Sprite (2D and UI)`
- single sprite
- no slicing

## Current interpretation
- `VerdantFrontier` and `EchoCaverns` are the current combat-space place backgrounds.
- `CavernServiceHub` is the current safe planning/service-space background.

The current repo keeps the service hub under the same `Locations` root so place identity remains easy to track.
No separate `Services/` top-level art root is required yet.

## Current gaps
- No dedicated world-map background assets are shipped yet.
- No alternate post-run-specific background assets are shipped yet.
- No additional service/town-safe spaces beyond `CavernServiceHub` are shipped yet.
