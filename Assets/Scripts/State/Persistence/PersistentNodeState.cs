using System;
using UnityEngine;

namespace Survivalon.Runtime
{
    [Serializable]
    public sealed class PersistentNodeState
    {
        [SerializeField]
        private string nodeIdValue = string.Empty;

        [SerializeField]
        private NodeState state = NodeState.Locked;

        [SerializeField]
        private int unlockProgress;

        [SerializeField]
        private int unlockThreshold;

        public PersistentNodeState()
        {
        }

        public PersistentNodeState(NodeId nodeId, int unlockThreshold, NodeState state = NodeState.Locked)
        {
            if (unlockThreshold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unlockThreshold), "Unlock threshold cannot be negative.");
            }

            nodeIdValue = nodeId.Value;
            this.unlockThreshold = unlockThreshold;
            this.state = state;
        }

        public NodeId NodeId => new NodeId(nodeIdValue);

        public NodeState State => state;

        public int UnlockProgress => unlockProgress;

        public int UnlockThreshold => unlockThreshold;

        public bool IsReplayAvailable => state != NodeState.Locked;

        public bool IsCompleted => state == NodeState.Cleared || state == NodeState.Mastered;

        public void ApplyUnlockProgress(int progressDelta)
        {
            if (progressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(progressDelta), "Unlock progress delta cannot be negative.");
            }

            if (unlockThreshold <= 0)
            {
                throw new InvalidOperationException("Untracked nodes cannot receive unlock progress.");
            }

            if (state == NodeState.Locked)
            {
                throw new InvalidOperationException("Locked nodes cannot receive unlock progress.");
            }

            if (progressDelta == 0 || IsCompleted)
            {
                return;
            }

            unlockProgress = Math.Min(unlockProgress + progressDelta, unlockThreshold);

            if (unlockProgress >= unlockThreshold)
            {
                unlockProgress = unlockThreshold;
                state = NodeState.Cleared;
                return;
            }

            if (unlockProgress > 0 && state == NodeState.Available)
            {
                state = NodeState.InProgress;
            }
        }

        public void MarkAvailable()
        {
            if (state == NodeState.Locked)
            {
                state = NodeState.Available;
            }
        }

        public void MarkMastered()
        {
            if (state == NodeState.Cleared || state == NodeState.Mastered)
            {
                state = NodeState.Mastered;
            }
        }
    }
}
