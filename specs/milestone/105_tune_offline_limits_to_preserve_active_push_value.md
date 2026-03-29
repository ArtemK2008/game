# Milestone 105 - Tune Offline Limits To Preserve Active Push Value

## Goal
- Keep the shipped offline-progress flow clearly secondary to active play.
- Ensure offline farming support cannot become the best way to advance current progression.

## Delivered
- Tightened `OfflineProgressEligibilityResolver` so offline eligibility now requires both:
  - the existing safe world-map / `Farm-ready` ordinary combat context
  - explicit authored `RegionMaterialYieldContent` on the saved node
- Tightened `OfflineProgressClaimResolver` so offline claims now:
  - fail closed when the saved node does not carry explicit authored farm-yield content
  - cap counted whole hours to `2` instead of `8`
- Updated the focused offline persistence / claim tests to cover:
  - ineligible cleared push nodes without explicit farm-yield content
  - the lower hour cap
  - safe-save stamping staying ineligible for non-explicit farm contexts

## Behavior Change
- Offline farming support is now narrower and more conservative:
  - only clearly repeat-farm world contexts with explicit authored region-material yield are eligible
  - cleared push-oriented ordinary combat nodes without explicit farm-yield content are no longer offline-eligible
  - offline claim value is still limited to ordinary `Region material` only
  - offline claim value is now capped to `2` counted whole hours per return instead of `8`
- Active push value stays dominant because offline gains now remain a small farming supplement rather than a large banked material payout.

## Tests
- Updated `OfflineProgressEligibilityResolverTests` to prove cleared world-map nodes without explicit farm yield stay offline-ineligible.
- Updated `OfflineProgressClaimResolverTests` to prove:
  - the new lower cap is enforced
  - saved nodes without explicit farm-yield content do not produce a claim
- Updated `SafeResumePersistenceServiceTests` to prove resolved safe world saves without explicit farm-yield content stay offline-ineligible.

## Out Of Scope
- New offline reward categories.
- Any passive income while the game is open.
- Town/service offline payouts.
- Boss/gate or unresolved-run offline payouts.
- Deeper offline menus, history, or timer dashboards.

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
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m105_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m105_editmode.log`
  - passed: `666 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - Unity wrote the XML results to its default LocalLow path instead of the requested project log path:
    - results: `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
    - log: `C:\IT_related\myGame\Survivalon\Logs\m105_editmode.log`
