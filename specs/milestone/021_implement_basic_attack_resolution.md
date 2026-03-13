# Milestone 021 - Implement Basic Attack Resolution

## Delivered
- Added a minimal timed combat resolution loop for exactly one player entity and one enemy entity.
- Added explicit combat runtime state via `CombatEntityRuntimeState` and `CombatEncounterState`.
- Added `CombatEncounterResolver` to advance combat over elapsed time using attack intervals from the base stat model.
- Kept target selection explicit and fixed:
  - player attacks enemy
  - enemy attacks player
- Applied damage with the existing mitigation rule from `CombatStatCalculator`.
- Marked defeated entities as no longer alive and stopped combat when one side was defeated.
- Adapted `RunLifecycleController` so combat-compatible runs auto-resolve into `RunResolved` once combat produces a winner.
- Updated the combat shell view and node placeholder flow to show live combat progress and resolved outcomes.

## Tests
- Added `CombatEncounterResolverTests` for:
  - player attacks enemy over time
  - enemy attacks player over time
  - attack interval timing
  - mitigation effects
  - defeat outcome and winner reporting
  - no further combat after defeat
  - invalid setup rejection
- Updated lifecycle and UI tests to cover combat encounter state, auto-resolution, and resolved combat shell output.

## Intentionally Left Out
- Multi-target combat
- Movement or positioning
- Skills, crit, status effects, penetration, or advanced AI
- Reward resolution or loot flow
- Full combat HUD or animation-driven timing

## Verification
- Unity EditMode batch run passed:
  - `Logs/m021_editmode_results.xml`
  - `Logs/m021_editmode.log`
