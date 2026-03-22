# Milestone 069 - Implement Old-Location Relevance Rule In Content

## Summary
- Added one small content-owned rule that keeps `Verdant Frontier` useful after deeper progression opens.
- Kept the rule narrow and readable by attaching it to the shipped frontier farm node instead of the whole location identity.
- Reused the existing region material -> refinement -> persistent progression material -> project loop instead of adding a new economy system.

## What Changed
- Added one new node-owned content seam:
  - `RegionMaterialYieldContentDefinition`
- `WorldNode` and `NodePlaceholderState` now carry optional region-material-yield content for standard combat nodes.
- `BootstrapWorldGraphBuilder` now assigns `Region material yield +1` only to the shipped `Forest Farm` node.
- `RunRewardResolutionService` now applies that node-owned bonus when ordinary region-material rewards are resolved.
- `NodePlaceholderScreenTextBuilder` now surfaces that rule in live flow as:
  - `Revisit value: Region material yield +1`

## Shipped Behavior After 069
- `Verdant Frontier` farm runs now grant:
  - `Soft currency x1`
  - `Region material x2`
- The extra material is not location-wide and does not apply to every frontier node.
- The existing `Farm Yield Project` still stacks on top of that node-specific bonus, so the shipped frontier farm can now reach `Region material x3` when purchased.
- After cavern progression opens, the current shipped loop still has one explicit reason to revisit the earlier frontier content:
  - run the frontier farm node
  - refine `Region material x3 -> Persistent progression material x1` in town
  - spend persistent progression material on account-wide projects

## Intentionally Not Changed
- No new currencies
- No biome or faction system
- No broader reward redesign
- No new refinement recipes
- No Milestone 070+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` again reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file in this shell.
- Verification then used the direct waited Unity batch EditMode fallback without `-quit`.
- Final result:
  - `464 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m069_editmode_results.xml`
  - `Logs/m069_editmode.log`
