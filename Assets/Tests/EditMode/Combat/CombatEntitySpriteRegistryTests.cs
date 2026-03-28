using NUnit.Framework;
using Survivalon.Combat;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatEntitySpriteRegistryTests
    {
        [Test]
        public void LoadOrNull_ShouldResolveAllCurrentPlayerAndEnemySpriteSets()
        {
            CombatEntitySpriteRegistry registry = CombatEntitySpriteRegistry.LoadOrNull();

            Assert.That(registry, Is.Not.Null);
            Assert.That(registry.TryGetSprite("player_main", CombatEntityVisualStateId.Idle, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_main", CombatEntityVisualStateId.Attack, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_main", CombatEntityVisualStateId.Hit, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_main", CombatEntityVisualStateId.Defeat, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_striker", CombatEntityVisualStateId.Idle, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_striker", CombatEntityVisualStateId.Attack, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_striker", CombatEntityVisualStateId.Hit, out _), Is.True);
            Assert.That(registry.TryGetSprite("player_striker", CombatEntityVisualStateId.Defeat, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_001", CombatEntityVisualStateId.Idle, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_001", CombatEntityVisualStateId.Attack, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_001", CombatEntityVisualStateId.Hit, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_001", CombatEntityVisualStateId.Defeat, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_002", CombatEntityVisualStateId.Idle, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_002", CombatEntityVisualStateId.Attack, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_002", CombatEntityVisualStateId.Hit, out _), Is.True);
            Assert.That(registry.TryGetSprite("enemy_002", CombatEntityVisualStateId.Defeat, out _), Is.True);
            Assert.That(registry.TryGetSprite("boss_001", CombatEntityVisualStateId.Idle, out _), Is.True);
            Assert.That(registry.TryGetSprite("boss_001", CombatEntityVisualStateId.Attack, out _), Is.True);
            Assert.That(registry.TryGetSprite("boss_001", CombatEntityVisualStateId.Hit, out _), Is.True);
            Assert.That(registry.TryGetSprite("boss_001", CombatEntityVisualStateId.Defeat, out _), Is.True);
            Assert.That(
                registry.TryGetSprite("region_001_node_004_enemy_001", CombatEntityVisualStateId.Idle, out _),
                Is.True);
            Assert.That(
                registry.TryGetSprite("region_001_node_002_enemy_002", CombatEntityVisualStateId.Attack, out _),
                Is.True);
            Assert.That(
                registry.TryGetSprite("region_001_node_005_boss_001", CombatEntityVisualStateId.Defeat, out _),
                Is.True);
        }
    }
}
