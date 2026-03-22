# Refactor milestone 03 — characters runtime domain split

## Goal
Keep the refactor behavior-preserving while separating authored/static character content from runtime character services and runtime interaction option models.

## What changed
- Moved runtime character services, resolvers, and runtime option models out of `Assets/Scripts/Data/Characters/` into `Assets/Scripts/Characters/`.
- Kept authored/static character definitions and catalogs in `Assets/Scripts/Data/Characters/`.
- Added one small authored/static split for skill packages:
  - `PlayableCharacterSkillPackageDefinition` now holds static catalog data
  - `PlayableCharacterSkillPackageOption` now stays on the runtime side and carries assignment state only
- Moved the direct EditMode tests for the moved runtime classes into `Assets/Tests/EditMode/Characters/`.

## Ownership outcome
- `Data/Characters` now reads as character content data only.
- `Characters` now reads as runtime character logic and runtime option state.
- Direct tests mirror that ownership more closely without changing selection, assignment, gear, or progression behavior.

## SRP / boundary check
- Character catalogs and profile definitions still own authored/static content.
- Runtime selection, package assignment, gear assignment, and character resolution stay in the runtime `Characters` domain.
- UI classes and persistence classes only had their imports updated; no character rules moved into screens.
- The refactor did not change gameplay, persistence, or player-facing flow.
