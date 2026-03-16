using System;

namespace Survivalon.Combat
{
    public sealed class CombatEntityState
    {
        public CombatEntityState(
            CombatEntityId entityId,
            string displayName,
            CombatSide side,
            CombatStatBlock baseStats,
            bool isAlive = true,
            bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be null or whitespace.", nameof(displayName));
            }

            EntityId = entityId;
            DisplayName = displayName;
            Side = side;
            BaseStats = baseStats;
            IsAlive = isAlive;
            IsActive = isActive;
        }

        public CombatEntityId EntityId { get; }

        public string DisplayName { get; }

        public CombatSide Side { get; }

        public CombatStatBlock BaseStats { get; }

        public bool IsAlive { get; }

        public bool IsActive { get; }
    }
}

