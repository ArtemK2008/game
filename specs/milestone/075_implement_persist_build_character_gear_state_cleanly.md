# Milestone 075 - Persist Build/Character/Gear State Cleanly

## Goal
- Tighten the existing persistence flow so current build-preparation state survives restart/load cleanly.
- Keep the milestone narrow and persistence-focused rather than adding broader save systems or new build mechanics.

## Delivered
- Added one small world-map build-preparation interaction seam that persists successful character/package/gear changes immediately through the existing safe-resume persistence service.
- Reused the existing persistent game-state model and startup normalization flow instead of introducing new save models.
- Added focused restart/load coverage for:
  - world-map build-preparation changes
  - town/service build-preparation changes
  - non-persistence of temporary run-only upgrade choice state

## Behavior Change
- Selected playable character now survives restart/load cleanly after world-map build-prep changes.
- Per-character assigned skill package now survives restart/load cleanly after world-map and town/service build-prep changes.
- Per-character equipped primary/support gear now survives restart/load cleanly after world-map and town/service build-prep changes.
- Owned gear state continues to persist through the existing persistent game-state model and startup normalization.
- Startup/safe-resume restoration still returns to a clear world-level context rather than restoring unresolved run-only state.
- This milestone mostly hardened existing build-preparation persistence behavior rather than introducing a new player-facing system.

## Tests
- Updated `BootstrapStartupScreenFlowTests` to prove world-map build-prep changes survive restart/load and remain character-specific.
- Updated `BootstrapStartupTownServiceFlowTests` to prove town/service build-prep changes survive restart/load.
- Updated `SafeResumePersistenceServiceTests` to prove temporary run-only upgrade choice state is not persisted through save/load.

## Out Of Scope
- Mid-run save
- Offline progress
- Save slots
- Cloud sync
- New build-preparation UI
- New characters, gear, packages, or persistence mechanics beyond the current shipped state

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - Result: success
  - Log: `Logs/unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - Result: compile/import passed inside the helper flow, but the known artifact issue reproduced and `Logs/editmode_results.xml` was not created
- Fallback:
  - `& .\tools\run_editmode_tests.ps1 -ResultsPath 'C:\IT_related\myGame\Survivalon\Logs\m075_editmode_results.xml' -LogPath 'C:\IT_related\myGame\Survivalon\Logs\m075_editmode.log'`
  - Result: `508 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - Artifacts:
    - `Logs/m075_editmode_results.xml`
    - `Logs/m075_editmode.log`
