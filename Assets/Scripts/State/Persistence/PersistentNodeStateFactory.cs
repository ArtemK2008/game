using System;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    public static class PersistentNodeStateFactory
    {
        public static PersistentNodeState Create(
            NodeId nodeId,
            int unlockThreshold,
            NodeState initialState = NodeState.Available,
            int initialProgress = 0)
        {
            if (unlockThreshold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unlockThreshold), "Unlock threshold cannot be negative.");
            }

            if (initialProgress < 0 || initialProgress > unlockThreshold)
            {
                throw new ArgumentOutOfRangeException(nameof(initialProgress), "Initial progress must be between zero and the unlock threshold.");
            }

            NodeState normalizedState = NormalizeInitialState(initialState);
            PersistentNodeState nodeState = new PersistentNodeState(nodeId, unlockThreshold, normalizedState);

            if (normalizedState != NodeState.Locked && initialProgress > 0)
            {
                nodeState.ApplyUnlockProgress(initialProgress);
            }

            if (initialState == NodeState.Mastered)
            {
                nodeState.MarkMastered();
            }

            return nodeState;
        }

        private static NodeState NormalizeInitialState(NodeState initialState)
        {
            return initialState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
        }
    }
}

