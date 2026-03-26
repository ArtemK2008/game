# Milestone 050a - Harden Run-Time Skill Upgrade Shape

## Decision
- The Milestone 050 behavior was kept, but the temporary run-only choice no longer pretends to be separate full combat skills.
- `Burst Strike` remains the one real triggered active skill.
- The run-only choice is now modeled as a temporary upgrade/modifier applied to that base skill for the current run.

## Delivered
- Removed the fake sibling skill definitions for `Burst Tempo` and `Burst Payload` from `CombatSkillCatalog`.
- Changed `CombatRunTimeSkillUpgradeOption` so it now carries only upgrade metadata:
  - stable upgrade id
  - display name
  - description
- Added `CombatTriggeredActiveSkillUpgradeResolver` so the shipped run-only modifier behavior is interpreted in one place:
  - `Burst Tempo` shortens `Burst Strike` interval to `1.75` seconds
  - `Burst Payload` increases `Burst Strike` direct-damage multiplier to `3x`
  - no modifier keeps the base `Burst Strike` behavior of `2.5` seconds and `2x`
- `CombatEntityState`, `CombatEntityRuntimeState`, and `CombatSkillExecutionRequest` now carry the selected temporary triggered-skill upgrade separately from the base triggered active skill.
- `CombatDirectDamageSkillEffectResolver` and `CombatTriggeredActiveSkillTimingResolver` now resolve the current behavior from:
  - base skill identity
  - selected run-only upgrade
- `NodePlaceholderScreen` no longer uses a fixed `132f` run-time upgrade panel height; the placeholder panel now sizes from content through layout components.

## Behavior Change
- Gameplay behavior remains unchanged from Milestone 050:
  - packages still determine whether `Burst Strike` is present
  - run start still pauses for one run-only choice when `Burst Strike` is present
  - `Burst Tempo` still makes the current run's `Burst Strike` trigger faster
  - `Burst Payload` still makes the current run's `Burst Strike` hit harder
  - the choice is still run-only and not persisted
  - replay still asks for the choice again

## SRP Notes
- `CombatSkillCatalog` is back to owning only real shipped skill identities.
- `CombatRunTimeSkillUpgradeCatalog` continues to own only the tiny shipped choice set, not the combat math for those choices.
- `CombatTriggeredActiveSkillUpgradeResolver` now owns the temporary modifier interpretation so timing/damage behavior is no longer split across multiple fake skill ids.
- `RunLifecycleController` still only orchestrates run-start selection state and flow; it does not interpret combat numbers.
- `NodePlaceholderScreen` still only wires placeholder UI and now relies on layout-driven sizing instead of a fragile fixed panel height.

## Tests
- Added `CombatTriggeredActiveSkillUpgradeResolverTests` to verify the isolated base/tempo/payload rules.
- Updated skill definition tests to lock that `Burst Tempo` and `Burst Payload` are no longer separate full combat skills in `CombatSkillCatalog`.
- Updated damage/timing/executor/encounter tests to verify the same combat behavior now resolves from:
  - base `Burst Strike`
  - selected run-only upgrade
- Updated run-flow and placeholder UI tests to verify:
  - the selected run-only upgrade remains separate from the base triggered active skill
  - the placeholder upgrade panel still shows readable choices and sizes from content without a fixed layout element

## Out Of Scope
- Additional run-time upgrades
- Broader in-run upgrade chains
- Cooldown UI, mana systems, cast bars, or broader combat UI redesign
- Any Milestone 051 or later work
