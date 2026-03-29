# Milestone 101 - add one more meaningful progression sink or powerup branch

## Goal
- Add one more meaningful long-term progression branch without creating a disconnected new subsystem.
- Reuse the existing account-wide project board, town/service progression hub, persistence model, and material-refinement loop.

## Delivered
- Chose the smallest honest 101 implementation: one new account-wide project on the existing progression board.
- Added `Refinement Efficiency Project` to the shipped account-wide upgrade catalog.
- Extended the account-wide progression effect model with one new resolved effect:
  - `RegionMaterialRefinementOutputBonus`
- Added one small town conversion effect seam so the same authored conversion rule can be read consistently by:
  - the live town/service conversion interaction
  - the town/service projection/readability state
- Updated the town/service progression summary so the existing refinement row and projected material-to-project loop reflect the purchased refinement-efficiency branch automatically.

## Behavior Change
- `Cavern Service Hub` now exposes one additional persistent project:
  - `Refinement Efficiency Project`
  - cost: `Persistent progression material x2`
- Before purchase, the existing refinement action remains:
  - `Region material x3 -> Persistent progression material x1`
- After purchase:
  - that same refinement action becomes `Region material x3 -> Persistent progression material x2`
  - the stronger output persists across restart/load through the existing progression save model
  - the live conversion row and material-power-path summary both show the stronger refinement value
- Existing combat, world progression, menu/settings, save/resume, and town routing behavior stay unchanged.

## Tests
- Added or updated focused EditMode coverage for:
  - authored progression catalog exposure of the new project
  - resolved account-wide progression effects for the new refinement-output bonus
  - progression-board purchase flow for the new project
  - town progression purchase persistence for the new project
  - town conversion behavior with and without the purchased refinement bonus
  - town/service presentation and live startup -> town flow readability for the new project branch

## Out Of Scope
- Milestone 102 or later work
- a new progression subsystem, new currency, or new menu flow
- additional towns, regions, enemies, characters, or inventory systems
- conversion chains beyond the current fixed `Region Material Refinement`
- broader town/service redesign

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - passed
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback:
  - Unity `6000.3.10f1`
  - log path: `C:\IT_related\myGame\Survivalon\Logs\m101_editmode.log`
  - result path requested: `C:\IT_related\myGame\Survivalon\Logs\m101_editmode_results.xml`
  - the batch run failed before test results were written because Unity could not connect to the Package Manager local server process in this session
- Additional wrapper attempt:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m101_wrapper_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m101_wrapper_editmode.log"`
  - returned without producing the requested artifacts
- Later Milestone `102` audit recheck:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - passed again
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - reproduced the known helper artifact issue again and still did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
  - direct Unity batch fallback again failed before writing results because Unity could not connect to the Package Manager local server process in this session
  - log: `C:\IT_related\myGame\Survivalon\Logs\m102_editmode.log`
