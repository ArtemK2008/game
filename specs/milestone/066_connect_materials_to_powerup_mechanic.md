# Milestone 066 - connect materials to powerup mechanic

## Goal
Make the already-shipped farming-materials -> refinement -> project loop explicit and readable in the current MVP instead of adding a second mechanic-heavy system on top of Milestones 064 and 065.

## What changed
- Kept the shipped mechanics exactly the same:
  - `Region material x3 -> Persistent progression material x1`
  - direct account-wide project purchasing in `Cavern Service Hub`
- Added one small town/service presentation-state seam:
  - `TownServiceMaterialPowerPathState`
- Extended `TownServiceScreenStateResolver` so the town/service screen now resolves:
  - current refinement readiness or next-refinement progress
  - persistent progression material projected after the current refinement path
  - already-affordable project targets
  - additional project targets the current refinement path would enable
- Extended `TownServiceScreenTextBuilder` so the progression section now explicitly shows:
  - `Material power path`
  - the current farm -> refine -> invest loop
  - the current project targets that loop supports

## Current behavior after 066
- Repeated region-material farming now has a clearer readable path into lasting account-wide power:
  - farm `Region material`
  - refine it in `Cavern Service Hub`
  - spend `Persistent progression material` on account-wide projects
- The town/service shell now makes that loop visible without adding:
  - new buttons
  - new currencies
  - a new project tree
  - a broader crafting system
- Existing conversion, project purchase, and build-preparation interactions behave the same as before.

## SRP / implementation notes
- `TownServiceScreenStateResolver` owns the material-to-project loop interpretation for presentation state.
- `TownServiceScreenTextBuilder` formats that already-resolved state and does not decide project policy.
- `TownServiceScreen` remains UI wiring only.
- No conversion or project-purchase rules moved into UI classes.

## Tests
- Updated town resolver tests to prove the material-to-power state resolves structurally.
- Updated town presentation/UI tests to prove the new loop is visible and readable.
- Updated startup -> town flow coverage to prove the real service shell shows the loop through the shipped bootstrap path.

## Verification
- `tools/unity_compile_check.ps1` was run first.
- `tools/unity_editmode_verify.ps1` was attempted next.
- The helper again failed to leave its expected results artifact in this shell.
- Verification then used the direct waited Unity batch EditMode fallback.
- Result: `453 passed`, `0 failed`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m066_editmode_results.xml`
  - `Logs/m066_editmode.log`

## Out Of Scope
- No new refinement recipes
- No new currencies
- No new project tree or chained research flow
- No Milestone 067+ work
