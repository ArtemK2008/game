# Milestone 047a - Harden Passive Skill Model Shape

## Goal
- Keep the Milestone 047 passive-skill seam small, readable, and safe before more skill work builds on it.
- Preserve current gameplay behavior exactly.

## Delivered
- Removed the effect-specific `DirectDamageMultiplier` payload from `CombatSkillDefinition`.
- Kept `CombatSkillDefinition` limited to generic skill metadata:
  - skill id
  - display name
  - category
  - activation type
  - effect type
- Moved the currently shipped passive-effect interpretation fully into `CombatPassiveSkillEffectResolver`.
- Kept the implementation intentionally small:
  - `CombatPassiveSkillEffectResolver` explicitly recognizes the current shipped passive by skill id
  - `Relentless Assault` still grants the same `+20%` outgoing direct-damage multiplier
- Hardened passive-skill handling against null entries:
  - `CombatEntityState` now snapshots passive skills while skipping null entries
  - `CombatPassiveSkillEffectResolver` also safely skips null entries defensively

## Behavior
- No gameplay behavior changed.
- `Striker` still receives `Relentless Assault` from the current skill package.
- `Vanguard` still has no passive skills.
- Automated combat outcomes remain unchanged.

## SRP Reasoning
- `CombatSkillDefinition` was starting to mix two responsibilities:
  - generic skill identity/metadata
  - effect-specific numeric payload data
- This cleanup restores a narrower role for `CombatSkillDefinition` and keeps the current passive effect rule inside the passive effect layer where that behavior belongs.
- `CombatSkillExecutor` remains focused on executing resolved skill effects and does not regain passive-aggregation responsibility.

## Tests
- Added `CombatSkillDefinitionTests` to verify the definition no longer exposes the direct-damage multiplier payload.
- Updated `CombatPassiveSkillEffectResolverTests` to verify:
  - `Relentless Assault` still resolves the same `1.2` multiplier
  - null passive entries do not break passive resolution
- Updated `CombatEntityStateTests` to verify null passive entries are skipped safely during combat-entity construction.
- Existing combat execution and encounter-flow tests remain in place to protect behavior.

## Intentionally Left Out
- Any new passive skills
- Any new gameplay behavior
- Any active-skill or loadout work
- Any Milestone 048 or later implementation

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct Unity batch invocation and report that explicitly.
