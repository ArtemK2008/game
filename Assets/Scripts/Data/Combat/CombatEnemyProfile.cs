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
            CombatStatBlock baseStats,
            CombatEnemyBehaviorType behaviorType,
            CombatHostileEntityType hostileEntityType)
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
            BehaviorType = behaviorType;
            HostileEntityType = hostileEntityType;
        }

        public string ProfileId { get; }

        public string EntityIdSuffix { get; }

        public string DisplayName { get; }

        public CombatStatBlock BaseStats { get; }

        public CombatEnemyBehaviorType BehaviorType { get; }

        public CombatHostileEntityType HostileEntityType { get; }
    }
}
