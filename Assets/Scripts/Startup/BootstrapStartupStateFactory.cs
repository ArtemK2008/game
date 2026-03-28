using System;
using Survivalon.Core;
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

            PersistentGameState gameState = persistenceService.LoadOrCreate(worldMapFactory.CreateGameState());
            return CreateFromGameState(worldMapFactory, gameState);
        }

        public BootstrapStartupState CreateFresh(BootstrapWorldMapFactory worldMapFactory)
        {
            if (worldMapFactory == null)
            {
                throw new ArgumentNullException(nameof(worldMapFactory));
            }

            PersistentGameState gameState = worldMapFactory.CreateGameState();
            return CreateFromGameState(worldMapFactory, gameState);
        }

        public bool TryCreateContinue(
            BootstrapWorldMapFactory worldMapFactory,
            out BootstrapStartupState startupState)
        {
            if (worldMapFactory == null)
            {
                throw new ArgumentNullException(nameof(worldMapFactory));
            }

            if (!persistenceService.TryLoadExisting(out PersistentGameState persistedGameState))
            {
                startupState = null;
                return false;
            }

            startupState = CreateFromGameState(worldMapFactory, persistedGameState);
            if (!HasSupportedContinueTarget(startupState))
            {
                startupState = null;
                return false;
            }

            return true;
        }

        private BootstrapStartupState CreateFromGameState(
            BootstrapWorldMapFactory worldMapFactory,
            PersistentGameState gameState)
        {
            WorldGraph worldGraph = worldMapFactory.CreateWorldGraph();
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

        private static bool HasSupportedContinueTarget(BootstrapStartupState startupState)
        {
            if (startupState == null)
            {
                throw new ArgumentNullException(nameof(startupState));
            }

            PersistentSafeResumeState safeResumeState = startupState.GameState.SafeResumeState;
            if (safeResumeState.HasSafeResumeTarget)
            {
                switch (safeResumeState.TargetType)
                {
                    case SafeResumeTargetType.WorldMap:
                        return true;
                    case SafeResumeTargetType.TownService:
                        return startupState.WorldGraph.GetNode(safeResumeState.ResumeNodeId).NodeType ==
                               NodeType.ServiceOrProgression;
                    default:
                        return false;
                }
            }

            PersistentWorldState worldState = startupState.GameState.WorldState;
            return worldState.HasCurrentNode || worldState.HasLastSafeNode;
        }
    }
}

