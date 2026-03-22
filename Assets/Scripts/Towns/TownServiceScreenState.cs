using System;
using System.Collections.Generic;
using Survivalon.Core;
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
            IReadOnlyList<TownServiceProgressionOptionState> progressionOptions,
            string selectedCharacterDisplayName,
            string assignedSkillPackageDisplayName,
            string primaryGearDisplayName,
            string supportGearDisplayName)
        {
            ServiceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            ProgressionOptions = progressionOptions ?? throw new ArgumentNullException(nameof(progressionOptions));
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
        }

        public TownServiceContextDefinition ServiceContext { get; }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeId OriginNodeId { get; }

        public int PersistentProgressionMaterialAmount { get; }

        public IReadOnlyList<TownServiceProgressionOptionState> ProgressionOptions { get; }

        public string SelectedCharacterDisplayName { get; }

        public string AssignedSkillPackageDisplayName { get; }

        public string PrimaryGearDisplayName { get; }

        public string SupportGearDisplayName { get; }
    }
}
