using Survivalon.Core;

namespace Survivalon.Data.World
{
    public static class LocationIdentityCatalog
    {
        public static readonly LocationIdentityDefinition VerdantFrontier =
            new LocationIdentityDefinition(
                "location_identity_verdant_frontier",
                "Verdant Frontier",
                "Frontier salvage",
                "Region material farming",
                "Frontier raiders");

        public static readonly LocationIdentityDefinition EchoCaverns =
            new LocationIdentityDefinition(
                "location_identity_echo_caverns",
                "Echo Caverns",
                "Cavern relic caches",
                "Persistent progression gains",
                "Gate guardians",
                bossPersistentProgressionMaterialBonus: 1);

        public static LocationIdentityDefinition CreateFallback(RegionId regionId)
        {
            return new LocationIdentityDefinition(
                $"location_identity_{regionId.Value}",
                regionId.Value,
                "Regional stockpile",
                "Mixed regional value",
                "Mixed local threats",
                bossPersistentProgressionMaterialBonus: 0,
                isFallbackIdentity: true);
        }
    }
}
