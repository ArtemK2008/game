using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class NextNodeUnlockServiceTests
    {
        [Test]
        public void ShouldPersistUnlockedUntrackedNodeWithoutFakeThreshold()
        {
            WorldNode sourceNode = new WorldNode(
                new NodeId("region_001_node_001"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Cleared);
            WorldNode targetNode = new WorldNode(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.ServiceOrProgression,
                NodeState.Locked);
            WorldGraph worldGraph = CreateGraph(
                new[] { sourceNode, targetNode },
                new WorldNodeConnection(sourceNode.NodeId, targetNode.NodeId));
            PersistentWorldState worldState = new PersistentWorldState();
            NextNodeUnlockService service = new NextNodeUnlockService();

            int unlockedNodeCount = service.UnlockConnectedNodesWhenSourceClears(worldGraph, worldState, sourceNode.NodeId);

            Assert.That(unlockedNodeCount, Is.EqualTo(1));
            Assert.That(worldState.TryGetNodeState(targetNode.NodeId, out PersistentNodeState unlockedNodeState), Is.True);
            Assert.That(unlockedNodeState.State, Is.EqualTo(NodeState.Available));
            Assert.That(unlockedNodeState.UnlockProgress, Is.EqualTo(0));
            Assert.That(unlockedNodeState.UnlockThreshold, Is.EqualTo(0));
        }

        [Test]
        public void ShouldUnlockLockedConnectedTrackedNodeWhenSourceNodeIsCleared()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
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
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
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

        private static WorldGraph CreateGraph(IEnumerable<WorldNode> nodes, params WorldNodeConnection[] connections)
        {
            List<WorldNode> nodeList = new List<WorldNode>(nodes);
            List<WorldRegion> regions = nodeList
                .GroupBy(node => node.RegionId)
                .Select((group, index) => new WorldRegion(
                    group.Key,
                    index,
                    group.First().NodeId,
                    group.Select(node => node.NodeId),
                    ResourceCategory.RegionMaterial,
                    $"band_{index}"))
                .ToList();

            return new WorldGraph(regions, nodeList, connections);
        }
    }
}
