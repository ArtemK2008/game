# Milestone 098 - Add More Nodes To Prove World Structure Depth

## Goal
- Expand the current world graph enough to prove deeper structure without adding a new region, a new enemy family, or broader navigation/UI changes.

## Delivered
- Expanded the authored bootstrap world graph from 7 nodes to 10 nodes while keeping the same two shipped regions.
- Added three new `Echo Caverns` combat nodes behind the existing service hub:
  - `Echo Approach`
  - `Relic Cache`
  - `Gate Antechamber`
- Kept the existing forest opening readable and unchanged in immediate forward-route complexity.
- Kept the existing `Frontier Gate -> Cavern Gate` boss-unlock target intact so milestone progression readability did not regress.
- Updated the bootstrap persistent world-state seeding to include the new cavern combat nodes.
- Updated focused world-graph and world-map tests to cover the deeper cavern structure and live readable summary output.

## Behavior Change
- The current playable world graph is now meaningfully deeper.
- `Echo Caverns` no longer consists only of `Cavern Service Hub` plus `Cavern Gate`; it now also has a small internal combat branch:
  - `Cavern Service Hub -> Echo Approach -> Gate Antechamber -> Cavern Gate`
  - `Cavern Service Hub -> Relic Cache`
- The initial `Verdant Frontier` world-map state stays low-friction and still surfaces the same immediate forward choices:
  - `Forest Farm`
  - `Raider Holdout`
  - `Cavern Service Hub`
- Returning to the world map from `Cavern Service Hub` now shows deeper cavern forward routes in the live summary.

## Tests
- Updated `Assets/Tests/EditMode/World/BootstrapWorldGraphBuilderTests.cs`
- Updated `Assets/Tests/EditMode/World/BootstrapWorldStateSeederTests.cs`
- Updated `Assets/Tests/EditMode/World/WorldMapWorldStateSummaryResolverTests.cs`
- Updated `Assets/Tests/EditMode/World/WorldNodeEntryFlowControllerTests.cs`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`

## Out Of Scope
- No new region/location identity
- No new enemy family/profile set
- No new progression sink branch
- No new character
- No world-map UI redesign or save/resume semantic change

## Verification
- Compile/import check:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result artifact issue reproduced: `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml` was not created
- Direct Unity batch fallback:
  - results: `C:\IT_related\myGame\Survivalon\Logs\m098_editmode_results.xml`
  - log: `C:\IT_related\myGame\Survivalon\Logs\m098_editmode.log`
  - final result: `637 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
