# Milestone 106 - Tighten Spec / Code Alignment

## Goal
- Audit already-shipped systems against the governing current specs and compact current-state snapshot.
- Fix only real mismatches without broad redesign.

## Delivered
- Audited the current shipped startup, offline, world/post-run, town/build, and current snapshot/spec seams for concrete mismatches.
- Fixed one real runtime mismatch in `PostRunResultPresentationStateResolver`:
  - boss-gate unlock text now preserves the exact authored node display name from the world graph
  - the resolver no longer rewrites authored player-facing names into sentence-case variants
  - fallback behavior still fails closed to the unlocked node id when authored node data cannot be resolved
- Updated focused post-run and startup flow tests to match the authored-name behavior.
- Corrected the compact build snapshot wording where it had drifted behind shipped offline behavior:
  - the offline state is now described as an active persisted offline-progress model, not only as future-facing compatibility
  - the snapshot now explicitly states that offline elapsed time is anchored to app/session startup, so open-app main-menu waiting does not count

## Behavior Change
- Boss-gate unlock presentation now matches authored player-facing node data exactly:
  - `Cavern Gate opened`
  - `Scorched Approach opened`
- No broader feature behavior changed.
- Offline, menu/settings, world, town, roster, and combat systems were otherwise kept as-is after audit because the shipped runtime already matched the current governing specs closely enough.

## Tests
- Updated:
  - `PostRunResultPresentationStateResolverTests`
  - `PostRunSummaryTextBuilderTests`
  - `BootstrapStartupProgressionScreenFlowTests`
- No broad new test suite was added because this pass fixed one concrete runtime mismatch and one docs-drift area.

## Out Of Scope
- Broader UX or system redesigns.
- New milestone content beyond 106.
- Historical milestone-note rewrites where the notes remain valid as historical records.
- Any mismatch that would require a larger architectural change instead of a narrow alignment correction.

## Verification
- Compile/import check:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - passed
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- Standard EditMode verification helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - reproduced the known missing-results helper artifact
  - expected results file was not created: `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback:
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m106_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m106_editmode.log`
  - passed: `666 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - Unity wrote the XML results to its default LocalLow path instead of the requested project log path:
    - results: `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
    - log: `C:\IT_related\myGame\Survivalon\Logs\m106_editmode.log`
