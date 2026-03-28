# Milestone 090 - Implement Milestone Audio Cues

## Goal
- Strengthen milestone-result audio readability in the current shipped post-run flow.
- Reuse the existing runtime-safe UI/system feedback seam instead of adding a broader audio framework.

## Delivered
- Extended the shared UI/system feedback ids and registry with three specific milestone-result cues:
  - `state_node_clear`
  - `state_route_unlock`
  - `state_boss_reward`
- Updated `Assets/Resources/UiSystemFeedbackAudioClipRegistry.asset` to wire the already committed specific milestone clips:
  - `Assets/Audio/System/state_node_clear.wav`
  - `Assets/Audio/System/state_route_unlock.wav`
  - `Assets/Audio/System/state_boss_reward.wav`
- Replaced the old generic post-run unlock/boss-clear sound resolution with a small ordered milestone-result seam:
  - `PostRunFeedbackSoundState`
  - `PostRunFeedbackSoundStateResolver`
- Kept `NodePlaceholderScreen` as a requester only:
  - it now replays the resolver's ordered milestone-result sound ids without owning cue-policy logic
- Updated `specs/09_presentation/audio_asset_manifest.json` and `specs/00_overview/current_build_state.md` so the final runtime wiring is documented accurately.

## Behavior Change
- The current prototype now uses stronger, more specific milestone-result confirmation in resolved post-run:
  - clear-threshold reward spikes request `state_node_clear`
  - route/gate unlock outcomes request `state_route_unlock`
  - boss reward moments, including boss gear reward cases, request `state_boss_reward`
- Same-moment milestone audio stays intentionally small:
  - boss reward takes precedence over the node-clear cue
  - route unlock can layer as the secondary cue when both outcomes happen together
- The older generic `state_unlock` and `state_boss_clear` clips remain committed assets but are no longer part of the current shipped post-run milestone-result wiring.
- Ordinary UI/system feedback, combat SFX, music context split, save/resume, rewards, progression logic, replay flow, and combat balance remain unchanged.

## Tests
- Updated `Assets/Tests/EditMode/Core/UiSystemFeedbackAudioClipRegistryTests.cs`
  - verifies the shared registry resolves the current runtime-wired UI/system subset, including the three specific milestone-result clips
  - verifies the wired UI/system clips use sane non-streaming import settings
- Updated `Assets/Tests/EditMode/Run/PostRunFeedbackSoundStateResolverTests.cs`
  - verifies ordinary post-run stays silent
  - verifies route unlock resolves to `state_route_unlock`
  - verifies clear-threshold unlock resolves to `state_node_clear` plus `state_route_unlock`
  - verifies boss reward unlock resolves to `state_boss_reward` plus `state_route_unlock`
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies the live boss post-run flow requests `state_boss_reward` plus `state_route_unlock`

## Out Of Scope
- Any broader audio framework, mixer/settings work, queueing/transition system, music changes, ambience, or combat-SFX changes
- Any change to save/resume, reward values, progression logic, world flow, replay flow, or combat formulas
- Rewiring broader committed audio assets beyond the milestone-result cues in this note

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Wrapper fallback attempt during iteration:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m090_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m090_editmode.log"`
  - result: the wrapper path was not relied on for final verification because it reused stale artifacts during iteration
- Direct waited fallback used for final verification:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m090_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m090_editmode.log`
  - final result:
    - `586 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m090_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m090_editmode.log`
