using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.Data.Characters
{
    public sealed class PlayableCharacterResolver
    {
        public PlayableCharacterProfile ResolveCurrent(PersistentGameState gameState)
        {
            return PlayableCharacterCatalog.Get(ResolveCurrentState(gameState).CharacterId);
        }

        public PersistentCharacterState ResolveCurrentState(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState fallbackState = null;

            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                if (!PlayableCharacterCatalog.Contains(characterState.CharacterId) ||
                    !characterState.IsUnlocked ||
                    !characterState.IsSelectable)
                {
                    continue;
                }

                fallbackState ??= characterState;
                if (characterState.IsActive)
                {
                    return characterState;
                }
            }

            return fallbackState ?? throw new InvalidOperationException("No playable character is available in persistent game state.");
        }
    }
}
