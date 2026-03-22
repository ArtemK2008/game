# Refactor milestone 02 — shared UI support boundary

## Goal
Keep the refactor narrow and behavior-preserving while moving the shared runtime UI helper out of the `World` domain into a true shared runtime package.

## What changed
- Moved `RuntimeUiSupport` from `Assets/Scripts/World/` to `Assets/Scripts/Core/`.
- Updated direct runtime consumers in:
  - startup placeholder UI
  - combat shell view
  - town/service screen
  - world map screen
  - node placeholder screen
- Kept runtime behavior unchanged; the move only corrected package ownership and imports.
- Added focused EditMode smoke tests for the shared helper under `Assets/Tests/EditMode/Core/`.

## Ownership outcome
- `RuntimeUiSupport` now sits in `Survivalon.Core`, which matches its real cross-domain ownership.
- `World` no longer owns a helper that is used equally by startup, combat, town, and world placeholder screens.
- Direct helper tests now live in the same `Core` test domain as the moved runtime helper.

## SRP / boundary check
- `RuntimeUiSupport` remains a small shared helper for runtime UI setup and common component creation.
- The touched screens still own their own layout/wiring only; the refactor did not move screen behavior into the helper.
- No gameplay, persistence, progression, combat, or navigation rules changed.
