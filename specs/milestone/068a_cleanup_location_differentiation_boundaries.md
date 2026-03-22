# Milestone 068a - Cleanup Location Differentiation Boundaries

## Summary
- Kept the current shipped 068 player-facing behavior intact.
- Removed the mechanical boss-reward bonus from `LocationIdentityDefinition`.
- Moved the current `+1 persistent progression material` cavern bonus into specific boss-node content so future bosses in the same location can differ cleanly.
- Left location identity data focused on authored place flavor/emphasis and presentation only.

## What Changed
- Added one small node-owned content seam:
  - `BossRewardContentDefinition`
- `WorldNode` and `NodePlaceholderState` now carry optional boss-reward content separately from location identity.
- `BootstrapWorldGraphBuilder` now assigns that reward-content only to the shipped `Cavern Gate` boss node.
- `RunRewardResolutionService` now reads the extra boss reward from node-specific boss content instead of from the location identity.
- `LocationIdentityDefinition` now carries only:
  - display name
  - reward source wording
  - reward focus wording
  - enemy emphasis wording
  - authored-vs-fallback marker

## Preserved Behavior
- `Verdant Frontier` gate boss still grants `Persistent progression material x2`.
- `Echo Caverns` gate boss still grants `Persistent progression material x3`.
- World map, node placeholder, town/service shell, and post-run location identity presentation remain intact.
- Reward mechanics did not broaden beyond the existing shipped boss reward difference.

## Intentionally Not Changed
- No biome/faction framework
- No broader reward redesign
- No new currencies
- No new enemy-family system
- No milestone 069+ work
- No tooling-script change, because the current missing verify-artifact issue still appears shell/environment-related rather than a clear repo-local script defect

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` again reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file in this shell.
- Verification then used the direct waited Unity batch EditMode fallback.
- Final result:
  - `461 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m068a_editmode_results.xml`
  - `Logs/m068a_editmode.log`
