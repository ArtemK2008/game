using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class NextNodeUnlockServiceTests
    {
        [Test]
        public void ShouldRejectMissingWorldGraph()
        {
            NextNodeUnlockService service = new NextNodeUnlockService();

            Assert.That(
                () => service.UnlockConnectedNodesWhenSourceClears(null, new PersistentWorldState(), new NodeId("node_001")),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldGraph"));
        }

        [Test]
        public void ShouldRejectMissingWorldState()
        {
            NextNodeUnlockService service = new NextNodeUnlockService();

            Assert.That(
                () => service.UnlockConnectedNodesWhenSourceClears(BootstrapWorldTestData.CreateWorldGraph(), null, new NodeId("node_001")),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldState"));
        }

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

        [Test]
        public void ShouldNotUnlockConnectedNodesWhenSourceIsNotCleared()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            NextNodeUnlockService service = new NextNodeUnlockService();

            int unlockedNodeCount = service.UnlockConnectedNodesWhenSourceClears(
                worldGraph,
                worldState,
                new NodeId("region_001_node_002"));

            Assert.That(unlockedNodeCount, Is.EqualTo(0));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
            Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Locked));
        }

        [Test]
        public void ShouldUnlockConnectedNodesWhenSourceIsMastered()
        {
            NodeId sourceNodeId = new NodeId("source_node");
            NodeId targetNodeId = new NodeId("target_node");
            WorldNode sourceNode = new WorldNode(sourceNodeId, new RegionId("region_001"), NodeType.Combat, NodeState.Cleared);
            WorldNode targetNode = new WorldNode(targetNodeId, new RegionId("region_001"), NodeType.BossOrGate, NodeState.Locked);
            WorldGraph worldGraph = CreateGraph(
                new[] { sourceNode, targetNode },
                new WorldNodeConnection(sourceNodeId, targetNodeId));
            PersistentWorldState worldState = PersistentStateTestData.CreateWorldState(
                sourceNodeId,
                new[] { sourceNodeId },
                PersistentStateTestData.CreateNodeState(sourceNodeId, 3, NodeState.Mastered, 3));
            NextNodeUnlockService service = new NextNodeUnlockService();

            int unlockedNodeCount = service.UnlockConnectedNodesWhenSourceClears(worldGraph, worldState, sourceNodeId);

            Assert.That(unlockedNodeCount, Is.EqualTo(1));
            Assert.That(worldState.TryGetNodeState(targetNodeId, out PersistentNodeState targetNodeState), Is.True);
            Assert.That(targetNodeState.State, Is.EqualTo(NodeState.Available));
            Assert.That(targetNodeState.UnlockThreshold, Is.EqualTo(3));
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
