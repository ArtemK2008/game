using System;

namespace Survivalon.Runtime
{
    public sealed class RunResult
    {
        public RunResult(
            NodeId nodeId,
            RunResolutionState resolutionState,
            RunRewardPayload rewardPayload,
            int nodeProgressDelta,
            int persistentProgressionDelta,
            bool didUnlockRoute,
            RunNextActionContext nextActionContext)
        {
            if (nodeProgressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressDelta), "Node progress delta cannot be negative.");
            }

            if (persistentProgressionDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(persistentProgressionDelta), "Persistent progression delta cannot be negative.");
            }

            NodeId = nodeId;
            ResolutionState = resolutionState;
            RewardPayload = rewardPayload ?? throw new ArgumentNullException(nameof(rewardPayload));
            NodeProgressDelta = nodeProgressDelta;
            PersistentProgressionDelta = persistentProgressionDelta;
            DidUnlockRoute = didUnlockRoute;
            NextActionContext = nextActionContext ?? throw new ArgumentNullException(nameof(nextActionContext));
        }

        public NodeId NodeId { get; }

        public RunResolutionState ResolutionState { get; }

        public RunRewardPayload RewardPayload { get; }

        public int NodeProgressDelta { get; }

        public int PersistentProgressionDelta { get; }

        public bool DidUnlockRoute { get; }

        public RunNextActionContext NextActionContext { get; }
    }
}
