using System;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
{
    public sealed class NodeProgressMeterService
    {
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

            if (!TrackedNodeProgressRules.ShouldTrack(nodeContext.NodeType))
            {
                return NodeProgressUpdateResult.Untracked(nodeContext.NodeState);
            }

            PersistentNodeState nodeState = worldState.GetOrAddNodeState(
                nodeContext.NodeId,
                TrackedNodeProgressRules.GetDefaultThreshold(nodeContext.NodeType),
                nodeContext.NodeState);
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
            return TrackedNodeProgressRules.ShouldTrack(nodeType);
        }

        public static int GetDefaultThreshold(NodeType nodeType)
        {
            return TrackedNodeProgressRules.GetDefaultThreshold(nodeType);
        }
    }
}
