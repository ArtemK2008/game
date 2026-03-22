using System;
using Survivalon.Characters;
using Survivalon.Data.Characters;

namespace Survivalon.State.Persistence
{
    /// <summary>
    /// Нормализует persistent state играбельных персонажей и их package/gear baseline на старте.
    /// </summary>
    public sealed class PersistentPlayableCharacterInitializer
    {
        private readonly PlayableCharacterSelectionService selectionService;
        private readonly PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;
        private readonly PersistentGearStateInitializer gearStateInitializer;

        public PersistentPlayableCharacterInitializer(
            PlayableCharacterSelectionService selectionService = null,
            PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService = null,
            PersistentGearStateInitializer gearStateInitializer = null)
        {
            this.selectionService = selectionService ?? new PlayableCharacterSelectionService();
            this.skillPackageAssignmentService =
                skillPackageAssignmentService ?? new PlayableCharacterSkillPackageAssignmentService(this.selectionService);
            this.gearStateInitializer = gearStateInitializer ?? new PersistentGearStateInitializer();
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
            gearStateInitializer.EnsureInitialized(gameState);
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

