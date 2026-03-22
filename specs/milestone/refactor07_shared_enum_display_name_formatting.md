# Refactor milestone 07 - shared enum/display-name formatting

## Scope
- extract duplicated player-facing label formatting for shared core enum/category values
- keep sentence composition inside existing world/town/run text builders
- preserve current shipped wording

## What was duplicated
- `Assets/Scripts/Towns/TownServiceScreenTextBuilder.cs`
  - resource-category display names
- `Assets/Scripts/Run/PostRunSummaryTextBuilder.cs`
  - resource-category display names
- `Assets/Scripts/World/WorldMapScreenTextBuilder.cs`
  - node-type and node-state display names
- `Assets/Scripts/World/NodePlaceholderScreenTextBuilder.cs`
  - node-type display names

## Refactor
- added `Assets/Scripts/Core/PlayerFacingCoreLabelFormatter.cs`
- moved shared label formatting there for:
  - `ResourceCategory`
  - `NodeType`
  - `NodeState`
- left screen-specific full-sentence wording inside:
  - `WorldMapScreenTextBuilder`
  - `NodePlaceholderScreenTextBuilder`
  - `TownServiceScreenTextBuilder`
  - `PostRunSummaryTextBuilder`
- kept world-map path-role wording local because it is still only used inside the world-map slice

## Result
- shared core labels now have one source of truth
- world/town/run builders still own sentence composition and screen-specific wording
- player-facing text stayed unchanged

## Verification
- compile/import first via `tools/unity_compile_check.ps1`
- EditMode verification via `tools/unity_editmode_verify.ps1`
- if the known helper artifact issue reproduces, verification uses `tools/run_editmode_tests.ps1`
