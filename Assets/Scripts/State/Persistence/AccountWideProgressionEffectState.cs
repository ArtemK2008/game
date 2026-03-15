using System;

namespace Survivalon.Runtime
{
    public readonly struct AccountWideProgressionEffectState
    {
        public AccountWideProgressionEffectState(
            int playerMaxHealthBonus,
            int playerAttackPowerBonus)
        {
            if (playerMaxHealthBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playerMaxHealthBonus), "Player max-health bonus cannot be negative.");
            }

            if (playerAttackPowerBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playerAttackPowerBonus), "Player attack-power bonus cannot be negative.");
            }

            PlayerMaxHealthBonus = playerMaxHealthBonus;
            PlayerAttackPowerBonus = playerAttackPowerBonus;
        }

        public int PlayerMaxHealthBonus { get; }

        public int PlayerAttackPowerBonus { get; }
    }
}
