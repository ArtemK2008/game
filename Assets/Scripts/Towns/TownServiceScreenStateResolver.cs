using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Towns
{
    public sealed class TownServiceScreenStateResolver
    {
        private readonly PlayableCharacterSelectionService characterSelectionService;
        private readonly AccountWideProgressionBoardService progressionBoardService;
        private readonly PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;
        private readonly PlayableCharacterGearAssignmentService gearAssignmentService;

        public TownServiceScreenStateResolver(
            PlayableCharacterSelectionService characterSelectionService = null,
            AccountWideProgressionBoardService progressionBoardService = null,
            PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService = null,
            PlayableCharacterGearAssignmentService gearAssignmentService = null)
        {
            this.characterSelectionService = characterSelectionService ?? new PlayableCharacterSelectionService();
            this.progressionBoardService = progressionBoardService ?? new AccountWideProgressionBoardService();
            this.skillPackageAssignmentService = skillPackageAssignmentService ??
                new PlayableCharacterSkillPackageAssignmentService(this.characterSelectionService);
            this.gearAssignmentService = gearAssignmentService ??
                new PlayableCharacterGearAssignmentService(this.characterSelectionService);
        }

        public TownServiceScreenState Resolve(NodePlaceholderState placeholderState, PersistentGameState gameState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (placeholderState.NodeType != NodeType.ServiceOrProgression)
            {
                throw new ArgumentException(
                    "Town service screen state requires a service-or-progression node context.",
                    nameof(placeholderState));
            }

            if (placeholderState.TownServiceContext == null)
            {
                throw new ArgumentException(
                    "Town service screen state requires a concrete town service context definition.",
                    nameof(placeholderState));
            }

            PersistentCharacterState selectedCharacterState = characterSelectionService.ResolveSelectedState(gameState);
            PlayableCharacterProfile selectedCharacter = PlayableCharacterCatalog.Get(selectedCharacterState.CharacterId);
            PlayableCharacterSkillPackageOption assignedSkillPackage = PlayableCharacterSkillPackageCatalog.Get(
                selectedCharacterState.CharacterId,
                selectedCharacterState.SkillPackageId);

            return new TownServiceScreenState(
                placeholderState.TownServiceContext,
                placeholderState.NodeId,
                placeholderState.RegionId,
                placeholderState.OriginNodeId,
                gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                gameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial),
                BuildProgressionOptions(gameState),
                BuildConversionOptions(gameState),
                skillPackageAssignmentService.BuildOptionsForSelectedCharacter(gameState),
                BuildGearAssignmentOptions(gameState),
                selectedCharacter.DisplayName,
                assignedSkillPackage.DisplayName,
                ResolveEquippedGearDisplayName(selectedCharacterState, GearCategory.PrimaryCombat),
                ResolveEquippedGearDisplayName(selectedCharacterState, GearCategory.SecondarySupport));
        }

        private IReadOnlyList<TownServiceProgressionOptionState> BuildProgressionOptions(PersistentGameState gameState)
        {
            List<TownServiceProgressionOptionState> progressionOptions = new List<TownServiceProgressionOptionState>();

            for (int index = 0; index < AccountWideProgressionUpgradeCatalog.All.Count; index++)
            {
                AccountWideProgressionUpgradeDefinition upgradeDefinition =
                    AccountWideProgressionUpgradeCatalog.All[index];
                progressionOptions.Add(new TownServiceProgressionOptionState(
                    upgradeDefinition.UpgradeId,
                    upgradeDefinition.DisplayName,
                    upgradeDefinition.CostResourceCategory,
                    upgradeDefinition.CostAmount,
                    progressionBoardService.IsPurchased(gameState, upgradeDefinition.UpgradeId),
                    progressionBoardService.CanPurchase(gameState, upgradeDefinition.UpgradeId)));
            }

            return progressionOptions;
        }

        private static IReadOnlyList<TownServiceConversionOptionState> BuildConversionOptions(PersistentGameState gameState)
        {
            List<TownServiceConversionOptionState> conversionOptions = new List<TownServiceConversionOptionState>();

            for (int index = 0; index < TownServiceConversionCatalog.All.Count; index++)
            {
                TownServiceConversionDefinition conversionDefinition = TownServiceConversionCatalog.All[index];
                int availableInputAmount =
                    gameState.ResourceBalances.GetAmount(conversionDefinition.InputResourceCategory);
                conversionOptions.Add(new TownServiceConversionOptionState(
                    conversionDefinition.ConversionId,
                    conversionDefinition.DisplayName,
                    conversionDefinition.InputResourceCategory,
                    conversionDefinition.InputAmount,
                    conversionDefinition.OutputResourceCategory,
                    conversionDefinition.OutputAmount,
                    availableInputAmount,
                    availableInputAmount >= conversionDefinition.InputAmount));
            }

            return conversionOptions;
        }

        private IReadOnlyList<PlayableCharacterGearAssignmentOption> BuildGearAssignmentOptions(PersistentGameState gameState)
        {
            List<PlayableCharacterGearAssignmentOption> gearAssignmentOptions =
                new List<PlayableCharacterGearAssignmentOption>();
            AppendGearAssignmentOptions(gearAssignmentOptions, gameState, GearCategory.PrimaryCombat);
            AppendGearAssignmentOptions(gearAssignmentOptions, gameState, GearCategory.SecondarySupport);
            return gearAssignmentOptions;
        }

        private void AppendGearAssignmentOptions(
            List<PlayableCharacterGearAssignmentOption> targetOptions,
            PersistentGameState gameState,
            GearCategory gearCategory)
        {
            IReadOnlyList<PlayableCharacterGearAssignmentOption> categoryOptions =
                gearAssignmentService.BuildOptionsForSelectedCharacter(gameState, gearCategory);

            for (int index = 0; index < categoryOptions.Count; index++)
            {
                targetOptions.Add(categoryOptions[index]);
            }
        }

        private static string ResolveEquippedGearDisplayName(
            PersistentCharacterState characterState,
            GearCategory gearCategory)
        {
            if (!characterState.LoadoutState.TryGetEquippedGearState(gearCategory, out EquippedGearState equippedGearState))
            {
                return "none";
            }

            return GearCatalog.Get(equippedGearState.GearId).DisplayName;
        }
    }
}
