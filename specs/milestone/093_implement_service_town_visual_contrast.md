# Milestone 093 - Implement Service/Town Visual Contrast

## Goal
- Make the current service/town context feel calmer and visually distinct from combat.
- Keep the implementation small, readable, and limited to the already prepared `Cavern Service Hub` background hookup.

## Delivered
- Added one small runtime-safe service background seam:
  - `TownServiceBackgroundRegistry`
  - `TownServiceBackgroundResolver`
- Added the runtime-safe registry asset:
  - `Assets/Resources/TownServiceBackgroundRegistry.asset`
- Updated `BootstrapStartup` to resolve the current town/service background at the startup composition seam and pass it into the service screen.
- Updated `TownServiceScreen` to render a dedicated background-art image behind the existing service panel and scroll content without changing town/service mechanics.

## Asset Usage
- Used the prepared canonical service background already present in the repo:
  - `Assets/Art/Locations/CavernServiceHub/Backgrounds/service_background.png`
- No new art was requested.
- Combat backgrounds and combat-shell visuals were left unchanged.

## Behavior Change
- Entering `Cavern Service Hub` now renders the prepared service background behind the current town/service shell.
- The live service/town shell now reads as calmer and more distinct from combat while preserving the same existing progression, conversion, build-preparation, return, and stop flows.
- The service background is resolved through a dedicated town/service seam rather than reused through combat background logic.

## SRP Boundary
- `TownServiceBackgroundRegistry` owns authored runtime-safe service background references only.
- `TownServiceBackgroundResolver` owns service-context-to-background resolution.
- `BootstrapStartup` requests the resolved background at composition time.
- `TownServiceScreen` renders the already resolved background and does not own service background lookup policy.

## Tests
- Added `Assets/Tests/EditMode/Towns/TownServiceBackgroundRegistryTests.cs`
  - verifies the runtime-safe registry resolves the shipped `Cavern Service Hub` background
- Updated `Assets/Tests/EditMode/Towns/TownServiceScreenUiTests.cs`
  - verifies the town/service screen renders the resolved service background
  - verifies the rendered service background is distinct from the current `Echo Caverns` combat background
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupTownServiceFlowTests.cs`
  - verifies the live startup -> town/service flow renders the resolved `Cavern Service Hub` background in runtime

## Out Of Scope
- Milestone `094`
- Portrait/world-icon hookup
- Combat-shell visual changes beyond the already shipped combat backgrounds
- Combat VFX hookup
- Any change to combat logic, rewards, progression, save/resume, or audio behavior

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
  - `-testResults C:\IT_related\myGame\Survivalon\Logs\m093_editmode_results.xml`
  - `-logFile C:\IT_related\myGame\Survivalon\Logs\m093_editmode.log`
  - final result:
    - `597 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m093_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m093_editmode.log`
