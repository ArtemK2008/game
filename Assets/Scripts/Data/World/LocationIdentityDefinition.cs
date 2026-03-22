using System;

namespace Survivalon.Data.World
{
    public sealed class LocationIdentityDefinition
    {
        public LocationIdentityDefinition(
            string locationIdentityId,
            string displayName,
            string rewardSourceDisplayName,
            string rewardFocusDisplayName,
            string enemyEmphasisDisplayName,
            bool isFallbackIdentity = false)
        {
            if (string.IsNullOrWhiteSpace(locationIdentityId))
            {
                throw new ArgumentException("Location identity id cannot be null or whitespace.", nameof(locationIdentityId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Location display name cannot be null or whitespace.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(rewardSourceDisplayName))
            {
                throw new ArgumentException(
                    "Location reward source display name cannot be null or whitespace.",
                    nameof(rewardSourceDisplayName));
            }

            if (string.IsNullOrWhiteSpace(rewardFocusDisplayName))
            {
                throw new ArgumentException(
                    "Location reward focus display name cannot be null or whitespace.",
                    nameof(rewardFocusDisplayName));
            }

            if (string.IsNullOrWhiteSpace(enemyEmphasisDisplayName))
            {
                throw new ArgumentException(
                    "Location enemy emphasis display name cannot be null or whitespace.",
                    nameof(enemyEmphasisDisplayName));
            }

            LocationIdentityId = locationIdentityId;
            DisplayName = displayName;
            RewardSourceDisplayName = rewardSourceDisplayName;
            RewardFocusDisplayName = rewardFocusDisplayName;
            EnemyEmphasisDisplayName = enemyEmphasisDisplayName;
            IsFallbackIdentity = isFallbackIdentity;
        }

        public string LocationIdentityId { get; }

        public string DisplayName { get; }

        public string RewardSourceDisplayName { get; }

        public string RewardFocusDisplayName { get; }

        public string EnemyEmphasisDisplayName { get; }

        public bool IsFallbackIdentity { get; }
    }
}
