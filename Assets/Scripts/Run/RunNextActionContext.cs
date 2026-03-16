using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
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
