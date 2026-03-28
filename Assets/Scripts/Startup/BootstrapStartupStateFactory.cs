using System;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Startup
{
    public sealed class BootstrapStartupStateFactory
    {
        private readonly SafeResumePersistenceService persistenceService;
        private readonly GameStartupFlowResolver startupFlowResolver;
        private readonly PersistentPlayableCharacterInitializer playableCharacterInitializer;
        private readonly PersistentWorldStateStartupNormalizer worldStateStartupNormalizer;

        public BootstrapStartupStateFactory(
            SafeResumePersistenceService persistenceService,
            GameStartupFlowResolver startupFlowResolver = null,
            PersistentPlayableCharacterInitializer playableCharacterInitializer = null,
            PersistentWorldStateStartupNormalizer worldStateStartupNormalizer = null)
        {
            this.persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
            this.startupFlowResolver = startupFlowResolver ?? new GameStartupFlowResolver();
            this.playableCharacterInitializer = playableCharacterInitializer ?? new PersistentPlayableCharacterInitializer();
            this.worldStateStartupNormalizer = worldStateStartupNormalizer ?? new PersistentWorldStateStartupNormalizer();
        }

        public BootstrapStartupState Create(BootstrapWorldMapFactory worldMapFactory)
        {
            if (worldMapFactory == null)
            {
                throw new ArgumentNullException(nameof(worldMapFactory));
            }

            WorldGraph worldGraph = worldMapFactory.CreateWorldGraph();
            PersistentGameState gameState = persistenceService.LoadOrCreate(worldMapFactory.CreateGameState());
            playableCharacterInitializer.EnsureInitialized(gameState);
            worldStateStartupNormalizer.Normalize(worldGraph, gameState.WorldState);
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

