using System;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class CombatEncounterResolverTests
    {
        [Test]
        public void ShouldAdvancePlayerAttacksAgainstEnemyOverTime()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 10f, 0.25f, 0f));

            bool advanced = new CombatEncounterResolver().TryAdvance(encounterState, 1f);

            Assert.That(advanced, Is.True);
            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.EqualTo(100f));
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(90f));
        }

        [Test]
        public void ShouldAdvanceEnemyHostilityAgainstPlayerOverTime()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 0.5f, 0f),
                new CombatStatBlock(100f, 7f, 1f, 0f));

            bool advanced = new CombatEncounterResolver().TryAdvance(encounterState, 1f);

            Assert.That(advanced, Is.True);
            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.EqualTo(93f));
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(100f));
        }

        [Test]
        public void ShouldKeepEnemyHostilityTargetedAtPlayerSide()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 9f, 0.5f, 12f),
                new CombatStatBlock(100f, 8f, 1f, 0f));
            CombatEncounterResolver resolver = new CombatEncounterResolver();

            resolver.TryAdvance(encounterState, 1.25f);

            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.LessThan(encounterState.PlayerEntity.MaxHealth));
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(encounterState.EnemyEntity.MaxHealth));
            Assert.That(encounterState.PlayerEntity.IsAlive, Is.True);
            Assert.That(encounterState.EnemyEntity.IsAlive, Is.True);
        }

        [Test]
        public void ShouldRespectAttackIntervalTiming()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 9f, 2f, 0f),
                new CombatStatBlock(100f, 1f, 0.25f, 0f));
            CombatEncounterResolver resolver = new CombatEncounterResolver();

            resolver.TryAdvance(encounterState, 0.49f);

            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(100f));
            Assert.That(encounterState.ElapsedCombatSeconds, Is.EqualTo(0.49f).Within(0.001f));

            resolver.TryAdvance(encounterState, 0.01f);

            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(91f));
            Assert.That(encounterState.ElapsedCombatSeconds, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void ShouldApplyMitigationWhenResolvingDamage()
        {
            CombatEncounterState lowDefenseEncounter = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 0.5f, 0f),
                new CombatStatBlock(100f, 30f, 1f, 0f));
            CombatEncounterState highDefenseEncounter = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 0.5f, 50f),
                new CombatStatBlock(100f, 30f, 1f, 0f));
            CombatEncounterResolver resolver = new CombatEncounterResolver();

            resolver.TryAdvance(lowDefenseEncounter, 1f);
            resolver.TryAdvance(highDefenseEncounter, 1f);

            Assert.That(lowDefenseEncounter.PlayerEntity.CurrentHealth, Is.EqualTo(70f));
            Assert.That(highDefenseEncounter.PlayerEntity.CurrentHealth, Is.EqualTo(80f));
        }

        [Test]
        public void ShouldResolveCombatWhenOneSideIsDefeatedAndReportWinner()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 60f, 1f, 0f),
                new CombatStatBlock(50f, 5f, 0.5f, 0f));

            bool advanced = new CombatEncounterResolver().TryAdvance(encounterState, 1f);

            Assert.That(advanced, Is.True);
            Assert.That(encounterState.IsResolved, Is.True);
            Assert.That(encounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(encounterState.WinnerSide, Is.EqualTo(CombatSide.Player));
            Assert.That(encounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(encounterState.EnemyEntity.IsActive, Is.False);
            Assert.That(encounterState.EnemyEntity.CanAct, Is.False);
            Assert.That(encounterState.HasActiveEnemy, Is.False);
            Assert.That(encounterState.ActiveEnemyCount, Is.EqualTo(0));
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(0f));
        }

        [Test]
        public void ShouldStopResolvingAttacksAfterDefeat()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 60f, 1f, 0f),
                new CombatStatBlock(50f, 5f, 0.5f, 0f));
            CombatEncounterResolver resolver = new CombatEncounterResolver();

            resolver.TryAdvance(encounterState, 1f);
            float playerHealthAfterVictory = encounterState.PlayerEntity.CurrentHealth;
            bool advancedAfterResolution = resolver.TryAdvance(encounterState, 1f);

            Assert.That(advancedAfterResolution, Is.False);
            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.EqualTo(playerHealthAfterVictory));
            Assert.That(encounterState.ElapsedCombatSeconds, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void ShouldPreventDefeatedEnemyFromActingAfterVictory()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 60f, 1f, 0f),
                new CombatStatBlock(50f, 9f, 1f, 0f));
            CombatEncounterResolver resolver = new CombatEncounterResolver();

            resolver.TryAdvance(encounterState, 1f);
            float playerHealthAfterVictory = encounterState.PlayerEntity.CurrentHealth;

            encounterState.EnemyEntity.AdvanceAttackTimer(10f);

            Assert.That(encounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(encounterState.EnemyEntity.IsActive, Is.False);
            Assert.That(encounterState.EnemyEntity.CanAct, Is.False);
            Assert.That(encounterState.EnemyEntity.TimeUntilNextAttackSeconds, Is.EqualTo(0f));
            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.EqualTo(playerHealthAfterVictory));
        }

        [Test]
        public void ShouldRejectInvalidCombatSetup()
        {
            CombatShellContext invalidContext = new CombatShellContext(
                new NodeId("invalid_node"),
                new CombatEntityState(
                    new CombatEntityId("wrong_side_player"),
                    "Wrong Player",
                    CombatSide.Enemy,
                    new CombatStatBlock(100f, 10f, 1f, 0f)),
                new CombatEntityState(
                    new CombatEntityId("enemy_main"),
                    "Enemy Unit",
                    CombatSide.Enemy,
                    new CombatStatBlock(100f, 10f, 1f, 0f)));

            Assert.That(
                () => new CombatEncounterState(invalidContext),
                Throws.InvalidOperationException.With.Message.Contains("CombatSide.Player"));

            CombatEncounterState validEncounter = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 10f, 1f, 0f));

            Assert.That(
                () => new CombatEncounterResolver().TryAdvance(validEncounter, 0f),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ShouldRejectCombatAdvanceWhenResolvedTargetSelectionFindsNoActiveEnemy()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 10f, 1f, 0f));
            encounterState.EnemyEntity.ApplyDamage(encounterState.EnemyEntity.MaxHealth);

            Assert.That(
                () => new CombatEncounterResolver().TryAdvance(encounterState, 1f),
                Throws.InvalidOperationException.With.Message.Contains("No active target"));
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
