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
            Assert.That(summary.CurrentRegionId, Is.EqualTo(BootstrapWorldScenario.ForestRegionId));
            Assert.That(summary.CurrentNodeId, Is.EqualTo(BootstrapWorldScenario.ForestPushNodeId));
            Assert.That(summary.CurrentNodeState, Is.EqualTo(NodeState.InProgress));
            Assert.That(summary.SelectableDestinationCount, Is.EqualTo(3));
            Assert.That(summary.ForwardRouteNodeIds, Is.EqualTo(new[]
            {
                BootstrapWorldScenario.ForestFarmNodeId,
                BootstrapWorldScenario.CavernServiceNodeId,
            }));
            Assert.That(summary.BacktrackRouteNodeIds, Is.EqualTo(new[]
            {
                BootstrapWorldScenario.ForestEntryNodeId,
            }));
            Assert.That(summary.ReplayableFarmNodeIds, Is.Empty);
            Assert.That(summary.BlockedLinkedNodeIds, Is.EqualTo(new[]
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

            Assert.That(summary.ForwardRouteNodeIds, Is.EqualTo(new[] { new NodeId("node_reachable") }));
            Assert.That(summary.BacktrackRouteNodeIds, Is.Empty);
            Assert.That(summary.ReplayableFarmNodeIds, Is.EqualTo(new[] { new NodeId("node_cleared_farm") }));
            Assert.That(summary.BlockedLinkedNodeIds, Is.Empty);
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
    }
}
