# Milestone 044 - Implement Character-Linked Progression

## Delivered
- Added one small persistent character-owned progression path on top of the existing playable-character state.
- Kept the implementation intentionally small and character-linked instead of account-wide:
  - the selected playable character now gains `+1` persistent `ProgressionRank` after each successful combat-compatible run
  - non-combat runs and failed combat runs do not increase character rank
- Wired character-linked progression into future combat run entry:
  - `PlayableCharacterProgressionEffectResolver` now resolves the selected character's current rank into a combat bonus
  - the current milestone uses one simple effect only: `+5` max health per rank
- Extended the run persistence seam so the selected character's persistent state travels with the existing selected character profile during run setup.
- Updated combat setup so character-linked progression is applied separately from the existing account-wide progression sink and then layered together on future runs.
- Preserved the current character roster and UI scope:
  - `character_vanguard` remains the only shipped playable character
  - the existing character-selection placeholder UI remains minimal
  - no dedicated character progression screen was added in this milestone

## Tests
- Added `PlayableCharacterProgressionEffectResolverTests` to verify:
  - unranked characters resolve no bonus
  - rank bonus scales correctly at `+5` max health per rank
- Added `PlayableCharacterProgressionServiceTests` to verify:
  - successful combat runs increase character rank
  - failed combat runs do not increase character rank
  - successful non-combat runs do not increase character rank
- Updated `CombatEntityStateTests` to verify combat setup applies the rank-based character bonus to future player baseline stats.
- Updated `RunLifecycleControllerCombatTests` to verify:
  - successful combat runs increase the selected persistent character rank
  - future run entry applies character-linked progression on top of existing account-wide upgrades without regressing those account-wide effects
- Updated `BootstrapStartupCombatScreenFlowTests` to verify:
  - the resolved-world save path persists granted character progression rank
  - replay after a successful run uses the stronger selected character baseline

## Out Of Scope
- Character progression spend UI or a dedicated character-progression screen
- Additional character progression axes beyond the one simple rank-based health bonus
- Character-specific talent trees or specialization paths
- A second playable character
- Character skill-system expansion
- Gear/loadout redesign
- Any Milestone 045 or later character-system expansion

## Verification
- `tools/run_editmode_tests.ps1` returned immediately without producing result artifacts in this shell, so milestone verification used a direct waited Unity batch invocation.
- Verified artifacts:
  - `Logs/m044_editmode_results.xml`
  - `Logs/m044_editmode.log`
- Result: `307` passed, `0` failed.
