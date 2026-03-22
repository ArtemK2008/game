using System;
using System.Collections.Generic;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Characters
{
    /// <summary>
    /// Управляет назначением skill package для выбранного персонажа без смешивания с authored catalog.
    /// </summary>
    public sealed class PlayableCharacterSkillPackageAssignmentService
    {
        private readonly PlayableCharacterSelectionService selectionService;

        public PlayableCharacterSkillPackageAssignmentService(PlayableCharacterSelectionService selectionService = null)
        {
            this.selectionService = selectionService ?? new PlayableCharacterSelectionService();
        }

        public IReadOnlyList<PlayableCharacterSkillPackageOption> BuildOptionsForSelectedCharacter(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedCharacterState = selectionService.ResolveSelectedState(gameState);
            PlayableCharacterProfile selectedCharacter = PlayableCharacterCatalog.Get(selectedCharacterState.CharacterId);
            IReadOnlyList<PlayableCharacterSkillPackageDefinition> availableDefinitions =
                PlayableCharacterSkillPackageCatalog.GetDefinitions(selectedCharacter.CharacterId);
            string assignedSkillPackageId = ResolveAssignedSkillPackageId(selectedCharacter, selectedCharacterState);
            List<PlayableCharacterSkillPackageOption> resolvedOptions =
                new List<PlayableCharacterSkillPackageOption>(availableDefinitions.Count);

            for (int index = 0; index < availableDefinitions.Count; index++)
            {
                PlayableCharacterSkillPackageDefinition availableDefinition = availableDefinitions[index];
                resolvedOptions.Add(new PlayableCharacterSkillPackageOption(
                    availableDefinition.CharacterId,
                    availableDefinition.SkillPackageId,
                    availableDefinition.DisplayName,
                    availableDefinition.Summary,
                    isAssigned: availableDefinition.SkillPackageId == assignedSkillPackageId));
            }

            return resolvedOptions;
        }

        public string ResolveSelectedCharacterDisplayName(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedCharacterState = selectionService.ResolveSelectedState(gameState);
            return PlayableCharacterCatalog.Get(selectedCharacterState.CharacterId).DisplayName;
        }

        public bool TryAssignSelectedCharacterSkillPackage(PersistentGameState gameState, string skillPackageId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentCharacterState selectedCharacterState = selectionService.ResolveSelectedState(gameState);
            return TryAssignSkillPackage(gameState, selectedCharacterState.CharacterId, skillPackageId);
        }

        public bool TryAssignSkillPackage(PersistentGameState gameState, string characterId, string skillPackageId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (string.IsNullOrWhiteSpace(characterId))
            {
                throw new ArgumentException("Character id cannot be null or whitespace.", nameof(characterId));
            }

            if (string.IsNullOrWhiteSpace(skillPackageId))
            {
                throw new ArgumentException("Skill package id cannot be null or whitespace.", nameof(skillPackageId));
            }

            if (!gameState.TryGetCharacterState(characterId, out PersistentCharacterState characterState) ||
                !PlayableCharacterCatalog.Contains(characterId) ||
                !PlayableCharacterSkillPackageCatalog.Contains(characterId, skillPackageId))
            {
                return false;
            }

            characterState.SetSkillPackageId(skillPackageId);
            return true;
        }

        public void EnsureValidAssignments(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            for (int index = 0; index < gameState.CharacterStates.Count; index++)
            {
                PersistentCharacterState characterState = gameState.CharacterStates[index];
                if (characterState == null || !PlayableCharacterCatalog.Contains(characterState.CharacterId))
                {
                    continue;
                }

                PlayableCharacterProfile characterProfile = PlayableCharacterCatalog.Get(characterState.CharacterId);
                string resolvedSkillPackageId = ResolveAssignedSkillPackageId(characterProfile, characterState);
                if (characterState.SkillPackageId != resolvedSkillPackageId)
                {
                    characterState.SetSkillPackageId(resolvedSkillPackageId);
                }
            }
        }

        private static string ResolveAssignedSkillPackageId(
            PlayableCharacterProfile characterProfile,
            PersistentCharacterState characterState)
        {
            if (PlayableCharacterSkillPackageCatalog.Contains(characterProfile.CharacterId, characterState.SkillPackageId))
            {
                return characterState.SkillPackageId;
            }

            if (PlayableCharacterSkillPackageCatalog.Contains(
                characterProfile.CharacterId,
                characterProfile.DefaultSkillPackageId))
            {
                return characterProfile.DefaultSkillPackageId;
            }

            IReadOnlyList<PlayableCharacterSkillPackageDefinition> fallbackDefinitions =
                PlayableCharacterSkillPackageCatalog.GetDefinitions(characterProfile.CharacterId);
            if (fallbackDefinitions.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No valid skill packages are configured for character '{characterProfile.CharacterId}'.");
            }

            return fallbackDefinitions[0].SkillPackageId;
        }
    }
}
