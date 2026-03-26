# Milestone 048 - Implement One Auto-Triggered Active Skill

## Decision
- Milestone 046 created the baseline attack seam and Milestone 047 added one passive layer, but combat still had no live triggered active skill running through that same structure.
- The smallest correct next step was to reuse the existing character skill-package seam and add exactly one periodic direct-damage skill that could trigger automatically in the current 1v1 combat loop.
- `Striker` already owned the more offense-oriented combat identity, so the least speculative place for the first live active skill was `Striker`'s default package.

## Delivered
- Added one live auto-triggered active combat skill through the current character skill-package path:
  - `Burst Strike`
  - category: triggered active
  - activation: periodic auto-trigger
  - current trigger interval: `2.5` seconds
  - current effect: boosted direct damage through the existing combat-skill executor seam
- Extended `CombatSkillPackageCatalog` so the current shipped skill-package ids can resolve one triggered active skill in addition to passive skills.
- Extended `CombatEntityState` / `CombatEntityRuntimeState` so combat entities can carry one resolved triggered active skill plus its runtime timer state.
- Updated `CombatEncounterResolver` so encounter progression now advances the triggered-active timer, recognizes when that skill becomes due, and executes it automatically without changing the manual-input model.
- Preserved current combat constraints:
  - combat remains fully automated
  - targeting remains current 1v1 auto-target selection
  - baseline attacks still run on their original timing
  - the existing passive-skill behavior still applies to direct-damage skills, which means `Relentless Assault` also boosts `Burst Strike`
- Kept the live runtime scope intentionally small:
  - only one active skill exists
  - only `Striker`'s default package grants it
  - no cooldown UI, casting bar, mana system, or loadout editor was added

## SRP Refactor
- `CombatSkillExecutor` already owned applying resolved combat skill effects.
- Adding skill-specific direct-damage coefficients directly into that executor would have mixed effect application with skill-profile interpretation.
- This milestone extracted the second responsibility into `CombatDirectDamageSkillEffectResolver`.
- Trigger interval interpretation was also isolated in `CombatTriggeredActiveSkillTimingResolver`, so the active-skill timing rule stays out of generic combat metadata and does not sprawl inside encounter orchestration.

## Tests
- Added `CombatDirectDamageSkillEffectResolverTests` to verify:
  - baseline attack keeps its `1.0x` attack-power multiplier
  - `Burst Strike` resolves the expected boosted direct-damage multiplier
- Added `CombatTriggeredActiveSkillTimingResolverTests` to verify:
  - `Burst Strike` resolves the expected `2.5s` periodic trigger interval
  - missing triggered active skills safely resolve to no timer
- Updated `CombatSkillExecutorTests` to verify:
  - the active skill executes through the current combat skill seam
  - the existing passive still amplifies the active skill's direct damage
- Updated `CombatEncounterResolverTests` to verify:
  - the active skill triggers automatically on its timer
  - the active skill routes through `ICombatSkillExecutor`
  - the active skill changes actual runtime combat outcome meaningfully
- Updated `CombatEntityStateTests` and `RunLifecycleControllerCombatTests` to verify:
  - `Striker` resolves the current active skill from the existing package path
  - `Vanguard` still has no active skill on combat entry
  - future run entry still uses the selected playable character without regressing existing passive/baseline behavior

## Out Of Scope
- Additional active skills
- Manual/castable skills
- Cooldown UI, cast bars, or mana/resource systems
- Skill loadout editing
- Multi-skill sequencing systems
- Any Milestone 049 or later work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
