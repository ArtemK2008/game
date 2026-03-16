using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public static class BootstrapWorldTestData
    {
        public static PersistentGameState CreateGameState()
        {
            return new BootstrapWorldMapFactory().CreateGameState();
        }

        public static WorldGraph CreateWorldGraph()
        {
            return new BootstrapWorldMapFactory().CreateWorldGraph();
        }

        public static PersistentWorldState CreateWorldState()
        {
            return CreateGameState().WorldState;
        }
    }
}
