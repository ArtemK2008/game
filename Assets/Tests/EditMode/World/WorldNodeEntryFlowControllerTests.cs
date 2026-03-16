using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldNodeEntryFlowControllerTests
    {
        [Test]
        public void ShouldRejectMissingWorldGraphDuringConstruction()
        {
            Assert.That(
                () => new WorldNodeEntryFlowController(null, new PersistentWorldState()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldGraph"));
        }

        [Test]
        public void ShouldRejectMissingWorldStateDuringConstruction()
        {
            Assert.That(
                () => new WorldNodeEntryFlowController(BootstrapWorldTestData.CreateWorldGraph(), null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldState"));
        }

        [Test]
        public void ShouldEnterReachableNodeAndUpdateWorldContext()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
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
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
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
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
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
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
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
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool enteredUnreachableAvailableNode = controller.TryEnterNode(new NodeId("node_unreachable_available"), out NodePlaceholderState unavailableNodeState);
            bool enteredLockedNode = controller.TryEnterNode(new NodeId("node_locked"), out NodePlaceholderState lockedNodeState);

            Assert.That(enteredUnreachableAvailableNode, Is.False);
            Assert.That(enteredLockedNode, Is.False);
            Assert.That(unavailableNodeState, Is.Null);
            Assert.That(lockedNodeState, Is.Null);
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(new NodeId("node_current")));
        }

        [Test]
        public void ShouldUseLastSafeNodeAsOriginWhenCurrentNodeIsMissing()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = new PersistentWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId lastSafeNodeId = new NodeId("node_current");

            worldState.SetLastSafeNode(lastSafeNodeId);
            worldState.ReplaceReachableNodes(new[] { lastSafeNodeId });
            worldState.ReplaceNodeStates(new[]
            {
                PersistentStateTestData.CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
            });

            bool entered = controller.TryEnterNode(new NodeId("node_cleared_farm"), out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState.OriginNodeId, Is.EqualTo(lastSafeNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(lastSafeNodeId));
        }

        [Test]
        public void ShouldRejectEntryWhenCurrentAndLastSafeContextAreMissing()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = new PersistentWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            worldState.ReplaceNodeStates(new[]
            {
                PersistentStateTestData.CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
            });

            Assert.That(
                () => controller.TryEnterNode(new NodeId("node_cleared_farm"), out _),
                Throws.InvalidOperationException.With.Message.Contains("current node or last safe node"));
        }
    }
}

