using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class BootstrapWorldStateSeeder
    {
        public PersistentGameState Create()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(BootstrapWorldScenario.ForestPushNodeId);
            gameState.WorldState.SetLastSafeNode(BootstrapWorldScenario.ForestEntryNodeId);
            gameState.WorldState.ReplaceReachableNodes(new[] { BootstrapWorldScenario.ForestEntryNodeId });
            gameState.WorldState.ReplaceNodeStates(CreatePersistentNodeStates());

            return gameState;
        }

        private static PersistentNodeState[] CreatePersistentNodeStates()
        {
            int combatThreshold = TrackedNodeProgressRules.GetDefaultThreshold(NodeType.Combat);
            int bossThreshold = TrackedNodeProgressRules.GetDefaultThreshold(NodeType.BossOrGate);

            return new[]
            {
                CreateNodeState(BootstrapWorldScenario.ForestEntryNodeId, combatThreshold, NodeState.Cleared, combatThreshold),
                CreateNodeState(BootstrapWorldScenario.ForestPushNodeId, combatThreshold, NodeState.InProgress, 1),
                CreateNodeState(BootstrapWorldScenario.ForestGateNodeId, bossThreshold, NodeState.Locked, 0),
                CreateNodeState(BootstrapWorldScenario.ForestFarmNodeId, combatThreshold, NodeState.Available, 0),
                CreateNodeState(BootstrapWorldScenario.ForestEliteNodeId, combatThreshold, NodeState.Available, 0),
                CreateNodeState(BootstrapWorldScenario.CavernPushNodeId, combatThreshold, NodeState.Available, 0),
                CreateNodeState(BootstrapWorldScenario.CavernFarmNodeId, combatThreshold, NodeState.Available, 0),
                CreateNodeState(BootstrapWorldScenario.CavernApproachNodeId, combatThreshold, NodeState.Available, 0),
                CreateNodeState(BootstrapWorldScenario.CavernGateNodeId, bossThreshold, NodeState.Locked, 0),
                CreateNodeState(BootstrapWorldScenario.SunscorchEntryNodeId, combatThreshold, NodeState.Locked, 0),
                CreateNodeState(BootstrapWorldScenario.SunscorchPushNodeId, combatThreshold, NodeState.Locked, 0),
                CreateNodeState(BootstrapWorldScenario.SunscorchFarmNodeId, combatThreshold, NodeState.Locked, 0),
            };
        }

        private static PersistentNodeState CreateNodeState(
            NodeId nodeId,
            int unlockThreshold,
            NodeState nodeState,
            int unlockProgress)
        {
            return PersistentNodeStateFactory.Create(nodeId, unlockThreshold, nodeState, unlockProgress);
        }
    }
}

