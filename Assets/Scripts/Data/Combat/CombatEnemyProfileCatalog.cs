using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Data.Combat
{
    public static class CombatEnemyProfileCatalog
    {
        private static readonly CombatEnemyProfile EnemyUnitProfile = new CombatEnemyProfile(
            "combat_enemy_standard_unit",
            "enemy_001",
            "Enemy Unit",
            new CombatStatBlock(
                maxHealth: 75f,
                attackPower: 8f,
                attackRate: 0.9f,
                defense: 4f));
        private static readonly CombatEnemyProfile BulwarkRaiderProfile = new CombatEnemyProfile(
            "combat_enemy_bulwark_raider",
            "enemy_002",
            "Bulwark Raider",
            new CombatStatBlock(
                maxHealth: 105f,
                attackPower: 8f,
                attackRate: 0.9f,
                defense: 4f));
        private static readonly CombatEnemyProfile GateEnemyProfile = new CombatEnemyProfile(
            "combat_enemy_gate",
            "boss_001",
            "Gate Enemy",
            new CombatStatBlock(
                maxHealth: 180f,
                attackPower: 16f,
                attackRate: 0.85f,
                defense: 18f));
        private static readonly IReadOnlyList<CombatEnemyProfile> AllProfiles = Array.AsReadOnly(new[]
        {
            EnemyUnitProfile,
            BulwarkRaiderProfile,
            GateEnemyProfile,
        });

        public static CombatEnemyProfile EnemyUnit => EnemyUnitProfile;

        public static CombatEnemyProfile BulwarkRaider => BulwarkRaiderProfile;

        public static CombatEnemyProfile GateEnemy => GateEnemyProfile;

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

            throw new ArgumentOutOfRangeException(nameof(profileId), profileId, "Unknown enemy profile id.");
        }
    }
}
