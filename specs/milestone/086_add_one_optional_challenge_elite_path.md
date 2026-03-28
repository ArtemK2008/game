# Milestone 086 - Add One Optional Challenge/Elite Path

## Goal
- Add one small optional elite/challenge path to the live prototype without turning it into a new subsystem.
- Keep it clearly subordinate to the current main push loop, service-hub access, and boss-gate progression.

## Delivered
- Added one new optional authored side-path combat node in the bootstrap world:
  - `Raider Holdout`
  - branches from the current forest push node
  - uses the shipped `Bulwark Raider` combat encounter
- Added one tiny authored/readability seam for optional challenge meaning:
  - `OptionalChallengeContentDefinition`
  - `OptionalChallengePresentationState`
  - `OptionalChallengePresentationStateResolver`
- Threaded that authored optional-challenge meaning through existing world/node-entry presentation flow:
  - world-map node labels now show `Elite challenge` for the shipped optional side node
  - the node-entry placeholder summary now shows `Encounter: Elite challenge`
- Kept the reward flow on existing ordinary reward seams and added only one modest authored yield distinction:
  - `Raider Holdout` grants the normal ordinary soft-currency reward
  - it also grants `Region material x3` through the existing region-material reward path with a node-owned `+2` yield bonus
- Kept the optional path clearly secondary:
  - it does not unlock the current service hub
  - it does not unlock the forest gate boss
  - it does not unlock the cavern gate

## Behavior Change
- The live prototype now has exactly one optional elite/challenge side destination in `Verdant Frontier`.
- That side content is clearly readable as optional challenge content in existing world-map and node-entry flow.
- The path is non-mandatory and subordinate to the main push/boss loop.
- Save/resume, autosave, replay, reward timing, grant timing, boss reward flow, and world-map safe-resume behavior stay unchanged.

## Tests
- Added `Assets/Tests/EditMode/World/OptionalChallengePresentationStateResolverTests.cs`
  - verifies only nodes with authored optional challenge content resolve the `Elite challenge` presentation label
- Updated `Assets/Tests/EditMode/World/BootstrapWorldGraphBuilderTests.cs`
  - verifies the new `Raider Holdout` node exists, is connected as an optional side branch, and carries the authored elite label plus yield bonus
- Updated `Assets/Tests/EditMode/World/BootstrapWorldStateSeederTests.cs`
  - verifies the new node receives bootstrap persistent state
- Updated `Assets/Tests/EditMode/World/NodeReachabilityResolverTests.cs`
  - verifies the new side branch appears in forward reachability from the current bootstrap world context
- Updated `Assets/Tests/EditMode/World/RouteChoiceSupportTests.cs`
  - verifies the current branching world-map choice now includes the optional elite side path
- Updated `Assets/Tests/EditMode/World/WorldMapWorldStateSummaryResolverTests.cs`
  - verifies the world-state summary now lists the extra forward side path
- Updated `Assets/Tests/EditMode/World/WorldMapScreenControllerTests.cs`
  - verifies the optional elite node is projected as a selectable forward option with the challenge label
- Updated `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - verifies the world-map node label shows `Elite challenge`
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - verifies the live world map shows the optional elite side path marker
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenPresentationTests.cs`
  - verifies node-entry summary text shows `Encounter: Elite challenge`
- Updated `Assets/Tests/EditMode/World/WorldNodeEntryFlowControllerTests.cs`
  - verifies the authored optional challenge content is carried into node-entry placeholder state
- Updated `Assets/Tests/EditMode/Run/RunRewardResolutionServiceTests.cs`
  - verifies the optional elite challenge grants a bounded ordinary reward bundle: `Soft currency x1, Region material x3`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - verifies the optional elite path is visible without replacing main routes
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`
  - verifies the optional elite run resolves cleanly, grants its bounded ordinary rewards, and does not unlock the main boss route

## Out Of Scope
- Any elite-tier subsystem, difficulty menu, affix/modifier layer, or multiple challenge tiers
- Any new save-state concept, offline/resume change, or automation change
- Any boss redesign, new currency, or broader loot/reward redesign
- Any large world-graph expansion beyond this one optional side path

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Script fallback attempt:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m086_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m086_editmode.log"`
  - result: exited without the requested result/log artifacts, so verification moved to the direct Unity batch fallback
- Direct waited fallback:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m086_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m086_editmode.log`
  - result: `563 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m086_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m086_editmode.log`
