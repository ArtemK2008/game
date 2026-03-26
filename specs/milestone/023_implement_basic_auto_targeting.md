# Milestone 023 - Implement Basic Auto-Targeting

## Delivered
- Added `CombatAutoTargetSelector` as the explicit player-side and enemy-side target selection helper for the current combat prototype.
- Kept the rule deterministic and minimal:
  - player side selects the active enemy target
  - enemy side selects the active player target
- Moved target selection out of hardcoded direct resolver assumptions and into a small isolated class.
- Kept the current prototype explicitly 1v1 while making target choice readable in code.
- Added explicit rejection for invalid targeting states where no active opposing target exists.

## Tests
- Added `CombatAutoTargetSelectorTests` for:
  - selecting the only active enemy for the player side
  - selecting the only active player target for the enemy side
  - rejecting selection when the only enemy target is defeated/inactive
  - deterministic repeat selection on the same encounter state
- Updated `CombatEncounterResolverTests` to verify the resolver rejects invalid combat advance when no active target exists.
- Existing combat resolver tests continued to verify timed attacks, mitigation, defeat, and outcome behavior after the target-selection extraction.

## Out Of Scope
- Random target choice
- Threat, aggro, or role-priority targeting
- Multi-enemy or multi-target combat selection
- Distance, area, or positional targeting rules
- Advanced enemy-side AI systems beyond the same explicit 1v1 selection rule

## Verification
- Unity EditMode batch run passed:
  - `Logs/m023_editmode_results.xml`
  - `Logs/m023_editmode.log`
