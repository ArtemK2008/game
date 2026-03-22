using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Towns;

namespace Survivalon.Towns
{
    public sealed class TownServiceScreenState
    {
        public TownServiceScreenState(
            TownServiceContextDefinition serviceContext,
            NodeId nodeId,
            RegionId regionId,
            NodeId originNodeId,
            int persistentProgressionMaterialAmount,
            int regionMaterialAmount,
            TownServiceMaterialPowerPathState materialPowerPathState,
            IReadOnlyList<TownServiceProgressionOptionState> progressionOptions,
            IReadOnlyList<TownServiceConversionOptionState> conversionOptions,
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions,
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions,
            string selectedCharacterDisplayName,
            string assignedSkillPackageDisplayName,
            string primaryGearDisplayName,
            string supportGearDisplayName)
        {
            ServiceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            MaterialPowerPath = materialPowerPathState ?? throw new ArgumentNullException(nameof(materialPowerPathState));
            ProgressionOptions = progressionOptions ?? throw new ArgumentNullException(nameof(progressionOptions));
            ConversionOptions = conversionOptions ?? throw new ArgumentNullException(nameof(conversionOptions));
            SkillPackageOptions = skillPackageOptions ?? throw new ArgumentNullException(nameof(skillPackageOptions));
            GearAssignmentOptions = gearAssignmentOptions ?? throw new ArgumentNullException(nameof(gearAssignmentOptions));
            SelectedCharacterDisplayName = selectedCharacterDisplayName ??
                throw new ArgumentNullException(nameof(selectedCharacterDisplayName));
            AssignedSkillPackageDisplayName = assignedSkillPackageDisplayName ??
                throw new ArgumentNullException(nameof(assignedSkillPackageDisplayName));
            PrimaryGearDisplayName = primaryGearDisplayName ??
                throw new ArgumentNullException(nameof(primaryGearDisplayName));
            SupportGearDisplayName = supportGearDisplayName ??
                throw new ArgumentNullException(nameof(supportGearDisplayName));
            NodeId = nodeId;
            RegionId = regionId;
            OriginNodeId = originNodeId;
            PersistentProgressionMaterialAmount = persistentProgressionMaterialAmount;
            RegionMaterialAmount = regionMaterialAmount;
        }

        public TownServiceContextDefinition ServiceContext { get; }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeId OriginNodeId { get; }

        public int PersistentProgressionMaterialAmount { get; }

        public int RegionMaterialAmount { get; }

        public TownServiceMaterialPowerPathState MaterialPowerPath { get; }

        public IReadOnlyList<TownServiceProgressionOptionState> ProgressionOptions { get; }

        public IReadOnlyList<TownServiceConversionOptionState> ConversionOptions { get; }

        public IReadOnlyList<PlayableCharacterSkillPackageOption> SkillPackageOptions { get; }

        public IReadOnlyList<PlayableCharacterGearAssignmentOption> GearAssignmentOptions { get; }

        public string SelectedCharacterDisplayName { get; }

        public string AssignedSkillPackageDisplayName { get; }

        public string PrimaryGearDisplayName { get; }

        public string SupportGearDisplayName { get; }
    }
}
