# Milestone 068 - Implement Basic Location Based Enemy Resource Differentiation

## Summary
- Kept the milestone small and content-data-driven.
- Reused the two shipped location identities from Milestone 067:
  - `Verdant Frontier`
  - `Echo Caverns`
- Added one small explicit enemy-emphasis field and one small explicit reward-emphasis rule to location identity data.
- Used that data to make the shipped locations differ through actual behavior and readable presentation without adding a new combat or economy system.

## What Changed
- `LocationIdentityDefinition` now carries:
  - `EnemyEmphasisDisplayName`
  - `BossPersistentProgressionMaterialBonus`
- Shipped authored location identities now differ as follows:
  - `Verdant Frontier`
    - reward focus: region-material farming
    - enemy emphasis: frontier raiders
    - boss reward bonus: `0`
  - `Echo Caverns`
    - reward focus: persistent progression gains
    - enemy emphasis: gate guardians
    - boss reward bonus: `+1 persistent progression material`
- `RunRewardResolutionService` now applies that location-owned boss bonus on successful boss clears.
- Node placeholder and town/service overview text now surface enemy emphasis from location state/data.

## Current Shipped Effect
- `Verdant Frontier` remains the repeatable region-material farming location.
- `Echo Caverns` is now more clearly the progression-focused location:
  - its town/service shell still exposes progression spending and refinement
  - its gate boss now grants a higher persistent progression reward than the frontier boss
- Current shipped post-run output now makes that difference visible:
  - frontier boss reward: `Persistent progression material x2`
  - cavern boss reward: `Persistent progression material x3`

## Intentionally Not Added
- No biome system
- No faction system
- No new currencies
- No large reward redesign
- No new enemy family system
- No milestone 069+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` again reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file in this shell.
- Verification then used the direct waited Unity batch EditMode fallback.
- Final result:
  - `458 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m068_editmode_results.xml`
  - `Logs/m068_editmode.log`
