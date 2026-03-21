using System;

namespace Survivalon.Data.Gear
{
    public sealed class GearProfile
    {
        public GearProfile(
            string gearId,
            string displayName,
            GearCategory gearCategory,
            float attackPowerBonus = 0f)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            if (attackPowerBonus < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(attackPowerBonus),
                    attackPowerBonus,
                    "Attack power bonus cannot be negative.");
            }

            GearId = gearId;
            DisplayName = displayName;
            GearCategory = gearCategory;
            AttackPowerBonus = attackPowerBonus;
        }

        public string GearId { get; }

        public string DisplayName { get; }

        public GearCategory GearCategory { get; }

        public float AttackPowerBonus { get; }
    }
}
