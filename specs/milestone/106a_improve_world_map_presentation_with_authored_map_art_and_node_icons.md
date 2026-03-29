# Milestone 106a - Improve World Map Presentation With Authored Map Art And Node Icons

## Goal
- Improve the current world-map presentation for demo use after Milestone 106 without redesigning world-graph logic, reachability, selection flow, or safe-resume behavior.
- Use the newly provided authored world-map background and node-state sheet through the smallest clean runtime seam.

## Delivered
- Preserved the provided authored source files:
  - `Assets/Art/WorldMap/Backgrounds/world_map_background.png`
  - `Assets/Art/WorldMap/Nodes/node_states_sheet.png`
- Split the node-state sheet into canonical per-state outputs alongside the source sheet:
  - `Assets/Art/WorldMap/Nodes/locked.png`
  - `Assets/Art/WorldMap/Nodes/available.png`
  - `Assets/Art/WorldMap/Nodes/current.png`
  - `Assets/Art/WorldMap/Nodes/cleared.png`
- Normalized the world-map background, retained node sheet, and split node outputs into the same single-sprite 2D import style already used by the repo's gameplay sprite assets.
- Added one runtime-safe art seam:
  - `WorldMapArtRegistry`
  - `WorldMapArtResolver`
- Added one authored-layout seam:
  - `WorldMapNodeLayoutResolver`
- Replaced the old placeholder list-only surface in the live world-map screen with `WorldMapSurfaceSectionView`, which renders:
  - the authored map background
  - current world-graph connection lines
  - overlaid interactive node buttons
  - authored node-state icons
- Added `Assets/Resources/WorldMapArtRegistry.asset` so the live world-map screen resolves the background and node-state icons without hardcoded asset lookups.
- Updated focused world-map tests and added `WorldMapArtRegistryTests`.
- Updated the current art/build snapshot docs so the new world-map art and hookup are recorded as shipped.

## Behavior Change
- The live world map no longer reads as a small bottom strip.
- The authored world-map background is now the main visual surface, with current node selection and entry behavior overlaid on top of it.
- Current shipped bootstrap nodes now use authored map positions and authored `locked` / `available` / `current` / `cleared` node-state icons.
- Unknown or test-only graphs still fail closed to the existing vertical scroll fallback layout instead of forcing guessed authored placement.
- World progression logic, route reachability, safe resume, and entry confirmation behavior did not change.

## Tests
- Added:
  - `Assets/Tests/EditMode/World/WorldMapArtRegistryTests.cs`
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`

## Out Of Scope
- New world-map gameplay systems.
- New map progression rules or world-graph restructuring.
- Extra node art states beyond the authored four-state sheet.
- Boss/service/farm-specific icon families not supported by the provided sheet.
- Broader UI redesign outside the world-map presentation surface.

## Verification
- Compile/import check:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - passed
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- Standard EditMode verification helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - reproduced the known missing-results helper artifact
  - expected helper artifact was not created: `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback:
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106a_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106a_editmode.log`
  - passed: `666 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - Unity did not write the requested project-local result/log artifacts for this fallback run
  - actual XML results written by Unity:
    - `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
