# Milestone 081 - Add One Automation Comfort Upgrade

## Goal
- Connect account-wide progression to one small farming-comfort improvement without breaking the current push/farm distinction.

## Delivered
- Added one new account-wide progression upgrade in the existing authored catalog:
  - `Farm Replay Project`
- Threaded that upgrade through the existing account-wide progression effect model as one small resolved comfort flag:
  - `EnablesFarmReadyQuickReplayShortcut`
- Updated world-map quick-replay availability so the current node replay shortcut is available when either:
  - the existing temporary explicit return-to-world offer exists, or
  - `Farm Replay Project` is purchased and the current node is already `Farm-ready`
- Kept farm-ready ownership in the existing world-map farm-readiness resolver.
- Kept the world-map controller consuming already-resolved progression effects rather than progression state or purchase rules directly.

## Behavior Change
- Purchasing `Farm Replay Project` now improves farming comfort for repeatable ordinary combat content only.
- When the current node is already `Farm-ready`, the world map can show `Replay <current node>` even without the temporary return-to-world shortcut offer.
- This does not change push flow:
  - uncleared push nodes still require normal selection/entry
  - boss/gate nodes do not gain the shortcut
  - service/progression nodes do not gain the shortcut
- This is not unattended automation:
  - no auto-repeat loop
  - no background farming
  - no new save/resume routing

## Tests
- Added/updated focused EditMode coverage for:
  - account-wide progression effect resolution for `Farm Replay Project`
  - authored progression catalog exposure for the new upgrade
  - world-map controller shortcut availability with and without the purchased comfort effect
  - world-map UI behavior proving a selected reachable node still overrides the shortcut
  - startup/world-map flow proving a saved purchased upgrade improves farming comfort for a `Farm-ready` current node while still restoring into the existing world-map safe context

## Out Of Scope
- Auto-repeat loops or unattended farming
- New automation systems, settings, or menus
- Any broadening of the `Farm-ready` rule beyond the existing Milestone 080 definition
- Post-run replay changes
- Safe-resume target-routing changes

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m081_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m081_editmode.log"`
  - final result: `540 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m081_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m081_editmode.log`
