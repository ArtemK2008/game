using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.World
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
                CreateNodeState(BootstrapWorldScenario.CavernGateNodeId, bossThreshold, NodeState.Locked, 0),
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
