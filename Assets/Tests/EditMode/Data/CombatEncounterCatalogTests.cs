using NUnit.Framework;
using Survivalon.Data.Combat;

namespace Survivalon.Tests.EditMode.Data
{
    public sealed class CombatEncounterCatalogTests
    {
        [Test]
        public void ShouldExposeShippedStandardEnemyEncountersThroughEncounterCatalog()
        {
            Assert.That(
                CombatEncounterCatalog.EnemyUnitEncounter.EnemyProfile,
                Is.SameAs(CombatStandardEnemyProfileCatalog.EnemyUnit));
            Assert.That(
                CombatEncounterCatalog.BulwarkRaiderEncounter.EnemyProfile,
                Is.SameAs(CombatStandardEnemyProfileCatalog.BulwarkRaider));
        }

        [Test]
        public void ShouldExposeGatePlaceholderEncounterThroughSeparateBossCatalog()
        {
            Assert.That(
                CombatEncounterCatalog.GatePlaceholderEncounter.EnemyProfile,
                Is.SameAs(CombatBossPlaceholderProfileCatalog.GateEnemy));
            Assert.That(
                CombatEncounterCatalog.GatePlaceholderEncounter.EnemyProfile.BehaviorType,
                Is.EqualTo(CombatEnemyBehaviorType.GatePlaceholder));
        }
    }
}
