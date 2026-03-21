using System;
using Survivalon.Combat;

namespace Survivalon.Data.Combat
{
    public sealed class CombatEnemyProfile
    {
        public CombatEnemyProfile(
            string profileId,
            string entityIdSuffix,
            string displayName,
            CombatStatBlock baseStats)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new ArgumentException("Enemy profile id cannot be null or whitespace.", nameof(profileId));
            }

            if (string.IsNullOrWhiteSpace(entityIdSuffix))
            {
                throw new ArgumentException("Enemy entity id suffix cannot be null or whitespace.", nameof(entityIdSuffix));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Enemy display name cannot be null or whitespace.", nameof(displayName));
            }

            ProfileId = profileId;
            EntityIdSuffix = entityIdSuffix;
            DisplayName = displayName;
            BaseStats = baseStats;
        }

        public string ProfileId { get; }

        public string EntityIdSuffix { get; }

        public string DisplayName { get; }

        public CombatStatBlock BaseStats { get; }
    }
}
