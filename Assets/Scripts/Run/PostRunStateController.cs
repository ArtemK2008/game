using System;

namespace Survivalon.Runtime
{
    public sealed class PostRunStateController
    {
        private readonly NodePlaceholderState nodeContext;
        private readonly RunResult runResult;

        public PostRunStateController(NodePlaceholderState nodeContext, RunResult runResult)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.runResult = runResult ?? throw new ArgumentNullException(nameof(runResult));
        }

        public NodePlaceholderState NodeContext => nodeContext;

        public RunResult RunResult => runResult;

        public bool CanReplayNode => runResult.NextActionContext.CanReplayNode;

        public bool CanReturnToWorld => runResult.NextActionContext.CanChooseAnotherNode;

        public bool CanStopSession => runResult.NextActionContext.CanStopSession;

        public RunLifecycleController CreateReplayLifecycleController(
            PersistentWorldState persistentWorldState = null)
        {
            if (!CanReplayNode)
            {
                throw new InvalidOperationException("Replay is not available for the current post-run state.");
            }

            return new RunLifecycleController(
                nodeContext,
                persistentWorldState: persistentWorldState);
        }
    }
}
