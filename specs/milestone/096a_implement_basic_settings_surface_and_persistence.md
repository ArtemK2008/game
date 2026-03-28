# Milestone 096a - Implement Basic Settings Surface And Persistence

## Goal
- Follow up on Milestones `095` and `096` with the smallest honest real settings implementation.
- Replace the placeholder settings-entry surface with one compact shared settings surface while preserving the shipped compact main menu and compact pause/system menu behavior.

## Delivered
- Added one small dedicated user-settings seam:
  - `UserSettingsState`
  - `IUserSettingsStorage`
  - `FileUserSettingsStorage`
  - `UserSettingsPersistenceService`
  - `IDisplaySettingsApplier`
  - `UnityDisplaySettingsApplier`
  - `UserSettingsApplier`
- Added one shared compact settings UI surface:
  - `CompactSettingsView`
- Added one tiny shared helper for duplicated compact system-menu host/button setup:
  - `CompactSystemMenuUiFactory`
- Updated startup and in-game menu composition so both the compact main menu and the compact system menu reuse the same real settings surface.
- Added the current MVP settings set:
  - master volume
  - music volume
  - SFX volume
  - fullscreen / windowed mode
- Settings now apply immediately and persist through a dedicated player-settings save path separate from gameplay safe-resume state.

## Behavior Change
- `Settings` in the compact main menu now opens a real compact settings surface instead of placeholder copy.
- `Settings` in the compact in-game system menu now opens that same real compact settings surface.
- Settings changes now:
  - affect current runtime audio/music output immediately
  - affect the current desktop fullscreen/windowed mode through a dedicated display-apply seam
  - persist predictably across restart/load
- `Start`, `Continue`, `Quit`, `Resume`, and `Exit` behavior from Milestones `095` and `096` remain unchanged.

## Tests
- Added `Assets/Tests/EditMode/Core/UserSettingsPersistenceServiceTests.cs`
  - verifies default loading and sanitized save/load behavior
- Added `Assets/Tests/EditMode/Core/UserSettingsApplierTests.cs`
  - verifies combined master/music/SFX volume application and display-mode application
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies the real compact settings surface from the main menu
  - verifies settings persistence across restart/load
  - verifies the same real settings surface from the world-map system menu
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - verifies system-menu settings controls and change callback wiring
- Updated `Assets/Tests/EditMode/Towns/TownServiceScreenUiTests.cs`
  - verifies system-menu settings controls and change callback wiring

## Out Of Scope
- Milestone `097` and later beyond this narrow MVP settings slice
- Save slots, deeper graphics menus, key rebinding, gameplay automation settings, localization, or broader accessibility work
- Any change to gameplay safe-resume semantics, combat balance, rewards, progression, town logic, or visual/audio milestone content beyond supporting the new volume/display controls

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
- result: success
- log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper second:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
- result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback used for final verification:
  - `-batchmode`
  - `-nographics`
  - `-projectPath C:\IT_related\myGame\Survivalon`
  - `-runTests`
  - `-runSynchronously`
  - `-testPlatform EditMode`
  - `-testResults C:\IT_related\myGame\Survivalon\Logs\m096a_editmode_results.xml`
  - `-logFile C:\IT_related\myGame\Survivalon\Logs\m096a_editmode.log`
- final result:
  - `634 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- artifacts:
  - `C:\IT_related\myGame\Survivalon\Logs\m096a_editmode_results.xml`
  - `C:\IT_related\myGame\Survivalon\Logs\m096a_editmode.log`
