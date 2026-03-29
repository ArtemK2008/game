using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Data.Combat
{
    public static class CombatStandardEnemyProfileCatalog
    {
        private static readonly CombatEnemyProfile EnemyUnitProfile = new CombatEnemyProfile(
            "combat_enemy_standard_unit",
            "enemy_001",
            "Enemy Unit",
            new CombatStatBlock(
                maxHealth: 75f,
                attackPower: 7f,
                attackRate: 1.25f,
                defense: 2f),
            CombatEnemyBehaviorType.FastPressure,
            CombatHostileEntityType.StandardEnemy);
        private static readonly CombatEnemyProfile BulwarkRaiderProfile = new CombatEnemyProfile(
            "combat_enemy_bulwark_raider",
            "enemy_002",
            "Bulwark Raider",
            new CombatStatBlock(
                maxHealth: 105f,
                attackPower: 9f,
                attackRate: 0.85f,
                defense: 6f),
            CombatEnemyBehaviorType.BulwarkPressure,
            CombatHostileEntityType.StandardEnemy);
        private static readonly CombatEnemyProfile RuinSentinelProfile = new CombatEnemyProfile(
            "combat_enemy_ruin_sentinel",
            "enemy_003",
            "Ruin Sentinel",
            new CombatStatBlock(
                maxHealth: 95f,
                attackPower: 11f,
                attackRate: 0.95f,
                defense: 8f),
            CombatEnemyBehaviorType.SentinelPressure,
            CombatHostileEntityType.StandardEnemy);
        private static readonly IReadOnlyList<CombatEnemyProfile> AllProfiles = Array.AsReadOnly(new[]
        {
            EnemyUnitProfile,
            BulwarkRaiderProfile,
            RuinSentinelProfile,
        });

        public static CombatEnemyProfile EnemyUnit => EnemyUnitProfile;

        public static CombatEnemyProfile BulwarkRaider => BulwarkRaiderProfile;

        public static CombatEnemyProfile RuinSentinel => RuinSentinelProfile;

        public static IReadOnlyList<CombatEnemyProfile> All => AllProfiles;

        public static CombatEnemyProfile Get(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new ArgumentException("Enemy profile id cannot be null or whitespace.", nameof(profileId));
            }

            for (int index = 0; index < AllProfiles.Count; index++)
            {
                if (AllProfiles[index].ProfileId == profileId)
                {
                    return AllProfiles[index];
                }
            }

            throw new ArgumentOutOfRangeException(nameof(profileId), profileId, "Unknown standard enemy profile id.");
        }
    }
}
