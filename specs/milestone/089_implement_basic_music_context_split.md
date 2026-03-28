# Milestone 089 - Implement Basic Music Context Split

## Goal
- Add the smallest honest calm-versus-gameplay music split for the current prototype.
- Keep music subordinate to the existing world/combat/town/post-run flow and reuse the current runtime-safe audio pattern.

## Delivered
- Added one small shared music seam in `Assets/Scripts/Core/`:
  - `MusicContextId`
  - `MusicAudioClipRegistry`
  - `MusicAudioHost`
  - `MusicContextResolver`
- Added a runtime-safe registry asset at:
  - `Assets/Resources/MusicAudioClipRegistry.asset`
- Wired the existing committed music clips into the current prototype contexts:
  - `Assets/Audio/Music/music_gameplay_loop.wav`
  - `Assets/Audio/Music/music_calm_loop.wav`
- Reused `BootstrapStartup` as the composition root:
  - calm music for startup placeholder, world map, and town/service screen
  - music context requests from `NodePlaceholderScreen` so active combat uses gameplay music and post-run/non-combat placeholder states return to calm music
- Updated `specs/09_presentation/audio_asset_manifest.json` so the runtime-wired music subset now matches the actual shipped hookup.

## Behavior Change
- The current prototype now has a minimal two-context music split:
  - active combat shell uses gameplay music
  - startup/menu-placeholder, world map, town/service, and post-run use calm music
- Music now follows current screen/run-context transitions through one shared host instead of leaving the committed music clips unwired.
- Save/resume, replay flow, rewards, progression, combat balance, and existing UI/system/combat SFX behavior remain unchanged.

## Tests
- Added `Assets/Tests/EditMode/Core/MusicAudioClipRegistryTests.cs`
  - verifies the runtime-safe registry resolves both milestone-089 music clips
- Added `Assets/Tests/EditMode/Core/MusicContextResolverTests.cs`
  - verifies the minimal calm/gameplay split for combat-shell visibility
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies one shared `MusicAudioHost` survives current safe-context screen transitions and keeps calm music there
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - verifies combat entry switches the shared host to gameplay music
  - verifies post-run switches the shared host back to calm music

## Out Of Scope
- Music settings, mixers, playlists, fades/transition systems, adaptive music, regional themes, boss themes, ambience, or additional music contexts
- Any change to save/resume, rewards, progression, replay flow, or combat formulas
- Any broader redesign of the existing audio architecture

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Script fallback attempt:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m089_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m089_editmode.log"`
  - result: wrapper fallback was attempted but did not produce the requested artifacts
- Direct waited fallback:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m089_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m089_editmode.log`
  - final result:
    - `585 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m089_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m089_editmode.log`
