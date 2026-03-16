using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
{
    public sealed class RunPersistentContext
    {
        public RunPersistentContext(
            PersistentWorldState persistentWorldState = null,
            ResourceBalancesState resourceBalancesState = null,
            PersistentProgressionState persistentProgressionState = null,
            PlayableCharacterProfile playableCharacter = null)
        {
            PersistentWorldState = persistentWorldState;
            ResourceBalancesState = resourceBalancesState;
            PersistentProgressionState = persistentProgressionState;
            PlayableCharacter = playableCharacter;
        }

        public PersistentWorldState PersistentWorldState { get; }

        public ResourceBalancesState ResourceBalancesState { get; }

        public PersistentProgressionState PersistentProgressionState { get; }

        public PlayableCharacterProfile PlayableCharacter { get; }

        public static RunPersistentContext FromGameState(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PlayableCharacterProfile playableCharacter = new PlayableCharacterResolver().ResolveCurrent(gameState);

            return new RunPersistentContext(
                gameState.WorldState,
                gameState.ResourceBalances,
                gameState.ProgressionState,
                playableCharacter);
        }
    }
}
