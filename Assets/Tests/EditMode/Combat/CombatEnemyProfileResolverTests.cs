using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatEnemyProfileResolverTests
    {
        [Test]
        public void ShouldResolveBulwarkRaiderForForestPushCombatNode()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatStandardEnemyProfileCatalog.BulwarkRaider));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Bulwark Raider"));
            Assert.That(resolvedProfile.BaseStats.MaxHealth, Is.EqualTo(105f));
            Assert.That(resolvedProfile.BaseStats.AttackPower, Is.EqualTo(9f));
            Assert.That(resolvedProfile.BaseStats.AttackRate, Is.EqualTo(0.85f));
            Assert.That(resolvedProfile.BaseStats.Defense, Is.EqualTo(6f));
            Assert.That(resolvedProfile.BehaviorType, Is.EqualTo(CombatEnemyBehaviorType.BulwarkPressure));
        }

        [Test]
        public void ShouldResolveEnemyUnitForOtherStandardCombatNodes()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatStandardEnemyProfileCatalog.EnemyUnit));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(resolvedProfile.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(resolvedProfile.BaseStats.AttackPower, Is.EqualTo(7f));
            Assert.That(resolvedProfile.BaseStats.AttackRate, Is.EqualTo(1.25f));
            Assert.That(resolvedProfile.BaseStats.Defense, Is.EqualTo(2f));
            Assert.That(resolvedProfile.BehaviorType, Is.EqualTo(CombatEnemyBehaviorType.FastPressure));
        }

        [Test]
        public void ShouldExposeDifferentPressureStatsAcrossShippedStandardEnemyProfiles()
        {
            CombatEnemyProfile enemyUnitProfile = CombatStandardEnemyProfileCatalog.EnemyUnit;
            CombatEnemyProfile bulwarkRaiderProfile = CombatStandardEnemyProfileCatalog.BulwarkRaider;

            Assert.That(enemyUnitProfile.BaseStats.AttackRate, Is.GreaterThan(bulwarkRaiderProfile.BaseStats.AttackRate));
            Assert.That(enemyUnitProfile.BaseStats.MaxHealth, Is.LessThan(bulwarkRaiderProfile.BaseStats.MaxHealth));
            Assert.That(enemyUnitProfile.BaseStats.Defense, Is.LessThan(bulwarkRaiderProfile.BaseStats.Defense));
            Assert.That(enemyUnitProfile.BaseStats.AttackPower, Is.LessThan(bulwarkRaiderProfile.BaseStats.AttackPower));
            Assert.That(enemyUnitProfile.BehaviorType, Is.Not.EqualTo(bulwarkRaiderProfile.BehaviorType));
        }

        [Test]
        public void ShouldResolveGateEnemyForBossCombatNodes()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatBossPlaceholderProfileCatalog.GateEnemy));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Gate Enemy"));
            Assert.That(resolvedProfile.EntityIdSuffix, Is.EqualTo("boss_001"));
            Assert.That(resolvedProfile.BehaviorType, Is.EqualTo(CombatEnemyBehaviorType.GatePlaceholder));
        }

        [Test]
        public void ShouldRejectCombatNodeWithoutEncounterContent()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            Assert.That(
                () => resolver.Resolve(new NodePlaceholderState(
                    new NodeId("combat_node"),
                    new RegionId("region_001"),
                    NodeType.Combat,
                    NodeState.Available,
                    new NodeId("origin_node"))),
                Throws.InvalidOperationException.With.Message.Contains("encounter content"));
        }

        [Test]
        public void ShouldRejectNonCombatNodeContext()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            Assert.That(
                () => resolver.Resolve(NodePlaceholderTestData.CreateServicePlaceholderState()),
                Throws.InvalidOperationException.With.Message.Contains("combat-compatible"));
        }
    }
}
