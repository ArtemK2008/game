using System;
using Survivalon.Data.Gear;

namespace Survivalon.World
{
    public sealed class BossRewardContentDefinition
    {
        public BossRewardContentDefinition(
            int persistentProgressionMaterialBonus,
            string gearRewardId = null)
        {
            if (persistentProgressionMaterialBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(persistentProgressionMaterialBonus),
                    "Boss progression material bonus cannot be negative.");
            }

            if (!string.IsNullOrWhiteSpace(gearRewardId) && !GearCatalog.Contains(gearRewardId))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(gearRewardId),
                    gearRewardId,
                    "Boss gear reward id must be a known shipped gear id when provided.");
            }

            PersistentProgressionMaterialBonus = persistentProgressionMaterialBonus;
            GearRewardId = gearRewardId;
        }

        public int PersistentProgressionMaterialBonus { get; }

        public string GearRewardId { get; }
    }
}
