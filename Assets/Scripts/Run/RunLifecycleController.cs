using System;

namespace Survivalon.Runtime
{
    public sealed class RunLifecycleController
    {
        private readonly NodePlaceholderState nodeContext;
        private RunLifecycleState currentState;
        private RunResult runResult;

        public RunLifecycleController(NodePlaceholderState nodeContext)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            currentState = RunLifecycleState.RunStart;
        }

        public NodePlaceholderState NodeContext => nodeContext;

        public RunLifecycleState CurrentState => currentState;

        public bool HasRunResult => runResult != null;

        public RunResult RunResult => runResult ??
            throw new InvalidOperationException("Run result is not available until the run is resolved.");

        public bool TryEnterActiveState()
        {
            if (currentState != RunLifecycleState.RunStart)
            {
                return false;
            }

            currentState = RunLifecycleState.RunActive;
            return true;
        }

        public bool TryResolveRun(RunResolutionState resolutionState)
        {
            if (currentState != RunLifecycleState.RunActive)
            {
                return false;
            }

            runResult = new RunResult(
                nodeContext.NodeId,
                resolutionState,
                RunRewardPayload.Empty,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
            currentState = RunLifecycleState.RunResolved;
            return true;
        }

        public bool TryEnterPostRunState()
        {
            if (currentState != RunLifecycleState.RunResolved || runResult == null)
            {
                return false;
            }

            currentState = RunLifecycleState.PostRun;
            return true;
        }
    }
}
