# Milestone 069a - Harden Old-Location Relevance Authoring Boundaries

## Summary
- Hardened the authored-data boundary for the shipped old-location relevance rule from 069.
- Kept current player-facing behavior unchanged.
- Prevented node-level revisit-value content from drifting away from the reward conditions that actually grant region material.

## What Changed
- Added one small shared support seam:
  - `RegionMaterialRewardSupportResolver`
- `WorldGraphValidation` now rejects authored nodes that declare `RegionMaterialYieldContentDefinition` in regions that do not support region-material rewards.
- `WorldNodeEntryFlowController` now carries a resolved `SupportsRegionMaterialRewards` flag into `NodePlaceholderState`.
- `NodePlaceholderScreenTextBuilder` now shows the frontier revisit-value line only when that runtime state says the entered node really supports region-material rewards.
- `RunRewardResolutionService` now uses the same support resolver instead of duplicating the region-material eligibility check inline.

## Shipped Behavior After 069a
- `Forest Farm` still grants the extra frontier revisit value from 069.
- The current shipped frontier placeholder still shows:
  - `Revisit value: Region material yield +1`
- Forest gate, cavern service, and other current nodes stay unchanged.
- Invalid future authored combinations such as attaching region-material-yield content to a non-region-material region are now rejected before they can become live runtime content.
- Placeholder text can no longer advertise revisit value when the entered node would not actually yield region material.

## Intentionally Not Changed
- No new region economy system
- No new currencies
- No new reward categories
- No Milestone 070+ work
- No town, conversion, or boss behavior changes

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` still reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file in this shell.
- Verification then used the direct waited Unity batch EditMode fallback without `-quit`.
- Final result:
  - `466 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m069a_editmode_results.xml`
  - `Logs/m069a_editmode.log`
