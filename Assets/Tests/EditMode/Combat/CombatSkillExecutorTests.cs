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

        private static CombatEncounterState CreateEncounterState(
            CombatStatBlock playerStats,
            CombatStatBlock enemyStats)
        {
            CombatShellContext combatContext = new CombatShellContext(
                new NodeId("region_001_node_004"),
                new CombatEntityState(
                    new CombatEntityId("player_main"),
                    "Player Unit",
                    CombatSide.Player,
                    playerStats),
                new CombatEntityState(
                    new CombatEntityId("enemy_main"),
                    "Enemy Unit",
                    CombatSide.Enemy,
                    enemyStats));

            return new CombatEncounterState(combatContext);
        }
    }
}
