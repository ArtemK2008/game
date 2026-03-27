# Milestone 081a - Cleanup Automation Comfort Upgrade Boundaries

## Goal
- Keep Milestone 081 behavior exactly the same while tightening SRP around the farm-replay comfort upgrade wiring.

## What Changed
- Removed direct `AccountWideProgressionEffectResolver` usage from `WorldMapScreen`.
- `WorldMapScreen` now accepts already-resolved `AccountWideProgressionEffectState` from its composition seam instead of resolving progression effects itself.
- `BootstrapStartup` now resolves account-wide progression effects before showing the world map and passes the resolved state into `WorldMapScreen`.
- `WorldMapScreenController` still consumes already-resolved effect state only.
- Updated the directly affected world-map screen tests so the purchased farm-replay comfort behavior is provided through the same explicit resolved-effect seam.

## Behavior Change
- No player-facing behavior changed.
- The world-map replay shortcut rule is still exactly:
  - available when the temporary explicit return-to-world offer exists, or
  - available when `Farm Replay Project` is purchased and the current node is already `Farm-ready`
- Selection override behavior is unchanged.
- Save/resume behavior is unchanged.
- Farm-ready ownership remains in `WorldNodeFarmReadinessResolver`.

## Tests
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - the direct `WorldMapScreen` tests that depend on purchased farm-replay comfort now pass resolved progression effects explicitly
- Existing controller and startup flow coverage still proves the same 081 behavior:
  - farm-ready replay comfort works when purchased
  - selected-node entry still overrides the shortcut
  - safe resume still restores to the world-map safe context without any routing changes

## Out Of Scope
- Any change to the farm-replay comfort rule itself
- Any new replay system or automation system
- Any save/resume or persistence redesign
- Any broad world-map or progression refactor beyond this small boundary cleanup

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m081a_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m081a_editmode.log"`
  - result: `540 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m081a_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m081a_editmode.log`
