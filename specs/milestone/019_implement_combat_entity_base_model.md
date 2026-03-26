# Milestone 019 - Implement combat entity base model

## Delivered
- Added `CombatEntityId` and `CombatEntityState` as the shared base combat entity model for the current combat shell.
- Player-side and enemy-side allegiance remain explicit through `CombatSide`, while each combat entity now carries typed identity, display name, and alive/active state.
- Refactored the Milestone 018 combat shell so `CombatShellContext` and `CombatShellContextFactory` use `CombatEntityState` instead of ad hoc participant-only placeholders.
- `CombatShellView` now renders the combat entity model directly, including side plus alive/active state readability.

## Tests
- Added Edit Mode coverage for player-side entity creation, enemy-side entity creation, default alive/active state, preserved side/allegiance, and clean combat-shell integration.

## Out Of Scope
- This milestone does not add stats, attacks, movement, targeting, defeat resolution, or combat behavior yet. It only establishes the shared combat-entity base model needed for later combat milestones.

## Verification
- Verified with a real Unity batch Edit Mode run. `Logs/m019_editmode_results.xml` was produced and the run completed with `63` tests passed and exit code `0`.
