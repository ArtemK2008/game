# Milestone 096 - Implement Compact Pause/System Menu

## Goal
- Add one compact in-game system menu for the current prototype.
- Reuse the compact settings-entry approach without adding unsafe pause or resume semantics.

## Delivered
- Added one shared `CompactSystemMenuView` overlay with:
  - `Resume`
  - `Settings`
  - `Exit`
- Wired that compact system menu into the current live screens:
  - `WorldMapScreen`
  - `TownServiceScreen`
  - `NodePlaceholderScreen`
- Updated `BootstrapStartup` so safe exit from the world map now routes back to the compact main menu through the existing safe-stop seam.
- Kept safe exit policy context-specific:
  - world map can safely exit to the main menu
  - town/service can safely exit to the main menu
  - node/combat can only safely exit from resolved post-run when the existing stop-session path is valid
- While the system menu is open on an active node/combat screen, automatic combat time no longer advances.
- Reused the existing compact settings-entry copy rather than introducing a deeper settings system.

## Behavior Change
- The current world map, town/service shell, and node/combat shell now expose a compact system menu.
- `Resume` closes that menu and returns to the current context.
- `Settings` opens the same compact settings-entry surface already used by the main menu.
- `Exit` only appears as available in contexts that already support a safe-stop under the current persistence model.
- Active unresolved combat does not permit unsafe exit and autobattle pauses while the system menu is open.

## Tests
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTestBase.cs`
  - added helpers for opening/resuming/settings within the system menu
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies world-map system-menu settings/resume flow
  - verifies safe exit from world map and town/service back to the compact main menu
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - verifies direct world-map system-menu settings and safe exit wiring
- Updated `Assets/Tests/EditMode/Towns/TownServiceScreenUiTests.cs`
  - verifies direct town/service system-menu settings and safe exit wiring
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenUiTestBase.cs`
  - added a focused frame-advance helper for pause-menu coverage
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies unsafe exit is disabled during active combat
  - verifies system-menu exit becomes available again at resolved post-run
  - verifies active combat does not advance while the system menu is open

## Out Of Scope
- Milestone `097` and later
- Mid-run suspend/resume or unresolved-combat continue
- Deeper settings expansion, save-slot systems, or broader startup/menu redesign
- Any change to combat balance, rewards, progression, audio, or visual milestone behavior beyond the compact pause/system menu

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
  - `-testResults C:\IT_related\myGame\Survivalon\Logs\m096_editmode_results_final.xml`
  - `-logFile C:\IT_related\myGame\Survivalon\Logs\m096_editmode_final.log`
- final result:
  - `630 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- artifacts:
  - `C:\IT_related\myGame\Survivalon\Logs\m096_editmode_results_final.xml`
  - `C:\IT_related\myGame\Survivalon\Logs\m096_editmode_final.log`
