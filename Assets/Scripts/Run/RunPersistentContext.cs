using System;

namespace Survivalon.Runtime
{
    public sealed class RunPersistentContext
    {
        public RunPersistentContext(
            PersistentWorldState persistentWorldState = null,
            ResourceBalancesState resourceBalancesState = null,
            PersistentProgressionState persistentProgressionState = null)
        {
            PersistentWorldState = persistentWorldState;
            ResourceBalancesState = resourceBalancesState;
            PersistentProgressionState = persistentProgressionState;
        }

        public PersistentWorldState PersistentWorldState { get; }

        public ResourceBalancesState ResourceBalancesState { get; }

        public PersistentProgressionState PersistentProgressionState { get; }

        public static RunPersistentContext FromGameState(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            return new RunPersistentContext(
                gameState.WorldState,
                gameState.ResourceBalances,
                gameState.ProgressionState);
        }
    }
}
