using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

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
