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
    }
}
