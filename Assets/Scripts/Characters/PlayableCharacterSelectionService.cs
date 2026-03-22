using System;
using System.Collections.Generic;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Characters
{
    /// <summary>
    /// Разрешает текущий выбор играбельного персонажа и безопасно применяет его в persistent state.
    /// </summary>
    public sealed class PlayableCharacterSelectionService
    {
        public IReadOnlyList<PlayableCharacterSelectionOption> BuildSelectableOptions(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedState = ResolveSelectedState(gameState);
            List<PlayableCharacterSelectionOption> selectionOptions = new List<PlayableCharacterSelectionOption>();

            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                if (!IsSelectablePlayableCharacter(characterState))
                {
                    continue;
                }

                PlayableCharacterProfile characterProfile = PlayableCharacterCatalog.Get(characterState.CharacterId);
                selectionOptions.Add(new PlayableCharacterSelectionOption(
                    characterProfile.CharacterId,
                    characterProfile.DisplayName,
                    characterState.CharacterId == selectedState.CharacterId));
            }

            return selectionOptions;
        }

        public PersistentCharacterState ResolveSelectedState(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState fallbackState = null;

            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                if (!IsSelectablePlayableCharacter(characterState))
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

        public bool TrySelectCharacter(PersistentGameState gameState, string characterId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (!TryResolveSelectableCharacter(gameState, characterId, out PersistentCharacterState selectedState))
            {
                return false;
            }

            ApplySelectedCharacter(gameState, selectedState.CharacterId);
            return true;
        }

        public void EnsureValidSelection(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedState = ResolveSelectedState(gameState);
            ApplySelectedCharacter(gameState, selectedState.CharacterId);
        }

        private static bool TryResolveSelectableCharacter(
            PersistentGameState gameState,
            string characterId,
            out PersistentCharacterState selectedState)
        {
            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                if (characterState.CharacterId == characterId && IsSelectablePlayableCharacter(characterState))
                {
                    selectedState = characterState;
                    return true;
                }
            }

            selectedState = null;
            return false;
        }

        private static void ApplySelectedCharacter(PersistentGameState gameState, string selectedCharacterId)
        {
            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                bool shouldBeActive = IsSelectablePlayableCharacter(characterState) &&
                    characterState.CharacterId == selectedCharacterId;
                characterState.SetActive(shouldBeActive);
            }
        }

        private static bool IsSelectablePlayableCharacter(PersistentCharacterState characterState)
        {
            return characterState != null &&
                PlayableCharacterCatalog.Contains(characterState.CharacterId) &&
                characterState.IsUnlocked &&
                characterState.IsSelectable;
        }
    }
}
