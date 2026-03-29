using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunHudPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldRejectMissingRunHudState()
        {
            Assert.That(
                () => RunHudTextBuilder.BuildSummaryText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("runHudState"));
        }

        [Test]
        public void Resolve_ShouldBuildReadableTrackedCombatHudStateFromRuntimeEncounter()
        {
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            NodePlaceholderState placeholderState = NodePlaceholderTestData.CreatePushCombatPlaceholderState();
            CombatEncounterState encounterState = CreateEncounterState(placeholderState);
            RunHudStateResolver resolver = new RunHudStateResolver();

            RunHudState runHudState = resolver.Resolve(
                placeholderState,
                RunLifecycleState.RunActive,
                encounterState,
                worldState);
            string summaryText = RunHudTextBuilder.BuildSummaryText(runHudState);

            Assert.That(runHudState.LocationDisplayName, Is.EqualTo("Verdant Frontier"));
            Assert.That(runHudState.NodeDisplayName, Is.EqualTo("Raider Trail"));
            Assert.That(runHudState.NodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(runHudState.RunStateDisplayName, Is.EqualTo("Auto-battle active"));
            Assert.That(runHudState.OutcomeDisplayName, Is.EqualTo("Ongoing"));
            Assert.That(runHudState.PlayerCurrentHealth, Is.EqualTo(120f));
            Assert.That(runHudState.PlayerMaxHealth, Is.EqualTo(120f));
            Assert.That(runHudState.EnemyCurrentHealth, Is.EqualTo(105f));
            Assert.That(runHudState.EnemyMaxHealth, Is.EqualTo(105f));
            Assert.That(runHudState.HasTrackedProgressContext, Is.True);
            Assert.That(runHudState.CurrentProgress, Is.EqualTo(1));
            Assert.That(runHudState.ProgressThreshold, Is.EqualTo(3));
            Assert.That(runHudState.ProgressGoalDisplayName, Is.EqualTo("node clear"));
            Assert.That(RunHudTextBuilder.BuildContextTitle(runHudState), Is.EqualTo("Verdant Frontier | Raider Trail"));
            Assert.That(summaryText, Does.Contain("Status: Auto-battle active | Outcome: Ongoing | Time: 0s"));
            Assert.That(summaryText, Does.Contain("Health: Vanguard 120 / 120 | Bulwark Raider 105 / 105"));
            Assert.That(summaryText, Does.Contain("Objective: 1 / 3 toward node clear"));
        }

        [Test]
        public void Resolve_ShouldDescribeGateClearProgressForBossRunHud()
        {
            NodePlaceholderState placeholderState = NodePlaceholderTestData.CreateCavernGateBossPlaceholderState();
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(placeholderState.NodeId);
            worldState.SetLastSafeNode(placeholderState.OriginNodeId);
            worldState.GetOrAddNodeState(placeholderState.NodeId, 3, NodeState.Available, 0);
            CombatEncounterState encounterState = CreateEncounterState(placeholderState);
            encounterState.Resolve(CombatEncounterOutcome.PlayerVictory);
            RunHudStateResolver resolver = new RunHudStateResolver();

            RunHudState runHudState = resolver.Resolve(
                placeholderState,
                RunLifecycleState.RunResolved,
                encounterState,
                worldState);
            string summaryText = RunHudTextBuilder.BuildSummaryText(runHudState);

            Assert.That(runHudState.LocationDisplayName, Is.EqualTo("Echo Caverns"));
            Assert.That(runHudState.NodeDisplayName, Is.EqualTo("Cavern Gate"));
            Assert.That(runHudState.RunStateDisplayName, Is.EqualTo("Auto-battle resolved"));
            Assert.That(runHudState.OutcomeDisplayName, Is.EqualTo("PlayerVictory"));
            Assert.That(runHudState.ProgressGoalDisplayName, Is.EqualTo("gate clear"));
            Assert.That(runHudState.BossEncounterDisplayName, Is.EqualTo("Gate boss"));
            Assert.That(runHudState.BossStakeSummary, Is.EqualTo("Gate clear, Boss rewards"));
            Assert.That(RunHudTextBuilder.BuildContextTitle(runHudState), Is.EqualTo("Boss encounter | Echo Caverns | Cavern Gate"));
            Assert.That(summaryText, Does.Contain("Status: Auto-battle resolved | Outcome: PlayerVictory | Time: 0s"));
            Assert.That(summaryText, Does.Contain("Health: Vanguard 120 / 120 | Gate Boss 180 / 180"));
            Assert.That(summaryText, Does.Contain("Boss: Gate boss | Stakes: Gate clear, Boss rewards"));
            Assert.That(summaryText, Does.Contain("Objective: 0 / 3 toward gate clear"));
        }

        private static CombatEncounterState CreateEncounterState(NodePlaceholderState placeholderState)
        {
            CombatShellContext combatContext = new CombatShellContextFactory().Create(
                placeholderState,
                null,
                null,
                default);
            return new CombatEncounterState(combatContext);
        }
    }
}
