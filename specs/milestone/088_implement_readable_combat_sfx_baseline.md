# Milestone 088 - Implement Readable Combat SFX Baseline

## Goal
- Add one minimal combat-SFX seam for the current autobattle prototype.
- Keep combat audio readable, low-noise, and subordinate to the existing combat shell, post-run flow, and persistence model.

## Delivered
- Added one small runtime-safe combat audio seam:
  - `CombatFeedbackSoundId`
  - `CombatFeedbackAudioClipRegistry`
  - `CombatFeedbackAudioHost`
- Added one small combat transition-to-sound resolver seam:
  - `CombatFeedbackSnapshot`
  - `CombatFeedbackSoundStateResolver`
- Added a runtime-safe registry asset at:
  - `Assets/Resources/CombatFeedbackAudioClipRegistry.asset`
- Wired baseline combat SFX into the live placeholder combat flow only:
  - player basic attack
  - enemy basic attack
  - player hit fallback for damage transitions not already covered by the paired attack cue
  - enemy hit fallback for damage transitions not already covered by the paired attack or `Burst Strike` cue
  - enemy defeat
  - player defeat
  - low-health danger feedback
  - `Burst Strike`
- Reused the existing `BootstrapStartup -> NodePlaceholderScreen` composition path so the combat screen only requests already-resolved sound ids while playback remains host-owned.
- Updated `specs/09_presentation/audio_asset_manifest.json` so the currently wired combat subset is reflected accurately.

## Behavior Change
- Ordinary combat now emits readable baseline combat SFX in the live autobattle flow.
- The current placeholder combat shell now requests baseline attack/hit/defeat sounds from combat-state transitions instead of staying silent.
- `Burst Strike` now has a distinct baseline combat cue when it auto-triggers in the current shipped skill flow.
- Low-health danger feedback now plays once on the downward threshold-crossing into danger rather than repeating every frame while health stays low.
- Boss and optional elite encounters reuse the same baseline combat SFX set; this milestone does not add boss-specific or elite-specific combat audio variants.
- Save/resume, replay flow, reward timing, progression, combat balance, and post-run logic stay unchanged.

## Tests
- Added `Assets/Tests/EditMode/Combat/CombatFeedbackAudioClipRegistryTests.cs`
  - verifies the runtime-safe registry resolves all required milestone-088 combat clips
  - verifies the wired combat clips use non-streaming import settings
- Added `Assets/Tests/EditMode/Combat/CombatFeedbackSoundStateResolverTests.cs`
  - verifies `Burst Strike` resolves distinctly from a basic attack
  - verifies the low-health danger cue triggers only on the threshold cross
  - verifies player defeat does not also spam the danger cue
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies baseline combat feedback requests happen in the live autobattle screen flow
  - verifies `Burst Strike` feedback appears in the live boss combat flow
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies bootstrap keeps one shared `CombatFeedbackAudioHost` across current screen transitions

## Out Of Scope
- Music, ambience, settings/menu audio, or combat-specific boss/elite audio variety
- A broader audio manager framework, mixer/settings pipeline, or combat-audio rebalance
- Any change to save/resume, autosave, rewards, progression, replay flow, or combat formulas
- New content-specific combat SFX beyond the current shipped baseline set

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Script fallback attempt:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m088_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m088_editmode.log"`
  - result: wrapper fallback was attempted but did not produce the requested artifacts
- Direct waited fallback:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m088_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m088_editmode.log`
  - final result:
    - `580 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m088_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m088_editmode.log`
