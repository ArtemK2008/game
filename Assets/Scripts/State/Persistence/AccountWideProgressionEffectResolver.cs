using System;

namespace Survivalon.Runtime
{
    public sealed class AccountWideProgressionEffectResolver
    {
        public AccountWideProgressionEffectState Resolve(PersistentProgressionState progressionState)
        {
            if (progressionState == null)
            {
                throw new ArgumentNullException(nameof(progressionState));
            }

            int playerMaxHealthBonus = 0;
            int playerAttackPowerBonus = 0;
            int ordinaryRegionMaterialRewardBonus = 0;

            foreach (AccountWideProgressionUpgradeDefinition upgradeDefinition in AccountWideProgressionUpgradeCatalog.All)
            {
                if (!progressionState.TryGetEntry(upgradeDefinition.ProgressionId, out ProgressionEntryState entry))
                {
                    continue;
                }

                if (!entry.IsUnlocked || entry.CurrentValue <= 0)
                {
                    continue;
                }

                playerMaxHealthBonus += upgradeDefinition.PlayerMaxHealthBonus * entry.CurrentValue;
                playerAttackPowerBonus += upgradeDefinition.PlayerAttackPowerBonus * entry.CurrentValue;
                ordinaryRegionMaterialRewardBonus +=
                    upgradeDefinition.OrdinaryRegionMaterialRewardBonus * entry.CurrentValue;
            }

            return new AccountWideProgressionEffectState(
                playerMaxHealthBonus,
                playerAttackPowerBonus,
                ordinaryRegionMaterialRewardBonus);
        }
    }
}
