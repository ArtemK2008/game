# Milestone 062 - connect progression sink to town/service layer

## Goal
Make the existing persistent account-wide progression sink usable from the current town/service shell without broadening the hub into a larger town system.

## Decision
- The smallest honest 062 step is to keep `Cavern Service Hub` as the only town/service context and add a short direct purchase interaction there.
- The existing account-wide progression purchase rules stay in the progression domain.
- The town shell only gains button-level interaction plus immediate screen refresh after a purchase.

## Delivered
- Added `TownServiceProgressionInteractionService` as the small town-side orchestration seam for:
  - reusing the existing account-wide progression purchase rules
  - persisting successful purchases immediately through the existing safe-resume persistence boundary
- `TownServiceScreen` now renders one purchase button per visible account-wide project:
  - affordable projects show `Buy ...`
  - already-purchased projects show `... Purchased`
  - unaffordable projects show `... Unavailable`
- Buying from the town shell now:
  - spends `Persistent progression material`
  - marks the purchased upgrade in persistent progression state
  - saves the updated game state immediately
  - refreshes the town shell progression summary and button states in place

## Behavior
- `Cavern Service Hub` remains a distinct non-combat service context.
- The service shell now supports a short progression interaction loop:
  - enter service hub
  - review current progression material and projects
  - buy one affordable project
  - see the updated summary immediately
- Already-purchased projects are no longer buyable.
- Unaffordable projects remain visible but unavailable.
- Build preparation in the town shell remains read-only.
- Return-to-world and stop-session behavior remain unchanged.
- Existing downstream progression effects still apply to future combat and reward outcomes through the existing progression-effect seams.

## SRP notes
- `TownServiceScreen` remains UI wiring/layout only.
- `TownServiceScreenStateResolver` still resolves service-shell view state from persistent state.
- `TownServiceScreenTextBuilder` still formats readable summary text only.
- `AccountWideProgressionBoardService` remains the owner of purchase-policy rules.
- `TownServiceProgressionInteractionService` owns only the short town-side interaction orchestration and immediate persistence handoff.

## Verification
- Followed the compile/import-first workflow.
- Ran `tools/unity_compile_check.ps1` first.
- A first compile/import pass exposed one missing test `using` after the new town interaction flow was wired in; that was fixed and the compile/import pass was rerun successfully.
- `tools/run_editmode_tests.ps1` detached again without producing artifacts in this shell.
- Verification then used the direct waited Unity batch EditMode invocation.
- Final result: `436 passed`, `0 failed`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m062_editmode_results.xml`
  - `Logs/m062_editmode.log`

## Intentionally left out
- Milestone 063 or later town/build expansion
- moving gear/package editing off the world map
- broader town navigation, NPC systems, or multi-building service structure
- new progression currencies or new upgrade families
- long multi-step purchase flows
