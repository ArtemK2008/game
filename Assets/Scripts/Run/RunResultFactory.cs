using System;

namespace Survivalon.Runtime
{
    public static class RunResultFactory
    {
        public static RunResult Create(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            RunProgressResolution progressResolution)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            return new RunResult(
                nodeContext.NodeId,
                resolutionState,
                RunRewardPayload.Empty,
                progressResolution.NodeProgressDelta,
                progressResolution.NodeProgressUpdate.CurrentProgress,
                progressResolution.NodeProgressUpdate.ProgressThreshold,
                0,
                progressResolution.DidUnlockRoute,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
        }
    }
}
