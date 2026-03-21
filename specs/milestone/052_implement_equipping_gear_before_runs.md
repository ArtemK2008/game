# Milestone 052 - Implement Equipping Gear Before Runs

## Decision
- The smallest player-facing place for 052 is the existing world-map placeholder build area.
- The milestone keeps one live gear category, `PrimaryCombat`, and one shipped owned gear item, `Training Blade`.
- The player can now equip or unequip that one starter gear item for the currently selected character before future runs.

## Delivered
- Added `PlayableCharacterGearAssignmentService` so selected-character gear assignment rules stay out of `WorldMapScreen`.
- Added `PlayableCharacterGearAssignmentOption` for the one shipped placeholder gear option the current world map needs to render.
- Extended `PersistentLoadoutState` with explicit per-category helpers:
  - resolve equipped gear by category
  - set equipped gear by category
  - clear equipped gear by category
- Updated the current world-map placeholder UI to show:
  - the selected character's currently equipped `PrimaryCombat` gear
  - one equip/unequip button for `Training Blade`
- Kept the current world-map placeholder scroll viewport usable after the extra build row by compacting the fixed summary heights instead of redesigning the screen.

## Behavior
- The selected character can now equip or unequip `Training Blade` before runs from the current world-map placeholder.
- That equip state is persistent between runs and survives the existing save/load/bootstrap normalization path.
- The gear assignment follows the currently selected character.
- Gear still has no live combat/stat effect in this milestone.

## SRP Notes
- `GearCatalog` still owns shipped gear definitions only.
- `PersistentGearStateInitializer` still owns startup/save normalization of gear state.
- `PlayableCharacterGearAssignmentService` now owns interactive selected-character gear assignment rules.
- `WorldMapScreen` only wires the placeholder UI and delegates equip/clear behavior to the assignment service.
- Combat/stat interpretation was intentionally not added.

## Tests
- Added `PlayableCharacterGearAssignmentServiceTests` for selected-character equip/clear validation.
- Updated world-map presentation and UI tests to cover the new build summary text, the gear button, and post-refresh scroll usability.
- Updated startup/persistent-character initialization tests to verify valid equipped primary gear survives initialization/load.
- Updated run-context coverage so the selected character's equipped gear reaches run context through the new assignment service path without changing combat baseline.

## Intentionally Left Out
- gear combat/stat effects
- additional gear items or categories
- inventory browsing
- loot/drop acquisition
- broader gear UI or item comparison
- any Milestone 053 or later work
