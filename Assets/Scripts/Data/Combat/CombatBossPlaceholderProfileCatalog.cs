using System;
using Survivalon.Combat;

namespace Survivalon.Data.Combat
{
    public static class CombatBossPlaceholderProfileCatalog
    {
        private static readonly CombatEnemyProfile GateEnemyProfile = new CombatEnemyProfile(
            "combat_enemy_gate",
            "boss_001",
            "Gate Enemy",
            new CombatStatBlock(
                maxHealth: 180f,
                attackPower: 16f,
                attackRate: 0.85f,
                defense: 18f),
            CombatEnemyBehaviorType.GatePlaceholder);

        public static CombatEnemyProfile GateEnemy => GateEnemyProfile;

        public static CombatEnemyProfile Get(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new ArgumentException("Enemy profile id cannot be null or whitespace.", nameof(profileId));
            }

            if (GateEnemyProfile.ProfileId != profileId)
            {
                throw new ArgumentOutOfRangeException(nameof(profileId), profileId, "Unknown boss placeholder enemy profile id.");
            }

            return GateEnemyProfile;
        }
    }
}
