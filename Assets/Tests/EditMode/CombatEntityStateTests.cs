using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class CombatEntityStateTests
    {
        [Test]
        public void ShouldCreatePlayerSideCombatEntityWithAliveAndActiveDefaults()
        {
            CombatEntityState combatEntity = new CombatEntityState(
                new CombatEntityId("player_main"),
                "Player Unit",
                CombatSide.Player);

            Assert.That(combatEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(combatEntity.DisplayName, Is.EqualTo("Player Unit"));
            Assert.That(combatEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(combatEntity.IsAlive, Is.True);
            Assert.That(combatEntity.IsActive, Is.True);
        }

        [Test]
        public void ShouldCreateEnemySideCombatEntityWithExplicitIdentity()
        {
            CombatEntityState combatEntity = new CombatEntityState(
                new CombatEntityId("region_001_node_004_enemy_001"),
                "Enemy Unit",
                CombatSide.Enemy);

            Assert.That(combatEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(combatEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(combatEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(combatEntity.IsAlive, Is.True);
            Assert.That(combatEntity.IsActive, Is.True);
        }

        [Test]
        public void ShouldCreateCombatShellContextUsingCombatEntityState()
        {
            CombatShellContextFactory factory = new CombatShellContextFactory();
            CombatShellContext combatContext = factory.Create(new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002")));

            Assert.That(combatContext.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(combatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(combatContext.PlayerEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(combatContext.EnemyEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(combatContext.EnemyEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(combatContext.PlayerEntity.IsAlive, Is.True);
            Assert.That(combatContext.EnemyEntity.IsActive, Is.True);
        }
    }
}
