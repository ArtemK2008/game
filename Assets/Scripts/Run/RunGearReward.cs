using System;
using Survivalon.Data.Gear;

namespace Survivalon.Run
{
    public sealed class RunGearReward
    {
        public RunGearReward(string gearId)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            GearProfile gearProfile = GearCatalog.Get(gearId);
            GearId = gearProfile.GearId;
            DisplayName = gearProfile.DisplayName;
            GearCategory = gearProfile.GearCategory;
        }

        public string GearId { get; }

        public string DisplayName { get; }

        public GearCategory GearCategory { get; }
    }
}
