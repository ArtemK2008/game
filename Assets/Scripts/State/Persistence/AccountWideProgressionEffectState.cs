using System;

namespace Survivalon.Runtime
{
    public readonly struct AccountWideProgressionEffectState
    {
        public AccountWideProgressionEffectState(int playerMaxHealthBonus)
        {
            if (playerMaxHealthBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playerMaxHealthBonus), "Player max-health bonus cannot be negative.");
            }

            PlayerMaxHealthBonus = playerMaxHealthBonus;
        }

        public int PlayerMaxHealthBonus { get; }
    }
}
