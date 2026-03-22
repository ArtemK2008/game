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
                bossProgressionMaterialRewardBonus: 0)
        {
        }

        public AccountWideProgressionEffectState(
            int playerMaxHealthBonus,
            int playerAttackPowerBonus,
            int ordinaryRegionMaterialRewardBonus,
            int bossProgressionMaterialRewardBonus)
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

            PlayerMaxHealthBonus = playerMaxHealthBonus;
            PlayerAttackPowerBonus = playerAttackPowerBonus;
            OrdinaryRegionMaterialRewardBonus = ordinaryRegionMaterialRewardBonus;
            BossProgressionMaterialRewardBonus = bossProgressionMaterialRewardBonus;
        }

        public int PlayerMaxHealthBonus { get; }

        public int PlayerAttackPowerBonus { get; }

        public int OrdinaryRegionMaterialRewardBonus { get; }

        public int BossProgressionMaterialRewardBonus { get; }
    }
}

