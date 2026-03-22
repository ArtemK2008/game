using System;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.Data.Progression
{
    /// <summary>
    /// Описывает authored definition account-wide progression upgrade без persistent mutation logic.
    /// </summary>
    public sealed class AccountWideProgressionUpgradeDefinition
    {
        public AccountWideProgressionUpgradeDefinition(
            AccountWideUpgradeId upgradeId,
            string displayName,
            string progressionId,
            ResourceCategory costResourceCategory,
            int costAmount,
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus)
            : this(
                upgradeId,
                displayName,
                progressionId,
                costResourceCategory,
                costAmount,
                playerMaxHealthBonus,
                playerAttackPowerBonus,
                ordinaryRegionMaterialRewardBonus,
                bossProgressionMaterialRewardBonus: 0)
        {
        }

        public AccountWideProgressionUpgradeDefinition(
            AccountWideUpgradeId upgradeId,
            string displayName,
            string progressionId,
            ResourceCategory costResourceCategory,
            int costAmount,
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus,
            int bossProgressionMaterialRewardBonus)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

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

            if (bossProgressionMaterialRewardBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bossProgressionMaterialRewardBonus),
                    "Boss progression-material reward bonus cannot be negative.");
            }

            UpgradeId = upgradeId;
            DisplayName = displayName;
            ProgressionId = progressionId;
            LayerType = ProgressionLayerType.AccountWide;
            CostResourceCategory = costResourceCategory;
            CostAmount = costAmount;
            PlayerMaxHealthBonus = playerMaxHealthBonus;
            PlayerAttackPowerBonus = playerAttackPowerBonus;
            OrdinaryRegionMaterialRewardBonus = ordinaryRegionMaterialRewardBonus;
            BossProgressionMaterialRewardBonus = bossProgressionMaterialRewardBonus;
        }

        public AccountWideUpgradeId UpgradeId { get; }

        public string DisplayName { get; }

        public string ProgressionId { get; }

        public ProgressionLayerType LayerType { get; }

        public ResourceCategory CostResourceCategory { get; }

        public int CostAmount { get; }

        public int PlayerMaxHealthBonus { get; }

        public int PlayerAttackPowerBonus { get; }

        public int OrdinaryRegionMaterialRewardBonus { get; }

        public int BossProgressionMaterialRewardBonus { get; }
    }
}

