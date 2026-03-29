# Milestone 106d - Clean Up World Map Node Visuals For Demo Presentation

## Goal
- Tighten the already-shipped 106a/106b/106c world-map presentation so the authored bootstrap map reads cleanly in the current 16:9 demo view.
- Preserve the authored background surface, authored node-icon seam, world-graph logic, selection flow, reachability, progression, and safe-resume behavior.

## Delivered
- Kept the existing authored-map architecture in place:
  - `WorldMapSurfaceSectionView`
  - `WorldMapNodeLayoutResolver`
  - `WorldMapArtRegistry`
  - `WorldMapArtResolver`
  - the current map-plus-sidebar split
- Removed the large visible square node backings from the authored bootstrap map presentation.
- Kept generous click targets while shrinking the visible node treatment to a smaller icon-first presentation.
- Added restrained authored-map-only glow treatment for current/selected nodes instead of large filled plates.
- Tightened authored-map label behavior so only the selected node shows an on-map nameplate by default; the current node now reads through icon/highlight plus the existing sidebar summary.
- Reduced authored-map chrome and padding around the surface so the background art fills the main pane more cleanly without changing world logic or adding new systems.
- Narrowed the sidebar sizing slightly so the map surface stays the visual hero in the current demo view.

## Behavior Change
- The authored bootstrap map now presents clean node icons directly over the map art instead of large translucent square blocks.
- Current and selected node states remain readable, but through smaller icon-first treatment and subtle glow instead of oversized plates.
- Only the selected node now shows a visible authored-map nameplate by default.
- Unknown/test graphs still keep the existing fallback vertical-scroll presentation and broader per-node readability.
- Entry flow, progression, node semantics, reachability, safe resume, and other gameplay behavior did not change.

## Tests
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`

## Out Of Scope
- New world-map gameplay systems.
- New tooltips, animations, or presentation subsystems.
- New authored world logic, new nodes, or new progression behavior.
- Any redesign of the world-map controller/state architecture.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106d_worldmap_cleanup_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106d_worldmap_cleanup_editmode.log`
  - passed: `675 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - result artifact: `C:\IT_related\myGame\Survivalon\Logs\m106d_worldmap_cleanup_editmode_results.xml`
  - log artifact: `C:\IT_related\myGame\Survivalon\Logs\m106d_worldmap_cleanup_editmode.log`
  - Unity also wrote its default results file to: `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
