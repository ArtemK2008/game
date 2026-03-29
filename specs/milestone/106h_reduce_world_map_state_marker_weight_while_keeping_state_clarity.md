# Milestone 106h - Reduce World Map State-Marker Weight While Keeping State Clarity

## Goal
- Keep the authored-map state-marker architecture from `106g`, but make it lighter and more art-friendly for demo use.
- Preserve world-map logic, selection behavior, reachability, safe resume, and the meaning-first node icon model.

## Delivered
- Kept the change inside the current presentation seams:
  - `WorldMapScreenStateResolver`
  - `WorldMapSurfaceSectionView`
- Preserved explicit state separation:
  - selected and current still use full outer rings
  - reachable and replayable still use separate state markers
- Reduced overall marker weight:
  - thinner marker geometry
  - smaller marker sizes
  - lower alpha/opacity
  - less neon/glow feel over the authored map art
- Changed lower-priority states from heavy full rings to lighter bottom-arc markers:
  - reachable -> restrained green bottom arc
  - replayable cleared -> softer teal bottom arc
- Kept the state hierarchy readable:
  - selected -> strongest gold full ring
  - current -> clearly visible cyan full ring
  - reachable -> lighter green bottom arc
  - replayable cleared -> softer teal bottom arc
  - locked -> dim icon only, no positive marker
- Kept hit targets centered on the visible node icon and kept selected labels non-raycast.

## Behavior Change
- Authored-map node state markers are now less visually heavy and integrate with the map art more cleanly.
- Selected/current remain immediately readable through full rings, while reachable/replayable no longer read like full-circle debug overlays.
- Gameplay behavior did not change.

## Tests
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`

## Out Of Scope
- Any new gameplay systems.
- Any new tooltip, animation, or hover layer.
- Any change to world logic, node semantics, progression, reachability, or entry flow.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106h_marker_weight_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106h_marker_weight_editmode.log`
  - passed: `679 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - results: `C:\IT_related\myGame\Survivalon\Logs\m106h_marker_weight_editmode_results.xml`
  - log: `C:\IT_related\myGame\Survivalon\Logs\m106h_marker_weight_editmode.log`
  - Unity also wrote its default results file: `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
