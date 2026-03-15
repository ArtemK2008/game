using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class AccountWideProgressionEffectResolverTests
    {
        [Test]
        public void ShouldReturnZeroEffectsWhenNoAccountWideUpgradesArePurchased()
        {
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            AccountWideProgressionEffectState effects = resolver.Resolve(new PersistentProgressionState());

            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(0));
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
        }

        [Test]
        public void ShouldRejectMissingProgressionState()
        {
            AccountWideProgressionEffectResolver resolver = new AccountWideProgressionEffectResolver();

            Assert.That(
                () => resolver.Resolve(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("progressionState"));
        }
    }
}
