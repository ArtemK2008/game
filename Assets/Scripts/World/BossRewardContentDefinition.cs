using System;

namespace Survivalon.World
{
    public sealed class BossRewardContentDefinition
    {
        public BossRewardContentDefinition(int persistentProgressionMaterialBonus)
        {
            if (persistentProgressionMaterialBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(persistentProgressionMaterialBonus),
                    "Boss progression material bonus cannot be negative.");
            }

            PersistentProgressionMaterialBonus = persistentProgressionMaterialBonus;
        }

        public int PersistentProgressionMaterialBonus { get; }
    }
}
