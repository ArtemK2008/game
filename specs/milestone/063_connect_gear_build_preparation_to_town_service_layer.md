# Milestone 063 - connect gear/build preparation to town/service layer

## Goal
Make the current town/service shell usable as a short safe-context build-preparation surface without expanding it into a broader hub system.

## Decision
- The smallest honest 063 step is to keep `Cavern Service Hub` as the only town/service context and expose the already-shipped selected-character build controls there.
- Existing package and gear assignment rules stay in their current character/build domain services.
- The town shell only gains button-level build interaction plus immediate refresh after a successful change.

## Delivered
- Added `TownServiceBuildPreparationInteractionService` as the small town-side orchestration seam for:
  - reusing the existing selected-character skill-package assignment rules
  - reusing the existing selected-character gear assignment rules
  - persisting successful build changes immediately through the existing safe world-context persistence boundary
- `TownServiceScreenStateResolver` now exposes:
  - valid skill-package options for the currently selected character
  - valid primary/support gear options for the currently selected character
- `TownServiceScreen` now renders build-preparation action buttons inside the existing service shell:
  - skill package assignment buttons
  - primary/support gear equip or unequip buttons
- Successful town-side build changes now:
  - update persistent selected-character state or loadout state
  - save the updated game state immediately
  - refresh the visible town-service build summary and button labels in place

## Behavior Change
- `Cavern Service Hub` remains a distinct non-combat service context.
- The service shell now supports a short build-preparation interaction loop:
  - enter service hub
  - review the currently selected character, assigned package, and equipped gear
  - change package or primary/support gear
  - see the updated build summary immediately
- Town-side build changes now affect future run entry through the existing persistent character/build seams.
- The world map keeps its existing build controls for now; town build preparation is an additional safe-context access point, not a replacement screen.
- Progression purchasing, return-to-world, and stop-session behavior remain unchanged.

## SRP Notes
- `TownServiceScreen` remains UI wiring/layout only.
- `TownServiceScreenStateResolver` still resolves presentation state from persistent game state.
- `TownServiceScreenTextBuilder` still formats display text only.
- `PlayableCharacterSkillPackageAssignmentService` and `PlayableCharacterGearAssignmentService` remain the owners of assignment-policy rules.
- `TownServiceBuildPreparationInteractionService` owns only the short town-side interaction orchestration and immediate persistence handoff.

## Verification
- Followed the compile/import-first workflow.
- Ran `tools/unity_compile_check.ps1` first.
- Ran `tools/unity_editmode_verify.ps1` next, but it returned without producing its expected EditMode results file in this shell.
- Verification then used the direct waited Unity batch EditMode invocation.
- Final result: `441 passed`, `0 failed`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m063_editmode_results.xml`
  - `Logs/m063_editmode.log`

## Out Of Scope
- Milestone 064 or later town/service expansion
- moving world-map build controls out of the current MVP flow
- new gear categories, new package options, or new progression systems
- deep inventory screens, tabs, buildings, or longer town navigation flows
