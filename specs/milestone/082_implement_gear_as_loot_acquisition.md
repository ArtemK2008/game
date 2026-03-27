# Milestone 082 - Implement Gear-As-Loot Acquisition

## Goal
- Expand the live loot/gear loop with one tightly controlled deterministic acquisition path.
- Keep the change small, readable, and aligned with the existing run reward, persistence, and equip seams.

## Delivered
- Added one new shipped earned gear item to the code-driven gear catalog:
  - `gear_primary_gatebreaker_blade` / `Gatebreaker Blade`
  - category: `PrimaryCombat`
  - effect: `+4` attack power when equipped
- Added one deterministic live reward source for that item:
  - the forest `Gate Boss` now carries `Gatebreaker Blade` as boss reward content
  - the reward resolves only when the boss run succeeds and the gear is not already owned
- Extended the existing run reward payload/grant path with one small gear-reward bucket instead of inventing a separate loot framework.
- Extended persistent owned-gear mutation so the awarded gear is added to account-owned gear state and survives save/load/bootstrap/safe-resume.
- Tightened gear-state initialization so startup/bootstrap only guarantees the shipped starter-owned gear ids instead of auto-owning every catalog item.
- Extended the current post-run summary with one compact `Gear rewards:` line when boss-earned gear is granted.

## Behavior Change
- The current build now has one live earned gear acquisition path:
  - defeating the forest gate boss grants `Gatebreaker Blade` if it is not already owned
- That earned gear is persisted in owned gear state, survives restart/load, and later appears in the existing world-map and town/service equip flows.
- Repeated clears do not create duplicate owned-gear entries.
- Broader loot behavior is still unchanged:
  - no randomness
  - no loot tables
  - no rarity system
  - no item instances
  - no selling, salvage, or crafting

## Tests
- Updated `Assets/Tests/EditMode/Data/GearCatalogTests.cs`
  - verifies the new shipped gear definition
- Updated `Assets/Tests/EditMode/Combat/PlayableCharacterGearCombatEffectResolverTests.cs`
  - verifies `Gatebreaker Blade` grants `+4` attack power when equipped
- Updated `Assets/Tests/EditMode/State/Persistence/PersistentGearStateInitializerTests.cs`
  - verifies startup normalization does not auto-grant the earned boss gear
- Updated `Assets/Tests/EditMode/Run/RunRewardResolutionServiceTests.cs`
  - verifies deterministic boss gear reward resolution and owned-gear dedup suppression
- Updated `Assets/Tests/EditMode/Run/RunRewardGrantServiceTests.cs`
  - verifies owned-gear mutation and dedup behavior
- Updated `Assets/Tests/EditMode/Run/PostRunSummaryTextBuilderTests.cs`
  - verifies compact player-facing gear reward summary output
- Updated `Assets/Tests/EditMode/Run/RunLifecycleControllerCombatTests.cs`
  - verifies the live run flow grants the earned gear into persistent game state
- Updated `Assets/Tests/EditMode/World/BootstrapWorldGraphBuilderTests.cs`
  - verifies the forest gate boss carries the shipped gear reward content
- Updated `Assets/Tests/EditMode/World/WorldNodeEntryFlowControllerTests.cs`
  - verifies the boss entry state exposes that reward content
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`
  - verifies the end-to-end shipped flow grants, saves, loads, and later equips `Gatebreaker Blade`

## Out Of Scope
- Random drops or broad loot tables
- Additional earned gear sources
- A general itemization framework
- Gear rarity, item instances, crafting, selling, or salvage
- Broader gear UI redesign beyond the existing equip placeholder flows

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m082_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m082_editmode.log"`
  - result: `546 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m082_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m082_editmode.log`
