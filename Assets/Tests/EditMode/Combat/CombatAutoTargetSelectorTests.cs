using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Combat
{
    public sealed class CombatAutoTargetSelectorTests
    {
        [Test]
        public void ShouldSelectOnlyActiveEnemyForPlayerSideCombatant()
        {
            CombatEncounterState encounterState = CreateEncounterState();

            CombatEntityRuntimeState target = new CombatAutoTargetSelector().SelectTarget(
                encounterState,
                CombatSide.Player);

            Assert.That(target, Is.SameAs(encounterState.EnemyEntity));
        }

        [Test]
        public void ShouldSelectOnlyActivePlayerForEnemySideCombatant()
        {
            CombatEncounterState encounterState = CreateEncounterState();

            CombatEntityRuntimeState target = new CombatAutoTargetSelector().SelectTarget(
                encounterState,
                CombatSide.Enemy);

            Assert.That(target, Is.SameAs(encounterState.PlayerEntity));
        }

        [Test]
        public void ShouldSkipDefeatedEnemyAndRejectSelectionWhenNoActiveEnemyExists()
        {
            CombatEncounterState encounterState = CreateEncounterState();
            encounterState.EnemyEntity.ApplyDamage(encounterState.EnemyEntity.MaxHealth);

            Assert.That(
                () => new CombatAutoTargetSelector().SelectTarget(encounterState, CombatSide.Player),
                Throws.InvalidOperationException.With.Message.Contains("No active target"));
        }

        [Test]
        public void ShouldRemainDeterministicForStableEncounterState()
        {
            CombatEncounterState encounterState = CreateEncounterState();
            CombatAutoTargetSelector selector = new CombatAutoTargetSelector();

            CombatEntityRuntimeState firstTarget = selector.SelectTarget(encounterState, CombatSide.Player);
            CombatEntityRuntimeState secondTarget = selector.SelectTarget(encounterState, CombatSide.Player);

            Assert.That(firstTarget, Is.SameAs(secondTarget));
        }

        private static CombatEncounterState CreateEncounterState()
        {
            CombatShellContext combatContext = new CombatShellContext(
                new NodeId("region_001_node_004"),
                new CombatEntityState(
                    new CombatEntityId("player_main"),
                    "Player Unit",
                    CombatSide.Player,
                    new CombatStatBlock(120f, 14f, 1.2f, 12f)),
                new CombatEntityState(
                    new CombatEntityId("enemy_main"),
                    "Enemy Unit",
                    CombatSide.Enemy,
                    new CombatStatBlock(75f, 8f, 0.9f, 4f)));

            return new CombatEncounterState(combatContext);
        }
    }
}

