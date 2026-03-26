# Milestone 042b - Extract Startup Domain Structure

## Delivered
- Moved the startup/bootstrap runtime slice out of the `Assets/Scripts` root into `Assets/Scripts/Startup/`.
- Moved the directly corresponding startup-focused EditMode tests into `Assets/Tests/EditMode/Startup/`.
- Updated the moved runtime files to use the explicit namespace `Survivalon.Runtime.Startup`.
- Updated the moved tests to use the matching namespace `Survivalon.Tests.EditMode.Startup`.
- Kept the change scoped to startup/bootstrap flow only; no broader runtime namespace rewrite was attempted.

## Why This Improves Readability
- The startup/bootstrap flow was already a coherent domain slice, but it was scattered across the root script folder.
- Grouping these files under a dedicated startup folder makes the entry-flow code easier to find and review.
- The startup-specific namespace now matches the folder/domain boundary, so the code reads as an explicit feature slice instead of another flat root-level runtime file.
- Mirroring that same grouping in the startup-focused tests keeps runtime and test navigation aligned.

## Tests
- No new behavior tests were needed because this was a behavior-preserving structure refactor.
- Existing startup/bootstrap EditMode tests were updated to the new folder/namespace structure and continue to cover:
  - startup routing
  - safe-resume startup state creation
  - post-run return/stop transitions
  - combat and progression startup flow

## Out Of Scope
- Root-level core identifiers and enums such as `NodeId`, `RegionId`, and reward/resource categories
- World, run, combat, and persistence namespaces outside the startup slice
- Assembly-definition layout
- Broader cross-domain namespace layering that would require repo-wide churn

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/startup_structure_refactor_editmode_results.xml`
  - `Logs/startup_structure_refactor_editmode.log`
