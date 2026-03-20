using System;
using Survivalon.Data.Characters;

namespace Survivalon.State.Persistence
{
    public sealed class PersistentPlayableCharacterInitializer
    {
        private readonly PlayableCharacterSelectionService selectionService;
        private readonly PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;

        public PersistentPlayableCharacterInitializer(
            PlayableCharacterSelectionService selectionService = null,
            PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService = null)
        {
            this.selectionService = selectionService ?? new PlayableCharacterSelectionService();
            this.skillPackageAssignmentService =
                skillPackageAssignmentService ?? new PlayableCharacterSkillPackageAssignmentService(this.selectionService);
        }

        public void EnsureInitialized(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            for (int index = 0; index < PlayableCharacterCatalog.All.Count; index++)
            {
                EnsureCharacterState(gameState, PlayableCharacterCatalog.All[index]);
            }

            skillPackageAssignmentService.EnsureValidAssignments(gameState);
            selectionService.EnsureValidSelection(gameState);
        }

        private static void EnsureCharacterState(PersistentGameState gameState, PlayableCharacterProfile characterProfile)
        {
            if (!gameState.TryGetCharacterState(characterProfile.CharacterId, out PersistentCharacterState characterState))
            {
                characterState = new PersistentCharacterState(
                    characterProfile.CharacterId,
                    isUnlocked: true,
                    isSelectable: true,
                    isActive: false,
                    skillPackageId: characterProfile.DefaultSkillPackageId);
                gameState.AddCharacterState(characterState);
            }

            characterState.Unlock();
            characterState.SetSelectable(true);
        }
    }
}

