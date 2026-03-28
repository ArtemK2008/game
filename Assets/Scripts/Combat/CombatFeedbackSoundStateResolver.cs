using System;

namespace Survivalon.Combat
{
    /// <summary>
    /// Translates combat-state transitions into a compact set of sound feedback events.
    /// </summary>
    public sealed class CombatFeedbackSoundStateResolver
    {
        private const float HealthEpsilon = 0.001f;
        private const float TimerResetEpsilon = 0.0001f;
        private const float LowHealthDangerThresholdRatio = 0.35f;

        public CombatFeedbackSoundState Resolve(
            CombatFeedbackSnapshot previousSnapshot,
            CombatFeedbackSnapshot currentSnapshot)
        {
            if (!previousSnapshot.HasEncounter || !currentSnapshot.HasEncounter)
            {
                return CombatFeedbackSoundState.None;
            }

            bool didEnemyTakeDamage = currentSnapshot.EnemyCurrentHealth + HealthEpsilon <
                previousSnapshot.EnemyCurrentHealth;
            bool didPlayerTakeDamage = currentSnapshot.PlayerCurrentHealth + HealthEpsilon <
                previousSnapshot.PlayerCurrentHealth;
            bool didPlayerBasicAttack = didEnemyTakeDamage &&
                DidTimerReset(
                    previousSnapshot.PlayerBaselineAttackTimerSeconds,
                    currentSnapshot.PlayerBaselineAttackTimerSeconds);
            bool didEnemyBasicAttack = didPlayerTakeDamage &&
                DidTimerReset(
                    previousSnapshot.EnemyBaselineAttackTimerSeconds,
                    currentSnapshot.EnemyBaselineAttackTimerSeconds);
            bool playerHasBurstStrike = HasBurstStrike(previousSnapshot) || HasBurstStrike(currentSnapshot);
            bool shouldPlayBurstStrike = didEnemyTakeDamage &&
                playerHasBurstStrike &&
                DidTimerReset(
                    previousSnapshot.PlayerTriggeredActiveSkillTimerSeconds,
                    currentSnapshot.PlayerTriggeredActiveSkillTimerSeconds);
            bool shouldPlayEnemyDefeat = previousSnapshot.EnemyIsAlive && !currentSnapshot.EnemyIsAlive;
            bool shouldPlayPlayerDefeat = previousSnapshot.PlayerIsAlive && !currentSnapshot.PlayerIsAlive;

            return new CombatFeedbackSoundState(
                shouldPlayPlayerAttack: didPlayerBasicAttack && !shouldPlayBurstStrike,
                shouldPlayEnemyAttack: didEnemyBasicAttack,
                shouldPlayPlayerHit: didPlayerTakeDamage && !didEnemyBasicAttack,
                shouldPlayEnemyHit: didEnemyTakeDamage && !shouldPlayBurstStrike && !didPlayerBasicAttack,
                shouldPlayEnemyDefeat: shouldPlayEnemyDefeat,
                shouldPlayPlayerDefeat: shouldPlayPlayerDefeat,
                shouldPlayDangerLowHealth: !shouldPlayPlayerDefeat &&
                    previousSnapshot.PlayerHealthRatio > LowHealthDangerThresholdRatio &&
                    currentSnapshot.PlayerHealthRatio <= LowHealthDangerThresholdRatio &&
                    currentSnapshot.PlayerIsAlive,
                shouldPlayBurstStrike: shouldPlayBurstStrike);
        }

        private static bool HasBurstStrike(CombatFeedbackSnapshot snapshot)
        {
            return string.Equals(
                snapshot.PlayerTriggeredActiveSkillId,
                CombatSkillCatalog.BurstStrike.SkillId,
                StringComparison.Ordinal);
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

    public readonly struct CombatFeedbackSoundState
    {
        public static readonly CombatFeedbackSoundState None = new CombatFeedbackSoundState(
            shouldPlayPlayerAttack: false,
            shouldPlayEnemyAttack: false,
            shouldPlayPlayerHit: false,
            shouldPlayEnemyHit: false,
            shouldPlayEnemyDefeat: false,
            shouldPlayPlayerDefeat: false,
            shouldPlayDangerLowHealth: false,
            shouldPlayBurstStrike: false);

        public CombatFeedbackSoundState(
            bool shouldPlayPlayerAttack,
            bool shouldPlayEnemyAttack,
            bool shouldPlayPlayerHit,
            bool shouldPlayEnemyHit,
            bool shouldPlayEnemyDefeat,
            bool shouldPlayPlayerDefeat,
            bool shouldPlayDangerLowHealth,
            bool shouldPlayBurstStrike)
        {
            ShouldPlayPlayerAttack = shouldPlayPlayerAttack;
            ShouldPlayEnemyAttack = shouldPlayEnemyAttack;
            ShouldPlayPlayerHit = shouldPlayPlayerHit;
            ShouldPlayEnemyHit = shouldPlayEnemyHit;
            ShouldPlayEnemyDefeat = shouldPlayEnemyDefeat;
            ShouldPlayPlayerDefeat = shouldPlayPlayerDefeat;
            ShouldPlayDangerLowHealth = shouldPlayDangerLowHealth;
            ShouldPlayBurstStrike = shouldPlayBurstStrike;
        }

        public bool ShouldPlayPlayerAttack { get; }

        public bool ShouldPlayEnemyAttack { get; }

        public bool ShouldPlayPlayerHit { get; }

        public bool ShouldPlayEnemyHit { get; }

        public bool ShouldPlayEnemyDefeat { get; }

        public bool ShouldPlayPlayerDefeat { get; }

        public bool ShouldPlayDangerLowHealth { get; }

        public bool ShouldPlayBurstStrike { get; }
    }
}
