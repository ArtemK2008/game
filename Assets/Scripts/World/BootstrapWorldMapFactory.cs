namespace Survivalon.Runtime
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
