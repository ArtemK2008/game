using System;

namespace Survivalon.Data.Combat
{
    public sealed class CombatEncounterDefinition
    {
        public CombatEncounterDefinition(string encounterId, CombatEnemyProfile enemyProfile)
        {
            if (string.IsNullOrWhiteSpace(encounterId))
            {
                throw new ArgumentException("Combat encounter id cannot be null or whitespace.", nameof(encounterId));
            }

            EnemyProfile = enemyProfile ?? throw new ArgumentNullException(nameof(enemyProfile));
            EncounterId = encounterId;
        }

        public string EncounterId { get; }

        public CombatEnemyProfile EnemyProfile { get; }
    }
}
