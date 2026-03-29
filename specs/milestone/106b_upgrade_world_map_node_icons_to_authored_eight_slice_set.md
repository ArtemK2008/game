# Milestone 106b - Upgrade World Map Node Icons To Authored Eight-Slice Set

## Goal
- Tighten the staged 106a world-map presentation so the live node icons reflect authored node meaning instead of only the earlier generic four-state placeholder model.
- Preserve the authored world-map background surface, sidebar/map split, world-graph logic, reachability, selection flow, and safe-resume behavior.

## Delivered
- Preserved the new authored source sheet at:
  - `Assets/Art/WorldMap/Source/world_nodes_sheet_v2.png`
- Split that source sheet using the strict 4x2 equal-cell contract into canonical outputs under `Assets/Art/WorldMap/Nodes/`:
  - `ordinary_combat.png`
  - `farm.png`
  - `elite.png`
  - `boss_gate.png`
  - `service.png`
  - `locked.png`
  - `current.png`
  - `region_transition.png`
- Removed the no-longer-honest generic outputs:
  - `available.png`
  - `cleared.png`
- Upgraded the world-map art runtime seam from a four-state node-art model to an authored node-icon-kind model:
  - `WorldMapArtRegistry` now stores all eight authored icon references
  - `WorldMapArtResolver` now maps world-map node meaning and lock/current overrides to an icon kind
  - `WorldMapScreenController` now passes explicit authored node-yield meaning into `WorldMapNodeOption`
- Added focused coverage for:
  - registry loading of the eight authored icons
  - resolver mapping for locked/current/service/boss/elite/farm/ordinary cases
  - live world-map UI using the new icon family
- Updated the current art/build snapshot docs so the eight-slice sheet, canonical outputs, and prepared-only `region_transition` truth are recorded as shipped.

## Behavior Change
- The live world map no longer uses the earlier generic `locked` / `available` / `current` / `cleared` icon family.
- Live node icons now represent node meaning first, with lock/current overrides:
  - locked nodes -> `locked`
  - current-context or selected node -> `current`
  - service/progression nodes -> `service`
  - boss/gate nodes -> `boss_gate`
  - elite challenge combat nodes -> `elite`
  - ordinary combat nodes with explicit region-material yield content -> `farm`
  - other ordinary combat nodes -> `ordinary_combat`
- `region_transition.png` is currently prepared canonical art only. It is not runtime-used yet because the shipped world graph does not expose a separate honest region-transition node meaning.
- World progression logic, reachability, selection, entry behavior, and safe-resume behavior did not change.

## Tests
- Added:
  - `Assets/Tests/EditMode/World/WorldMapArtResolverTests.cs`
- Updated:
  - `Assets/Tests/EditMode/World/WorldMapArtRegistryTests.cs`
  - `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`

## Out Of Scope
- Another world-map screen redesign.
- New gameplay semantics just to force use of `region_transition`.
- New nodes, regions, progression systems, or world-graph behavior changes.
- Broader UI-art systems outside the current world-map art seam.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106b_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106b_editmode.log`
  - passed: `666 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - Unity did not create the requested project-local fallback result/log artifacts for this run
  - actual XML results written by Unity:
    - `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
