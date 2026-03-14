using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class BootstrapWorldStateSeederTests
    {
        [Test]
        public void ShouldCreateExpectedBootstrapPersistentWorldState()
        {
            PersistentGameState gameState = new BootstrapWorldStateSeeder().Create();
            PersistentWorldState worldState = gameState.WorldState;

            Assert.That(worldState.CurrentNodeId, Is.EqualTo(BootstrapWorldScenario.ForestPushNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(BootstrapWorldScenario.ForestEntryNodeId));
            Assert.That(worldState.ReachableNodeIdValues, Is.EqualTo(new[] { BootstrapWorldScenario.ForestEntryNodeId.Value }));
            Assert.That(worldState.NodeStates.Count, Is.EqualTo(5));

            AssertPersistentNodeState(worldState, BootstrapWorldScenario.ForestEntryNodeId, NodeState.Cleared, 3, 3);
            AssertPersistentNodeState(worldState, BootstrapWorldScenario.ForestPushNodeId, NodeState.InProgress, 1, 3);
            AssertPersistentNodeState(worldState, BootstrapWorldScenario.ForestGateNodeId, NodeState.Locked, 0, 3);
            AssertPersistentNodeState(worldState, BootstrapWorldScenario.ForestFarmNodeId, NodeState.Available, 0, 3);
            AssertPersistentNodeState(worldState, BootstrapWorldScenario.CavernGateNodeId, NodeState.Locked, 0, 3);
            Assert.That(worldState.TryGetNodeState(BootstrapWorldScenario.CavernServiceNodeId, out _), Is.False);
        }

        private static void AssertPersistentNodeState(
            PersistentWorldState worldState,
            NodeId nodeId,
            NodeState expectedNodeState,
            int expectedProgress,
            int expectedThreshold)
        {
            Assert.That(worldState.TryGetNodeState(nodeId, out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.State, Is.EqualTo(expectedNodeState));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(expectedProgress));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(expectedThreshold));
        }
    }
}
