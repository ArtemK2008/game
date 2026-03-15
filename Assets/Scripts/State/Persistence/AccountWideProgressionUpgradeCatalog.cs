using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public static class AccountWideProgressionUpgradeCatalog
    {
        private static readonly AccountWideProgressionUpgradeDefinition CombatBaselineProject =
            new AccountWideProgressionUpgradeDefinition(
                AccountWideUpgradeId.CombatBaselineProject,
                "account_wide_combat_baseline_project",
                ResourceCategory.PersistentProgressionMaterial,
                costAmount: 1,
                playerMaxHealthBonus: 10);
        private static readonly AccountWideProgressionUpgradeDefinition[] AllDefinitions =
        {
            CombatBaselineProject,
        };

        public static IReadOnlyList<AccountWideProgressionUpgradeDefinition> All => AllDefinitions;

        public static AccountWideProgressionUpgradeDefinition Get(AccountWideUpgradeId upgradeId)
        {
            switch (upgradeId)
            {
                case AccountWideUpgradeId.CombatBaselineProject:
                    return CombatBaselineProject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeId), upgradeId, "Unknown account-wide upgrade id.");
            }
        }
    }
}
