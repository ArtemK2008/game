# Milestone 106e - Clarify World Map Node Availability And Hitbox Alignment

## Goal
- Tighten the already-shipped 106a/106b/106c/106d world-map presentation so node interaction feels accurate and node availability/state is readable at a glance on the authored bootstrap map.
- Preserve authored map structure, world logic, selection flow, reachability rules, progression, and safe-resume behavior.

## Delivered
- Kept the existing authored-map architecture and only refined the presentation/state seam:
  - `WorldMapSurfaceSectionView`
  - `WorldMapScreenStateResolver`
- Tightened authored-map hit targets so they stay centered on the visible node icon art.
- Kept hit targets generous, but reduced the authored-map hit area slightly so clicks feel closer to the visible icon.
- Added restrained meaning-preserving state accents without bringing back large square node plates:
  - reachable/enterable nodes now get a brighter availability accent
  - replayable cleared nodes get a subtler replay/readability accent
  - locked nodes are dimmer
  - current and selected nodes remain distinct through their existing separate highlight treatments
- Kept the selected-only on-map label behavior from `106d`.

## Behavior Change
- On the authored bootstrap map, node availability is now easier to read at first glance:
  - locked
  - reachable / enterable
  - current
  - selected
  - cleared replayable
- Hitbox feel is tighter because the authored-map click target remains centered on the visible node icon instead of reading as a larger off-center block.
- No world-graph, progression, selection, entry, or safe-resume behavior changed.

## Tests
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`

## Out Of Scope
- New gameplay systems.
- Tooltips, animation, or hover layers.
- New world-map controller logic or node semantics.
- New art assets or a broader map redesign.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106e_worldmap_availability_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106e_worldmap_availability_editmode.log`
  - passed: `678 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - result artifact: `C:\IT_related\myGame\Survivalon\Logs\m106e_worldmap_availability_editmode_results.xml`
  - log artifact: `C:\IT_related\myGame\Survivalon\Logs\m106e_worldmap_availability_editmode.log`
  - Unity also wrote its default results file to: `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
