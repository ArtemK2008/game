# Milestone 103 - Implement Minimal Offline-Progress Eligibility Model

## Goal
- Add the smallest honest offline-progress foundation by persisting explicit eligibility only for clearly farm-ready safe contexts.

## Delivered
- Added `OfflineProgressEligibilityKind` as the minimal persisted eligibility category.
- Added `OfflineProgressEligibilityResolver` to decide eligibility from authored world data plus the current safe-save target.
- Extended the existing offline-progress compatibility state so it now stores:
  - a stable-save UTC timestamp
  - an explicit offline-progress eligibility kind
- Kept the old legacy serialized compatibility field load-safe:
  - legacy saves still restore the stable-save timestamp
  - legacy compatibility data without explicit context now fails closed to `None`
- Updated `SafeResumePersistenceService` so safe-save stamping now records:
  - `FarmReadyWorldNode` only for resolved world-map contexts anchored to completed ordinary combat content
  - `None` for town/service safe saves and all other ambiguous or unsupported contexts
- Wired production persistence creation through `BootstrapStartup` so the shipped runtime uses the authored world graph when deciding eligibility.

## Behavior Change
- The current build now knows whether the last persisted safe context is offline-progress-eligible.
- Eligibility is intentionally narrow and fail-closed:
  - resolved world-level safe contexts on completed ordinary combat nodes marked `Farm-ready` are eligible
  - town/service safe contexts are not eligible
  - unresolved combat, boss/gate, service/progression, and ambiguous contexts remain ineligible
- No offline rewards, elapsed-time payout, claim flow, or new UI were added.

## Tests
- Added `OfflineProgressEligibilityResolverTests`.
- Updated `PersistentStateModelTests` for default and legacy compatibility loading of the new explicit eligibility kind.
- Updated `SafeResumePersistenceServiceTests` to cover:
  - eligible farm-ready world safe save
  - ineligible town/service safe save
  - unchanged stable-save timestamp behavior

## Out Of Scope
- Offline reward payout.
- Offline claim/review UI.
- Background simulation or passive-income systems.
- Broadening eligibility beyond clearly farm-ready world combat contexts.

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
  - attempted with results path `C:\IT_related\myGame\Survivalon\Logs\m103_editmode_results.xml`
  - failed before writing test results because Unity could not connect to the Package Manager local server
  - log: `C:\IT_related\myGame\Survivalon\Logs\m103_editmode.log`
- Wrapper fallback:
  - `tools/run_editmode_tests.ps1` was invoked with milestone-specific paths
  - returned without producing the requested results or log artifacts
