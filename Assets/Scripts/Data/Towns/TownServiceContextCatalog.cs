using System;

namespace Survivalon.Data.Towns
{
    public static class TownServiceContextCatalog
    {
        private static readonly TownServiceContextDefinition CavernServiceHubContext =
            new TownServiceContextDefinition(
                "town_service_cavern_hub",
                "Cavern Service Hub",
                hasProgressionHubAccess: true,
                hasBuildPreparationAccess: true);

        public static TownServiceContextDefinition CavernServiceHub => CavernServiceHubContext;

        public static TownServiceContextDefinition Get(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId))
            {
                throw new ArgumentException("Town service context id cannot be null or whitespace.", nameof(contextId));
            }

            return contextId switch
            {
                "town_service_cavern_hub" => CavernServiceHubContext,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(contextId),
                    contextId,
                    "Unknown town service context id."),
            };
        }
    }
}
