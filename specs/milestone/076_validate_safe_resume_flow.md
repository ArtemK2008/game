# Milestone 076 - Validate Safe Resume Flow

## Goal
- Confirm that the shipped safe-resume path restores the game into one clear safe context after restart/load.
- Prove that resolved post-run autosave and town/service persistence do not reopen unresolved run state or temporary run-only choice state.

## Delivered
- Treated Milestone 076 as an acceptance/hardening milestone after verifying that the current shipped model already resumes through the world-map safe context.
- Tightened existing startup-flow EditMode coverage instead of adding a new save system or new resume target types.
- Added one focused startup-flow regression test for temporary run-only upgrade choice state.
- Strengthened existing restart/load tests so they now explicitly prove that only the intended world-level screen is active after resume and that stale run/service screens do not stay active.

## Behavior Change
- No new gameplay, save-system, or UI-flow behavior was introduced.
- This milestone confirmed that the current shipped safe-resume model is:
  - restart/load returns to a clear world-map safe context
  - resolved post-run autosave does not reopen unresolved run/post-run UI
  - town/service-origin saved state also resumes through that same clear world-map context
  - temporary run-only upgrade choice state is not resumed

## Tests
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - strengthened the resolved post-run restart/load test to assert that only the world map is active after resume
  - added coverage proving that a temporary run-only upgrade choice does not resume after restart/load and must be chosen fresh on re-entry
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupTownServiceFlowTests.cs`
  - strengthened the town/service restart/load coverage to assert that restart resumes to a readable world-map safe context, not an already-open service screen or unresolved run screen

## Out Of Scope
- No new safe-resume target types
- No direct resume into town/service context
- No mid-run save or suspend/resume
- No offline progress
- No save slots, cloud sync, or save UI changes
- No broader `BootstrapStartup` redesign

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - log: `Logs/unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - compile/import passed there too, but the known helper artifact issue reproduced and `Logs/editmode_results.xml` was not created
- Fallback verification:
  - `& .\tools\run_editmode_tests.ps1 -ResultsPath 'C:\IT_related\myGame\Survivalon\Logs\m076_editmode_results.xml' -LogPath 'C:\IT_related\myGame\Survivalon\Logs\m076_editmode.log'`
- Final result:
  - `509 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- Artifacts:
  - `Logs/m076_editmode_results.xml`
  - `Logs/m076_editmode.log`
