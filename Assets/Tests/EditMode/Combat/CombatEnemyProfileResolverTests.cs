using NUnit.Framework;
using Survivalon.Combat;
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

            Assert.That(resolvedProfile, Is.SameAs(CombatEnemyProfileCatalog.BulwarkRaider));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Bulwark Raider"));
            Assert.That(resolvedProfile.BaseStats.MaxHealth, Is.EqualTo(105f));
            Assert.That(resolvedProfile.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(resolvedProfile.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(resolvedProfile.BaseStats.Defense, Is.EqualTo(4f));
        }

        [Test]
        public void ShouldResolveEnemyUnitForOtherStandardCombatNodes()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatEnemyProfileCatalog.EnemyUnit));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Enemy Unit"));
        }

        [Test]
        public void ShouldResolveGateEnemyForBossCombatNodes()
        {
            CombatEnemyProfileResolver resolver = new CombatEnemyProfileResolver();

            CombatEnemyProfile resolvedProfile = resolver.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState());

            Assert.That(resolvedProfile, Is.SameAs(CombatEnemyProfileCatalog.GateEnemy));
            Assert.That(resolvedProfile.DisplayName, Is.EqualTo("Gate Enemy"));
            Assert.That(resolvedProfile.EntityIdSuffix, Is.EqualTo("boss_001"));
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
