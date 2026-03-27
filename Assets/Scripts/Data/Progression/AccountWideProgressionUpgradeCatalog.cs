using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.Data.Progression
{
    /// <summary>
    /// Хранит authored catalog account-wide progression upgrades для текущего shipped prototype.
    /// </summary>
    public static class AccountWideProgressionUpgradeCatalog
    {
        private static readonly AccountWideProgressionUpgradeDefinition CombatBaselineProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.CombatBaselineProject,
                "Combat Baseline Project",
                "account_wide_combat_baseline_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 1,
                playerMaxHealthBonus: 10,
                playerAttackPowerBonus: 0,
                ordinaryRegionMaterialRewardBonus: 0);
        private static readonly AccountWideProgressionUpgradeDefinition PushOffenseProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.PushOffenseProject,
                "Push Offense Project",
                "account_wide_push_offense_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 2,
                playerMaxHealthBonus: 0,
                playerAttackPowerBonus: 4,
                ordinaryRegionMaterialRewardBonus: 0);
        private static readonly AccountWideProgressionUpgradeDefinition FarmYieldProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.FarmYieldProject,
                "Farm Yield Project",
                "account_wide_farm_yield_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 1,
                playerMaxHealthBonus: 0,
                playerAttackPowerBonus: 0,
                ordinaryRegionMaterialRewardBonus: 1);
        private static readonly AccountWideProgressionUpgradeDefinition BossSalvageProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.BossSalvageProject,
                "Boss Salvage Project",
                "account_wide_boss_salvage_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 2,
                playerMaxHealthBonus: 0,
                playerAttackPowerBonus: 0,
                ordinaryRegionMaterialRewardBonus: 0,
                bossProgressionMaterialRewardBonus: 1);
        private static readonly AccountWideProgressionUpgradeDefinition FarmReplayProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.FarmReplayProject,
                "Farm Replay Project",
                "account_wide_farm_replay_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 3,
                playerMaxHealthBonus: 0,
                playerAttackPowerBonus: 0,
                ordinaryRegionMaterialRewardBonus: 0,
                bossProgressionMaterialRewardBonus: 0,
                enablesFarmReadyQuickReplayShortcut: true);
        private static readonly AccountWideProgressionUpgradeDefinition[] AllDefinitions =
        {
            CombatBaselineProject,
            PushOffenseProject,
            FarmYieldProject,
            BossSalvageProject,
            FarmReplayProject,
        };

        public static IReadOnlyList<AccountWideProgressionUpgradeDefinition> All => AllDefinitions;

        public static AccountWideProgressionUpgradeDefinition Get(AccountWideUpgradeId upgradeId)
        {
            switch (upgradeId)
            {
                case AccountWideUpgradeId.CombatBaselineProject:
                    return CombatBaselineProject;
                case AccountWideUpgradeId.PushOffenseProject:
                    return PushOffenseProject;
                case AccountWideUpgradeId.FarmYieldProject:
                    return FarmYieldProject;
                case AccountWideUpgradeId.BossSalvageProject:
                    return BossSalvageProject;
                case AccountWideUpgradeId.FarmReplayProject:
                    return FarmReplayProject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeId), upgradeId, "Unknown account-wide upgrade id.");
            }
        }
    }
}

