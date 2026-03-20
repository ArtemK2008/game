using System;
using System.Collections.Generic;
using Survivalon.State.Persistence;

namespace Survivalon.Data.Characters
{
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
            IReadOnlyList<PlayableCharacterSkillPackageOption> availableOptions =
                PlayableCharacterSkillPackageCatalog.GetOptions(selectedCharacter.CharacterId);
            string assignedSkillPackageId = ResolveAssignedSkillPackageId(selectedCharacter, selectedCharacterState);
            List<PlayableCharacterSkillPackageOption> resolvedOptions =
                new List<PlayableCharacterSkillPackageOption>(availableOptions.Count);

            for (int index = 0; index < availableOptions.Count; index++)
            {
                PlayableCharacterSkillPackageOption availableOption = availableOptions[index];
                resolvedOptions.Add(new PlayableCharacterSkillPackageOption(
                    availableOption.CharacterId,
                    availableOption.SkillPackageId,
                    availableOption.DisplayName,
                    availableOption.Summary,
                    isAssigned: availableOption.SkillPackageId == assignedSkillPackageId));
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

            IReadOnlyList<PlayableCharacterSkillPackageOption> fallbackOptions =
                PlayableCharacterSkillPackageCatalog.GetOptions(characterProfile.CharacterId);
            if (fallbackOptions.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No valid skill packages are configured for character '{characterProfile.CharacterId}'.");
            }

            return fallbackOptions[0].SkillPackageId;
        }
    }
}
