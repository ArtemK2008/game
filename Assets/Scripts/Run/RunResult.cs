using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
{
    public sealed class RunResult
    {
        public RunResult(
            NodeId nodeId,
            RunResolutionState resolutionState,
            RunRewardPayload rewardPayload,
            int nodeProgressDelta,
            int nodeProgressValue,
            int nodeProgressThreshold,
            int persistentProgressionDelta,
            bool didUnlockRoute,
            RunNextActionContext nextActionContext)
        {
            if (nodeProgressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressDelta), "Node progress delta cannot be negative.");
            }

            if (nodeProgressValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressValue), "Node progress value cannot be negative.");
            }

            if (nodeProgressThreshold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressThreshold), "Node progress threshold cannot be negative.");
            }

            if (nodeProgressThreshold == 0 && nodeProgressValue != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressValue), "Untracked node progress must stay at zero.");
            }

            if (nodeProgressThreshold > 0 && nodeProgressValue > nodeProgressThreshold)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressValue), "Node progress value cannot exceed the node progress threshold.");
            }

            if (persistentProgressionDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(persistentProgressionDelta), "Persistent progression delta cannot be negative.");
            }

            NodeId = nodeId;
            ResolutionState = resolutionState;
            RewardPayload = rewardPayload ?? throw new ArgumentNullException(nameof(rewardPayload));
            NodeProgressDelta = nodeProgressDelta;
            NodeProgressValue = nodeProgressValue;
            NodeProgressThreshold = nodeProgressThreshold;
            PersistentProgressionDelta = persistentProgressionDelta;
            DidUnlockRoute = didUnlockRoute;
            NextActionContext = nextActionContext ?? throw new ArgumentNullException(nameof(nextActionContext));
        }

        public NodeId NodeId { get; }

        public RunResolutionState ResolutionState { get; }

        public RunRewardPayload RewardPayload { get; }

        public int NodeProgressDelta { get; }

        public int NodeProgressValue { get; }

        public int NodeProgressThreshold { get; }

        public bool HasTrackedNodeProgress => NodeProgressThreshold > 0;

        public int PersistentProgressionDelta { get; }

        public bool DidUnlockRoute { get; }

        public RunNextActionContext NextActionContext { get; }
    }
}
