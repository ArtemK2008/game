# Milestone 100 - Add Another Enemy Family/Profile Set

## Goal
- Strengthen location identity in the current playable graph by introducing one more real enemy family/profile set through the existing authored combat and world-content seams.

## Delivered
- Added one new standard enemy profile and encounter:
  - `Ruin Sentinel`
- Wired `Ruin Sentinel` through the existing standard-enemy catalog and standard-encounter catalog.
- Wired the prepared `RuinSentinel` split combat sprites through the existing `CombatEntitySpriteRegistry` runtime seam.
- Reassigned the current `Sunscorch Ruins` combat nodes to the new family:
  - `Scorched Approach`
  - `Ruin Span`
  - `Ash Cache`
- Updated `Sunscorch Ruins` authored enemy-emphasis text to match the shipped runtime content.

## Behavior Change
- `Sunscorch Ruins` now uses a distinct enemy family in live combat instead of reusing the existing forest/cavern standard enemies.
- The live combat shell now shows `Ruin Sentinel` visuals and stats for the current `Sunscorch Ruins` encounters.
- The new region now reads as mechanically distinct through enemy diversity as well as background identity.

## Tests
- Updated `Assets/Tests/EditMode/Combat/CombatEnemyProfileResolverTests.cs`
- Updated `Assets/Tests/EditMode/Combat/CombatEntitySpriteRegistryTests.cs`
- Updated `Assets/Tests/EditMode/Data/CombatEncounterDefinitionCatalogTests.cs`
- Updated `Assets/Tests/EditMode/Data/LocationIdentityCatalogTests.cs`
- Updated `Assets/Tests/EditMode/World/BootstrapWorldGraphBuilderTests.cs`
- Updated `Assets/Tests/EditMode/World/WorldNodeEntryFlowControllerTests.cs`
- Updated `Assets/Tests/EditMode/World/NodePlaceholderTestData.cs`
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`

## Out Of Scope
- No new region or world-map redesign
- No new character
- No new progression sink or power branch
- No combat-system redesign
- No new animation/VFX/audio framework work

## Verification
- Compile/import check:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - known helper artifact may leave `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml` missing
- Direct Unity batch fallback:
  - results: `C:\IT_related\myGame\Survivalon\Logs\m100_editmode_results.xml`
  - log: `C:\IT_related\myGame\Survivalon\Logs\m100_editmode.log`
