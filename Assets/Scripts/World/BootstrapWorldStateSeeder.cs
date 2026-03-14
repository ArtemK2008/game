namespace Survivalon.Runtime
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
            int combatThreshold = NodeProgressMeterService.GetDefaultThreshold(NodeType.Combat);
            int bossThreshold = NodeProgressMeterService.GetDefaultThreshold(NodeType.BossOrGate);

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
            NodeState initialState = nodeState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
            PersistentNodeState persistentNodeState = new PersistentNodeState(nodeId, unlockThreshold, initialState);

            if (initialState != NodeState.Locked && unlockProgress > 0)
            {
                persistentNodeState.ApplyUnlockProgress(unlockProgress);
            }

            if (nodeState == NodeState.Mastered)
            {
                persistentNodeState.MarkMastered();
            }

            return persistentNodeState;
        }
    }
}
