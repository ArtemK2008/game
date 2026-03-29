using NUnit.Framework;
using Survivalon.Data.Combat;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class CombatEncounterDefinitionCatalogTests
    {
        [Test]
        public void ShouldExposeShippedStandardEnemyEncountersThroughStandardEncounterCatalog()
        {
            Assert.That(
                CombatStandardEncounterCatalog.EnemyUnitEncounter.PrimaryEnemyProfile,
                Is.SameAs(CombatStandardEnemyProfileCatalog.EnemyUnit));
            Assert.That(
                CombatStandardEncounterCatalog.BulwarkRaiderEncounter.PrimaryEnemyProfile,
                Is.SameAs(CombatStandardEnemyProfileCatalog.BulwarkRaider));
            Assert.That(
                CombatStandardEncounterCatalog.RuinSentinelEncounter.PrimaryEnemyProfile,
                Is.SameAs(CombatStandardEnemyProfileCatalog.RuinSentinel));
            Assert.That(
                CombatStandardEncounterCatalog.EnemyUnitEncounter.EncounterType,
                Is.EqualTo(CombatEncounterType.StandardEnemy));
            Assert.That(
                CombatStandardEncounterCatalog.BulwarkRaiderEncounter.PrimaryEnemyProfile.HostileEntityType,
                Is.EqualTo(CombatHostileEntityType.StandardEnemy));
            Assert.That(
                CombatStandardEncounterCatalog.RuinSentinelEncounter.PrimaryEnemyProfile.HostileEntityType,
                Is.EqualTo(CombatHostileEntityType.StandardEnemy));
        }

        [Test]
        public void ShouldExposeExplicitGateBossEncounterThroughBossEncounterCatalog()
        {
            Assert.That(
                CombatBossEncounterCatalog.GateBossEncounter.BossProfile,
                Is.SameAs(CombatBossProfileCatalog.GateBoss));
            Assert.That(
                CombatBossEncounterCatalog.GateBossEncounter.EncounterType,
                Is.EqualTo(CombatEncounterType.Boss));
            Assert.That(
                CombatBossEncounterCatalog.GateBossEncounter.BossRoleType,
                Is.EqualTo(CombatBossRoleType.GateBoss));
            Assert.That(
                CombatBossEncounterCatalog.GateBossEncounter.BossProfile.BehaviorType,
                Is.EqualTo(CombatEnemyBehaviorType.GateBoss));
            Assert.That(
                CombatBossEncounterCatalog.GateBossEncounter.BossProfile.HostileEntityType,
                Is.EqualTo(CombatHostileEntityType.Boss));
        }

        [Test]
        public void ShouldRejectConstructingBossEncounterFromStandardEnemyProfile()
        {
            Assert.That(
                () => new CombatBossEncounterDefinition(
                    "combat_encounter_invalid_boss",
                    "boss_invalid",
                    CombatBossRoleType.GateBoss,
                    CombatStandardEnemyProfileCatalog.EnemyUnit),
                Throws.ArgumentException.With.Message.Contains("boss hostile profile"));
        }
    }
}
