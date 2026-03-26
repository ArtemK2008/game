# Milestone 050 - Implement Run-Time Upgrade Affecting Skills

## Decision
- The smallest honest run-time upgrade step was to reuse the existing triggered-active skill seam instead of building a broader in-run upgrade system.
- A run-only choice now appears only when the selected current package already grants `Burst Strike`.
- That keeps the feature small, keeps default `Vanguard` flow unchanged, and proves a real temporary combat choice without touching persistent package assignment.

## Delivered
- Added one compact run-start upgrade choice layer for the current triggered active skill path.
- The current run-only choices are:
  - `Burst Tempo`
    - replaces `Burst Strike` for the current run
    - keeps the same damage profile
    - triggers faster than the base `Burst Strike`
  - `Burst Payload`
    - replaces `Burst Strike` for the current run
    - keeps the same trigger timing
    - hits harder than the base `Burst Strike`
- Added `CombatRunTimeSkillUpgradeCatalog` and `CombatRunTimeSkillUpgradeOption` so the available temporary options stay explicit and out of UI code.
- Added `PlayableCharacterCombatSkillResolver` so package-based passive/active skill resolution can be reused by both combat-context creation and run-start choice availability without duplicating package logic.
- `RunLifecycleController` now:
  - exposes whether a run-time skill choice is required at run start
  - accepts one valid temporary choice for the current run only
  - blocks combat auto-start until that temporary choice is made when applicable
  - passes the selected temporary skill override into combat context creation
- `NodePlaceholderScreen` now shows a small placeholder run-start choice panel only when that temporary choice exists, then auto-starts combat immediately after the player picks one option.

## Behavior Change
- The new choice is **run-only**.
- It does **not** persist into character state, package assignment, replayed future runs, or account-wide progression.
- Current persistent package assignment still works exactly as before:
  - it determines whether `Burst Strike` is present on run entry
  - if `Burst Strike` is present, the run-time choice can temporarily tune it for that run
- Default `Vanguard` runs still auto-start immediately because they still enter with no triggered active skill by default.

## SRP Notes
- `CombatShellContextFactory` was starting to own both combat-context assembly and package-based skill resolution details.
- This milestone extracted the package-based active/passive resolution into `PlayableCharacterCombatSkillResolver`.
- That keeps:
  - combat context assembly in `CombatShellContextFactory`
  - temporary run-upgrade option policy in `CombatRunTimeSkillUpgradeCatalog`
  - run-start choice state and transition flow in `RunLifecycleController`
  - placeholder button wiring in `NodePlaceholderScreen`

## Tests
- Added `CombatRunTimeSkillUpgradeCatalogTests` to verify:
  - `Burst Strike` resolves the expected temporary run-only upgrade choices
  - missing triggered active skills resolve no run-time upgrade options
- Updated combat resolver/executor tests to verify:
  - the run-time upgraded active skill variants still resolve through the existing combat-skill seam
  - their timing/damage rules resolve as expected
- Updated `RunLifecycleControllerCombatTests` to verify:
  - runs with `Burst Strike` now require a temporary run-time choice before auto-start
  - that choice changes the resolved combat skill for the current run
  - the choice is not persisted into a new run controller
  - existing package assignment still feeds the temporary run-time choice path
- Updated node-placeholder and bootstrap flow tests to verify:
  - the placeholder choice panel appears for `Striker`/`Burst Strike` entry
  - combat auto-start resumes after choosing one option
  - replay requires the temporary run-time choice again

## Out Of Scope
- Broader in-run upgrade pools
- More than one temporary choice event per run
- In-run upgrade drafting/chains
- Cooldown UI, mana systems, or cast bars
- Any Milestone 051 or later work
