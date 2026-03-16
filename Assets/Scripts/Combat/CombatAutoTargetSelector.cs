using System;

namespace Survivalon.Combat
{
    public sealed class CombatAutoTargetSelector
    {
        public CombatEntityRuntimeState SelectTarget(
            CombatEncounterState encounterState,
            CombatSide attackerSide)
        {
            if (encounterState == null)
            {
                throw new ArgumentNullException(nameof(encounterState));
            }

            CombatEntityRuntimeState targetCandidate = attackerSide switch
            {
                CombatSide.Player => encounterState.EnemyEntity,
                CombatSide.Enemy => encounterState.PlayerEntity,
                _ => throw new ArgumentOutOfRangeException(nameof(attackerSide), attackerSide, "Unknown combat side."),
            };

            if (targetCandidate.CanAct)
            {
                return targetCandidate;
            }

            throw new InvalidOperationException(
                $"No active target is available for attacker side '{attackerSide}'.");
        }
    }
}

