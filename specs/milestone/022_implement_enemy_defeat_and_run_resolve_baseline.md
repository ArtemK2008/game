# Milestone 022 - Implement Enemy Defeat And Run Resolve Baseline

## Delivered
- Marked defeated combat entities as no longer active in combat runtime state.
- Added explicit encounter-level active-participant helpers for player-side and enemy-side combatants.
- Kept defeated enemies from remaining active threats after lethal damage in the 1v1 combat shell.
- Confirmed minimal run outcome mapping:
  - player victory -> `RunResolutionState.Succeeded`
  - player defeat -> `RunResolutionState.Failed`
- Kept the current combat shell flow readable without redesigning the UI.

## Tests
- Updated `CombatEncounterResolverTests` to verify:
  - defeated enemies become inactive
  - defeated enemies are excluded from active combat counts
  - defeated enemies cannot continue acting after victory
- Updated `RunLifecycleControllerTests` to verify:
  - player victory still resolves the run as success
  - player defeat resolves the run as failure
  - resolved combat state reflects the defeated side as inactive
- Updated `NodePlaceholderScreenUiTests` to verify the combat shell text shows the defeated enemy as `Alive: No | Act: No`.

## Intentionally Left Out
- Loot or reward resolution
- Multi-enemy encounter management
- Corpse, animation, VFX, or cleanup presentation systems
- Revive, respawn, or advanced defeat states

## Verification
- Unity EditMode batch run passed:
  - `Logs/m022_editmode_results.xml`
  - `Logs/m022_editmode.log`
