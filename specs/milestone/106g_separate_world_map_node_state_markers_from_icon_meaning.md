# Milestone 106g - Separate World Map Node State Markers From Icon Meaning

## Goal
- Tighten the already-shipped 106a-106f world-map presentation so node meaning and node state no longer rely on the same visual layer.
- Preserve the current authored map structure, selection flow, hitbox alignment, node semantics, world logic, and safe-resume behavior.

## Delivered
- Kept the change inside the current presentation seams:
  - `WorldMapArtResolver`
  - `WorldMapScreenStateResolver`
  - `WorldMapSurfaceSectionView`
- Stopped using selected/current icon override as the main authored-map state cue.
- Kept the node icon as the meaning layer for authored-map nodes:
  - ordinary combat
  - farm
  - elite
  - boss gate
  - service
  - locked
- Added one explicit separate ring marker layer for authored-map state readability:
  - selected -> strongest gold ring
  - current -> distinct cyan ring
  - reachable -> restrained green ring
  - replayable cleared -> softer teal ring
  - locked -> no positive ring
- Kept the prepared `current` icon asset available in authored content, but the live authored map now communicates current state through the separate marker layer instead of a current-specific icon override.
- Kept locked nodes dim and kept the selected-only on-map label behavior.
- Preserved non-raycast labels and centered hit targets.

## Behavior Change
- Authored world-map node state is now primarily communicated by explicit marker rings instead of depending mainly on icon tint/glow.
- Current-vs-selected-vs-reachable-vs-replayable reads more quickly because state no longer overrides the node’s meaning icon family.
- Gameplay behavior did not change.

## Tests
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapArtResolverTests.cs`
  - `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`

## Out Of Scope
- New gameplay systems.
- New map layout systems.
- New tooltips, animations, or extra node semantics.
- Any change to world logic, progression, reachability, or entry flow.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106g_state_marker_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106g_state_marker_editmode.log`
  - passed: `679 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - results: `C:\IT_related\myGame\Survivalon\Logs\m106g_state_marker_editmode_results.xml`
  - log: `C:\IT_related\myGame\Survivalon\Logs\m106g_state_marker_editmode.log`
  - no failed test cases remained in the result XML after the focused UI-test correction
