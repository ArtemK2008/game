using System;

namespace Survivalon.Data.Gear
{
    public sealed class GearProfile
    {
        public GearProfile(string gearId, string displayName, GearCategory gearCategory)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            GearId = gearId;
            DisplayName = displayName;
            GearCategory = gearCategory;
        }

        public string GearId { get; }

        public string DisplayName { get; }

        public GearCategory GearCategory { get; }
    }
}
