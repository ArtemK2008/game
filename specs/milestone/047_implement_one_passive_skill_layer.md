# Milestone 047 - Implement One Passive Skill Layer

## Decision
- Milestone 046 made the baseline attack skill-compatible, but combat still had no always-on skill layer beyond that basic attack.
- The current branch already carried `skillPackageId` on persistent characters and default skill-package ids on playable character profiles.
- The smallest correct step for Milestone 047 was to reuse that existing seam instead of inventing a new loadout screen, broader package editor, or speculative multi-skill system.

## Delivered
- Added one small live passive skill layer through the current character skill-package path.
- Introduced `CombatSkillPackageCatalog` so the current shipped skill-package ids can resolve passive combat skills.
- Added one live passive skill:
  - `Relentless Assault`
  - category: passive
  - activation: always on
  - current effect: `+20%` direct-damage output
- Wired passive-skill resolution into `CombatShellContextFactory`, so the selected playable character now enters combat with passive skills resolved from the current skill-package id.
- Extended `CombatEntityState` / `CombatEntityRuntimeState` so combat entities can carry resolved passive skills alongside the baseline attack definition.
- Added `CombatPassiveSkillEffectResolver` and used it from `CombatSkillExecutor` so passive aggregation stays isolated from the direct effect-application path.
- Kept the live behavior intentionally simple:
  - only one passive skill exists
  - only `Striker`'s default skill package currently grants that passive
  - `Vanguard` still enters combat with no passive skills
  - the passive applies to the current direct-damage combat path, which means it affects the automated baseline attack in the current prototype

## SRP Refactor
- `CombatSkillExecutor` would have started mixing two responsibilities if passive aggregation logic was embedded directly into direct-damage application:
  - applying a resolved combat skill effect
  - aggregating always-on passive modifiers from the acting combat entity
- This milestone extracted the second responsibility into `CombatPassiveSkillEffectResolver`.
- That keeps the executor readable while leaving a small explicit seam for later passive/active skill growth.

## Tests
- Added `CombatPassiveSkillEffectResolverTests` to verify:
  - no passive skills keep outgoing direct-damage multiplier at `1.0`
  - the current always-on passive resolves the expected `1.2` multiplier
- Updated `CombatSkillExecutorTests` to verify:
  - direct-damage application increases when the acting entity has the passive
- Updated `CombatEncounterResolverTests` to verify:
  - the automated baseline attack loop actually uses the passive layer during real combat advancement
- Updated `CombatEntityStateTests` to verify:
  - passive-skill lists are present on combat entities
  - `Striker`'s combat context resolves the current passive from the skill-package seam
- Updated `RunLifecycleControllerCombatTests` to verify:
  - selected playable character run entry still works
  - `Vanguard` enters with no passive skills
  - `Striker` enters with the resolved passive skill attached

## Intentionally Left Out
- More than one passive skill
- Active/castable skills
- Cooldown systems or skill UI
- Skill loadout editing
- Broader character-specific skill trees
- Any Milestone 048 or later work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
