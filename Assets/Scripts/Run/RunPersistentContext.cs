using System;
using Survivalon.Characters;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Run
{
    /// <summary>
    /// Хранит persistent-срез мира, ресурсов и выбранного персонажа для текущего run.
    /// </summary>
    public sealed class RunPersistentContext
    {
        public RunPersistentContext(
            PersistentWorldState persistentWorldState = null,
            ResourceBalancesState resourceBalancesState = null,
            PersistentProgressionState persistentProgressionState = null,
            PlayableCharacterProfile playableCharacter = null,
            PersistentCharacterState playableCharacterState = null,
            PersistentGameState persistentGameState = null)
        {
            PersistentWorldState = persistentWorldState;
            ResourceBalancesState = resourceBalancesState;
            PersistentProgressionState = persistentProgressionState;
            PlayableCharacter = playableCharacter;
            PlayableCharacterState = playableCharacterState;
            PersistentGameState = persistentGameState;
        }

        public PersistentWorldState PersistentWorldState { get; }

        public ResourceBalancesState ResourceBalancesState { get; }

        public PersistentProgressionState PersistentProgressionState { get; }

        public PlayableCharacterProfile PlayableCharacter { get; }

        public PersistentCharacterState PlayableCharacterState { get; }

        public PersistentGameState PersistentGameState { get; }

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
                playableCharacterState,
                gameState);
        }
    }
}

