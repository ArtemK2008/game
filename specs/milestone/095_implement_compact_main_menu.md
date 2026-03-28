# Milestone 095 - Implement Compact Main Menu

## Goal
- Add one compact startup main menu for the current prototype.
- Keep startup routing, resume policy, and quit behavior cleanly separated from menu UI.

## Delivered
- Evolved the existing startup placeholder into a compact startup menu surface with:
  - `Start`
  - `Continue`
  - `Settings`
  - `Quit`
- Added one small quit seam:
  - `IApplicationQuitService`
  - `ApplicationQuitService`
- Extended `SafeResumePersistenceService` with `TryLoadExisting(...)` so startup can decide continue availability without pushing storage logic into the menu view.
- Extended `BootstrapStartupStateFactory` with:
  - `CreateFresh(...)`
  - `TryCreateContinue(...)`
- Updated `BootstrapStartup` so:
  - startup now opens the compact main menu
  - `Start` applies a fresh bootstrap-world state and enters the world map
  - `Continue` resumes the last persisted safe world or town/service context when available
  - `Settings` opens a compact settings entry surface inside the startup menu
  - `Quit` routes through the dedicated quit service
- Extended the safe-resume seam so town/service-level safe contexts are stamped explicitly and can be continued back into the current `Cavern Service Hub` shell.
- Updated the shared startup test base so existing startup flow tests can auto-enter the playable flow from the new menu without broad test rewrites.

## Behavior Change
- The project no longer starts directly in the world map.
- Startup now opens a compact main menu first.
- `Continue` is visible but only interactable when a persisted safe world or town/service context exists and can resume into the current playable flow.
- `Continue` now resumes the actual last persisted safe context:
  - world-level safe saves reopen at the world map
  - town/service safe saves reopen in the town/service shell
- `Start` begins a fresh playable bootstrap session even when a saved state exists.
- `Settings` is currently a compact entry surface rather than a deep settings system.
- `Quit` behaves as a real application quit in player builds and stays safe in editor/test contexts.

## Tests
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTestBase.cs`
  - auto-enters the playable flow from the new menu for existing startup-flow coverage
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies initial compact main menu presentation
  - verifies settings entry
  - verifies continue-disabled behavior when no resumable safe context exists
  - verifies fresh start vs continue behavior for both world and town/service safe contexts
  - verifies quit requests are routed through the quit service
  - verifies stop-session returns to the compact main menu with continue available
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupStateFactoryTests.cs`
  - verifies `CreateFresh(...)`
  - verifies `TryCreateContinue(...)` for world, town/service, and non-resumable saved-state cases
- Updated `Assets/Tests/EditMode/Startup/BootstrapWorldContextTransitionServiceTests.cs`
  - verifies stopping from town/service stamps a town/service safe-resume target
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupTownServiceFlowTests.cs`
  - verifies restart/load can continue directly back into the current town/service shell after persisted town/service build-prep changes
- Updated `Assets/Tests/EditMode/State/Persistence/SafeResumePersistenceServiceTests.cs`
  - verifies `TryLoadExisting(...)`
  - verifies explicit town/service safe-resume persistence stamping

## Out Of Scope
- Milestone `096` and later
- Broader menu systems, scene-management rewrites, profile selection, save slots, credits, or other extra startup options
- Deep settings expansion beyond the compact entry surface
- Any change to combat, rewards, progression, save semantics, or world/town runtime behavior beyond startup entry routing

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback used for final verification:
  - `-batchmode`
  - `-nographics`
  - `-projectPath C:\IT_related\myGame\Survivalon`
  - `-runTests`
  - `-runSynchronously`
  - `-testPlatform EditMode`
  - `-testResults C:\IT_related\myGame\Survivalon\Logs\m095_editmode_results.xml`
  - `-logFile C:\IT_related\myGame\Survivalon\Logs\m095_editmode.log`
  - final result:
    - `614 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m095_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m095_editmode.log`
