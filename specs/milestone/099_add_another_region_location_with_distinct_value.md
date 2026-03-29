# Milestone 099 - Add Another Region/Location With Distinct Value

## Goal
- Expand the playable world with one more distinct region while keeping progression readable and reusing the existing authored world and combat-background seams.

## Delivered
- Added one new authored region, `Sunscorch Ruins`, behind the current `Cavern Gate`.
- Added three new `Sunscorch Ruins` combat nodes:
  - `Scorched Approach`
  - `Ruin Span`
  - `Ash Cache`
- Gave `Sunscorch Ruins` a distinct role as a deeper late-region-material branch rather than another progression-material/service region.
- Connected `Cavern Gate` to `Sunscorch Ruins` through the existing boss-gate and world-connection structure.
- Wired the provided combat background through the existing runtime-safe combat background registry:
  - `Assets/Art/Locations/SunscorchRuins/Backgrounds/combat_background.png`
- Updated focused world, location-identity, post-run unlock-summary, and live combat background tests.

## Behavior Change
- The shipped bootstrap world graph now spans three regions instead of two.
- Clearing `Cavern Gate` now opens `Scorched Approach`, giving the current world a readable deeper destination after `Echo Caverns`.
- `Sunscorch Ruins` adds a small internal branch:
  - `Scorched Approach -> Ruin Span`
  - `Scorched Approach -> Ash Cache`
- Live combat in `Sunscorch Ruins` now uses its distinct prepared combat background.
- Post-run unlock messaging for the cavern boss now reads as a specific new place opening instead of a raw internal node id.

## Tests
- Updated `Assets/Tests/EditMode/Data/LocationIdentityCatalogTests.cs`
- Updated `Assets/Tests/EditMode/Combat/CombatLocationBackgroundRegistryTests.cs`
- Updated `Assets/Tests/EditMode/Run/PostRunResultPresentationStateResolverTests.cs`
- Updated `Assets/Tests/EditMode/World/BootstrapWorldGraphBuilderTests.cs`
- Updated `Assets/Tests/EditMode/World/BootstrapWorldStateSeederTests.cs`
- Updated `Assets/Tests/EditMode/World/WorldNodeEntryFlowControllerTests.cs`
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupStateFactoryTests.cs`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`

## Out Of Scope
- No new enemy family/profile set
- No new progression sink or power branch
- No second character
- No world-map UI redesign
- No save/resume semantic change
- No broader menu/settings change

## Verification
- Compile/import check:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- Result:
  - verification blocked in this session by Unity batch-mode license initialization failure (`No valid Unity Editor license found. Please activate your license.`)
  - EditMode verification was not run after the failed compile/import gate, per the repo verification workflow
