using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldMapWorldStateSummaryResolverTests
    {
        [Test]
        public void ShouldResolveReadableCurrentContextAndPathSummaryForBootstrapWorld()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeAccessResolver nodeAccessResolver = new WorldNodeAccessResolver();
            WorldMapWorldStateSummaryResolver resolver = new WorldMapWorldStateSummaryResolver();

            WorldMapWorldStateSummary summary = resolver.Resolve(
                worldGraph,
                worldState,
                worldState.CurrentNodeId,
                ToNodeIdSet(nodeAccessResolver.GetEnterableNodes(worldGraph, worldState)),
                ToNodeIdSet(nodeAccessResolver.GetPathEnterableNodes(worldGraph, worldState)),
                ToNodeIdSet(nodeAccessResolver.GetForwardEnterableNodes(worldGraph, worldState)));

            Assert.That(summary.CurrentLocationDisplayName, Is.EqualTo("Verdant Frontier"));
            Assert.That(summary.CurrentNode.NodeId, Is.EqualTo(BootstrapWorldScenario.ForestPushNodeId));
            Assert.That(summary.CurrentNode.DisplayName, Is.EqualTo("Raider Trail"));
            Assert.That(summary.CurrentNodeState, Is.EqualTo(NodeState.InProgress));
            Assert.That(summary.SelectableDestinationCount, Is.EqualTo(3));
            Assert.That(ExtractNodeIds(summary.ForwardRouteNodes), Is.EqualTo(new[]
            {
                BootstrapWorldScenario.CavernServiceNodeId,
                BootstrapWorldScenario.ForestFarmNodeId,
            }));
            Assert.That(ExtractDisplayNames(summary.ForwardRouteNodes), Is.EqualTo(new[]
            {
                "Cavern Service Hub",
                "Forest Farm",
            }));
            Assert.That(ExtractNodeIds(summary.BacktrackRouteNodes), Is.EqualTo(new[]
            {
                BootstrapWorldScenario.ForestEntryNodeId,
            }));
            Assert.That(summary.ReplayableFarmNodes, Is.Empty);
            Assert.That(ExtractNodeIds(summary.BlockedLinkedNodes), Is.EqualTo(new[]
            {
                BootstrapWorldScenario.ForestGateNodeId,
            }));
        }

        [Test]
        public void ShouldResolveReplayableFarmNodesSeparatelyFromBacktrackRoutes()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeAccessResolver nodeAccessResolver = new WorldNodeAccessResolver();
            WorldMapWorldStateSummaryResolver resolver = new WorldMapWorldStateSummaryResolver();

            WorldMapWorldStateSummary summary = resolver.Resolve(
                worldGraph,
                worldState,
                worldState.CurrentNodeId,
                ToNodeIdSet(nodeAccessResolver.GetEnterableNodes(worldGraph, worldState)),
                ToNodeIdSet(nodeAccessResolver.GetPathEnterableNodes(worldGraph, worldState)),
                ToNodeIdSet(nodeAccessResolver.GetForwardEnterableNodes(worldGraph, worldState)));

            Assert.That(ExtractNodeIds(summary.ForwardRouteNodes), Is.EqualTo(new[] { new NodeId("node_reachable") }));
            Assert.That(summary.BacktrackRouteNodes, Is.Empty);
            Assert.That(ExtractNodeIds(summary.ReplayableFarmNodes), Is.EqualTo(new[] { new NodeId("node_cleared_farm") }));
            Assert.That(summary.BlockedLinkedNodes, Is.Empty);
        }

        private static HashSet<NodeId> ToNodeIdSet(IReadOnlyList<WorldNode> nodes)
        {
            HashSet<NodeId> nodeIds = new HashSet<NodeId>();
            for (int index = 0; index < nodes.Count; index++)
            {
                nodeIds.Add(nodes[index].NodeId);
            }

            return nodeIds;
        }

        private static IReadOnlyList<NodeId> ExtractNodeIds(
            IReadOnlyList<WorldMapNodeReferenceDisplayState> nodes)
        {
            List<NodeId> nodeIds = new List<NodeId>(nodes.Count);
            for (int index = 0; index < nodes.Count; index++)
            {
                nodeIds.Add(nodes[index].NodeId);
            }

            return nodeIds;
        }

        private static IReadOnlyList<string> ExtractDisplayNames(
            IReadOnlyList<WorldMapNodeReferenceDisplayState> nodes)
        {
            List<string> displayNames = new List<string>(nodes.Count);
            for (int index = 0; index < nodes.Count; index++)
            {
                displayNames.Add(nodes[index].DisplayName);
            }

            return displayNames;
        }
    }
}
