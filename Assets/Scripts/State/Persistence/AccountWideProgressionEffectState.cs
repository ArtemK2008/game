using System;

namespace Survivalon.State.Persistence
{
    public readonly struct AccountWideProgressionEffectState
    {
        public AccountWideProgressionEffectState(
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus)
            : this(
                playerMaxHealthBonus,
                playerAttackPowerBonus,
                ordinaryRegionMaterialRewardBonus,
                bossProgressionMaterialRewardBonus: 0,
                regionMaterialRefinementOutputBonus: 0,
                enablesFarmReadyQuickReplayShortcut: false)
        {
        }

        public AccountWideProgressionEffectState(
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus,
            int bossProgressionMaterialRewardBonus)
            : this(
                playerMaxHealthBonus,
                playerAttackPowerBonus,
                ordinaryRegionMaterialRewardBonus,
                bossProgressionMaterialRewardBonus,
                regionMaterialRefinementOutputBonus: 0,
                enablesFarmReadyQuickReplayShortcut: false)
        {
        }

        public AccountWideProgressionEffectState(
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus,
            int bossProgressionMaterialRewardBonus,
            bool enablesFarmReadyQuickReplayShortcut)
            : this(
                playerMaxHealthBonus,
                playerAttackPowerBonus,
                ordinaryRegionMaterialRewardBonus,
                bossProgressionMaterialRewardBonus,
                regionMaterialRefinementOutputBonus: 0,
                enablesFarmReadyQuickReplayShortcut)
        {
        }

        public AccountWideProgressionEffectState(
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus,
            int bossProgressionMaterialRewardBonus,
            int regionMaterialRefinementOutputBonus,
            bool enablesFarmReadyQuickReplayShortcut)
        {
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

            if (regionMaterialRefinementOutputBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(regionMaterialRefinementOutputBonus),
                    "Region-material refinement output bonus cannot be negative.");
            }

            PlayerMaxHealthBonus = playerMaxHealthBonus;
            PlayerAttackPowerBonus = playerAttackPowerBonus;
            OrdinaryRegionMaterialRewardBonus = ordinaryRegionMaterialRewardBonus;
            BossProgressionMaterialRewardBonus = bossProgressionMaterialRewardBonus;
            RegionMaterialRefinementOutputBonus = regionMaterialRefinementOutputBonus;
            EnablesFarmReadyQuickReplayShortcut = enablesFarmReadyQuickReplayShortcut;
        }

        public int PlayerMaxHealthBonus { get; }

        public int PlayerAttackPowerBonus { get; }

        public int OrdinaryRegionMaterialRewardBonus { get; }

        public int BossProgressionMaterialRewardBonus { get; }

        public int RegionMaterialRefinementOutputBonus { get; }

        public bool EnablesFarmReadyQuickReplayShortcut { get; }
    }
}

