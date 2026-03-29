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
            Assert.That(AccountWideProgressionUpgradeCatalog.All.Count, Is.EqualTo(6));
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
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.RefinementEfficiencyProject).DisplayName,
                Is.EqualTo("Refinement Efficiency Project"));
            Assert.That(
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.BossSalvageProject).DisplayName,
                Is.EqualTo("Boss Salvage Project"));
            Assert.That(
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.FarmReplayProject).DisplayName,
                Is.EqualTo("Farm Replay Project"));
        }

        [Test]
        public void ShouldKeepRefinementEfficiencyProjectStaticCostAndEffectValues()
        {
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.RefinementEfficiencyProject);

            Assert.That(upgradeDefinition.CostResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(upgradeDefinition.CostAmount, Is.EqualTo(2));
            Assert.That(upgradeDefinition.RegionMaterialRefinementOutputBonus, Is.EqualTo(1));
            Assert.That(upgradeDefinition.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.PlayerAttackPowerBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.BossProgressionMaterialRewardBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.EnablesFarmReadyQuickReplayShortcut, Is.False);
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
            Assert.That(upgradeDefinition.EnablesFarmReadyQuickReplayShortcut, Is.False);
        }

        [Test]
        public void ShouldKeepFarmReplayProjectStaticCostAndComfortFlagValues()
        {
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.FarmReplayProject);

            Assert.That(upgradeDefinition.CostResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(upgradeDefinition.CostAmount, Is.EqualTo(3));
            Assert.That(upgradeDefinition.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.PlayerAttackPowerBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.BossProgressionMaterialRewardBonus, Is.EqualTo(0));
            Assert.That(upgradeDefinition.EnablesFarmReadyQuickReplayShortcut, Is.True);
        }
    }
}
