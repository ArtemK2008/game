using System;

namespace Survivalon.Combat
{
    public sealed class CombatShellVisualStateResolver
    {
        private const float HealthEpsilon = 0.001f;
        private const float TimerResetEpsilon = 0.0001f;

        public CombatShellVisualState Resolve(
            CombatFeedbackSnapshot previousSnapshot,
            CombatFeedbackSnapshot currentSnapshot)
        {
            if (!previousSnapshot.HasEncounter || !currentSnapshot.HasEncounter)
            {
                return CombatShellVisualState.Idle;
            }

            bool didEnemyTakeDamage = currentSnapshot.EnemyCurrentHealth + HealthEpsilon <
                previousSnapshot.EnemyCurrentHealth;
            bool didPlayerTakeDamage = currentSnapshot.PlayerCurrentHealth + HealthEpsilon <
                previousSnapshot.PlayerCurrentHealth;
            bool didPlayerAttack = DidPlayerBurstStrike(previousSnapshot, currentSnapshot) ||
                DidTimerReset(
                    previousSnapshot.PlayerBaselineAttackTimerSeconds,
                    currentSnapshot.PlayerBaselineAttackTimerSeconds);
            bool didEnemyAttack = DidTimerReset(
                previousSnapshot.EnemyBaselineAttackTimerSeconds,
                currentSnapshot.EnemyBaselineAttackTimerSeconds);

            CombatEntityVisualStateId playerVisualState = ResolvePlayerState(
                currentSnapshot,
                didPlayerAttack,
                didPlayerTakeDamage);
            CombatEntityVisualStateId enemyVisualState = ResolveEnemyState(
                currentSnapshot,
                didEnemyAttack,
                didEnemyTakeDamage);

            return new CombatShellVisualState(playerVisualState, enemyVisualState);
        }

        private static CombatEntityVisualStateId ResolvePlayerState(
            CombatFeedbackSnapshot currentSnapshot,
            bool didPlayerAttack,
            bool didPlayerTakeDamage)
        {
            if (!currentSnapshot.PlayerIsAlive)
            {
                return CombatEntityVisualStateId.Defeat;
            }

            if (didPlayerAttack)
            {
                return CombatEntityVisualStateId.Attack;
            }

            if (didPlayerTakeDamage)
            {
                return CombatEntityVisualStateId.Hit;
            }

            return CombatEntityVisualStateId.Idle;
        }

        private static CombatEntityVisualStateId ResolveEnemyState(
            CombatFeedbackSnapshot currentSnapshot,
            bool didEnemyAttack,
            bool didEnemyTakeDamage)
        {
            if (!currentSnapshot.EnemyIsAlive)
            {
                return CombatEntityVisualStateId.Defeat;
            }

            if (didEnemyAttack)
            {
                return CombatEntityVisualStateId.Attack;
            }

            if (didEnemyTakeDamage)
            {
                return CombatEntityVisualStateId.Hit;
            }

            return CombatEntityVisualStateId.Idle;
        }

        private static bool DidPlayerBurstStrike(
            CombatFeedbackSnapshot previousSnapshot,
            CombatFeedbackSnapshot currentSnapshot)
        {
            bool hasBurstStrike = string.Equals(
                previousSnapshot.PlayerTriggeredActiveSkillId,
                CombatSkillCatalog.BurstStrike.SkillId,
                StringComparison.Ordinal) ||
                string.Equals(
                    currentSnapshot.PlayerTriggeredActiveSkillId,
                    CombatSkillCatalog.BurstStrike.SkillId,
                    StringComparison.Ordinal);

            return hasBurstStrike &&
                DidTimerReset(
                    previousSnapshot.PlayerTriggeredActiveSkillTimerSeconds,
                    currentSnapshot.PlayerTriggeredActiveSkillTimerSeconds);
        }

        private static bool DidTimerReset(float previousTimerSeconds, float currentTimerSeconds)
        {
            if (float.IsPositiveInfinity(previousTimerSeconds) || float.IsPositiveInfinity(currentTimerSeconds))
            {
                return false;
            }

            return currentTimerSeconds > previousTimerSeconds + TimerResetEpsilon;
        }
    }

    public enum CombatEntityVisualStateId
    {
        Idle = 0,
        Attack = 1,
        Hit = 2,
        Defeat = 3,
    }

    public readonly struct CombatShellVisualState
    {
        public static readonly CombatShellVisualState Idle = new CombatShellVisualState(
            CombatEntityVisualStateId.Idle,
            CombatEntityVisualStateId.Idle);

        public CombatShellVisualState(
            CombatEntityVisualStateId playerVisualState,
            CombatEntityVisualStateId enemyVisualState)
        {
            PlayerVisualState = playerVisualState;
            EnemyVisualState = enemyVisualState;
        }

        public CombatEntityVisualStateId PlayerVisualState { get; }

        public CombatEntityVisualStateId EnemyVisualState { get; }
    }
}
