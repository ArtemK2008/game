# Milestone 107 - Tighten test coverage around milestone-critical rules

## Goal
- Tighten unit coverage around shipped milestone-critical rules without redesigning systems or changing runtime behavior.
- Use the EditMode coverage pass first, then target brittle branching/fail-closed logic in core systems.

## Delivered
- Coverage-target selection was driven by the fresh coverage pass from:
  - `powershell -ExecutionPolicy Bypass -File .\tools\run_editmode_coverage.ps1`
- The chosen gaps were:
  - `Survivalon.Core.FileUserSettingsStorage`
    - low fail-closed storage coverage around missing, empty, unreadable, and malformed inputs
  - `Survivalon.State.Persistence.OfflineProgressClaimResolver`
    - branching offline-claim boundary rules around exact elapsed-time cutoff, safe-resume target type, stable-save presence, and missing-node fail-closed behavior
  - `Survivalon.State.Persistence.OfflineProgressClaimService`
    - missing negative-path coverage for invalid claim application without a persisted safe resume target
  - `Survivalon.Run.PostRunNextActionResolver`
    - missing precedence-order coverage for push vs service recommendations and for recommendation gating when world return is unavailable
- Added/updated focused EditMode tests:
  - added `Assets/Tests/EditMode/Core/FileUserSettingsStorageTests.cs`
  - updated `Assets/Tests/EditMode/State/Persistence/OfflineProgressClaimResolverTests.cs`
  - updated `Assets/Tests/EditMode/State/Persistence/OfflineProgressClaimServiceTests.cs`
  - updated `Assets/Tests/EditMode/Run/PostRunNextActionResolverTests.cs`
- Coverage-hardening only:
  - no runtime/production seam changed
  - no gameplay/UI/art/flow expansion was introduced

## Behavior Change
- No shipped runtime behavior changed.
- This milestone only hardened unit coverage around existing shipped rules and fail-closed behavior.

## Tests
- `FileUserSettingsStorageTests` now covers:
  - missing file -> `TryLoad` returns `false`
  - empty file -> `TryLoad` returns `false`
  - malformed file -> `TryLoad` returns `false`
  - unreadable locked file -> `TryLoad` returns `false`
  - save path creates parent directory and round-trips persisted settings
- `OfflineProgressClaimResolverTests` now also covers:
  - exact one-hour boundary claim
  - non-world-map safe-resume target rejection
  - no stable-save anchor rejection
  - missing-node fail-closed rejection
- `OfflineProgressClaimServiceTests` now also covers:
  - invalid claim attempt without a persisted safe resume target
- `PostRunNextActionResolverTests` now also covers:
  - newly unlocked push target taking precedence over service opportunity
  - available push recommendation when replay is unavailable
  - `Stop` recommendation when forward/service opportunities exist but returning to world is disabled

## Out Of Scope
- Any runtime redesign of offline progress, post-run flow, settings, or world-map systems
- UI/presentation polish, scene edits, art hookup, or gameplay changes
- Broad integration/end-to-end additions beyond the focused unit coverage needed for milestone-critical rules

## Verification
- Compile/import:
  - command: `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: passed
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - command: `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: helper reproduced the known missing-results artifact
  - missing helper artifact: `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback:
  - command:
    - `& 'C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe' -batchmode -nographics -projectPath 'C:\IT_related\myGame\Survivalon' -runTests -runSynchronously -testPlatform EditMode -testResults 'C:\IT_related\myGame\Survivalon\Logs\m107_editmode_results.xml' -logFile 'C:\IT_related\myGame\Survivalon\Logs\m107_editmode.log'`
  - result: passed
  - results: `C:\IT_related\myGame\Survivalon\Logs\m107_editmode_results.xml`
  - log: `C:\IT_related\myGame\Survivalon\Logs\m107_editmode.log`
  - Unity default results file: `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
- Coverage:
  - command: `powershell -ExecutionPolicy Bypass -File ".\tools\run_editmode_coverage.ps1"`
  - test results: `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode_test_results.xml`
  - coverage log: `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode_coverage.log`
  - HTML report: `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode\Report\index.htm`
  - alternate HTML entry: `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode\Report\index.html`
  - touched-class report pages:
    - `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode\Report\Survivalon.Runtime_FileUserSettingsStorage.html`
    - `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode\Report\Survivalon.Runtime_OfflineProgressClaimResolver.html`
    - `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode\Report\Survivalon.Runtime_OfflineProgressClaimService.html`
    - `C:\IT_related\myGame\Survivalon\Logs\coverage\editmode\Report\Survivalon.Runtime_PostRunNextActionResolver.html`
- Touched-area coverage improved in the refreshed report history:
  - `FileUserSettingsStorage`: `43.2% -> 89.1%` line coverage
  - `OfflineProgressClaimResolver`: `83.8% -> 91.1%` line coverage
  - `OfflineProgressClaimService`: `64.7% -> 76.4%` line coverage
  - `PostRunNextActionResolver`: `76.4% -> 88.2%` line coverage
