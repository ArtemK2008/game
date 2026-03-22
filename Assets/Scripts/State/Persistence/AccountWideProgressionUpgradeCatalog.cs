using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
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
        private static readonly AccountWideProgressionUpgradeDefinition[] AllDefinitions =
        {
            CombatBaselineProject,
            PushOffenseProject,
            FarmYieldProject,
            BossSalvageProject,
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeId), upgradeId, "Unknown account-wide upgrade id.");
            }
        }
    }
}

