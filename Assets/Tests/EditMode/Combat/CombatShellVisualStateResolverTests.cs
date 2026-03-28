using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatShellVisualStateResolverTests
    {
        [Test]
        public void Resolve_ShouldReturnIdleWhenNoEncounterIsPresent()
        {
            CombatShellVisualStateResolver resolver = new CombatShellVisualStateResolver();

            CombatShellVisualState visualState = resolver.Resolve(
                CombatFeedbackSnapshot.None,
                CombatFeedbackSnapshot.None);

            Assert.That(visualState.PlayerVisualState, Is.EqualTo(CombatEntityVisualStateId.Idle));
            Assert.That(visualState.EnemyVisualState, Is.EqualTo(CombatEntityVisualStateId.Idle));
        }

        [Test]
        public void Resolve_ShouldShowPlayerAttackAndEnemyHitForPlayerBaselineAttack()
        {
            CombatShellVisualStateResolver resolver = new CombatShellVisualStateResolver();

            CombatShellVisualState visualState = resolver.Resolve(
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

            Assert.That(visualState.PlayerVisualState, Is.EqualTo(CombatEntityVisualStateId.Attack));
            Assert.That(visualState.EnemyVisualState, Is.EqualTo(CombatEntityVisualStateId.Hit));
        }

        [Test]
        public void Resolve_ShouldShowEnemyAttackAndPlayerHitForEnemyAttack()
        {
            CombatShellVisualStateResolver resolver = new CombatShellVisualStateResolver();

            CombatShellVisualState visualState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 75f,
                    playerAttackTimer: 0.7f,
                    enemyAttackTimer: 0.1f),
                CreateSnapshot(
                    playerHealth: 112f,
                    enemyHealth: 75f,
                    playerAttackTimer: 0.45f,
                    enemyAttackTimer: 0.8f));

            Assert.That(visualState.PlayerVisualState, Is.EqualTo(CombatEntityVisualStateId.Hit));
            Assert.That(visualState.EnemyVisualState, Is.EqualTo(CombatEntityVisualStateId.Attack));
        }

        [Test]
        public void Resolve_ShouldKeepBurstStrikeOnPlayerAttackVisualAndEnemyHitVisual()
        {
            CombatShellVisualStateResolver resolver = new CombatShellVisualStateResolver();

            CombatShellVisualState visualState = resolver.Resolve(
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

            Assert.That(visualState.PlayerVisualState, Is.EqualTo(CombatEntityVisualStateId.Attack));
            Assert.That(visualState.EnemyVisualState, Is.EqualTo(CombatEntityVisualStateId.Hit));
        }

        [Test]
        public void Resolve_ShouldShowDefeatStateWhenEnemyDies()
        {
            CombatShellVisualStateResolver resolver = new CombatShellVisualStateResolver();

            CombatShellVisualState visualState = resolver.Resolve(
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 5f,
                    playerAttackTimer: 0.1f,
                    enemyAttackTimer: 0.6f),
                CreateSnapshot(
                    playerHealth: 120f,
                    enemyHealth: 0f,
                    playerAttackTimer: 0.83f,
                    enemyAttackTimer: 0.35f,
                    enemyIsAlive: false));

            Assert.That(visualState.PlayerVisualState, Is.EqualTo(CombatEntityVisualStateId.Attack));
            Assert.That(visualState.EnemyVisualState, Is.EqualTo(CombatEntityVisualStateId.Defeat));
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
