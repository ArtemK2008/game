using System;

namespace Survivalon.Data.Combat
{
    public abstract class CombatEncounterDefinition
    {
        protected CombatEncounterDefinition(string encounterId, CombatEncounterType encounterType)
        {
            if (string.IsNullOrWhiteSpace(encounterId))
            {
                throw new ArgumentException("Combat encounter id cannot be null or whitespace.", nameof(encounterId));
            }

            EncounterId = encounterId;
            EncounterType = encounterType;
        }

        public string EncounterId { get; }

        public CombatEncounterType EncounterType { get; }

        public abstract CombatEnemyProfile PrimaryEnemyProfile { get; }
    }
}
