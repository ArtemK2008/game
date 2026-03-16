using System;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.State.Persistence
{
    public sealed class AccountWideProgressionUpgradeDefinition
    {
        public AccountWideProgressionUpgradeDefinition(
            AccountWideUpgradeId upgradeId,
            string progressionId,
            ResourceCategory costResourceCategory,
            int costAmount,
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus)
        {
            if (string.IsNullOrWhiteSpace(progressionId))
            {
                throw new ArgumentException("Progression id cannot be null or whitespace.", nameof(progressionId));
            }

            if (costAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(costAmount), "Upgrade cost cannot be negative.");
            }

            if (playerMaxHealthBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playerMaxHealthBonus), "Player max-health bonus cannot be negative.");
            }

            if (playerAttackPowerBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playerAttackPowerBonus), "Player attack-power bonus cannot be negative.");
            }

            if (ordinaryRegionMaterialRewardBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ordinaryRegionMaterialRewardBonus),
                    "Ordinary region-material reward bonus cannot be negative.");
            }

            UpgradeId = upgradeId;
            ProgressionId = progressionId;
            LayerType = ProgressionLayerType.AccountWide;
            CostResourceCategory = costResourceCategory;
            CostAmount = costAmount;
            PlayerMaxHealthBonus = playerMaxHealthBonus;
            PlayerAttackPowerBonus = playerAttackPowerBonus;
            OrdinaryRegionMaterialRewardBonus = ordinaryRegionMaterialRewardBonus;
        }

        public AccountWideUpgradeId UpgradeId { get; }

        public string ProgressionId { get; }

        public ProgressionLayerType LayerType { get; }

        public ResourceCategory CostResourceCategory { get; }

        public int CostAmount { get; }

        public int PlayerMaxHealthBonus { get; }

        public int PlayerAttackPowerBonus { get; }

        public int OrdinaryRegionMaterialRewardBonus { get; }
    }
}
