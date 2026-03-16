using System.Collections.Generic;
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
    public sealed class RouteChoiceSupportTests
    {
        [Test]
        public void ShouldExposeMoreThanOneForwardOptionForBranchingCurrentNode()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<NodeId> forwardSelectableNodeIds = controller.GetForwardSelectableNodeIds();

            Assert.That(forwardSelectableNodeIds.Count, Is.EqualTo(2));
            Assert.That(forwardSelectableNodeIds, Has.Member(new NodeId("region_001_node_004")));
            Assert.That(forwardSelectableNodeIds, Has.Member(new NodeId("region_002_node_001")));
        }

        [Test]
        public void ShouldExcludeLockedBranchDestinationsFromForwardOptions()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<NodeId> forwardSelectableNodeIds = controller.GetForwardSelectableNodeIds();

            Assert.That(forwardSelectableNodeIds, Has.No.Member(new NodeId("region_001_node_003")));
            Assert.That(forwardSelectableNodeIds, Has.No.Member(new NodeId("region_002_node_002")));
        }

        [Test]
        public void ShouldExcludeDisconnectedAvailableNodesFromForwardOptions()
        {
            RegionId regionId = new RegionId("region_001");
            NodeId entryNodeId = new NodeId("region_001_node_001");
            NodeId currentNodeId = new NodeId("region_001_node_002");
            NodeId forwardBranchNodeId = new NodeId("region_001_node_003");
            NodeId disconnectedNodeId = new NodeId("region_001_node_004");

            WorldGraph worldGraph = new WorldGraph(
                new[]
                {
                    new WorldRegion(
                        regionId,
                        0,
                        entryNodeId,
                        new[]
                        {
                            entryNodeId,
                            currentNodeId,
                            forwardBranchNodeId,
                            disconnectedNodeId,
                        },
                        ResourceCategory.RegionMaterial,
                        "frontier"),
                },
                new[]
                {
                    new WorldNode(entryNodeId, regionId, NodeType.Combat, NodeState.Cleared),
                    new WorldNode(currentNodeId, regionId, NodeType.Combat, NodeState.InProgress),
                    new WorldNode(forwardBranchNodeId, regionId, NodeType.Combat, NodeState.Available),
                    new WorldNode(disconnectedNodeId, regionId, NodeType.ServiceOrProgression, NodeState.Available),
                },
                new[]
                {
                    new WorldNodeConnection(entryNodeId, currentNodeId),
                    new WorldNodeConnection(currentNodeId, forwardBranchNodeId),
                });

            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(currentNodeId);
            worldState.SetLastSafeNode(entryNodeId);
            worldState.ReplaceReachableNodes(new[] { entryNodeId });

            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<NodeId> forwardSelectableNodeIds = controller.GetForwardSelectableNodeIds();

            Assert.That(forwardSelectableNodeIds, Is.EquivalentTo(new[] { forwardBranchNodeId }));
            Assert.That(forwardSelectableNodeIds, Has.No.Member(disconnectedNodeId));
            Assert.That(controller.HasForwardRouteChoice, Is.False);
        }
    }
}
