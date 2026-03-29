# Milestone 106c - Make World Map Demo Ready At 16:9

## Goal
- Tighten the already-shipped 106a/106b world-map presentation so the current 16:9 game view reads like a game map instead of a debug-heavy layout.
- Preserve the authored background surface, authored node-icon seam, world-graph logic, reachability, selection flow, and safe-resume behavior.

## Delivered
- Kept the authored map surface as the primary screen structure and preserved:
  - `WorldMapSurfaceSectionView`
  - `WorldMapNodeLayoutResolver`
  - `WorldMapArtRegistry`
  - `WorldMapArtResolver`
- Tightened the world-map screen split so the authored map surface now claims the main width and the supporting sidebar stays secondary.
- Constrained the sidebar overview/build/action sections so their preferred widths no longer overwhelm the layout.
- Changed the sidebar build-option rows to stacked lists with narrower, wrapping labels so the side panel stays compact in the current 16:9 presentation.
- Gave the map surface a real preferred/min width so the layout system treats it as the visual hero instead of a small secondary strip.
- Kept the compact summary readable while removing the earlier debug-heavy feel:
  - summary remains compact and player-facing
  - build summary remains short
  - authored-map labels remain limited to current/selected nodes only
- Updated focused world-map and startup integration tests so they assert the shipped presentation truth instead of the older always-visible per-node caption behavior.

## Behavior Change
- The live world map now presents as a large authored map surface with a noticeably narrower supporting sidebar in the current 16:9 view.
- The main authored map no longer depends on scrolling for the shipped authored bootstrap graph.
- Node icons remain meaning-first from the 106b authored icon family, while node names on the map stay limited to current/selected contexts to avoid caption overlap.
- Entry flow, world progression, node reachability, safe resume, and gameplay behavior did not change.

## Tests
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`

## Out Of Scope
- New world-map gameplay systems.
- A full world-map redesign.
- New node meanings, new assets, new progression behavior, or new world logic.
- Tooltips, animation systems, or broader map interaction layers.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106c_worldmap_polish_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106c_worldmap_polish_editmode.log`
  - passed: `675 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - result artifact: `C:\IT_related\myGame\Survivalon\Logs\m106c_worldmap_polish_editmode_results.xml`
  - log artifact: `C:\IT_related\myGame\Survivalon\Logs\m106c_worldmap_polish_editmode.log`
