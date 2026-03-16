using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.World
{
    public sealed class BootstrapWorldMapFactory
    {
        private readonly BootstrapWorldGraphBuilder worldGraphBuilder;
        private readonly BootstrapWorldStateSeeder worldStateSeeder;
        private readonly PersistentPlayableCharacterInitializer playableCharacterInitializer;

        public BootstrapWorldMapFactory(
            BootstrapWorldGraphBuilder worldGraphBuilder = null,
            BootstrapWorldStateSeeder worldStateSeeder = null,
            PersistentPlayableCharacterInitializer playableCharacterInitializer = null)
        {
            this.worldGraphBuilder = worldGraphBuilder ?? new BootstrapWorldGraphBuilder();
            this.worldStateSeeder = worldStateSeeder ?? new BootstrapWorldStateSeeder();
            this.playableCharacterInitializer = playableCharacterInitializer ?? new PersistentPlayableCharacterInitializer();
        }

        public WorldGraph CreateWorldGraph()
        {
            return worldGraphBuilder.Create();
        }

        public PersistentGameState CreateGameState()
        {
            PersistentGameState gameState = worldStateSeeder.Create();
            playableCharacterInitializer.EnsureInitialized(gameState);
            return gameState;
        }
    }
}
