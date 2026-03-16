using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunProgressResolutionServiceTests
    {
        [Test]
        public void ShouldRejectMissingNodeContext()
        {
            RunProgressResolutionService service = new RunProgressResolutionService();

            Assert.That(
                () => service.Resolve(
                    null,
                    RunResolutionState.Succeeded,
                    combatEncounterState: null,
                    persistentWorldState: new PersistentWorldState(),
                    worldGraph: null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("nodeContext"));
        }

        [Test]
        public void ShouldGrantTrackedNodeProgressWithoutUnlockBeforeThreshold()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(NodePlaceholderTestData.CreateCombatPlaceholderState(), CombatEncounterOutcome.PlayerVictory),
                worldState,
                worldGraph: null);

            Assert.That(resolution.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(resolution.NodeProgressUpdate.IsTracked, Is.True);
            Assert.That(resolution.NodeProgressUpdate.CurrentProgress, Is.EqualTo(1));
            Assert.That(resolution.NodeProgressUpdate.ProgressThreshold, Is.EqualTo(3));
            Assert.That(resolution.NodeProgressUpdate.DidReachClearThreshold, Is.False);
            Assert.That(resolution.DidUnlockRoute, Is.False);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.State, Is.EqualTo(NodeState.InProgress));
        }

        [Test]
        public void ShouldUnlockConnectedNodeWhenTrackedNodeReachesThreshold()
        {
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            RunProgressResolutionService service = new RunProgressResolutionService();

            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
            pushNodeState.ApplyUnlockProgress(1);

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(NodePlaceholderTestData.CreatePushCombatPlaceholderState(), CombatEncounterOutcome.PlayerVictory),
                worldState,
                worldGraph);

            Assert.That(resolution.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(resolution.NodeProgressUpdate.DidReachClearThreshold, Is.True);
            Assert.That(resolution.DidUnlockRoute, Is.True);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out pushNodeState), Is.True);
            Assert.That(pushNodeState.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
            Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldKeepFailedTrackedCombatAtZeroProgressAndWithoutUnlock()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                RunResolutionState.Failed,
                CreateResolvedEncounter(NodePlaceholderTestData.CreateBossCombatPlaceholderState(), CombatEncounterOutcome.EnemyVictory),
                worldState,
                worldGraph: null);

            Assert.That(resolution.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(resolution.NodeProgressUpdate.IsTracked, Is.True);
            Assert.That(resolution.NodeProgressUpdate.CurrentProgress, Is.EqualTo(0));
            Assert.That(resolution.NodeProgressUpdate.ProgressThreshold, Is.EqualTo(3));
            Assert.That(resolution.NodeProgressUpdate.DidReachClearThreshold, Is.False);
            Assert.That(resolution.DidUnlockRoute, Is.False);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_005"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldReturnUntrackedResolutionForSuccessfulServiceNode()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(NodePlaceholderTestData.CreateCombatPlaceholderState(), CombatEncounterOutcome.PlayerVictory),
                worldState,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(resolution.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(resolution.NodeProgressUpdate.IsTracked, Is.False);
            Assert.That(resolution.NodeProgressUpdate.CurrentProgress, Is.EqualTo(0));
            Assert.That(resolution.DidUnlockRoute, Is.False);
            Assert.That(worldState.TryGetNodeState(NodePlaceholderTestData.CreateServicePlaceholderState().NodeId, out _), Is.False);
        }

        [Test]
        public void ShouldKeepTrackedCombatAtZeroProgressWhenEncounterStateIsMissing()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                null,
                worldState,
                worldGraph: null);

            Assert.That(resolution.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(resolution.NodeProgressUpdate.IsTracked, Is.True);
            Assert.That(resolution.NodeProgressUpdate.CurrentProgress, Is.EqualTo(0));
            Assert.That(resolution.NodeProgressUpdate.DidReachClearThreshold, Is.False);
            Assert.That(resolution.DidUnlockRoute, Is.False);
        }

        [Test]
        public void ShouldNotUnlockRouteWhenThresholdIsReachedWithoutWorldGraph()
        {
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            RunProgressResolutionService service = new RunProgressResolutionService();

            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
            pushNodeState.ApplyUnlockProgress(1);

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(NodePlaceholderTestData.CreatePushCombatPlaceholderState(), CombatEncounterOutcome.PlayerVictory),
                worldState,
                worldGraph: null);

            Assert.That(resolution.NodeProgressUpdate.DidReachClearThreshold, Is.True);
            Assert.That(resolution.DidUnlockRoute, Is.False);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
            Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Locked));
        }

        [Test]
        public void ShouldReturnUntrackedProgressStateWhenPersistentWorldStateIsMissing()
        {
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(NodePlaceholderTestData.CreateCombatPlaceholderState(), CombatEncounterOutcome.PlayerVictory),
                persistentWorldState: null,
                worldGraph: null);

            Assert.That(resolution.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(resolution.NodeProgressUpdate.IsTracked, Is.False);
            Assert.That(resolution.NodeProgressUpdate.CurrentProgress, Is.EqualTo(0));
            Assert.That(resolution.NodeProgressUpdate.ProgressThreshold, Is.EqualTo(0));
            Assert.That(resolution.DidUnlockRoute, Is.False);
        }

        private static CombatEncounterState CreateResolvedEncounter(
            NodePlaceholderState nodeState,
            CombatEncounterOutcome outcome)
        {
            CombatShellContextFactory contextFactory = new CombatShellContextFactory();
            CombatEncounterState encounterState = new CombatEncounterState(contextFactory.Create(nodeState));

            switch (outcome)
            {
                case CombatEncounterOutcome.PlayerVictory:
                    encounterState.EnemyEntity.ApplyDamage(encounterState.EnemyEntity.MaxHealth);
                    break;
                case CombatEncounterOutcome.EnemyVictory:
                    encounterState.PlayerEntity.ApplyDamage(encounterState.PlayerEntity.MaxHealth);
                    break;
                default:
                    Assert.Fail("Test helper only supports resolved outcomes.");
                    break;
            }

            encounterState.Resolve(outcome);
            return encounterState;
        }

    }
}
