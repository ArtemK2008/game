using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    /// <summary>
    /// Проверяет переходную логику combat sound feedback для текущего 1v1 autobattle-прототипа.
    /// </summary>
    public sealed class CombatFeedbackSoundStateResolverTests
    {
        [Test]
        public void Resolve_ShouldDifferentiateBurstStrikeFromBasicAttack()
        {
            CombatFeedbackSoundStateResolver resolver = new CombatFeedbackSoundStateResolver();
            CombatFeedbackSnapshot previousSnapshot = CreateSnapshot(
                playerCurrentHealth: 120f,
                enemyCurrentHealth: 75f,
                playerBaselineAttackTimerSeconds: 0.50f,
                enemyBaselineAttackTimerSeconds: 1.00f,
                playerTriggeredActiveSkillTimerSeconds: 0.25f,
                playerTriggeredActiveSkillId: CombatSkillCatalog.BurstStrike.SkillId);
            CombatFeedbackSnapshot currentSnapshot = CreateSnapshot(
                playerCurrentHealth: 120f,
                enemyCurrentHealth: 43f,
                playerBaselineAttackTimerSeconds: 0.25f,
                enemyBaselineAttackTimerSeconds: 0.75f,
                playerTriggeredActiveSkillTimerSeconds: 2.50f,
                playerTriggeredActiveSkillId: CombatSkillCatalog.BurstStrike.SkillId);

            CombatFeedbackSoundState soundState = resolver.Resolve(previousSnapshot, currentSnapshot);

            Assert.That(soundState.ShouldPlayBurstStrike, Is.True);
            Assert.That(soundState.ShouldPlayPlayerAttack, Is.False);
            Assert.That(soundState.ShouldPlayEnemyHit, Is.False);
        }

        [Test]
        public void Resolve_ShouldPreferAttackCueOverHitCueForBasicAttackTransitions()
        {
            CombatFeedbackSoundStateResolver resolver = new CombatFeedbackSoundStateResolver();
            CombatFeedbackSnapshot previousSnapshot = CreateSnapshot(
                playerCurrentHealth: 120f,
                enemyCurrentHealth: 75f,
                playerBaselineAttackTimerSeconds: 0.05f,
                enemyBaselineAttackTimerSeconds: 0.10f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);
            CombatFeedbackSnapshot currentSnapshot = CreateSnapshot(
                playerCurrentHealth: 108f,
                enemyCurrentHealth: 61f,
                playerBaselineAttackTimerSeconds: 1.20f,
                enemyBaselineAttackTimerSeconds: 1.35f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);

            CombatFeedbackSoundState soundState = resolver.Resolve(previousSnapshot, currentSnapshot);

            Assert.That(soundState.ShouldPlayPlayerAttack, Is.True);
            Assert.That(soundState.ShouldPlayEnemyAttack, Is.True);
            Assert.That(soundState.ShouldPlayPlayerHit, Is.False);
            Assert.That(soundState.ShouldPlayEnemyHit, Is.False);
        }

        [Test]
        public void Resolve_ShouldKeepHitCueWhenDamageHasNoMatchingAttackReset()
        {
            CombatFeedbackSoundStateResolver resolver = new CombatFeedbackSoundStateResolver();
            CombatFeedbackSnapshot previousSnapshot = CreateSnapshot(
                playerCurrentHealth: 120f,
                enemyCurrentHealth: 75f,
                playerBaselineAttackTimerSeconds: 0.80f,
                enemyBaselineAttackTimerSeconds: 0.90f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);
            CombatFeedbackSnapshot currentSnapshot = CreateSnapshot(
                playerCurrentHealth: 108f,
                enemyCurrentHealth: 61f,
                playerBaselineAttackTimerSeconds: 0.55f,
                enemyBaselineAttackTimerSeconds: 0.60f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);

            CombatFeedbackSoundState soundState = resolver.Resolve(previousSnapshot, currentSnapshot);

            Assert.That(soundState.ShouldPlayPlayerAttack, Is.False);
            Assert.That(soundState.ShouldPlayEnemyAttack, Is.False);
            Assert.That(soundState.ShouldPlayPlayerHit, Is.True);
            Assert.That(soundState.ShouldPlayEnemyHit, Is.True);
        }

        [Test]
        public void Resolve_ShouldRequestDangerOnlyOnThresholdCross()
        {
            CombatFeedbackSoundStateResolver resolver = new CombatFeedbackSoundStateResolver();
            CombatFeedbackSnapshot safeSnapshot = CreateSnapshot(
                playerCurrentHealth: 60f,
                enemyCurrentHealth: 75f,
                playerBaselineAttackTimerSeconds: 0.50f,
                enemyBaselineAttackTimerSeconds: 0.50f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);
            CombatFeedbackSnapshot lowHealthSnapshot = CreateSnapshot(
                playerCurrentHealth: 40f,
                enemyCurrentHealth: 75f,
                playerBaselineAttackTimerSeconds: 0.25f,
                enemyBaselineAttackTimerSeconds: 1.25f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);
            CombatFeedbackSnapshot stillLowSnapshot = CreateSnapshot(
                playerCurrentHealth: 24f,
                enemyCurrentHealth: 75f,
                playerBaselineAttackTimerSeconds: 0.10f,
                enemyBaselineAttackTimerSeconds: 1.00f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);

            CombatFeedbackSoundState thresholdCrossState = resolver.Resolve(safeSnapshot, lowHealthSnapshot);
            CombatFeedbackSoundState repeatedLowHealthState = resolver.Resolve(lowHealthSnapshot, stillLowSnapshot);

            Assert.That(thresholdCrossState.ShouldPlayDangerLowHealth, Is.True);
            Assert.That(repeatedLowHealthState.ShouldPlayDangerLowHealth, Is.False);
            Assert.That(repeatedLowHealthState.ShouldPlayEnemyAttack, Is.True);
            Assert.That(repeatedLowHealthState.ShouldPlayPlayerHit, Is.True);
        }

        [Test]
        public void Resolve_ShouldRequestDefeatWithoutLowHealthSpamWhenPlayerDies()
        {
            CombatFeedbackSoundStateResolver resolver = new CombatFeedbackSoundStateResolver();
            CombatFeedbackSnapshot previousSnapshot = CreateSnapshot(
                playerCurrentHealth: 20f,
                enemyCurrentHealth: 18f,
                playerBaselineAttackTimerSeconds: 0.30f,
                enemyBaselineAttackTimerSeconds: 0.15f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity);
            CombatFeedbackSnapshot currentSnapshot = new CombatFeedbackSnapshot(
                hasEncounter: true,
                playerCurrentHealth: 0f,
                playerMaxHealth: 120f,
                enemyCurrentHealth: 18f,
                enemyMaxHealth: 75f,
                playerIsAlive: false,
                enemyIsAlive: true,
                playerBaselineAttackTimerSeconds: 0f,
                enemyBaselineAttackTimerSeconds: 1f,
                playerTriggeredActiveSkillTimerSeconds: float.PositiveInfinity,
                playerTriggeredActiveSkillId: null);

            CombatFeedbackSoundState soundState = resolver.Resolve(previousSnapshot, currentSnapshot);

            Assert.That(soundState.ShouldPlayEnemyAttack, Is.True);
            Assert.That(soundState.ShouldPlayPlayerHit, Is.False);
            Assert.That(soundState.ShouldPlayPlayerDefeat, Is.True);
            Assert.That(soundState.ShouldPlayDangerLowHealth, Is.False);
        }

        private static CombatFeedbackSnapshot CreateSnapshot(
            float playerCurrentHealth,
            float enemyCurrentHealth,
            float playerBaselineAttackTimerSeconds,
            float enemyBaselineAttackTimerSeconds,
            float playerTriggeredActiveSkillTimerSeconds,
            string playerTriggeredActiveSkillId = null)
        {
            return new CombatFeedbackSnapshot(
                hasEncounter: true,
                playerCurrentHealth: playerCurrentHealth,
                playerMaxHealth: 120f,
                enemyCurrentHealth: enemyCurrentHealth,
                enemyMaxHealth: 75f,
                playerIsAlive: playerCurrentHealth > 0f,
                enemyIsAlive: enemyCurrentHealth > 0f,
                playerBaselineAttackTimerSeconds: playerBaselineAttackTimerSeconds,
                enemyBaselineAttackTimerSeconds: enemyBaselineAttackTimerSeconds,
                playerTriggeredActiveSkillTimerSeconds: playerTriggeredActiveSkillTimerSeconds,
                playerTriggeredActiveSkillId: playerTriggeredActiveSkillId);
        }
    }
}
