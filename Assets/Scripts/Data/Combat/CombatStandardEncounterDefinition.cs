using System;

namespace Survivalon.Data.Combat
{
    public sealed class CombatStandardEncounterDefinition : CombatEncounterDefinition
    {
        public CombatStandardEncounterDefinition(string encounterId, CombatEnemyProfile enemyProfile)
            : base(encounterId, CombatEncounterType.StandardEnemy)
        {
            EnemyProfile = enemyProfile ?? throw new ArgumentNullException(nameof(enemyProfile));
        }

        public CombatEnemyProfile EnemyProfile { get; }

        public override CombatEnemyProfile PrimaryEnemyProfile => EnemyProfile;
    }
}
