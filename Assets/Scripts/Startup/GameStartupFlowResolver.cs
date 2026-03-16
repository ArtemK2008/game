using System;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.Startup
{
    public sealed class GameStartupFlowResolver
    {
        public StartupEntryTarget ResolveInitialEntryTarget(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (gameState.SafeResumeState.HasSafeResumeTarget)
            {
                return StartupEntryTarget.WorldViewPlaceholder;
            }

            PersistentWorldState worldState = gameState.WorldState;
            if (worldState.HasLastSafeNode || worldState.HasCurrentNode)
            {
                return StartupEntryTarget.WorldViewPlaceholder;
            }

            return StartupEntryTarget.MainMenuPlaceholder;
        }
    }
}
