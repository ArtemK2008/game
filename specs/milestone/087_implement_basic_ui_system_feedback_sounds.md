# Milestone 087 - Implement Basic UI/System Feedback Sounds

## Goal
- Add one minimal centralized feedback-sound seam for the current prototype UI/system flow.
- Keep the implementation narrow, SRP-friendly, and subordinate to the existing world-map, node, and town/service flow.

## Delivered
- Added one small shared feedback-audio seam:
  - `UiSystemFeedbackSoundId`
  - `UiSystemFeedbackAudioHost`
- Added one small post-run sound-presentation seam:
  - `PostRunFeedbackSoundState`
  - `PostRunFeedbackSoundStateResolver`
- Wired the shared feedback host from `BootstrapStartup` into the highest-value existing UI/system surfaces only:
  - `WorldMapScreen`
  - `NodePlaceholderScreen`
  - `TownServiceScreen`
- Wired the required feedback event set:
  - `ui_click`
  - `ui_confirm`
  - `ui_error`
  - `state_unlock`
  - `state_boss_clear`
- Used the supplied clip files directly when available:
  - `Assets/Audio/UI/ui_click.wav`
  - `Assets/Audio/UI/ui_confirm.wav`
  - `Assets/Audio/UI/ui_error.wav`
  - `Assets/Audio/System/state_unlock.wav`
  - `Assets/Audio/System/state_boss_clear.wav`
- Kept clip playback fail-safe:
  - missing/unresolved clips do not throw and do not break the UI flow

## Behavior Change
- The current prototype now plays basic UI/system feedback sounds for key accepted, blocked, and important state-change moments.
- World-map interactions now request:
  - `ui_click` for normal selection/navigation
  - `ui_confirm` for accepted entry/replay requests
  - `ui_error` for blocked/unavailable entry attempts
- Node/post-run interactions now request:
  - `ui_confirm` for accepted replay / return / stop actions
  - `ui_error` for blocked run-only or post-run actions
  - `state_unlock` when the resolved post-run presentation contains a route/gate unlock outcome
  - `state_boss_clear` when a boss run resolves successfully into post-run
- Town/service interactions now request:
  - `ui_confirm` for accepted purchases, conversions, package assignments, gear equip/unequip, return, and stop
  - `ui_error` for blocked/unavailable purchase or conversion attempts and other rejected town actions
- Save/resume, replay flow, combat logic, reward logic, progression balance, and persistence behavior stay unchanged.

## Tests
- Added `Assets/Tests/EditMode/Run/PostRunFeedbackSoundStateResolverTests.cs`
  - verifies ordinary post-run shows no state-change sounds
  - verifies unlock-only post-run requests `state_unlock`
  - verifies successful boss gate clear requests `state_boss_clear` plus `state_unlock`
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - verifies selection + accepted entry request `ui_click` then `ui_confirm`
  - verifies blocked world-map actions request `ui_error`
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies accepted replay / return / stop actions request `ui_confirm`
  - verifies successful boss post-run presentation requests `state_boss_clear` plus `state_unlock`
- Updated `Assets/Tests/EditMode/Towns/TownServiceScreenUiTests.cs`
  - verifies accepted purchase / conversion / build-prep actions request `ui_confirm`
  - verifies blocked/unavailable town actions request `ui_error`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies the startup screen flow keeps one shared `UiSystemFeedbackAudioHost` across current screen transitions

## Out Of Scope
- Music, ambience, combat hit/attack/enemy SFX, or a broader audio framework
- A dedicated audio settings menu or broader sound-mixing/configuration work
- Any change to save/resume, autosave, reward timing, replay logic, or combat resolution
- Any new UI screen, modal, or broader presentation redesign beyond this shared feedback seam

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Script fallback attempt:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m087_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m087_editmode.log"`
  - result: attempted as the normal fallback, but final verification moved to the direct Unity batch fallback to produce a trustworthy fresh post-fix result
- Direct waited fallback:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m087_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m087_editmode.log`
  - result: `573 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m087_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m087_editmode.log`
