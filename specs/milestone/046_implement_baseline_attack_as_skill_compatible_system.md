# Milestone 046 - Implement Baseline Attack As Skill-Compatible System

## Decision
- The current automated combat loop already worked, but baseline attack resolution still lived as a direct special-case path inside `CombatEncounterResolver`.
- That was enough for the current prototype, but it was too narrow for the next skill-focused milestones because the basic attack did not pass through any explicit skill-shaped seam.
- The smallest useful fix for this milestone was to keep the current 1v1 automated behavior and extract only the missing skill-compatible structure around the existing basic attack.

## Delivered
- Added a small explicit combat-skill model for the current baseline attack:
  - `CombatSkillDefinition`
  - `CombatSkillCategory`
  - `CombatSkillActivationType`
  - `CombatSkillEffectType`
  - `CombatSkillCatalog.BasicAttack`
- Extended `CombatEntityState` so each combat entity now carries a baseline attack definition instead of implicitly relying on a one-off resolver rule.
- Kept the current automated timing model intentionally simple:
  - combat runtime state now tracks time until the next baseline attack explicitly
  - the current baseline attack interval still derives from the entity's `AttackRate`
- Added `CombatSkillExecutionRequest` so future skill work has an explicit execution-shaped request object to build on.
- Added `ICombatSkillExecutor` and `CombatSkillExecutor` as the first dedicated skill-application seam.
- Refactored `CombatEncounterResolver` so it now focuses on:
  - advancing encounter time
  - checking when the next automated baseline attack is due
  - selecting the target
  - delegating the actual effect application through the skill executor
- Preserved current combat behavior:
  - player and enemy still auto-attack on their existing timing
  - basic attack still deals direct damage using current attack-power and defense mitigation rules
  - combat still resolves automatically when one side is defeated

## SRP Refactor
- `CombatEncounterResolver` was mixing two responsibilities:
  - time/turn orchestration for the automated combat loop
  - applying the actual effect of an attack and resolving victory on kill
- This milestone extracted the second responsibility into `CombatSkillExecutor`.
- That keeps the resolver narrow enough for future passive/active skill work without forcing a broad combat redesign yet.

## Tests
- Added `CombatSkillExecutorTests` to verify:
  - the baseline attack skill applies direct damage through the new executor seam
  - defeating damage through that seam still resolves encounter outcome correctly
- Updated `CombatEncounterResolverTests` to verify:
  - due baseline attacks now route through `ICombatSkillExecutor`
  - renamed baseline-attack timer semantics still behave correctly after defeat
- Updated `CombatEntityStateTests` to verify:
  - combat entities now expose the baseline attack definition
  - combat shell setup resolves the expected baseline attack metadata into both combat entities

## Out Of Scope
- Passive skill effects
- Additional active skills
- Skill cooldown UI or casting UI
- Skill-package runtime behavior beyond the existing placeholder ids
- Multi-skill combat sequencing
- Any Milestone 047 or later work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
