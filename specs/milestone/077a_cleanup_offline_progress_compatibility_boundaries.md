# Milestone 077a - Cleanup Offline-Progress Compatibility Boundaries

## Goal
- Tighten the ownership and naming of the new offline-progress compatibility hook from Milestone 077.
- Keep player-facing behavior unchanged and keep offline rewards disabled.

## What Changed
- Renamed the persisted offline-progress compatibility substate so it reads as a stable-save anchor only:
  - `PersistentOfflineProgressStableSaveAnchorState`
- Renamed the saved metadata accessors to stable-save-anchor wording instead of offline-reward eligibility wording.
- Kept the hook owned by the existing stable safe-save seam in `SafeResumePersistenceService`.
- Kept the older serialized field names privately in the save model so existing `077` save data still loads while the public runtime API now uses the narrower stable-save-anchor terminology.
- Older serialized data still loads safely:
  - save data with no hook field still initializes safely
  - save data written with the older `077` field names still maps into the renamed stable-save anchor state
- Tightened tests so they now explicitly prove the hook is stamped only through stable safe-save paths and does not create rewards or mutate balances on load by itself.

## Behavior Change
- No player-facing behavior change.
- This is a behavior-preserving cleanup follow-up to Milestone 077.
- Offline rewards are still disabled.
- No offline claim flow exists yet.
- No elapsed-time reward calculation or background simulation exists yet.
- Safe resume still restores into the same clear world-map safe context.

## Tests
- Updated `Assets/Tests/EditMode/State/Persistence/SafeResumePersistenceServiceTests.cs`
  - stable resolved-world save stamps the stable-save anchor metadata
  - loading fallback state alone does not stamp the anchor or mutate balances
  - run-only temporary upgrade state still does not persist
- Updated `Assets/Tests/EditMode/State/Persistence/PersistentStateModelTests.cs`
  - older serialized data without the field still initializes safely
  - legacy `077` serialized field names still deserialize into the renamed stable-save anchor state
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - resolved post-run autosave still stamps the stable-save anchor
  - restart/load still resumes to the world map without reopening unresolved run state or granting extra rewards
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - stable world/service save still stamps the stable-save anchor while keeping the same safe-resume behavior

## Out Of Scope
- Offline reward grants
- Offline claim/review UI
- Elapsed-time reward calculation
- Background simulation
- New save modes or startup routing changes
- Broader persistence redesign

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `Logs/unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - the known helper artifact issue reproduced again and did not create `Logs/editmode_results.xml`
- Fallback if the known helper artifact issue reproduces:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath 'C:\IT_related\myGame\Survivalon\Logs\m077a_editmode_results.xml' -LogPath 'C:\IT_related\myGame\Survivalon\Logs\m077a_editmode.log'`
- Final result:
  - `512 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m077a_editmode_results.xml`
  - `Logs/m077a_editmode.log`
