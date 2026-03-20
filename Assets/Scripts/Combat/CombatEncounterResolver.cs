using System;

namespace Survivalon.Combat
{
    /// <summary>
    /// Оркеструет пошаговое продвижение боевого столкновения во времени.
    /// Продвигает таймеры сторон, определяет наступившие атаки, делегирует выбор цели
    /// и фиксирует исход боя, если одна из сторон погибает.
    /// </summary>
    public sealed class CombatEncounterResolver
    {
        private const float AttackTimingEpsilon = 0.0001f;
        private readonly CombatAutoTargetSelector combatAutoTargetSelector;
        private readonly ICombatSkillExecutor combatSkillExecutor;

        public CombatEncounterResolver(
            CombatAutoTargetSelector combatAutoTargetSelector = null,
            ICombatSkillExecutor combatSkillExecutor = null)
        {
            this.combatAutoTargetSelector = combatAutoTargetSelector ?? new CombatAutoTargetSelector();
            this.combatSkillExecutor = combatSkillExecutor ?? new CombatSkillExecutor();
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
                float playerTimeToBaselineAttack = encounterState.PlayerEntity.CanAct
                    ? encounterState.PlayerEntity.TimeUntilNextBaselineAttackSeconds
                    : float.PositiveInfinity;
                float playerTimeToTriggeredActiveSkill = encounterState.PlayerEntity.CanAct
                    ? encounterState.PlayerEntity.TimeUntilTriggeredActiveSkillSeconds
                    : float.PositiveInfinity;
                float enemyTimeToAttack = encounterState.EnemyEntity.CanAct
                    ? encounterState.EnemyEntity.TimeUntilNextBaselineAttackSeconds
                    : float.PositiveInfinity;
                float nextActionWindow = Math.Min(
                    Math.Min(playerTimeToBaselineAttack, playerTimeToTriggeredActiveSkill),
                    enemyTimeToAttack);

                if (float.IsPositiveInfinity(nextActionWindow))
                {
                    return true;
                }

                if (remainingSeconds + AttackTimingEpsilon < nextActionWindow)
                {
                    AdvanceEncounter(encounterState, remainingSeconds);
                    return true;
                }

                AdvanceEncounter(encounterState, nextActionWindow);
                remainingSeconds -= nextActionWindow;
                ResolveDueActions(encounterState);
            }

            return true;
        }

        private static void AdvanceEncounter(CombatEncounterState encounterState, float elapsedSeconds)
        {
            encounterState.PlayerEntity.AdvanceBaselineAttackTimer(elapsedSeconds);
            encounterState.PlayerEntity.AdvanceTriggeredActiveSkillTimer(elapsedSeconds);
            encounterState.EnemyEntity.AdvanceBaselineAttackTimer(elapsedSeconds);
            encounterState.AdvanceElapsedTime(elapsedSeconds);
        }

        private void ResolveDueActions(CombatEncounterState encounterState)
        {
            if (ResolvePlayerTriggeredActiveSkillIfDue(encounterState))
            {
                return;
            }

            if (ResolvePlayerAttackIfDue(encounterState))
            {
                return;
            }

            ResolveEnemyHostilityIfDue(encounterState);
        }

        private bool ResolvePlayerAttackIfDue(CombatEncounterState encounterState)
        {
            bool playerAttackIsDue = encounterState.PlayerEntity.CanAct &&
                encounterState.PlayerEntity.TimeUntilNextBaselineAttackSeconds <= AttackTimingEpsilon;
            if (!playerAttackIsDue)
            {
                return false;
            }

            ExecuteBaselineAttack(
                encounterState.PlayerEntity,
                combatAutoTargetSelector.SelectTarget(encounterState, encounterState.PlayerEntity.Side),
                encounterState);
            return encounterState.IsResolved;
        }

        private bool ResolvePlayerTriggeredActiveSkillIfDue(CombatEncounterState encounterState)
        {
            bool playerTriggeredActiveSkillIsDue = encounterState.PlayerEntity.CanAct &&
                encounterState.PlayerEntity.TriggeredActiveSkill != null &&
                encounterState.PlayerEntity.TimeUntilTriggeredActiveSkillSeconds <= AttackTimingEpsilon;
            if (!playerTriggeredActiveSkillIsDue)
            {
                return false;
            }

            ExecuteTriggeredActiveSkill(
                encounterState.PlayerEntity,
                combatAutoTargetSelector.SelectTarget(encounterState, encounterState.PlayerEntity.Side),
                encounterState);
            return encounterState.IsResolved;
        }

        private void ResolveEnemyHostilityIfDue(CombatEncounterState encounterState)
        {
            bool enemyAttackIsDue = encounterState.EnemyEntity.CanAct &&
                encounterState.EnemyEntity.TimeUntilNextBaselineAttackSeconds <= AttackTimingEpsilon;
            if (!enemyAttackIsDue)
            {
                return;
            }

            ExecuteBaselineAttack(
                encounterState.EnemyEntity,
                combatAutoTargetSelector.SelectTarget(encounterState, encounterState.EnemyEntity.Side),
                encounterState);
        }

        private void ExecuteBaselineAttack(
            CombatEntityRuntimeState attacker,
            CombatEntityRuntimeState defender,
            CombatEncounterState encounterState)
        {
            if (!attacker.CanAct || !defender.IsAlive)
            {
                return;
            }

            attacker.ResetBaselineAttackTimer();
            combatSkillExecutor.Execute(
                new CombatSkillExecutionRequest(attacker.BaselineAttackSkill, attacker, defender),
                encounterState);
        }

        private void ExecuteTriggeredActiveSkill(
            CombatEntityRuntimeState attacker,
            CombatEntityRuntimeState defender,
            CombatEncounterState encounterState)
        {
            if (!attacker.CanAct || attacker.TriggeredActiveSkill == null || !defender.IsAlive)
            {
                return;
            }

            attacker.ResetTriggeredActiveSkillTimer();
            combatSkillExecutor.Execute(
                new CombatSkillExecutionRequest(attacker.TriggeredActiveSkill, attacker, defender),
                encounterState);
        }
    }
}

