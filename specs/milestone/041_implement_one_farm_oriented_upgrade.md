# Milestone 041 - Implement One Farm-Oriented Upgrade

## Delivered
- Extended the existing account-wide progression sink with one additional farm-oriented upgrade:
  - `FarmYieldProject`
  - consumes `PersistentProgressionMaterial`
  - grants a permanent bonus to ordinary `RegionMaterial` run rewards
- Kept the upgrade count small:
  - the previous max-health upgrade remains
  - the previous push-oriented attack upgrade remains
  - one new reward-efficiency upgrade was added
- Wired the new resolved effect into the existing ordinary reward-resolution path, so future standard region-material combat runs now grant more ordinary region-material reward when the upgrade is purchased.
- Kept milestone rewards distinct and unchanged; the new farm upgrade improves ordinary farming output only.

## Tests
- Updated `AccountWideProgressionBoardServiceTests` to verify:
  - the new upgrade has explicit affordability and buyability behavior
  - it spends the required persistent progression material on purchase
  - it records purchased account-wide progression state correctly
- Updated `AccountWideProgressionEffectResolverTests` to verify:
  - the new upgrade resolves into an ordinary region-material reward bonus
  - existing survivability and push-offense effects still work
  - all purchased account-wide upgrades combine cleanly without regressing previous effects
- Updated `RunRewardResolutionServiceTests` to verify:
  - the new farm upgrade increases ordinary region-material rewards on successful standard region-material combat runs
  - milestone reward output stays distinct and unchanged
- Updated `RunLifecycleControllerCombatTests` to verify the purchased upgrade visibly changes future run rewards through higher ordinary region-material output.
- Updated `BootstrapStartupCombatScreenFlowTests` to verify the upgraded reward output is visible in the current post-run summary and persists through return-to-world save flow.
- Updated `CombatEntityStateTests` to verify the farm-oriented upgrade does not incorrectly change combat baseline stats.
- Updated `SafeResumePersistenceServiceTests` to verify all purchased account-wide upgrades persist and resolve after the normal safe save boundary.

## Out Of Scope
- Any automation-comfort upgrade path
- Additional farm-oriented upgrade entries beyond the one new reward-efficiency upgrade
- Dedicated town/service UI access to the progression sink
- Broader progression-tree expansion or specialization
- Any Milestone 042 or later character/progression work

## Verification
- Unity EditMode batch run passed.
- Result: `279` passed, `0` failed.
- Artifacts:
  - `Logs/m041_editmode_results.xml`
  - `Logs/m041_editmode.log`
