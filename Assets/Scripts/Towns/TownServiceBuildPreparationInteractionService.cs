using System;
using Survivalon.Data.Characters;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    public sealed class TownServiceBuildPreparationInteractionService
    {
        private readonly PlayableCharacterGearAssignmentService gearAssignmentService;
        private readonly SafeResumePersistenceService persistenceService;
        private readonly PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;

        public TownServiceBuildPreparationInteractionService(
            SafeResumePersistenceService persistenceService = null,
            PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService = null,
            PlayableCharacterGearAssignmentService gearAssignmentService = null)
        {
            this.persistenceService = persistenceService;
            this.skillPackageAssignmentService = skillPackageAssignmentService ??
                new PlayableCharacterSkillPackageAssignmentService();
            this.gearAssignmentService = gearAssignmentService ??
                new PlayableCharacterGearAssignmentService();
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

        public bool TryApplyGearAssignment(
            PersistentGameState gameState,
            PlayableCharacterGearAssignmentOption gearAssignmentOption)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (gearAssignmentOption == null)
            {
                throw new ArgumentNullException(nameof(gearAssignmentOption));
            }

            bool changed = gearAssignmentOption.IsEquipped
                ? gearAssignmentService.TryClearSelectedCharacterGear(gameState, gearAssignmentOption.GearCategory)
                : gearAssignmentService.TryAssignSelectedCharacterGear(gameState, gearAssignmentOption.GearId);
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
