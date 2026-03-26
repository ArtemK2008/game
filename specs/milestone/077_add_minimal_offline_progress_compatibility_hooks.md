# Milestone 077 - Add Minimal Offline-Progress Compatibility Hooks

## Goal
- Add the smallest honest persistence hooks needed so later offline-progress work can calculate elapsed time from stable saved state.
- Keep offline rewards disabled and preserve the current safe-resume world-map model.

## Delivered
- Added one small persisted compatibility substate:
  - `PersistentOfflineProgressCompatibilityState`
- `PersistentGameState` now carries that substate and lazily initializes it for older saved data that predates the new field.
- Reused the existing `SafeResumePersistenceService` save boundary instead of adding a parallel offline system.
- Stable resolved world/service saves now stamp:
  - a persisted UTC save timestamp
  - a persisted offline-progress eligibility marker tied to the stable save boundary
- Kept the existing safe-resume node/context data as the stable location anchor for future offline work.

## Behavior Change
- Current player-facing behavior is effectively unchanged.
- Offline rewards are still disabled.
- No offline claim popup exists.
- No background simulation exists.
- No extra resources, progression, or unlocks are granted on save or load.
- Temporary run-only state is still not persistent.

## Tests
- Updated `Assets/Tests/EditMode/State/Persistence/SafeResumePersistenceServiceTests.cs`
  - stable safe-save metadata is stamped, saved, and loaded correctly
  - saved resource balances remain unchanged by the new compatibility hook
- Updated `Assets/Tests/EditMode/State/Persistence/PersistentStateModelTests.cs`
  - older serialized game-state data without the new field still initializes a valid compatibility substate
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - resolved post-run autosave now explicitly proves the compatibility metadata is present on the saved snapshot
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - world/service safe persistence now explicitly proves the compatibility metadata is present on the saved snapshot
- Existing restart/load tests continued to prove that safe resume still returns to the same clear world-map context and that run-only temporary state does not resume.

## Out Of Scope
- No offline reward grant logic
- No offline reward payload calculation
- No offline claim/review UI
- No background/offline simulation
- No mid-run suspend/resume
- No save-slot, cloud-save, or broader persistence redesign

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `Logs/unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - compile/import passed there too, but the known helper artifact issue reproduced and `Logs/editmode_results.xml` was not created
- Fallback verification:
  - `& .\tools\run_editmode_tests.ps1 -ResultsPath 'C:\IT_related\myGame\Survivalon\Logs\m077_editmode_results.xml' -LogPath 'C:\IT_related\myGame\Survivalon\Logs\m077_editmode.log'`
- Final result:
  - `510 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- Artifacts:
  - `Logs/m077_editmode_results.xml`
  - `Logs/m077_editmode.log`
