using System;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Runtime.Run;

namespace Survivalon.Runtime.Combat
{
    public sealed class CombatEncounterResolver
    {
        private const float AttackTimingEpsilon = 0.0001f;
        private readonly CombatAutoTargetSelector combatAutoTargetSelector;

        public CombatEncounterResolver(CombatAutoTargetSelector combatAutoTargetSelector = null)
        {
            this.combatAutoTargetSelector = combatAutoTargetSelector ?? new CombatAutoTargetSelector();
        }

        public bool TryAdvance(CombatEncounterState encounterState, float elapsedSeconds)
        {
            if (encounterState == null)
            {
                throw new ArgumentNullException(nameof(encounterState));
            }

            if (elapsedSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), elapsedSeconds, "Elapsed time must be greater than zero.");
            }

            if (encounterState.IsResolved)
            {
                return false;
            }

            float remainingSeconds = elapsedSeconds;
            while (remainingSeconds > AttackTimingEpsilon && !encounterState.IsResolved)
            {
                float playerTimeToAttack = encounterState.PlayerEntity.CanAct
                    ? encounterState.PlayerEntity.TimeUntilNextAttackSeconds
                    : float.PositiveInfinity;
                float enemyTimeToAttack = encounterState.EnemyEntity.CanAct
                    ? encounterState.EnemyEntity.TimeUntilNextAttackSeconds
                    : float.PositiveInfinity;
                float nextAttackWindow = Math.Min(playerTimeToAttack, enemyTimeToAttack);

                if (float.IsPositiveInfinity(nextAttackWindow))
                {
                    return true;
                }

                if (remainingSeconds + AttackTimingEpsilon < nextAttackWindow)
                {
                    AdvanceEncounter(encounterState, remainingSeconds);
                    return true;
                }

                AdvanceEncounter(encounterState, nextAttackWindow);
                remainingSeconds -= nextAttackWindow;
                ResolveDueAttacks(encounterState);
            }

            return true;
        }

        private static void AdvanceEncounter(CombatEncounterState encounterState, float elapsedSeconds)
        {
            encounterState.PlayerEntity.AdvanceAttackTimer(elapsedSeconds);
            encounterState.EnemyEntity.AdvanceAttackTimer(elapsedSeconds);
            encounterState.AdvanceElapsedTime(elapsedSeconds);
        }

        private void ResolveDueAttacks(CombatEncounterState encounterState)
        {
            if (ResolvePlayerAttackIfDue(encounterState))
            {
                return;
            }

            ResolveEnemyHostilityIfDue(encounterState);
        }

        private bool ResolvePlayerAttackIfDue(CombatEncounterState encounterState)
        {
            bool playerAttackIsDue = encounterState.PlayerEntity.CanAct &&
                encounterState.PlayerEntity.TimeUntilNextAttackSeconds <= AttackTimingEpsilon;
            if (!playerAttackIsDue)
            {
                return false;
            }

            ResolveAttack(
                encounterState.PlayerEntity,
                combatAutoTargetSelector.SelectTarget(encounterState, encounterState.PlayerEntity.Side),
                encounterState);
            return encounterState.IsResolved;
        }

        private void ResolveEnemyHostilityIfDue(CombatEncounterState encounterState)
        {
            bool enemyAttackIsDue = encounterState.EnemyEntity.CanAct &&
                encounterState.EnemyEntity.TimeUntilNextAttackSeconds <= AttackTimingEpsilon;
            if (!enemyAttackIsDue)
            {
                return;
            }

            ResolveAttack(
                encounterState.EnemyEntity,
                combatAutoTargetSelector.SelectTarget(encounterState, encounterState.EnemyEntity.Side),
                encounterState);
        }

        private static void ResolveAttack(
            CombatEntityRuntimeState attacker,
            CombatEntityRuntimeState defender,
            CombatEncounterState encounterState)
        {
            if (defender == null)
            {
                throw new ArgumentNullException(nameof(defender));
            }

            if (!attacker.CanAct || !defender.IsAlive)
            {
                return;
            }

            float mitigatedDamage = CombatStatCalculator.CalculateMitigatedDamage(
                attacker.CombatEntity.BaseStats.AttackPower,
                defender.CombatEntity.BaseStats);

            attacker.ResetAttackTimer();
            defender.ApplyDamage(mitigatedDamage);

            if (!defender.IsAlive)
            {
                encounterState.Resolve(
                    attacker.Side == CombatSide.Player
                        ? CombatEncounterOutcome.PlayerVictory
                        : CombatEncounterOutcome.EnemyVictory);
            }
        }
    }
}
