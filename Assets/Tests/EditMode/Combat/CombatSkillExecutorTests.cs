using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatSkillExecutorTests
    {
        [Test]
        public void ShouldApplyDirectDamageWhenExecutingBasicAttackSkill()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 1f, 1f, 100f));
            CombatSkillExecutor executor = new CombatSkillExecutor();

            executor.Execute(
                new CombatSkillExecutionRequest(
                    encounterState.PlayerEntity.BaselineAttackSkill,
                    encounterState.PlayerEntity,
                    encounterState.EnemyEntity),
                encounterState);

            Assert.That(encounterState.IsResolved, Is.False);
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(95f));
        }

        [Test]
        public void ShouldResolveEncounterOutcomeWhenDirectDamageSkillDefeatsTarget()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 60f, 1f, 0f),
                new CombatStatBlock(50f, 1f, 1f, 0f));
            CombatSkillExecutor executor = new CombatSkillExecutor();

            executor.Execute(
                new CombatSkillExecutionRequest(
                    encounterState.PlayerEntity.BaselineAttackSkill,
                    encounterState.PlayerEntity,
                    encounterState.EnemyEntity),
                encounterState);

            Assert.That(encounterState.IsResolved, Is.True);
            Assert.That(encounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(encounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(0f));
        }

        [Test]
        public void ShouldIncreaseDirectDamageWhenSourceHasAlwaysOnPassiveSkill()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 1f, 1f, 0f),
                CombatSkillCatalog.RelentlessAssault);
            CombatSkillExecutor executor = new CombatSkillExecutor();

            executor.Execute(
                new CombatSkillExecutionRequest(
                    encounterState.PlayerEntity.BaselineAttackSkill,
                    encounterState.PlayerEntity,
                    encounterState.EnemyEntity),
                encounterState);

            Assert.That(encounterState.IsResolved, Is.False);
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(88f));
        }

        [Test]
        public void ShouldApplyTriggeredActiveSkillDirectDamageThroughSkillExecutor()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 1f, 1f, 0f));
            CombatSkillExecutor executor = new CombatSkillExecutor();

            executor.Execute(
                new CombatSkillExecutionRequest(
                    CombatSkillCatalog.BurstStrike,
                    encounterState.PlayerEntity,
                    encounterState.EnemyEntity),
                encounterState);

            Assert.That(encounterState.IsResolved, Is.False);
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(80f));
        }

        [Test]
        public void ShouldApplyPassiveDirectDamageModifierToTriggeredActiveSkill()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 1f, 1f, 0f),
                CombatSkillCatalog.RelentlessAssault);
            CombatSkillExecutor executor = new CombatSkillExecutor();

            executor.Execute(
                new CombatSkillExecutionRequest(
                    CombatSkillCatalog.BurstStrike,
                    encounterState.PlayerEntity,
                    encounterState.EnemyEntity),
                encounterState);

            Assert.That(encounterState.IsResolved, Is.False);
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(76f));
        }

        [Test]
        public void ShouldApplyRunTimeBurstPayloadUpgradeThroughSkillExecutor()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 1f, 1f, 0f));
            CombatSkillExecutor executor = new CombatSkillExecutor();

            executor.Execute(
                new CombatSkillExecutionRequest(
                    CombatSkillCatalog.BurstPayload,
                    encounterState.PlayerEntity,
                    encounterState.EnemyEntity),
                encounterState);

            Assert.That(encounterState.IsResolved, Is.False);
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(70f));
        }

        private static CombatEncounterState CreateEncounterState(
            CombatStatBlock playerStats,
            CombatStatBlock enemyStats,
            params CombatSkillDefinition[] playerPassiveSkills)
        {
            CombatShellContext combatContext = new CombatShellContext(
                new NodeId("region_001_node_004"),
                new CombatEntityState(
                    new CombatEntityId("player_main"),
                    "Player Unit",
                    CombatSide.Player,
                    playerStats,
                    passiveSkills: playerPassiveSkills),
                new CombatEntityState(
                    new CombatEntityId("enemy_main"),
                    "Enemy Unit",
                    CombatSide.Enemy,
                    enemyStats));

            return new CombatEncounterState(combatContext);
        }
    }
}
