# Milestone 079 - Implement Auto-Pick Baseline If Desired

## Goal
- Lower the current farming loop friction by letting the shipped run-start upgrade-choice seam continue into auto-battle without a manual click.
- Keep the change narrow, run-only, and aligned with the existing `Burst Strike` upgrade-choice seam.

## Delivered
- `RunLifecycleController` now auto-picks the first available run-time skill-upgrade option when automatic combat flow starts and no run-only upgrade has been selected yet.
- The current shipped deterministic baseline is `Burst Tempo`, because it is the first option in the existing `CombatRunTimeSkillUpgradeCatalog`.
- The existing run-only upgrade-choice model, option catalog, and presentation helpers remain in place for later milestones.
- Startup/world/node/town/persistence tests were updated so they validate the new low-friction baseline instead of the older blocking manual-choice step.

## Behavior Change
- Combat-compatible runs that expose the current shipped `Burst Strike` run-only upgrade seam no longer block auto-battle behind a manual click.
- The current run automatically uses `Burst Tempo` for that run only, then continues into the normal auto-battle loop.
- The auto-picked upgrade is still temporary:
  - it does not persist across replay-created fresh run controllers
  - it does not persist into world return, restart/load, or safe resume
  - it does not change persistent character, package, or save-state data
- Runs without the current run-only upgrade seam still behave as before.

## Tests
- Updated `Assets/Tests/EditMode/Run/RunLifecycleControllerCombatTests.cs`
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`
- Updated `Assets/Tests/EditMode/State/Persistence/SafeResumePersistenceServiceTests.cs`
- Updated `Assets/Tests/EditMode/Towns/TownServiceBuildPreparationInteractionServiceTests.cs`

## Out Of Scope
- Manual upgrade preferences or player-configurable auto-pick behavior
- New persistent state for run-only upgrade choices
- New upgrade pools, rarity, draft flow, or repeated in-run upgrade chains
- Broader replay/save/resume/offline-progress changes beyond the existing seams

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: reproduced the known helper artifact issue and did not create `Logs/editmode_results.xml`
- Fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m079_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m079_editmode.log"`
  - result: `523 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m079_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m079_editmode.log`
