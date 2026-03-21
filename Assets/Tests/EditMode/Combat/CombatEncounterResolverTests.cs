using System;
using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;

namespace Survivalon.Tests.EditMode.Combat
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

            encounterState.EnemyEntity.AdvanceBaselineAttackTimer(10f);

            Assert.That(encounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(encounterState.EnemyEntity.IsActive, Is.False);
            Assert.That(encounterState.EnemyEntity.CanAct, Is.False);
            Assert.That(encounterState.EnemyEntity.TimeUntilNextBaselineAttackSeconds, Is.EqualTo(0f));
            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.EqualTo(playerHealthAfterVictory));
        }

        [Test]
        public void ShouldExecuteDueBaselineAttackThroughSkillExecutor()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 7f, 0.5f, 0f));
            SpyCombatSkillExecutor skillExecutor = new SpyCombatSkillExecutor();
            CombatEncounterResolver resolver = new CombatEncounterResolver(combatSkillExecutor: skillExecutor);

            bool advanced = resolver.TryAdvance(encounterState, 1f);

            Assert.That(advanced, Is.True);
            Assert.That(skillExecutor.CallCount, Is.EqualTo(1));
            Assert.That(skillExecutor.LastExecutionRequest, Is.Not.Null);
            Assert.That(
                skillExecutor.LastExecutionRequest.SkillDefinition,
                Is.SameAs(encounterState.PlayerEntity.BaselineAttackSkill));
            Assert.That(skillExecutor.LastExecutionRequest.SourceEntity, Is.SameAs(encounterState.PlayerEntity));
            Assert.That(skillExecutor.LastExecutionRequest.TargetEntity, Is.SameAs(encounterState.EnemyEntity));
            Assert.That(encounterState.PlayerEntity.TimeUntilNextBaselineAttackSeconds, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void ShouldExecuteDueTriggeredActiveSkillThroughSkillExecutor()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 0.1f, 0f),
                new CombatStatBlock(100f, 7f, 0.1f, 0f),
                CombatSkillCatalog.BurstStrike);
            SpyCombatSkillExecutor skillExecutor = new SpyCombatSkillExecutor();
            CombatEncounterResolver resolver = new CombatEncounterResolver(combatSkillExecutor: skillExecutor);

            bool advanced = resolver.TryAdvance(encounterState, 2.5f);

            Assert.That(advanced, Is.True);
            Assert.That(skillExecutor.CallCount, Is.EqualTo(1));
            Assert.That(skillExecutor.LastExecutionRequest, Is.Not.Null);
            Assert.That(
                skillExecutor.LastExecutionRequest.SkillDefinition,
                Is.SameAs(encounterState.PlayerEntity.TriggeredActiveSkill));
            Assert.That(skillExecutor.LastExecutionRequest.SourceEntity, Is.SameAs(encounterState.PlayerEntity));
            Assert.That(skillExecutor.LastExecutionRequest.TargetEntity, Is.SameAs(encounterState.EnemyEntity));
            Assert.That(encounterState.PlayerEntity.TimeUntilTriggeredActiveSkillSeconds, Is.EqualTo(2.5f).Within(0.001f));
        }

        [Test]
        public void ShouldExecuteRunTimeUpgradedTriggeredActiveSkillThroughSkillExecutor()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 0.1f, 0f),
                new CombatStatBlock(100f, 7f, 0.1f, 0f),
                CombatSkillCatalog.BurstStrike,
                CombatRunTimeSkillUpgradeCatalog.BurstTempo);
            SpyCombatSkillExecutor skillExecutor = new SpyCombatSkillExecutor();
            CombatEncounterResolver resolver = new CombatEncounterResolver(combatSkillExecutor: skillExecutor);

            bool advanced = resolver.TryAdvance(encounterState, 1.75f);

            Assert.That(advanced, Is.True);
            Assert.That(skillExecutor.CallCount, Is.EqualTo(1));
            Assert.That(skillExecutor.LastExecutionRequest, Is.Not.Null);
            Assert.That(
                skillExecutor.LastExecutionRequest.SkillDefinition,
                Is.SameAs(encounterState.PlayerEntity.TriggeredActiveSkill));
            Assert.That(
                skillExecutor.LastExecutionRequest.RunTimeSkillUpgrade,
                Is.SameAs(CombatRunTimeSkillUpgradeCatalog.BurstTempo));
            Assert.That(encounterState.PlayerEntity.TimeUntilTriggeredActiveSkillSeconds, Is.EqualTo(1.75f).Within(0.001f));
        }

        [Test]
        public void ShouldApplyPassiveDirectDamageModifierDuringAutomatedBaselineAttackFlow()
        {
            CombatEncounterState encounterState = CreateEncounterState(
                new CombatStatBlock(100f, 10f, 1f, 0f),
                new CombatStatBlock(100f, 7f, 0.25f, 0f),
                null,
                playerPassiveSkills: new[] { CombatSkillCatalog.RelentlessAssault });

            bool advanced = new CombatEncounterResolver().TryAdvance(encounterState, 1f);

            Assert.That(advanced, Is.True);
            Assert.That(encounterState.PlayerEntity.CurrentHealth, Is.EqualTo(100f));
            Assert.That(encounterState.EnemyEntity.CurrentHealth, Is.EqualTo(88f));
        }

        [Test]
        public void ShouldAutoTriggerPeriodicActiveSkillAndChangeCombatOutcomeMeaningfully()
        {
            CombatEncounterState activeEncounterState = CreateEncounterState(
                new CombatStatBlock(110f, 18f, 1.35f, 8f),
                new CombatStatBlock(75f, 8f, 0.9f, 4f),
                CombatSkillCatalog.BurstStrike,
                playerPassiveSkills: new[] { CombatSkillCatalog.RelentlessAssault });
            CombatEncounterState controlEncounterState = CreateEncounterState(
                new CombatStatBlock(110f, 18f, 1.35f, 8f),
                new CombatStatBlock(75f, 8f, 0.9f, 4f),
                null,
                playerPassiveSkills: new[] { CombatSkillCatalog.RelentlessAssault });

            bool activeAdvanced = new CombatEncounterResolver().TryAdvance(activeEncounterState, 2.5f);
            bool controlAdvanced = new CombatEncounterResolver().TryAdvance(controlEncounterState, 2.5f);

            Assert.That(activeAdvanced, Is.True);
            Assert.That(controlAdvanced, Is.True);
            Assert.That(activeEncounterState.IsResolved, Is.True);
            Assert.That(activeEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(activeEncounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(controlEncounterState.IsResolved, Is.False);
            Assert.That(controlEncounterState.EnemyEntity.IsAlive, Is.True);
            Assert.That(controlEncounterState.EnemyEntity.CurrentHealth, Is.GreaterThan(0f));
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
            CombatStatBlock enemyStats,
            CombatSkillDefinition playerTriggeredActiveSkill = null,
            CombatRunTimeSkillUpgradeOption playerTriggeredActiveSkillUpgrade = null,
            params CombatSkillDefinition[] playerPassiveSkills)
        {
            CombatShellContext combatContext = new CombatShellContext(
                new NodeId("region_001_node_004"),
                new CombatEntityState(
                    new CombatEntityId("player_main"),
                    "Player Unit",
                    CombatSide.Player,
                    playerStats,
                    triggeredActiveSkill: playerTriggeredActiveSkill,
                    triggeredActiveSkillUpgrade: playerTriggeredActiveSkillUpgrade,
                    passiveSkills: playerPassiveSkills),
                new CombatEntityState(
                    new CombatEntityId("enemy_main"),
                    "Enemy Unit",
                    CombatSide.Enemy,
                    enemyStats));

            return new CombatEncounterState(combatContext);
        }

        private sealed class SpyCombatSkillExecutor : ICombatSkillExecutor
        {
            public int CallCount { get; private set; }

            public CombatSkillExecutionRequest LastExecutionRequest { get; private set; }

            public void Execute(CombatSkillExecutionRequest executionRequest, CombatEncounterState encounterState)
            {
                CallCount++;
                LastExecutionRequest = executionRequest;
            }
        }
    }
}

