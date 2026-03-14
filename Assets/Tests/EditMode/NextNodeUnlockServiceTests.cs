using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class NextNodeUnlockServiceTests
    {
        [Test]
        public void ShouldUnlockLockedConnectedNodeWhenSourceNodeIsCleared()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            NextNodeUnlockService service = new NextNodeUnlockService();
            NodeId sourceNodeId = new NodeId("region_001_node_002");
            NodeId unlockedNodeId = new NodeId("region_001_node_003");

            Assert.That(worldState.TryGetNodeState(sourceNodeId, out PersistentNodeState sourceNodeState), Is.True);
            sourceNodeState.ApplyUnlockProgress(1);
            sourceNodeState.ApplyUnlockProgress(1);
            Assert.That(sourceNodeState.State, Is.EqualTo(NodeState.Cleared));

            int unlockedNodeCount = service.UnlockConnectedNodesWhenSourceClears(worldGraph, worldState, sourceNodeId);

            Assert.That(unlockedNodeCount, Is.EqualTo(1));
            Assert.That(worldState.TryGetNodeState(unlockedNodeId, out PersistentNodeState unlockedNodeState), Is.True);
            Assert.That(unlockedNodeState.State, Is.EqualTo(NodeState.Available));
            Assert.That(unlockedNodeState.UnlockProgress, Is.EqualTo(0));
            Assert.That(unlockedNodeState.UnlockThreshold, Is.EqualTo(3));
        }

        [Test]
        public void ShouldNotDuplicateUnlocksWhenConnectedNodeIsAlreadyAvailable()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            NextNodeUnlockService service = new NextNodeUnlockService();
            NodeId sourceNodeId = new NodeId("region_001_node_002");
            NodeId unlockedNodeId = new NodeId("region_001_node_003");

            Assert.That(worldState.TryGetNodeState(sourceNodeId, out PersistentNodeState sourceNodeState), Is.True);
            sourceNodeState.ApplyUnlockProgress(1);
            sourceNodeState.ApplyUnlockProgress(1);

            int firstUnlockCount = service.UnlockConnectedNodesWhenSourceClears(worldGraph, worldState, sourceNodeId);
            int secondUnlockCount = service.UnlockConnectedNodesWhenSourceClears(worldGraph, worldState, sourceNodeId);

            Assert.That(firstUnlockCount, Is.EqualTo(1));
            Assert.That(secondUnlockCount, Is.EqualTo(0));
            Assert.That(worldState.TryGetNodeState(unlockedNodeId, out PersistentNodeState unlockedNodeState), Is.True);
            Assert.That(unlockedNodeState.State, Is.EqualTo(NodeState.Available));
        }
    }
}
