# Milestone 083 - Implement Basic Duplicate/Conversion Handling If Needed

## Goal
- Verify whether the current shipped deterministic gear-loot path actually needs duplicate conversion behavior.
- Add only the smallest honest milestone result and avoid inventing broader loot handling when the live loop still has value.

## Delivered
- Audited the current shipped duplicate case from Milestone 082:
  - forest gate boss -> `Gatebreaker Blade`
  - duplicate owned gear is already suppressed by the existing reward resolution/grant path
  - the same boss clear still grants the existing `Persistent progression material x2` boss reward
- Landed Milestone 083 as an acceptance/validation pass instead of adding runtime conversion behavior.
- Tightened focused EditMode coverage so the current shipped loop explicitly proves:
  - duplicate earned gear does not create duplicate owned-gear entries
  - duplicate earned gear does not kill reward value because the boss material reward remains
  - no conversion fallback is currently required for the shipped deterministic loop

## Behavior Change
- No player-facing runtime behavior changed.
- No duplicate-to-resource conversion rule was added.
- The current shipped loop remains:
  - first forest gate boss clear grants `Gatebreaker Blade`
  - later forest gate boss clears suppress the duplicate gear reward
  - those later clears still keep the existing boss material reward, so the repeated loop is not dead value yet

## Tests
- Updated `Assets/Tests/EditMode/Run/RunRewardResolutionServiceTests.cs`
  - verifies that when `Gatebreaker Blade` is already owned, the gear reward is suppressed while boss reward value remains present
- Updated `Assets/Tests/EditMode/Run/RunLifecycleControllerCombatTests.cs`
  - verifies the live boss run still grants `Persistent progression material x2` after the earned gear is already owned and does not create duplicate owned-gear entries

## Out Of Scope
- Any new duplicate conversion reward
- Any broader salvage, crafting, selling, or loot-framework work
- Additional gear reward sources
- Any save/resume or world-map flow change

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m083_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m083_editmode.log"`
  - result: `547 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m083_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m083_editmode.log`
