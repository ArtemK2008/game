# Milestone 092 - Implement Two Distinct Location Visual Identities

## Goal
- Make the two current shipped combat places read as distinct locations in the live runtime flow.
- Keep the implementation small, readable, and limited to combat-space background hookup.

## Delivered
- Added one small runtime-safe combat background seam:
  - `CombatLocationBackgroundRegistry`
  - `CombatLocationBackgroundResolver`
- Added the runtime-safe registry asset:
  - `Assets/Resources/CombatLocationBackgroundRegistry.asset`
- Extended `CombatShellPresentationStateResolver` so the combat shell now resolves:
  - player combat-state sprite
  - enemy combat-state sprite
  - current combat location background
- Updated `CombatShellView` so it renders a dedicated background-art image behind the existing combat shell content.
- Updated the live node/combat flow so background rendering stays inside the existing combat presentation path rather than adding ad hoc per-screen lookup logic.

## Asset Usage
- Used the prepared canonical combat backgrounds already present in the repo:
  - `Assets/Art/Locations/VerdantFrontier/Backgrounds/combat_background.png`
  - `Assets/Art/Locations/EchoCaverns/Backgrounds/combat_background.png`
- No new art was requested.
- The prepared service/town background remains untouched for Milestone `093`.

## Behavior Change
- Active combat in `Verdant Frontier` now shows the prepared Verdant Frontier combat background.
- Active combat in `Echo Caverns` now shows the prepared Echo Caverns combat background.
- The current combat shell now has two clearly different region looks without changing combat logic, reward flow, progression, persistence, or audio behavior.
- This milestone does not hook up:
  - service/town backgrounds
  - world-map backgrounds
  - combat VFX
  - animation controllers

## SRP Boundary
- `CombatLocationBackgroundRegistry` owns authored runtime-safe background references only.
- `CombatLocationBackgroundResolver` owns location-identity-to-background resolution.
- `CombatShellPresentationStateResolver` composes the complete combat presentation state from already-resolved combat and location inputs.
- `CombatShellView` only renders the resolved background and sprites.
- `NodePlaceholderScreen` remains a requester and does not interpret background asset rules directly.

## Tests
- Added `Assets/Tests/EditMode/Combat/CombatLocationBackgroundRegistryTests.cs`
  - verifies the runtime-safe registry resolves both shipped combat location backgrounds
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies the live combat shell renders a non-null background art image
  - verifies `Verdant Frontier` and `Echo Caverns` use distinct combat backgrounds in the live placeholder combat screen
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - verifies the world-map-to-combat runtime flow uses the resolved Verdant Frontier combat background in live startup/combat entry

## Out Of Scope
- Milestones `093` and `094`
- Service/town visual hookup
- World-map visual hookup
- Combat VFX hookup
- Animation controllers, parallax, shader systems, or broader rendering frameworks
- Any change to combat balance, save/resume, progression, rewards, or audio

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Wrapper fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m092_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m092_editmode.log"`
  - result: attempted, but did not create the requested artifacts
- Direct Unity batch fallback used for final verification:
  - `-batchmode`
  - `-nographics`
  - `-projectPath C:\IT_related\myGame\Survivalon`
  - `-runTests`
  - `-runSynchronously`
  - `-testPlatform EditMode`
  - `-testResults C:\IT_related\myGame\Survivalon\Logs\m092_editmode_results.xml`
  - `-logFile C:\IT_related\myGame\Survivalon\Logs\m092_editmode.log`
  - final result:
    - `596 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m092_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m092_editmode.log`
