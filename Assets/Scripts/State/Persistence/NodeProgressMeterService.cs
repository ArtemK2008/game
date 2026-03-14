using System;

namespace Survivalon.Runtime
{
    public sealed class NodeProgressMeterService
    {
        private const int CombatNodeProgressThreshold = 3;

        public NodeProgressUpdateResult ApplyRunProgress(
            PersistentWorldState worldState,
            NodePlaceholderState nodeContext,
            int progressDelta)
        {
            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (progressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(progressDelta), "Progress delta cannot be negative.");
            }

            if (!ShouldTrackProgress(nodeContext.NodeType))
            {
                return NodeProgressUpdateResult.Untracked(nodeContext.NodeState);
            }

            PersistentNodeState nodeState = worldState.GetOrAddNodeState(
                nodeContext.NodeId,
                GetDefaultThreshold(nodeContext.NodeType),
                ResolveInitialNodeState(nodeContext.NodeState));
            bool wasCompleted = nodeState.IsCompleted;

            if (progressDelta > 0)
            {
                nodeState.ApplyUnlockProgress(progressDelta);
            }

            return new NodeProgressUpdateResult(
                isTracked: true,
                currentProgress: nodeState.UnlockProgress,
                progressThreshold: nodeState.UnlockThreshold,
                didReachClearThreshold: !wasCompleted && nodeState.IsCompleted,
                nodeStateAfterUpdate: nodeState.State);
        }

        public static bool ShouldTrackProgress(NodeType nodeType)
        {
            return nodeType == NodeType.Combat || nodeType == NodeType.BossOrGate;
        }

        public static int GetDefaultThreshold(NodeType nodeType)
        {
            return ShouldTrackProgress(nodeType) ? CombatNodeProgressThreshold : 0;
        }

        private static NodeState ResolveInitialNodeState(NodeState nodeState)
        {
            return nodeState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
        }
    }
}
