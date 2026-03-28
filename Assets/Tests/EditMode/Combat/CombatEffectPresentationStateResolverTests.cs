using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatEffectPresentationStateResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnNoEffectsWhenNoEncounterIsPresent()
        {
            CombatEffectPresentationStateResolver resolver = new CombatEffectPresentationStateResolver();

            CombatEffectPresentationState effectState = resolver.Resolve(
                CombatFeedbackSnapshot.None,
                CombatFeedbackSnapshot.None);

            Assert.That(effectState.PlayerEffectSprite, Is.Null);
            Assert.That(effectState.EnemyEffectSprite, Is.Null);
            Assert.That(effectState.CenterEffectSprite, Is.Null);
        }

        [Test]
        public void Resolve_ShouldShowEnemyImpactForPlayerBasicAttack()
        {
            CombatEffectPresentationStateResolver resolver = new CombatEffectPresentationStateResolver();

            CombatEffectPresentationState effectState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 75f,
                    playerAttackTimer: 0.1f,
                    enemyAttackTimer: 0.6f),
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 61f,
                    playerAttackTimer: 0.83f,
                    enemyAttackTimer: 0.35f));

            Assert.That(effectState.PlayerEffectSprite, Is.Null);
            Assert.That(effectState.EnemyEffectSprite, Is.Not.Null);
            Assert.That(effectState.CenterEffectSprite, Is.Null);
        }

        [Test]
        public void Resolve_ShouldKeepEnemyImpactForDamageWithoutMatchingPlayerAttackReset()
        {
            CombatEffectPresentationStateResolver resolver = new CombatEffectPresentationStateResolver();

            CombatEffectPresentationState effectState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 75f,
                    playerAttackTimer: 0.8f,
                    enemyAttackTimer: 0.6f),
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 61f,
                    playerAttackTimer: 0.75f,
                    enemyAttackTimer: 0.35f));

            Assert.That(effectState.PlayerEffectSprite, Is.Null);
            Assert.That(effectState.EnemyEffectSprite, Is.Not.Null);
            Assert.That(effectState.CenterEffectSprite, Is.Null);
        }

        [Test]
        public void Resolve_ShouldPreferBurstStrikeCenterCueWithoutEnemyImpactCue()
        {
            CombatEffectPresentationStateResolver resolver = new CombatEffectPresentationStateResolver();

            CombatEffectPresentationState effectState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 110f,
                    enemyHealth: 105f,
                    playerAttackTimer: 0.5f,
                    enemyAttackTimer: 0.6f,
                    playerTriggeredSkillTimer: 0.1f,
                    playerTriggeredSkillId: CombatSkillCatalog.BurstStrike.SkillId),
                CreateSnapshot(
                    playerHealth: 110f,
                    enemyHealth: 87f,
                    playerAttackTimer: 0.25f,
                    enemyAttackTimer: 0.35f,
                    playerTriggeredSkillTimer: 2.5f,
                    playerTriggeredSkillId: CombatSkillCatalog.BurstStrike.SkillId));

            Assert.That(effectState.PlayerEffectSprite, Is.Null);
            Assert.That(effectState.EnemyEffectSprite, Is.Null);
            Assert.That(effectState.CenterEffectSprite, Is.Not.Null);
        }

        [Test]
        public void Resolve_ShouldShowDangerOnlyOnThresholdCross()
        {
            CombatEffectPresentationStateResolver resolver = new CombatEffectPresentationStateResolver();

            CombatEffectPresentationState thresholdCrossState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 50f,
                    enemyHealth: 70f,
                    playerAttackTimer: 0.6f,
                    enemyAttackTimer: 0.1f),
                CreateSnapshot(
                    playerHealth: 40f,
                    enemyHealth: 70f,
                    playerAttackTimer: 0.3f,
                    enemyAttackTimer: 0.9f));
            CombatEffectPresentationState stillLowState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 40f,
                    enemyHealth: 70f,
                    playerAttackTimer: 0.3f,
                    enemyAttackTimer: 0.9f),
                CreateSnapshot(
                    playerHealth: 35f,
                    enemyHealth: 70f,
                    playerAttackTimer: 0.1f,
                    enemyAttackTimer: 0.5f));

            Assert.That(thresholdCrossState.CenterEffectSprite, Is.Not.Null);
            Assert.That(stillLowState.CenterEffectSprite, Is.Null);
        }

        [Test]
        public void Resolve_ShouldPreferDefeatCueOverBurstAndDanger()
        {
            CombatEffectPresentationStateResolver resolver = new CombatEffectPresentationStateResolver();

            CombatEffectPresentationState effectState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 32f,
                    enemyHealth: 10f,
                    playerAttackTimer: 0.4f,
                    enemyAttackTimer: 0.6f,
                    playerTriggeredSkillTimer: 0.1f,
                    playerTriggeredSkillId: CombatSkillCatalog.BurstStrike.SkillId),
                CreateSnapshot(
                    playerHealth: 32f,
                    enemyHealth: 0f,
                    playerAttackTimer: 0.95f,
                    enemyAttackTimer: 0.35f,
                    playerTriggeredSkillTimer: 2.3f,
                    playerTriggeredSkillId: CombatSkillCatalog.BurstStrike.SkillId,
                    enemyIsAlive: false));

            Assert.That(effectState.PlayerEffectSprite, Is.Null);
            Assert.That(effectState.EnemyEffectSprite, Is.Not.Null);
            Assert.That(effectState.CenterEffectSprite, Is.Null);
        }

        private static CombatFeedbackSnapshot CreateSnapshot(
            float playerHealth,
            float enemyHealth,
            float playerAttackTimer,
            float enemyAttackTimer,
            float playerTriggeredSkillTimer = float.PositiveInfinity,
            string playerTriggeredSkillId = null,
            bool playerIsAlive = true,
            bool enemyIsAlive = true)
        {
            return new CombatFeedbackSnapshot(
                hasEncounter: true,
                playerCurrentHealth: playerHealth,
                playerMaxHealth: 120f,
                enemyCurrentHealth: enemyHealth,
                enemyMaxHealth: 105f,
                playerIsAlive: playerIsAlive,
                enemyIsAlive: enemyIsAlive,
                playerBaselineAttackTimerSeconds: playerAttackTimer,
                enemyBaselineAttackTimerSeconds: enemyAttackTimer,
                playerTriggeredActiveSkillTimerSeconds: playerTriggeredSkillTimer,
                playerTriggeredActiveSkillId: playerTriggeredSkillId);
        }
    }
}
