using NUnit.Framework;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class AccountWideProgressionEffectResolverTests
    {
        [Test]
        public void ShouldReturnZeroEffectsWhenNoAccountWideUpgradesArePurchased()
        {
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            AccountWideProgressionEffectState effects = resolver.Resolve(new PersistentProgressionState());

            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(effects.PlayerAttackPowerBonus, Is.EqualTo(0));
            Assert.That(effects.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(0));
        }

        [Test]
        public void ShouldResolvePurchasedCombatBaselineProjectIntoAppliedEffectState()
        {
            PersistentProgressionState progressionState = new PersistentProgressionState();
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.CombatBaselineProject);
            ProgressionEntryState progressionEntry = progressionState.GetOrAddEntry(
                upgradeDefinition.ProgressionId,
                upgradeDefinition.LayerType);
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            progressionEntry.Unlock();
            progressionEntry.IncreaseValue(1);

            AccountWideProgressionEffectState effects = resolver.Resolve(progressionState);

            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(10));
            Assert.That(effects.PlayerAttackPowerBonus, Is.EqualTo(0));
            Assert.That(effects.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(0));
        }

        [Test]
        public void ShouldResolvePurchasedPushOffenseProjectIntoAppliedEffectState()
        {
            PersistentProgressionState progressionState = new PersistentProgressionState();
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.PushOffenseProject);
            ProgressionEntryState progressionEntry = progressionState.GetOrAddEntry(
                upgradeDefinition.ProgressionId,
                upgradeDefinition.LayerType);
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            progressionEntry.Unlock();
            progressionEntry.IncreaseValue(1);

            AccountWideProgressionEffectState effects = resolver.Resolve(progressionState);

            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(effects.PlayerAttackPowerBonus, Is.EqualTo(4));
            Assert.That(effects.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(0));
        }

        [Test]
        public void ShouldResolvePurchasedFarmYieldProjectIntoAppliedEffectState()
        {
            PersistentProgressionState progressionState = new PersistentProgressionState();
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.FarmYieldProject);
            ProgressionEntryState progressionEntry = progressionState.GetOrAddEntry(
                upgradeDefinition.ProgressionId,
                upgradeDefinition.LayerType);
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            progressionEntry.Unlock();
            progressionEntry.IncreaseValue(1);

            AccountWideProgressionEffectState effects = resolver.Resolve(progressionState);

            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(effects.PlayerAttackPowerBonus, Is.EqualTo(0));
            Assert.That(effects.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(1));
        }

        [Test]
        public void ShouldResolveMultiplePurchasedProjectsWithoutRegressingExistingEffects()
        {
            PersistentProgressionState progressionState = new PersistentProgressionState();
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            UnlockPurchasedUpgrade(progressionState, AccountWideUpgradeId.CombatBaselineProject);
            UnlockPurchasedUpgrade(progressionState, AccountWideUpgradeId.PushOffenseProject);
            UnlockPurchasedUpgrade(progressionState, AccountWideUpgradeId.FarmYieldProject);

            AccountWideProgressionEffectState effects = resolver.Resolve(progressionState);

            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(10));
            Assert.That(effects.PlayerAttackPowerBonus, Is.EqualTo(4));
            Assert.That(effects.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(1));
        }

        [Test]
        public void ShouldRejectMissingProgressionState()
        {
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            Assert.That(
                () => resolver.Resolve(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("progressionState"));
        }

        private static void UnlockPurchasedUpgrade(
            PersistentProgressionState progressionState,
            AccountWideUpgradeId upgradeId)
        {
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(upgradeId);
            ProgressionEntryState progressionEntry = progressionState.GetOrAddEntry(
                upgradeDefinition.ProgressionId,
                upgradeDefinition.LayerType);
            progressionEntry.Unlock();
            progressionEntry.IncreaseValue(1);
        }
    }
}
