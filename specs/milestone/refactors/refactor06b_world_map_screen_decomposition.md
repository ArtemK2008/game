# Refactor milestone 06b — WorldMapScreen decomposition

## Scope
- decompose the oversized `WorldMapScreen` MonoBehaviour into small local section views
- keep world-map controller/state/text-builder behavior unchanged
- preserve layout, button behavior, texts, and refresh flow

## Why this refactor was needed
- `Assets/Scripts/World/WorldMapScreen.cs` owned:
  - overview text creation
  - build section text creation
  - character/package/gear button creation
  - node-list scroll view creation
  - entry-button creation
  - refresh orchestration
  - layout rebuild orchestration
- that made the MonoBehaviour carry too many UI-creation responsibilities in one file

## What changed
- added `Assets/Scripts/World/WorldMapOverviewSectionView.cs`
- added `Assets/Scripts/World/WorldMapBuildSectionView.cs`
- added `Assets/Scripts/World/WorldMapEntryActionSectionView.cs`
- added `Assets/Scripts/World/WorldMapNodeListSectionView.cs`
- rewrote `Assets/Scripts/World/WorldMapScreen.cs` so it now:
  - wires runtime dependencies
  - asks controller/text-builder/services for state
  - refreshes the extracted section views
  - keeps only high-level screen orchestration

## What stayed intentionally unchanged
- `WorldMapScreenController` still owns world-state and access-side projection
- `WorldMapScreenTextBuilder` still owns player-facing world-map wording
- object names, labels, button ids, and refresh behavior stayed the same so existing world-map tests and placeholder flow remain stable
- no gameplay, reachability, sorting, persistence, or selection logic changed

## Verification
- compile/import first via `tools/unity_compile_check.ps1`
- EditMode verification via `tools/unity_editmode_verify.ps1`
- known helper artifact issue reproduced again, so verification used the direct fallback:
  - `tools/run_editmode_tests.ps1`
