using System;
using Survivalon.Characters;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    /// <summary>
    /// Оркестрирует build-prep изменения на карте мира и сразу передает их в safe-resume persistence.
    /// </summary>
    public sealed class WorldMapBuildPreparationInteractionService
    {
        private readonly SafeResumePersistenceService persistenceService;
        private readonly PlayableCharacterSelectionService characterSelectionService;
        private readonly PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;
        private readonly PlayableCharacterGearAssignmentService gearAssignmentService;

        public WorldMapBuildPreparationInteractionService(
            SafeResumePersistenceService persistenceService = null,
            PlayableCharacterSelectionService characterSelectionService = null,
            PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService = null,
            PlayableCharacterGearAssignmentService gearAssignmentService = null)
        {
            this.persistenceService = persistenceService;
            this.characterSelectionService = characterSelectionService ?? new PlayableCharacterSelectionService();
            this.skillPackageAssignmentService = skillPackageAssignmentService ??
                new PlayableCharacterSkillPackageAssignmentService(this.characterSelectionService);
            this.gearAssignmentService = gearAssignmentService ??
                new PlayableCharacterGearAssignmentService(this.characterSelectionService);
        }

        public bool TrySelectCharacter(PersistentGameState gameState, string characterId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            bool changed = characterSelectionService.TrySelectCharacter(gameState, characterId);
            PersistIfChanged(gameState, changed);
            return changed;
        }

        public bool TryAssignSkillPackage(PersistentGameState gameState, string skillPackageId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            bool changed = skillPackageAssignmentService.TryAssignSelectedCharacterSkillPackage(
                gameState,
                skillPackageId);
            PersistIfChanged(gameState, changed);
            return changed;
        }

        public bool TryAssignGear(PersistentGameState gameState, string gearId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            bool changed = gearAssignmentService.TryAssignSelectedCharacterGear(gameState, gearId);
            PersistIfChanged(gameState, changed);
            return changed;
        }

        public bool TryClearGear(PersistentGameState gameState, GearCategory gearCategory)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            bool changed = gearAssignmentService.TryClearSelectedCharacterGear(gameState, gearCategory);
            PersistIfChanged(gameState, changed);
            return changed;
        }

        private void PersistIfChanged(PersistentGameState gameState, bool changed)
        {
            if (!changed || persistenceService == null)
            {
                return;
            }

            persistenceService.SaveResolvedWorldContext(gameState);
        }
    }
}
