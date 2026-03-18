using System;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Run
{
    public sealed class RunPersistentContext
    {
        public RunPersistentContext(
            PersistentWorldState persistentWorldState = null,
            ResourceBalancesState resourceBalancesState = null,
            PersistentProgressionState persistentProgressionState = null,
            PlayableCharacterProfile playableCharacter = null,
            PersistentCharacterState playableCharacterState = null)
        {
            PersistentWorldState = persistentWorldState;
            ResourceBalancesState = resourceBalancesState;
            PersistentProgressionState = persistentProgressionState;
            PlayableCharacter = playableCharacter;
            PlayableCharacterState = playableCharacterState;
        }

        public PersistentWorldState PersistentWorldState { get; }

        public ResourceBalancesState ResourceBalancesState { get; }

        public PersistentProgressionState PersistentProgressionState { get; }

        public PlayableCharacterProfile PlayableCharacter { get; }

        public PersistentCharacterState PlayableCharacterState { get; }

        public static RunPersistentContext FromGameState(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PlayableCharacterResolver characterResolver = new PlayableCharacterResolver();
            PersistentCharacterState playableCharacterState = characterResolver.ResolveCurrentState(gameState);
            PlayableCharacterProfile playableCharacter = PlayableCharacterCatalog.Get(playableCharacterState.CharacterId);

            return new RunPersistentContext(
                gameState.WorldState,
                gameState.ResourceBalances,
                gameState.ProgressionState,
                playableCharacter,
                playableCharacterState);
        }
    }
}

