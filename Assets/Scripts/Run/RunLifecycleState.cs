using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
{
    public enum RunLifecycleState
    {
        RunStart = 0,
        RunActive = 1,
        RunResolved = 2,
        PostRun = 3,
    }
}
