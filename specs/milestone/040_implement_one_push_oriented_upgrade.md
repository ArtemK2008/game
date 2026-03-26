# Milestone 040 - Implement One Push-Oriented Upgrade

## Delivered
- Extended the existing account-wide progression sink with one additional push-oriented upgrade:
  - `PushOffenseProject`
  - consumes `PersistentProgressionMaterial`
  - grants a permanent player attack-power bonus
- Kept the upgrade count small:
  - the previous max-health upgrade remains
  - one new attack-oriented upgrade was added
- Wired the new resolved effect into the existing combat-baseline integration path, so future combat runs now use the increased player attack baseline when the upgrade is purchased.
- Kept enemy baseline behavior unchanged.
- The new upgrade is visibly push-oriented in the current prototype because it improves harder future combat more directly and can turn the current placeholder boss/gate combat from failure into success.

## Tests
- Updated `AccountWideProgressionBoardServiceTests` to verify:
  - the new upgrade has explicit affordability/buyability behavior
  - it spends the required persistent progression material on purchase
  - it records purchased account-wide progression state correctly
- Updated `AccountWideProgressionEffectResolverTests` to verify:
  - the new upgrade resolves into an attack-power bonus
  - existing max-health resolution still works
  - both upgrades combine cleanly without regressing the previous effect
- Updated `CombatEntityStateTests` to verify the combat-shell context factory applies the new attack-power bonus to player combat stats.
- Updated `RunLifecycleControllerCombatTests` to verify the new push-oriented upgrade visibly changes future combat outcomes by flipping the current boss/gate combat result from failure to success.
- Updated `SafeResumePersistenceServiceTests` to verify both account-wide upgrades persist and resolve after the normal safe save boundary.

## Out Of Scope
- Any farm-oriented upgrade from Milestone 041
- Additional account-wide upgrade entries beyond the one new push-oriented upgrade
- Dedicated town/service UI access to the progression sink
- Broader progression-tree expansion or specialization

## Verification
- Unity EditMode batch run passed.
- Result: `270` passed, `0` failed.
- Artifacts:
  - `Logs/m040_editmode_results.xml`
  - `Logs/m040_editmode.log`
