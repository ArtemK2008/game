using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.State.Persistence
{
    public sealed class NodeProgressUpdateResult
    {
        public NodeProgressUpdateResult(
            bool isTracked,
            int currentProgress,
            int progressThreshold,
            bool didReachClearThreshold,
            NodeState nodeStateAfterUpdate)
        {
            if (currentProgress < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(currentProgress), "Current progress cannot be negative.");
            }

            if (progressThreshold < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(progressThreshold), "Progress threshold cannot be negative.");
            }

            if (progressThreshold == 0 && currentProgress != 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(currentProgress), "Untracked node progress must stay at zero.");
            }

            if (progressThreshold > 0 && currentProgress > progressThreshold)
            {
                throw new System.ArgumentOutOfRangeException(nameof(currentProgress), "Current progress cannot exceed the progress threshold.");
            }

            IsTracked = isTracked;
            CurrentProgress = currentProgress;
            ProgressThreshold = progressThreshold;
            DidReachClearThreshold = didReachClearThreshold;
            NodeStateAfterUpdate = nodeStateAfterUpdate;
        }

        public bool IsTracked { get; }

        public int CurrentProgress { get; }

        public int ProgressThreshold { get; }

        public bool DidReachClearThreshold { get; }

        public NodeState NodeStateAfterUpdate { get; }

        public static NodeProgressUpdateResult Untracked(NodeState nodeState)
        {
            return new NodeProgressUpdateResult(
                isTracked: false,
                currentProgress: 0,
                progressThreshold: 0,
                didReachClearThreshold: false,
                nodeStateAfterUpdate: nodeState);
        }
    }
}
