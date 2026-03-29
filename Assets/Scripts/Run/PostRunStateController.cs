using System;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class PostRunStateController
    {
        private readonly NodePlaceholderState nodeContext;
        private readonly RunResult runResult;
        private readonly WorldGraph worldGraph;

        public PostRunStateController(
            NodePlaceholderState nodeContext,
            RunResult runResult,
            WorldGraph worldGraph = null)
        {
            this.nodeContext = nodeContext ?? throw new ArgumentNullException(nameof(nodeContext));
            this.runResult = runResult ?? throw new ArgumentNullException(nameof(runResult));
            this.worldGraph = worldGraph;
        }

        public RunResult RunResult => runResult;

        public NodePlaceholderState NodeContext => nodeContext;

        public WorldGraph WorldGraph => worldGraph;

        public bool CanReplayNode => runResult.NextActionContext.CanReplayNode;

        public bool CanReturnToWorld => runResult.NextActionContext.CanChooseAnotherNode;

        public bool CanStopSession => runResult.NextActionContext.CanStopSession;

        public RunLifecycleController CreateReplayLifecycleController(
            WorldGraph worldGraph = null,
            RunPersistentContext persistentContext = null)
        {
            if (!CanReplayNode)
            {
                throw new InvalidOperationException("Replay is not available for the current post-run state.");
            }

            return new RunLifecycleController(
                nodeContext,
                worldGraph,
                persistentContext: persistentContext);
        }
    }
}

