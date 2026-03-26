# Refactor milestone 04 — progression authored-data split

## Goal
Keep the refactor behavior-preserving while separating authored/static account-wide progression content from persistent/runtime progression state and logic.

## What changed
- Moved static account-wide progression content from `Assets/Scripts/State/Persistence/` to `Assets/Scripts/Data/Progression/`:
  - upgrade ids
  - upgrade definitions
  - upgrade catalog
- Kept persistent progression state, purchase logic, and runtime effect resolution in `Assets/Scripts/State/Persistence/`.
- Updated direct consumers in `Run`, `Towns`, `Startup`, and tests to import the new data package.
- Added one focused data-domain test for the static account-wide progression catalog.

## Ownership outcome
- `Data/Progression` now reads as authored/static account-wide progression content.
- `State/Persistence` now reads as persistent progression state and mutation/effect logic only.
- Existing progression behavior, upgrade costs, upgrade effects, and purchase flow remain unchanged.

## SRP / boundary check
- Static ids/definitions/catalogs no longer live beside persistent state mutation code.
- Board service and effect resolver still own runtime progression behavior.
- Town, run, and startup classes only consume authored progression data through imports; no progression policy moved into UI or routing code.
