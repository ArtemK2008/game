using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldMapScreenControllerTests
    {
        [Test]
        public void ShouldBuildSelectableOptionsForReachableNodes()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<WorldMapNodeOption> nodeOptions = controller.BuildNodeOptions();

            AssertNodeOption(nodeOptions, new NodeId("region_001_node_001"), true, false, false, WorldMapPathRole.BacktrackRoute);
            AssertNodeOption(nodeOptions, new NodeId("region_001_node_002"), false, true, false, WorldMapPathRole.CurrentContext);
            AssertNodeOption(nodeOptions, new NodeId("region_001_node_003"), false, false, false, WorldMapPathRole.BlockedPath);
            AssertNodeOption(nodeOptions, new NodeId("region_001_node_004"), true, false, false, WorldMapPathRole.ForwardRoute);
            AssertNodeOption(nodeOptions, new NodeId("region_002_node_001"), true, false, false, WorldMapPathRole.ForwardRoute);
            AssertNodeOption(nodeOptions, new NodeId("region_002_node_002"), false, false, false, WorldMapPathRole.BlockedPath);
            AssertNodeDisplayName(nodeOptions, new NodeId("region_001_node_004"), "Forest Farm");
            AssertNodeDisplayName(nodeOptions, new NodeId("region_002_node_001"), "Cavern Service Hub");
        }

        [Test]
        public void ShouldAllowSelectingReachableNode()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            SessionContextState sessionContext = new SessionContextState();
            WorldMapScreenController controller = new WorldMapScreenController(
                worldGraph,
                worldState,
                sessionContext: sessionContext);
            NodeId selectableNodeId = new NodeId("region_002_node_001");

            bool selected = controller.TrySelectNode(selectableNodeId);
            bool hasSelectedNodeId = controller.TryGetSelectedNodeId(out NodeId selectedNodeId);
            IReadOnlyList<WorldMapNodeOption> nodeOptions = controller.BuildNodeOptions();

            Assert.That(selected, Is.True);
            Assert.That(hasSelectedNodeId, Is.True);
            Assert.That(selectedNodeId, Is.EqualTo(selectableNodeId));
            Assert.That(sessionContext.HasLastSelectedNode, Is.True);
            Assert.That(sessionContext.LastSelectedNodeId, Is.EqualTo(selectableNodeId));
            Assert.That(sessionContext.HasRecentPushTarget, Is.True);
            Assert.That(sessionContext.RecentPushTargetNodeId, Is.EqualTo(selectableNodeId));
            AssertNodeOption(nodeOptions, selectableNodeId, true, false, true, WorldMapPathRole.ForwardRoute);
        }

        [Test]
        public void ShouldResolveQuickRepeatNodeFromCurrentWorldContext()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            SessionContextState sessionContext = new SessionContextState();
            sessionContext.OfferReturnToWorldReentry(new NodeId("region_001_node_002"));
            WorldMapScreenController controller = new WorldMapScreenController(
                worldGraph,
                worldState,
                sessionContext: sessionContext);

            bool hasQuickRepeatNode = controller.TryGetQuickRepeatNode(
                out NodeId quickRepeatNodeId,
                out string quickRepeatNodeDisplayName,
                out NodeType quickRepeatNodeType);

            Assert.That(hasQuickRepeatNode, Is.True);
            Assert.That(quickRepeatNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(quickRepeatNodeDisplayName, Is.EqualTo("Raider Trail"));
            Assert.That(quickRepeatNodeType, Is.EqualTo(NodeType.Combat));
        }

        [Test]
        public void ShouldNotResolveQuickRepeatNodeWithoutExplicitReturnToWorldOffer()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            SessionContextState sessionContext = new SessionContextState();
            WorldMapScreenController controller = new WorldMapScreenController(
                worldGraph,
                worldState,
                sessionContext: sessionContext);

            bool hasQuickRepeatNode = controller.TryGetQuickRepeatNode(
                out NodeId quickRepeatNodeId,
                out string quickRepeatNodeDisplayName,
                out NodeType quickRepeatNodeType);

            Assert.That(hasQuickRepeatNode, Is.False);
            Assert.That(quickRepeatNodeId, Is.EqualTo(default(NodeId)));
            Assert.That(quickRepeatNodeDisplayName, Is.Null);
            Assert.That(quickRepeatNodeType, Is.EqualTo(default(NodeType)));
        }

        [Test]
        public void ShouldRejectSelectingLockedOrUnreachableNodes()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            bool selectedLockedNode = controller.TrySelectNode(new NodeId("region_001_node_003"));
            bool selectedCurrentNode = controller.TrySelectNode(new NodeId("region_001_node_002"));
            bool selectedUnreachableNode = controller.TrySelectNode(new NodeId("region_002_node_002"));
            bool hasSelectedNodeId = controller.TryGetSelectedNodeId(out _);

            Assert.That(selectedLockedNode, Is.False);
            Assert.That(selectedCurrentNode, Is.False);
            Assert.That(selectedUnreachableNode, Is.False);
            Assert.That(controller.HasSelection, Is.False);
            Assert.That(hasSelectedNodeId, Is.False);
        }

        [Test]
        public void ShouldReportForwardBranchChoiceWhenMultipleRoutesAreAvailable()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<NodeId> forwardSelectableNodeIds = controller.GetForwardSelectableNodeIds();

            Assert.That(controller.HasForwardRouteChoice, Is.True);
            Assert.That(controller.ForwardSelectableNodeCount, Is.EqualTo(2));
            Assert.That(forwardSelectableNodeIds, Is.EquivalentTo(new[]
            {
                new NodeId("region_001_node_004"),
                new NodeId("region_002_node_001"),
            }));
        }

        [Test]
        public void ShouldMakeClearedNodeSelectableForFarmingEvenWhenItIsNotPathReachable()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<WorldMapNodeOption> nodeOptions = controller.BuildNodeOptions();

            AssertNodeOption(nodeOptions, new NodeId("node_reachable"), true, false, false, WorldMapPathRole.ForwardRoute);
            AssertNodeOption(nodeOptions, new NodeId("node_cleared_farm"), true, false, false, WorldMapPathRole.ReplayableFarmNode);
            AssertNodeOption(nodeOptions, new NodeId("node_unreachable_available"), false, false, false, WorldMapPathRole.BlockedPath);
            AssertNodeOption(nodeOptions, new NodeId("node_locked"), false, false, false, WorldMapPathRole.BlockedPath);
        }

        [Test]
        public void ShouldRecomputeAccessAndPathRolesWhenWorldStateChangesAfterControllerConstruction()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            Assert.That(controller.GetForwardSelectableNodeIds(), Is.EqualTo(new[] { new NodeId("node_reachable") }));

            worldState.SetCurrentNode(new NodeId("node_reachable"));
            worldState.SetLastSafeNode(new NodeId("node_current"));
            worldState.ReplaceReachableNodes(new[] { new NodeId("node_current") });

            IReadOnlyList<WorldMapNodeOption> nodeOptions = controller.BuildNodeOptions();
            WorldMapWorldStateSummary summary = controller.BuildWorldStateSummary();

            Assert.That(controller.GetForwardSelectableNodeIds(), Is.Empty);
            Assert.That(controller.HasForwardRouteChoice, Is.False);
            Assert.That(controller.ForwardSelectableNodeCount, Is.EqualTo(0));
            AssertNodeOption(nodeOptions, new NodeId("node_current"), true, false, false, WorldMapPathRole.BacktrackRoute);
            AssertNodeOption(nodeOptions, new NodeId("node_reachable"), false, true, false, WorldMapPathRole.CurrentContext);
            AssertNodeOption(nodeOptions, new NodeId("node_cleared_farm"), true, false, false, WorldMapPathRole.ReplayableFarmNode);
            Assert.That(summary.CurrentNode.NodeId, Is.EqualTo(new NodeId("node_reachable")));
            Assert.That(summary.ForwardRouteNodes, Is.Empty);
            Assert.That(ExtractNodeIds(summary.BacktrackRouteNodes), Is.EqualTo(new[] { new NodeId("node_current") }));
            Assert.That(ExtractNodeIds(summary.ReplayableFarmNodes), Is.EqualTo(new[] { new NodeId("node_cleared_farm") }));
        }

        private static void AssertNodeOption(
            IReadOnlyList<WorldMapNodeOption> nodeOptions,
            NodeId nodeId,
            bool isSelectable,
            bool isCurrentContext,
            bool isSelected,
            WorldMapPathRole pathRole)
        {
            foreach (WorldMapNodeOption nodeOption in nodeOptions)
            {
                if (nodeOption.NodeId != nodeId)
                {
                    continue;
                }

                Assert.That(nodeOption.IsSelectable, Is.EqualTo(isSelectable));
                Assert.That(nodeOption.IsCurrentContext, Is.EqualTo(isCurrentContext));
                Assert.That(nodeOption.IsSelected, Is.EqualTo(isSelected));
                Assert.That(nodeOption.PathRole, Is.EqualTo(pathRole));
                return;
            }

            Assert.Fail($"World map node option '{nodeId}' was not found.");
        }

        private static void AssertNodeDisplayName(
            IReadOnlyList<WorldMapNodeOption> nodeOptions,
            NodeId nodeId,
            string expectedDisplayName)
        {
            foreach (WorldMapNodeOption nodeOption in nodeOptions)
            {
                if (nodeOption.NodeId != nodeId)
                {
                    continue;
                }

                Assert.That(nodeOption.NodeDisplayName, Is.EqualTo(expectedDisplayName));
                return;
            }

            Assert.Fail($"World map node option '{nodeId}' was not found.");
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

    }
}

