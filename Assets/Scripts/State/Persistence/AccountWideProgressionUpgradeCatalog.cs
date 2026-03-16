using System;
using System.Collections.Generic;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.State.Persistence
{
    public static class AccountWideProgressionUpgradeCatalog
    {
        private static readonly AccountWideProgressionUpgradeDefinition CombatBaselineProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.CombatBaselineProject,
                "account_wide_combat_baseline_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 1,
                playerMaxHealthBonus: 10,
                playerAttackPowerBonus: 0,
                ordinaryRegionMaterialRewardBonus: 0);
        private static readonly AccountWideProgressionUpgradeDefinition PushOffenseProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.PushOffenseProject,
                "account_wide_push_offense_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 2,
                playerMaxHealthBonus: 0,
                playerAttackPowerBonus: 4,
                ordinaryRegionMaterialRewardBonus: 0);
        private static readonly AccountWideProgressionUpgradeDefinition FarmYieldProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.FarmYieldProject,
                "account_wide_farm_yield_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 1,
                playerMaxHealthBonus: 0,
                playerAttackPowerBonus: 0,
                ordinaryRegionMaterialRewardBonus: 1);
        private static readonly AccountWideProgressionUpgradeDefinition[] AllDefinitions =
        {
            CombatBaselineProject,
            PushOffenseProject,
            FarmYieldProject,
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeId), upgradeId, "Unknown account-wide upgrade id.");
            }
        }
    }
}
