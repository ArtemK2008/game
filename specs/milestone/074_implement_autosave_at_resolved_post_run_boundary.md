# Milestone 074 - Implement Autosave At Resolved Post-Run Boundary

## Goal
- Make the resolved post-run boundary itself a real safe persistence point.
- Persist already-applied durable run outcomes before the player chooses replay, return to world, or stop.

## Delivered
- Reused the existing safe-resume persistence path at the resolved post-run boundary instead of adding a new save system.
- Added one small post-run boundary persistence hook:
  - `BootstrapWorldContextTransitionService.PersistResolvedPostRunBoundary`
- Wired `BootstrapStartup` to pass that hook into `NodePlaceholderScreen`.
- `NodePlaceholderScreen` now invokes that hook once when the lifecycle first enters `PostRun`.
- Kept the existing return-to-world and stop-session save path unchanged.

## Behavior Change
- When a run resolves and the screen enters post-run, the current durable state is now autosaved immediately:
  - world node state, node progress, and unlock changes
  - persistent resources and materials already granted by the run
  - persistent progression state
  - persistent character/build/loadout/gear state already carried by game state
  - safe resume target for the existing world-level resume model
- Closing the game from the resolved post-run screen no longer risks losing already resolved run rewards or progression.
- Replay still starts a fresh run from the same node and still does not persist temporary run-only state.

## Tests
- Updated `Assets/Tests/EditMode/Startup/BootstrapWorldContextTransitionServiceTests.cs`
  - added direct coverage for resolved post-run boundary persistence through the existing safe-resume service
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - autosave triggers on post-run entry before return/stop
  - saved state already contains resolved node progress, rewards, and character progression
  - replay still works after the earlier autosave
  - startup resumes to the world map safe context from a post-run autosave instead of reopening unresolved run state

## Out Of Scope
- No mid-run save or suspend
- No offline progress changes
- No save-slot or cloud-save work
- No new player-facing save UI
- No broader persistence redesign beyond the resolved post-run safe point

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File 'tools/unity_compile_check.ps1'`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `powershell -NoLogo -NoProfile -File 'tools/unity_editmode_verify.ps1'`
  - compile/import passed there too, but the known helper artifact issue reproduced and `Logs/editmode_results.xml` was not created
- Fallback verification:
  - `& .\tools\run_editmode_tests.ps1 -ResultsPath 'C:\IT_related\myGame\Survivalon\Logs\m074_editmode_results.xml' -LogPath 'C:\IT_related\myGame\Survivalon\Logs\m074_editmode.log'`
- Final result:
  - `505 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- Artifacts:
  - `Logs/m074_editmode_results.xml`
  - `Logs/m074_editmode.log`
