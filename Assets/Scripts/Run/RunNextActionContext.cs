namespace Survivalon.Runtime
{
    public sealed class RunNextActionContext
    {
        public RunNextActionContext(
            bool canReplayNode,
            bool canChooseAnotherNode,
            bool canStopSession)
        {
            CanReplayNode = canReplayNode;
            CanChooseAnotherNode = canChooseAnotherNode;
            CanStopSession = canStopSession;
        }

        public bool CanReplayNode { get; }

        public bool CanChooseAnotherNode { get; }

        public bool CanStopSession { get; }
    }
}
