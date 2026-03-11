using System;

namespace Survivalon.Runtime
{
    public sealed class GameStartupFlowResolver
    {
        public StartupEntryTarget ResolveInitialEntryTarget(PersistentWorldState worldState)
        {
            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (worldState.HasLastSafeNode || worldState.HasCurrentNode)
            {
                return StartupEntryTarget.WorldViewPlaceholder;
            }

            return StartupEntryTarget.MainMenuPlaceholder;
        }
    }
}
