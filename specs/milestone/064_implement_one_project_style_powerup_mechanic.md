# Milestone 064 - implement one project-style powerup mechanic

## Goal
Add one small persistent project/powerup mechanic beyond direct node unlocks by reusing the current account-wide progression sink, existing persistent progression material, and the current town/service purchase surface.

## Decision
- The smallest honest 064 step is to extend the existing account-wide project board instead of creating a second progression system.
- The new shipped project is `Boss Salvage Project`.
- It costs `Persistent progression material x2`.
- Its lasting output is `+1` persistent progression material on successful future boss clears.

This counts as a project-style powerup mechanic because it:
- consumes an existing persistent resource sink input
- persists in account-wide progression state
- improves future reward efficiency rather than only unlocking routes
- is bought through the existing town/service project board
- creates a medium-term repeatable growth goal around future boss farming

## Delivered
- Added `AccountWideUpgradeId.BossSalvageProject`.
- Extended the account-wide upgrade definition/effect model with one new structured effect field:
  - `BossProgressionMaterialRewardBonus`
- Added `Boss Salvage Project` to `AccountWideProgressionUpgradeCatalog`.
- Extended `AccountWideProgressionEffectResolver` so purchased boss-salvage state resolves into the live account-wide effect model.
- Extended `RunRewardResolutionService` so successful boss rewards now apply that resolved bonus on top of the current boss reward bundle.
- Kept the town/service screen structure unchanged while making the new project visible automatically through the existing progression section.

## Behavior Change
- Ordinary route/node unlock progression is unchanged.
- Existing combat baseline and ordinary reward behavior are unchanged unless the new project is purchased and the run is a successful boss clear.
- After purchase:
  - the project remains persistently unlocked
  - future successful boss clears grant `Persistent progression material x3` instead of `x2`
  - the town/service progression list shows `Boss Salvage Project` alongside the other current projects
- The current progression sink still remains small and MVP-readable:
  - `Combat Baseline Project`
  - `Push Offense Project`
  - `Farm Yield Project`
  - `Boss Salvage Project`

## SRP Notes
- Purchase policy remains in `AccountWideProgressionBoardService`.
- Account-wide project definition data remains in the progression definition/catalog classes.
- Gameplay effect resolution remains in the existing effect-resolution and reward-resolution seams.
- `TownServiceScreen` remains UI wiring and refresh only.
- No project-effect logic was pushed into UI builders or startup/bootstrap orchestration.

## Verification
- Followed the compile/import-first workflow from `AGENTS.md`.
- Ran:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- Then ran:
  - `tools/unity_editmode_verify.ps1`
  - compile/import passed again, but the helper still did not produce its expected EditMode results artifact in this shell
- Fallback verification used:
  - `tools/run_editmode_tests.ps1` with milestone-specific result/log paths
  - the sandboxed helper detached again, so the final successful verification used the same helper outside the sandbox
- Final EditMode result:
  - `449 passed`
  - `0 failed`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m064_editmode_results.xml`
  - `Logs/m064_editmode.log`

## Out Of Scope
- Milestone 065 or later powerup-system expansion
- project chains
- new currencies
- construction/building systems
- new town tabs or broader service navigation
- broader loot or crafting redesign
