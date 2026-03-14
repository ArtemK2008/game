using System;

namespace Survivalon.Runtime
{
    public sealed class BootstrapStartupStateFactory
    {
        private readonly SafeResumePersistenceService persistenceService;
        private readonly GameStartupFlowResolver startupFlowResolver;

        public BootstrapStartupStateFactory(
            SafeResumePersistenceService persistenceService,
            GameStartupFlowResolver startupFlowResolver = null)
        {
            this.persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
            this.startupFlowResolver = startupFlowResolver ?? new GameStartupFlowResolver();
        }

        public BootstrapStartupState Create(BootstrapWorldMapFactory worldMapFactory)
        {
            if (worldMapFactory == null)
            {
                throw new ArgumentNullException(nameof(worldMapFactory));
            }

            WorldGraph worldGraph = worldMapFactory.CreateWorldGraph();
            PersistentGameState gameState = persistenceService.LoadOrCreate(worldMapFactory.CreateGameState());
            WorldNodeEntryFlowController nodeEntryFlowController = new WorldNodeEntryFlowController(worldGraph, gameState.WorldState);
            SessionContextState sessionContext = new SessionContextState();
            sessionContext.SeedFromWorldState(gameState.WorldState);
            StartupEntryTarget entryTarget = startupFlowResolver.ResolveInitialEntryTarget(gameState);

            return new BootstrapStartupState(
                worldGraph,
                gameState,
                nodeEntryFlowController,
                sessionContext,
                entryTarget);
        }
    }
}
