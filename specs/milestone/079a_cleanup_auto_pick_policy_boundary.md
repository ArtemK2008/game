# Milestone 079a - Cleanup Auto-Pick Policy Boundary

## Goal
- Follow up Milestone 079 with a behavior-preserving cleanup of the run-only upgrade auto-pick seam.
- Keep the current shipped baseline outcome unchanged while removing the fragile lifecycle dependence on implicit option-list ordering.

## What Changed
- Added `RunTimeSkillUpgradeAutoPickResolver` under `Assets/Scripts/Run/`.
- Moved the current baseline auto-pick decision out of `RunLifecycleController` and into that focused resolver.
- `RunLifecycleController` now asks the resolver for the current automatic-flow selection instead of selecting `runTimeSkillUpgradeOptions[0]` inline.
- The resolver now explicitly targets the shipped baseline option by upgrade id:
  - `CombatRunTimeSkillUpgradeCatalog.BurstTempo`
- Added focused resolver tests to prove:
  - the shipped baseline remains `Burst Tempo`
  - the result does not depend on available-option ordering
  - no automatic selection is produced when the current shipped baseline is unavailable

## Behavior Change
- Player-facing behavior is unchanged from Milestone 079.
- Combat-compatible runs that expose the current `Burst Strike` run-only upgrade seam still auto-start with `Burst Tempo`.
- No manual blocking UI was reintroduced.
- The run-only upgrade remains temporary and non-persistent across replay, return to world, restart/load, and safe resume.

## Tests
- Added `Assets/Tests/EditMode/Run/RunTimeSkillUpgradeAutoPickResolverTests.cs`
- Updated `Assets/Tests/EditMode/Run/RunLifecycleControllerCombatTests.cs`
- Existing persistence/build tests that already proved the run-only upgrade stays non-persistent remain valid and passed unchanged under the new resolver seam.

## Out Of Scope
- Any new upgrade system, preference setting, or manual choice redesign
- Any new persistent state
- Save/resume or autosave behavior changes
- Additional run-time upgrade options or broader upgrade policy work

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: reproduced the known helper artifact issue and did not create `Logs/editmode_results.xml`
- Fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m079a_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m079a_editmode.log"`
  - result: `526 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m079a_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m079a_editmode.log`
