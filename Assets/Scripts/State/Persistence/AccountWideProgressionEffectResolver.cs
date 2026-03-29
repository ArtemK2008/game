using System;
using Survivalon.Data.Progression;

namespace Survivalon.State.Persistence
{
    /// <summary>
    /// Преобразует купленные persistent progression entries в применяемые account-wide runtime effects.
    /// </summary>
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
            int bossProgressionMaterialRewardBonus = 0;
            int regionMaterialRefinementOutputBonus = 0;
            bool enablesFarmReadyQuickReplayShortcut = false;

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
                bossProgressionMaterialRewardBonus +=
                    upgradeDefinition.BossProgressionMaterialRewardBonus * entry.CurrentValue;
                regionMaterialRefinementOutputBonus +=
                    upgradeDefinition.RegionMaterialRefinementOutputBonus * entry.CurrentValue;
                enablesFarmReadyQuickReplayShortcut |= upgradeDefinition.EnablesFarmReadyQuickReplayShortcut;
            }

            return new AccountWideProgressionEffectState(
                playerMaxHealthBonus,
                playerAttackPowerBonus,
                ordinaryRegionMaterialRewardBonus,
                bossProgressionMaterialRewardBonus,
                regionMaterialRefinementOutputBonus,
                enablesFarmReadyQuickReplayShortcut);
        }
    }
}

