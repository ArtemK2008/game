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
                "Gate guardians");

        public static readonly LocationIdentityDefinition SunscorchRuins =
            new LocationIdentityDefinition(
                "location_identity_sunscorch_ruins",
                "Sunscorch Ruins",
                "Sunscorch salvage",
                "Late region-material recovery",
                "Ruin sentinels");

        public static LocationIdentityDefinition CreateFallback(RegionId regionId)
        {
            return new LocationIdentityDefinition(
                $"location_identity_{regionId.Value}",
                regionId.Value,
                "Regional stockpile",
                "Mixed regional value",
                "Mixed local threats",
                isFallbackIdentity: true);
        }
    }
}
