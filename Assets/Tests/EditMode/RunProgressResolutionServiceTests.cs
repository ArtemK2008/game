using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunProgressResolutionServiceTests
    {
        [Test]
        public void ShouldGrantTrackedNodeProgressWithoutUnlockBeforeThreshold()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                CreateCombatNodeState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(CreateCombatNodeState(), CombatEncounterOutcome.PlayerVictory),
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
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            WorldGraph worldGraph = factory.CreateWorldGraph();
            RunProgressResolutionService service = new RunProgressResolutionService();

            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
            pushNodeState.ApplyUnlockProgress(1);

            RunProgressResolution resolution = service.Resolve(
                CreatePushCombatNodeState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(CreatePushCombatNodeState(), CombatEncounterOutcome.PlayerVictory),
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
                CreateBossCombatNodeState(),
                RunResolutionState.Failed,
                CreateResolvedEncounter(CreateBossCombatNodeState(), CombatEncounterOutcome.EnemyVictory),
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
        public void ShouldReturnUntrackedProgressStateWhenPersistentWorldStateIsMissing()
        {
            RunProgressResolutionService service = new RunProgressResolutionService();

            RunProgressResolution resolution = service.Resolve(
                CreateCombatNodeState(),
                RunResolutionState.Succeeded,
                CreateResolvedEncounter(CreateCombatNodeState(), CombatEncounterOutcome.PlayerVictory),
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

        private static NodePlaceholderState CreateCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        private static NodePlaceholderState CreateBossCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"));
        }

        private static NodePlaceholderState CreatePushCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"));
        }
    }
}
