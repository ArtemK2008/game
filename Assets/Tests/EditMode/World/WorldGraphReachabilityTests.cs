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
    public sealed class WorldGraphReachabilityTests
    {
        [Test]
        public void ShouldReachConnectedAvailableNodes()
        {
            WorldNode forestEntryNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode forestMidNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldNode caveEntryNode = CreateNode("region_002_node_001", "region_002", NodeState.Available);

            WorldGraph worldGraph = CreateGraph(
                new[] { forestEntryNode, forestMidNode, caveEntryNode },
                new WorldNodeConnection(forestEntryNode.NodeId, forestMidNode.NodeId),
                new WorldNodeConnection(forestMidNode.NodeId, caveEntryNode.NodeId));

            IReadOnlyList<NodeId> reachableNodeIds = worldGraph.GetReachableNodes(forestEntryNode.NodeId)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEquivalent(
                new[]
                {
                    forestMidNode.NodeId,
                    caveEntryNode.NodeId,
                },
                reachableNodeIds);
            Assert.That(worldGraph.CanReach(forestEntryNode.NodeId, caveEntryNode.NodeId), Is.True);
        }

        [Test]
        public void ShouldNotReachLockedNodes()
        {
            WorldNode currentNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode lockedNode = CreateNode("region_001_node_002", "region_001", NodeState.Locked);

            WorldGraph worldGraph = CreateGraph(
                new[] { currentNode, lockedNode },
                new WorldNodeConnection(currentNode.NodeId, lockedNode.NodeId));

            Assert.That(worldGraph.GetReachableNodes(currentNode.NodeId), Is.Empty);
            Assert.That(worldGraph.CanReach(currentNode.NodeId, lockedNode.NodeId), Is.False);
        }

        [Test]
        public void ShouldRespectExplicitConnections()
        {
            WorldNode leftNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode rightNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);

            WorldGraph worldGraph = CreateGraph(
                new[] { leftNode, rightNode },
                new WorldNodeConnection(rightNode.NodeId, leftNode.NodeId));

            Assert.That(worldGraph.CanReach(leftNode.NodeId, rightNode.NodeId), Is.False);
            Assert.That(worldGraph.CanReach(rightNode.NodeId, leftNode.NodeId), Is.True);
        }

        [Test]
        public void ShouldNotReachDisconnectedNodes()
        {
            WorldNode startNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode connectedNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldNode disconnectedNode = CreateNode("region_001_node_003", "region_001", NodeState.Available);

            WorldGraph worldGraph = CreateGraph(
                new[] { startNode, connectedNode, disconnectedNode },
                new WorldNodeConnection(startNode.NodeId, connectedNode.NodeId));

            Assert.That(worldGraph.CanReach(startNode.NodeId, connectedNode.NodeId), Is.True);
            Assert.That(worldGraph.CanReach(startNode.NodeId, disconnectedNode.NodeId), Is.False);
        }

        [Test]
        public void ShouldReturnEmptyReachabilityWhenStartNodeIsLocked()
        {
            WorldNode lockedStartNode = CreateNode("region_001_node_001", "region_001", NodeState.Locked);
            WorldNode availableTargetNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);

            WorldGraph worldGraph = CreateGraph(
                new[] { lockedStartNode, availableTargetNode },
                new WorldNodeConnection(lockedStartNode.NodeId, availableTargetNode.NodeId));

            Assert.That(worldGraph.GetReachableNodes(lockedStartNode.NodeId), Is.Empty);
            Assert.That(worldGraph.CanReach(lockedStartNode.NodeId, lockedStartNode.NodeId), Is.False);
            Assert.That(worldGraph.CanReach(lockedStartNode.NodeId, availableTargetNode.NodeId), Is.False);
        }

        [Test]
        public void ShouldHandleCyclesWithoutRepeatingNodes()
        {
            WorldNode startNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode middleNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldNode endNode = CreateNode("region_001_node_003", "region_001", NodeState.Available);

            WorldGraph worldGraph = CreateGraph(
                new[] { startNode, middleNode, endNode },
                new WorldNodeConnection(startNode.NodeId, middleNode.NodeId),
                new WorldNodeConnection(middleNode.NodeId, endNode.NodeId),
                new WorldNodeConnection(endNode.NodeId, startNode.NodeId));

            IReadOnlyList<NodeId> reachableNodeIds = worldGraph.GetReachableNodes(startNode.NodeId)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEqual(
                new[]
                {
                    middleNode.NodeId,
                    endNode.NodeId,
                },
                reachableNodeIds);
        }

        [Test]
        public void ShouldThrowWhenGettingMissingNode()
        {
            WorldNode startNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldGraph worldGraph = CreateGraph(new[] { startNode });

            KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(
                () => worldGraph.GetNode(new NodeId("region_001_node_999")));

            Assert.That(exception.Message, Does.Contain("region_001_node_999"));
        }

        [Test]
        public void ShouldThrowWhenGettingOutboundConnectionsForMissingNode()
        {
            WorldNode startNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldGraph worldGraph = CreateGraph(new[] { startNode });

            KeyNotFoundException exception = Assert.Throws<KeyNotFoundException>(
                () => worldGraph.GetOutboundConnections(new NodeId("region_001_node_999")));

            Assert.That(exception.Message, Does.Contain("region_001_node_999"));
        }

        private static WorldNode CreateNode(string nodeIdValue, string regionIdValue, NodeState state)
        {
            return new WorldNode(
                new NodeId(nodeIdValue),
                new RegionId(regionIdValue),
                NodeType.Combat,
                state);
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
