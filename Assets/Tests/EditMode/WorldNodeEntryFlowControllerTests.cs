using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class WorldNodeEntryFlowControllerTests
    {
        [Test]
        public void ShouldEnterReachableNodeAndUpdateWorldContext()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId selectedNodeId = new NodeId("region_002_node_001");

            bool entered = controller.TryEnterNode(selectedNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(selectedNodeId));
            Assert.That(placeholderState.OriginNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(selectedNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.ReachableNodeIdValues, Does.Contain("region_001_node_001"));
            Assert.That(worldState.ReachableNodeIdValues, Does.Contain("region_001_node_002"));
        }

        [Test]
        public void ShouldRejectEnteringLockedOrUnreachableNode()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool enteredLockedNode = controller.TryEnterNode(new NodeId("region_001_node_003"), out NodePlaceholderState lockedNodeState);
            bool enteredCurrentNode = controller.TryEnterNode(new NodeId("region_001_node_002"), out NodePlaceholderState currentNodeState);

            Assert.That(enteredLockedNode, Is.False);
            Assert.That(enteredCurrentNode, Is.False);
            Assert.That(lockedNodeState, Is.Null);
            Assert.That(currentNodeState, Is.Null);
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("region_001_node_001")));
        }

        [Test]
        public void ShouldAllowEnteringReachableClearedNodeWithoutRegressingPersistentState()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId clearedNodeId = new NodeId("region_001_node_001");

            Assert.That(worldState.TryGetNodeState(clearedNodeId, out PersistentNodeState clearedNodeStateBeforeEntry), Is.True);
            Assert.That(clearedNodeStateBeforeEntry.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(clearedNodeStateBeforeEntry.UnlockProgress, Is.EqualTo(clearedNodeStateBeforeEntry.UnlockThreshold));

            bool entered = controller.TryEnterNode(clearedNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(clearedNodeId));
            Assert.That(placeholderState.NodeState, Is.EqualTo(NodeState.Cleared));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(clearedNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.TryGetNodeState(clearedNodeId, out PersistentNodeState clearedNodeStateAfterEntry), Is.True);
            Assert.That(clearedNodeStateAfterEntry.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(clearedNodeStateAfterEntry.UnlockProgress, Is.EqualTo(clearedNodeStateAfterEntry.UnlockThreshold));
        }

        [Test]
        public void ShouldAllowEnteringPersistentlyClearedNodeEvenWhenItIsNotPathReachable()
        {
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId clearedFarmNodeId = new NodeId("node_cleared_farm");

            bool entered = controller.TryEnterNode(clearedFarmNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(clearedFarmNodeId));
            Assert.That(placeholderState.NodeState, Is.EqualTo(NodeState.Cleared));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(clearedFarmNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("node_current")));
            Assert.That(worldState.TryGetNodeState(clearedFarmNodeId, out PersistentNodeState clearedNodeState), Is.True);
            Assert.That(clearedNodeState.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(clearedNodeState.UnlockProgress, Is.EqualTo(clearedNodeState.UnlockThreshold));
        }

        [Test]
        public void ShouldRejectUnavailableNodeWhenItIsNotReachableAndNotPersistentlyCleared()
        {
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool enteredUnreachableAvailableNode = controller.TryEnterNode(new NodeId("node_unreachable_available"), out NodePlaceholderState unavailableNodeState);
            bool enteredLockedNode = controller.TryEnterNode(new NodeId("node_locked"), out NodePlaceholderState lockedNodeState);

            Assert.That(enteredUnreachableAvailableNode, Is.False);
            Assert.That(enteredLockedNode, Is.False);
            Assert.That(unavailableNodeState, Is.Null);
            Assert.That(lockedNodeState, Is.Null);
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(new NodeId("node_current")));
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
