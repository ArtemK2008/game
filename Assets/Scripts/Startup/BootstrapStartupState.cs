using System;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Startup
{
    public sealed class BootstrapStartupState
    {
        public BootstrapStartupState(
            WorldGraph worldGraph,
            PersistentGameState gameState,
            WorldNodeEntryFlowController nodeEntryFlowController,
            SessionContextState sessionContext,
            StartupEntryTarget entryTarget)
        {
            WorldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            GameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            NodeEntryFlowController = nodeEntryFlowController ?? throw new ArgumentNullException(nameof(nodeEntryFlowController));
            SessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
            EntryTarget = entryTarget;
        }

        public WorldGraph WorldGraph { get; }

        public PersistentGameState GameState { get; }

        public WorldNodeEntryFlowController NodeEntryFlowController { get; }

        public SessionContextState SessionContext { get; }

        public StartupEntryTarget EntryTarget { get; }
    }
}

