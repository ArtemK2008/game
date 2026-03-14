using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class WorldMapScreenControllerTests
    {
        [Test]
        public void ShouldBuildSelectableOptionsForReachableNodes()
        {
            WorldGraph worldGraph = CreateGraph();
            PersistentWorldState worldState = CreateWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<WorldMapNodeOption> nodeOptions = controller.BuildNodeOptions();

            AssertNodeOption(nodeOptions, new NodeId("region_001_node_001"), true, false, false);
            AssertNodeOption(nodeOptions, new NodeId("region_001_node_002"), false, true, false);
            AssertNodeOption(nodeOptions, new NodeId("region_001_node_003"), false, false, false);
            AssertNodeOption(nodeOptions, new NodeId("region_001_node_004"), true, false, false);
            AssertNodeOption(nodeOptions, new NodeId("region_002_node_001"), true, false, false);
            AssertNodeOption(nodeOptions, new NodeId("region_002_node_002"), false, false, false);
        }

        [Test]
        public void ShouldAllowSelectingReachableNode()
        {
            WorldGraph worldGraph = CreateGraph();
            PersistentWorldState worldState = CreateWorldState();
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
            AssertNodeOption(nodeOptions, selectableNodeId, true, false, true);
        }

        [Test]
        public void ShouldRejectSelectingLockedOrUnreachableNodes()
        {
            WorldGraph worldGraph = CreateGraph();
            PersistentWorldState worldState = CreateWorldState();
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
            WorldGraph worldGraph = CreateGraph();
            PersistentWorldState worldState = CreateWorldState();
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
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
            WorldMapScreenController controller = new WorldMapScreenController(worldGraph, worldState);

            IReadOnlyList<WorldMapNodeOption> nodeOptions = controller.BuildNodeOptions();

            AssertNodeOption(nodeOptions, new NodeId("node_reachable"), true, false, false);
            AssertNodeOption(nodeOptions, new NodeId("node_cleared_farm"), true, false, false);
            AssertNodeOption(nodeOptions, new NodeId("node_unreachable_available"), false, false, false);
            AssertNodeOption(nodeOptions, new NodeId("node_locked"), false, false, false);
        }

        private static void AssertNodeOption(
            IReadOnlyList<WorldMapNodeOption> nodeOptions,
            NodeId nodeId,
            bool isSelectable,
            bool isCurrentContext,
            bool isSelected)
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
                return;
            }

            Assert.Fail($"World map node option '{nodeId}' was not found.");
        }

        private static WorldGraph CreateGraph()
        {
            BootstrapWorldMapFactory bootstrapWorldMapFactory = new BootstrapWorldMapFactory();
            return bootstrapWorldMapFactory.CreateWorldGraph();
        }

        private static PersistentWorldState CreateWorldState()
        {
            BootstrapWorldMapFactory bootstrapWorldMapFactory = new BootstrapWorldMapFactory();
            return bootstrapWorldMapFactory.CreateGameState().WorldState;
        }

        private static WorldGraph CreateFarmAccessGraph()
        {
            RegionId regionId = new RegionId("region_001");
            WorldNode currentNode = new WorldNode(new NodeId("node_current"), regionId, NodeType.ServiceOrProgression, NodeState.Available);
            WorldNode reachableNode = new WorldNode(new NodeId("node_reachable"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode clearedFarmNode = new WorldNode(new NodeId("node_cleared_farm"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode unreachableAvailableNode = new WorldNode(new NodeId("node_unreachable_available"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode lockedNode = new WorldNode(new NodeId("node_locked"), regionId, NodeType.BossOrGate, NodeState.Locked);

            return new WorldGraph(
                new[]
                {
                    new WorldRegion(
                        regionId,
                        0,
                        currentNode.NodeId,
                        new[]
                        {
                            currentNode.NodeId,
                            reachableNode.NodeId,
                            clearedFarmNode.NodeId,
                            unreachableAvailableNode.NodeId,
                            lockedNode.NodeId,
                        },
                        ResourceCategory.RegionMaterial,
                        "farm_access"),
                },
                new[]
                {
                    currentNode,
                    reachableNode,
                    clearedFarmNode,
                    unreachableAvailableNode,
                    lockedNode,
                },
                new[]
                {
                    new WorldNodeConnection(currentNode.NodeId, reachableNode.NodeId),
                });
        }

        private static PersistentWorldState CreateFarmAccessWorldState()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(new NodeId("node_current"));
            worldState.ReplaceReachableNodes(new[] { new NodeId("node_current") });
            worldState.ReplaceNodeStates(new[]
            {
                CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
                CreateNodeState(new NodeId("node_locked"), 3, NodeState.Locked, 0),
            });
            return worldState;
        }

        private static PersistentNodeState CreateNodeState(
            NodeId nodeId,
            int unlockThreshold,
            NodeState nodeState,
            int unlockProgress)
        {
            NodeState initialState = nodeState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
            PersistentNodeState persistentNodeState = new PersistentNodeState(nodeId, unlockThreshold, initialState);

            if (initialState != NodeState.Locked && unlockProgress > 0)
            {
                persistentNodeState.ApplyUnlockProgress(unlockProgress);
            }

            return persistentNodeState;
        }
    }
}
