using System;
using Survivalon.Characters;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    /// <summary>
    /// Оркестрирует package и gear-подготовку в town/service shell с немедленным persistence handoff.
    /// </summary>
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
