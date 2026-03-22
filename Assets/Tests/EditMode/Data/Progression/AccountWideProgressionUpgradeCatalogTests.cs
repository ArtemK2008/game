using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Progression;

namespace Survivalon.Tests.EditMode.Data.Progression
{
    /// <summary>
    /// Проверяет статический shipped catalog account-wide progression upgrades после переноса в Data.Progression.
    /// </summary>
    public sealed class AccountWideProgressionUpgradeCatalogTests
    {
        [Test]
        public void ShouldExposeAllShippedAccountWideUpgradeDefinitions()
        {
            Assert.That(AccountWideProgressionUpgradeCatalog.All.Count, Is.EqualTo(4));
            Assert.That(
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.CombatBaselineProject).DisplayName,
                Is.EqualTo("Combat Baseline Project"));
            Assert.That(
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.PushOffenseProject).DisplayName,
                Is.EqualTo("Push Offense Project"));
            Assert.That(
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.FarmYieldProject).DisplayName,
                Is.EqualTo("Farm Yield Project"));
            Assert.That(
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.BossSalvageProject).DisplayName,
                Is.EqualTo("Boss Salvage Project"));
        }

        [Test]
        public void ShouldKeepBossSalvageProjectStaticCostAndEffectValues()
        {
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.BossSalvageProject);

            Assert.That(upgradeDefinition.CostResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(upgradeDefinition.CostAmount, Is.EqualTo(2));
            Assert.That(upgradeDefinition.BossProgressionMaterialRewardBonus, Is.EqualTo(1));
            Assert.That(upgradeDefinition.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.PlayerAttackPowerBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(0));
        }
    }
}
