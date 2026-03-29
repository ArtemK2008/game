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
        public void ShouldResolveRuinSentinelForSunscorchCombatNodes()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreateSunscorchCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatStandardEnemyProfileCatalog.RuinSentinel));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Ruin Sentinel"));
            Assert.That(resolvedProfile.EntityIdSuffix, Is.EqualTo("enemy_003"));
            Assert.That(resolvedProfile.BaseStats.MaxHealth, Is.EqualTo(95f));
            Assert.That(resolvedProfile.BaseStats.AttackPower, Is.EqualTo(11f));
            Assert.That(resolvedProfile.BaseStats.AttackRate, Is.EqualTo(0.95f));
            Assert.That(resolvedProfile.BaseStats.Defense, Is.EqualTo(8f));
            Assert.That(resolvedProfile.BehaviorType, Is.EqualTo(CombatEnemyBehaviorType.SentinelPressure));
        }

        [Test]
        public void ShouldExposeDifferentPressureStatsAcrossShippedStandardEnemyProfiles()
        {
            CombatEnemyProfile enemyUnitProfile = CombatStandardEnemyProfileCatalog.EnemyUnit;
            CombatEnemyProfile bulwarkRaiderProfile = CombatStandardEnemyProfileCatalog.BulwarkRaider;
            CombatEnemyProfile ruinSentinelProfile = CombatStandardEnemyProfileCatalog.RuinSentinel;

            Assert.That(enemyUnitProfile.BaseStats.AttackRate, Is.GreaterThan(bulwarkRaiderProfile.BaseStats.AttackRate));
            Assert.That(enemyUnitProfile.BaseStats.MaxHealth, Is.LessThan(bulwarkRaiderProfile.BaseStats.MaxHealth));
            Assert.That(enemyUnitProfile.BaseStats.Defense, Is.LessThan(bulwarkRaiderProfile.BaseStats.Defense));
            Assert.That(enemyUnitProfile.BaseStats.AttackPower, Is.LessThan(bulwarkRaiderProfile.BaseStats.AttackPower));
            Assert.That(enemyUnitProfile.BehaviorType, Is.Not.EqualTo(bulwarkRaiderProfile.BehaviorType));
            Assert.That(ruinSentinelProfile.BaseStats.AttackPower, Is.GreaterThan(bulwarkRaiderProfile.BaseStats.AttackPower));
            Assert.That(ruinSentinelProfile.BaseStats.AttackRate, Is.LessThan(enemyUnitProfile.BaseStats.AttackRate));
            Assert.That(ruinSentinelProfile.BaseStats.Defense, Is.GreaterThan(bulwarkRaiderProfile.BaseStats.Defense));
            Assert.That(ruinSentinelProfile.BehaviorType, Is.Not.EqualTo(enemyUnitProfile.BehaviorType));
            Assert.That(ruinSentinelProfile.BehaviorType, Is.Not.EqualTo(bulwarkRaiderProfile.BehaviorType));
        }

        [Test]
        public void ShouldResolveGateEnemyForBossCombatNodes()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatBossProfileCatalog.GateBoss));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Gate Boss"));
            Assert.That(resolvedProfile.EntityIdSuffix, Is.EqualTo("boss_001"));
            Assert.That(resolvedProfile.BehaviorType, Is.EqualTo(CombatEnemyBehaviorType.GateBoss));
            Assert.That(resolvedProfile.HostileEntityType, Is.EqualTo(CombatHostileEntityType.Boss));
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
