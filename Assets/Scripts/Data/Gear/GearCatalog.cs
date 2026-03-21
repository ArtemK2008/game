using System;
using System.Collections.Generic;

namespace Survivalon.Data.Gear
{
    public static class GearCatalog
    {
        private static readonly GearProfile TrainingBladeProfile = new GearProfile(
            GearIds.TrainingBlade,
            "Training Blade",
            GearCategory.PrimaryCombat);
        private static readonly IReadOnlyList<GearProfile> AllProfiles = Array.AsReadOnly(new[]
        {
            TrainingBladeProfile,
        });

        public static GearProfile TrainingBlade => TrainingBladeProfile;

        public static IReadOnlyList<GearProfile> All => AllProfiles;

        public static bool Contains(string gearId)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                return false;
            }

            for (int index = 0; index < AllProfiles.Count; index++)
            {
                if (AllProfiles[index].GearId == gearId)
                {
                    return true;
                }
            }

            return false;
        }

        public static GearProfile Get(string gearId)
        {
            if (string.IsNullOrWhiteSpace(gearId))
            {
                throw new ArgumentException("Gear id cannot be null or whitespace.", nameof(gearId));
            }

            for (int index = 0; index < AllProfiles.Count; index++)
            {
                if (AllProfiles[index].GearId == gearId)
                {
                    return AllProfiles[index];
                }
            }

            throw new ArgumentOutOfRangeException(nameof(gearId), gearId, "Unknown gear id.");
        }
    }
}
