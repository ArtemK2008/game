namespace Survivalon.Runtime
{
    public sealed class BootstrapWorldMapFactory
    {
        private readonly BootstrapWorldGraphBuilder worldGraphBuilder;
        private readonly BootstrapWorldStateSeeder worldStateSeeder;

        public BootstrapWorldMapFactory(
            BootstrapWorldGraphBuilder worldGraphBuilder = null,
            BootstrapWorldStateSeeder worldStateSeeder = null)
        {
            this.worldGraphBuilder = worldGraphBuilder ?? new BootstrapWorldGraphBuilder();
            this.worldStateSeeder = worldStateSeeder ?? new BootstrapWorldStateSeeder();
        }

        public WorldGraph CreateWorldGraph()
        {
            return worldGraphBuilder.Create();
        }

        public PersistentGameState CreateGameState()
        {
            return worldStateSeeder.Create();
        }
    }
}
