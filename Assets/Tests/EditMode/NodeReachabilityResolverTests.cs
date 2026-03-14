using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class NodeReachabilityResolverTests
    {
        [Test]
        public void ShouldReachAvailableConnectedNodesAsForwardDestinations()
        {
            WorldNode currentNode = CreateNode("region_001_node_001", "region_001", NodeState.Cleared);
            WorldNode availableForwardNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldNode lockedForwardNode = CreateNode("region_001_node_003", "region_001", NodeState.Locked);
            WorldGraph worldGraph = CreateGraph(
                new[] { currentNode, availableForwardNode, lockedForwardNode },
                new WorldNodeConnection(currentNode.NodeId, availableForwardNode.NodeId),
                new WorldNodeConnection(currentNode.NodeId, lockedForwardNode.NodeId));
            PersistentWorldState worldState = new PersistentWorldState();
            NodeReachabilityResolver resolver = new NodeReachabilityResolver();

            worldState.SetCurrentNode(currentNode.NodeId);

            IReadOnlyList<NodeId> reachableNodeIds = resolver.GetForwardReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEqual(new[] { availableForwardNode.NodeId }, reachableNodeIds);
        }

        [Test]
        public void ShouldNotReachLockedNodesEvenWhenStoredAsBacktrackTargets()
        {
            WorldNode currentNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode lockedPriorNode = CreateNode("region_001_node_002", "region_001", NodeState.Locked);
            WorldGraph worldGraph = CreateGraph(new[] { currentNode, lockedPriorNode });
            PersistentWorldState worldState = new PersistentWorldState();
            NodeReachabilityResolver resolver = new NodeReachabilityResolver();

            worldState.SetCurrentNode(currentNode.NodeId);
            worldState.ReplaceReachableNodes(new[] { lockedPriorNode.NodeId });

            Assert.That(resolver.GetBacktrackReachableNodes(worldGraph, worldState), Is.Empty);
            Assert.That(resolver.GetReachableNodes(worldGraph, worldState), Is.Empty);
        }

        [Test]
        public void ShouldAllowBacktrackingToPreviouslyReachableNodes()
        {
            WorldNode priorNode = CreateNode("region_001_node_001", "region_001", NodeState.Cleared);
            WorldNode branchNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldNode currentNode = CreateNode("region_002_node_001", "region_002", NodeState.Available);
            WorldGraph worldGraph = CreateGraph(new[] { priorNode, branchNode, currentNode });
            PersistentWorldState worldState = new PersistentWorldState();
            NodeReachabilityResolver resolver = new NodeReachabilityResolver();

            worldState.SetCurrentNode(currentNode.NodeId);
            worldState.SetLastSafeNode(priorNode.NodeId);
            worldState.ReplaceReachableNodes(new[] { priorNode.NodeId, branchNode.NodeId });

            IReadOnlyList<NodeId> backtrackNodeIds = resolver.GetBacktrackReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEquivalent(new[] { priorNode.NodeId, branchNode.NodeId }, backtrackNodeIds);
        }

        [Test]
        public void ShouldNotReachDisconnectedNodesWithoutBacktrackContext()
        {
            WorldNode currentNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode disconnectedNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldGraph worldGraph = CreateGraph(new[] { currentNode, disconnectedNode });
            PersistentWorldState worldState = new PersistentWorldState();
            NodeReachabilityResolver resolver = new NodeReachabilityResolver();

            worldState.SetCurrentNode(currentNode.NodeId);

            Assert.That(resolver.GetReachableNodes(worldGraph, worldState), Is.Empty);
        }

        [Test]
        public void ShouldKeepForwardAndBacktrackDestinationsDistinct()
        {
            WorldNode priorNode = CreateNode("region_001_node_001", "region_001", NodeState.Cleared);
            WorldNode currentNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldNode nextNode = CreateNode("region_002_node_001", "region_002", NodeState.Available);
            WorldGraph worldGraph = CreateGraph(
                new[] { priorNode, currentNode, nextNode },
                new WorldNodeConnection(currentNode.NodeId, nextNode.NodeId));
            PersistentWorldState worldState = new PersistentWorldState();
            NodeReachabilityResolver resolver = new NodeReachabilityResolver();

            worldState.SetCurrentNode(currentNode.NodeId);
            worldState.SetLastSafeNode(priorNode.NodeId);
            worldState.ReplaceReachableNodes(new[] { priorNode.NodeId });

            IReadOnlyList<NodeId> forwardNodeIds = resolver.GetForwardReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();
            IReadOnlyList<NodeId> backtrackNodeIds = resolver.GetBacktrackReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();
            IReadOnlyList<NodeId> reachableNodeIds = resolver.GetReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEqual(new[] { nextNode.NodeId }, forwardNodeIds);
            CollectionAssert.AreEqual(new[] { priorNode.NodeId }, backtrackNodeIds);
            CollectionAssert.AreEqual(new[] { nextNode.NodeId, priorNode.NodeId }, reachableNodeIds);
        }

        [Test]
        public void ShouldTreatPersistentlyUnlockedNodeAsForwardReachable()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            NodeReachabilityResolver resolver = new NodeReachabilityResolver();
            NodeId unlockedGateNodeId = new NodeId("region_001_node_003");

            worldState.GetOrAddNodeState(unlockedGateNodeId, 3, NodeState.Locked).MarkAvailable();

            IReadOnlyList<NodeId> forwardNodeIds = resolver.GetForwardReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEquivalent(
                new[]
                {
                    new NodeId("region_001_node_004"),
                    new NodeId("region_002_node_001"),
                    unlockedGateNodeId,
                },
                forwardNodeIds);
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
