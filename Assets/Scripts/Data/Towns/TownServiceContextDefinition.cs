using System;

namespace Survivalon.Data.Towns
{
    public sealed class TownServiceContextDefinition
    {
        public TownServiceContextDefinition(
            string contextId,
            string displayName,
            bool hasProgressionHubAccess,
            bool hasBuildPreparationAccess)
        {
            if (string.IsNullOrWhiteSpace(contextId))
            {
                throw new ArgumentException("Town service context id cannot be null or whitespace.", nameof(contextId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Town service display name cannot be null or whitespace.", nameof(displayName));
            }

            if (!hasProgressionHubAccess && !hasBuildPreparationAccess)
            {
                throw new ArgumentException(
                    "Town service context must expose at least one hub function.",
                    nameof(hasProgressionHubAccess));
            }

            ContextId = contextId;
            DisplayName = displayName;
            HasProgressionHubAccess = hasProgressionHubAccess;
            HasBuildPreparationAccess = hasBuildPreparationAccess;
        }

        public string ContextId { get; }

        public string DisplayName { get; }

        public bool HasProgressionHubAccess { get; }

        public bool HasBuildPreparationAccess { get; }
    }
}
