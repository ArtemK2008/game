using System;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Run
{
    public static class RunResultFactory
    {
        public static RunResult Create(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            RunProgressResolution progressResolution,
            RunRewardPayload rewardPayload = null)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            return new RunResult(
                nodeContext.NodeId,
                resolutionState,
                rewardPayload ?? RunRewardPayload.Empty,
                progressResolution.NodeProgressDelta,
                progressResolution.NodeProgressUpdate.CurrentProgress,
                progressResolution.NodeProgressUpdate.ProgressThreshold,
                0,
                progressResolution.DidUnlockRoute,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true),
                progressResolution.BossProgressionGateUnlockSummary);
        }
    }
}

