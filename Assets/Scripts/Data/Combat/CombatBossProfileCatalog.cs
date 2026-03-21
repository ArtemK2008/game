using System;
using Survivalon.Combat;

namespace Survivalon.Data.Combat
{
    public static class CombatBossProfileCatalog
    {
        private static readonly CombatEnemyProfile GateBossProfile = new CombatEnemyProfile(
            "combat_boss_gate",
            "boss_001",
            "Gate Boss",
            new CombatStatBlock(
                maxHealth: 180f,
                attackPower: 16f,
                attackRate: 0.85f,
                defense: 18f),
            CombatEnemyBehaviorType.GateBoss,
            CombatHostileEntityType.Boss);

        public static CombatEnemyProfile GateBoss => GateBossProfile;

        public static CombatEnemyProfile Get(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new ArgumentException("Boss profile id cannot be null or whitespace.", nameof(profileId));
            }

            if (GateBossProfile.ProfileId != profileId)
            {
                throw new ArgumentOutOfRangeException(nameof(profileId), profileId, "Unknown boss profile id.");
            }

            return GateBossProfile;
        }
    }
}
