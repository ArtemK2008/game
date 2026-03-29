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
- scene wiring

## Core rule
Each shipped place should expose one clearly named canonical background file in its own place folder.

Current background assets stay grouped by place under:

`Assets/Art/Locations/<PlaceName>/Backgrounds/`

This includes both combat locations and the current safe-space/service location.

The current world-map presentation background stays under:

`Assets/Art/WorldMap/Backgrounds/`

The current authored world-map node icon set stays under:

`Assets/Art/WorldMap/Nodes/`

## Canonical file naming
Use:
- `combat_background.png` for combat-facing places
- `service_background.png` for service/town safe-space places

## Current shipped canonical files

### Combat backgrounds
- `Assets/Art/Locations/VerdantFrontier/Backgrounds/combat_background.png`
- `Assets/Art/Locations/EchoCaverns/Backgrounds/combat_background.png`
- `Assets/Art/Locations/SunscorchRuins/Backgrounds/combat_background.png`

### Service / town-safe background
- `Assets/Art/Locations/CavernServiceHub/Backgrounds/service_background.png`

### World-map presentation background
- `Assets/Art/WorldMap/Backgrounds/world_map_background.png`

### World-map node icon source and canonical outputs
Source sheet:
- `Assets/Art/WorldMap/Source/world_nodes_sheet_v2.png`

Canonical outputs:
- `Assets/Art/WorldMap/Nodes/ordinary_combat.png`
- `Assets/Art/WorldMap/Nodes/farm.png`
- `Assets/Art/WorldMap/Nodes/elite.png`
- `Assets/Art/WorldMap/Nodes/boss_gate.png`
- `Assets/Art/WorldMap/Nodes/service.png`
- `Assets/Art/WorldMap/Nodes/locked.png`
- `Assets/Art/WorldMap/Nodes/current.png`
- `Assets/Art/WorldMap/Nodes/region_transition.png`

Current split contract:
- strict 4x2 equal-cell grid
- left-to-right then top-to-bottom:
  - `ordinary_combat`
  - `farm`
  - `elite`
  - `boss_gate`
  - `service`
  - `locked`
  - `current`
  - `region_transition`

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
- `VerdantFrontier`, `EchoCaverns`, and `SunscorchRuins` are the current combat-space place backgrounds.
- `CavernServiceHub` is the current safe planning/service-space background.
- `world_map_background.png` is the current authored world-map presentation surface background.
- The live world-map screen currently uses the authored node icons for:
  - ordinary combat
  - farm-oriented combat
  - elite challenge combat
  - boss/gate
  - service/progression
  - locked override
  - current-context / selected override
- `region_transition.png` is currently prepared-only and not runtime-used because the shipped world graph does not yet expose a separate honest region-transition node meaning.

The current repo keeps the service hub under the same `Locations` root so place identity remains easy to track.
No separate `Services/` top-level art root is required yet.

## Current gaps
- No alternate post-run-specific background assets are shipped yet.
- No additional service/town-safe spaces beyond `CavernServiceHub` are shipped yet.
