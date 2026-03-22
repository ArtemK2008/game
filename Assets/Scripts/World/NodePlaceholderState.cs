using System;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Towns;
using Survivalon.Data.World;

namespace Survivalon.World
{
    public sealed class NodePlaceholderState
    {
        public NodePlaceholderState(
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState nodeState,
            NodeId originNodeId,
            CombatEncounterDefinition combatEncounter = null,
            BossProgressionGateDefinition bossProgressionGate = null,
            TownServiceContextDefinition townServiceContext = null,
            LocationIdentityDefinition locationIdentity = null,
            BossRewardContentDefinition bossRewardContent = null,
            RegionMaterialYieldContentDefinition regionMaterialYieldContent = null,
            bool supportsRegionMaterialRewards = false)
        {
            if (combatEncounter != null && nodeType != NodeType.Combat && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Combat encounter data requires a combat-compatible placeholder node type.",
                    nameof(combatEncounter));
            }

            if (bossProgressionGate != null && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Boss progression gate data requires a boss-or-gate placeholder node type.",
                    nameof(bossProgressionGate));
            }

            if (townServiceContext != null && nodeType != NodeType.ServiceOrProgression)
            {
                throw new ArgumentException(
                    "Town service context data requires a service-or-progression placeholder node type.",
                    nameof(townServiceContext));
            }

            if (bossRewardContent != null && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Boss reward content requires a boss-or-gate placeholder node type.",
                    nameof(bossRewardContent));
            }

            if (regionMaterialYieldContent != null && nodeType != NodeType.Combat)
            {
                throw new ArgumentException(
                    "Region material yield content requires a standard combat placeholder node type.",
                    nameof(regionMaterialYieldContent));
            }

            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            NodeState = nodeState;
            OriginNodeId = originNodeId;
            LocationIdentity = locationIdentity ?? LocationIdentityCatalog.CreateFallback(regionId);
            CombatEncounter = combatEncounter;
            BossProgressionGate = bossProgressionGate;
            TownServiceContext = townServiceContext;
            BossRewardContent = bossRewardContent;
            RegionMaterialYieldContent = regionMaterialYieldContent;
            SupportsRegionMaterialRewards = supportsRegionMaterialRewards;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState NodeState { get; }

        public NodeId OriginNodeId { get; }

        public LocationIdentityDefinition LocationIdentity { get; }

        public bool UsesCombatShell => NodeType == NodeType.Combat || NodeType == NodeType.BossOrGate;

        public CombatEncounterDefinition CombatEncounter { get; }

        public BossProgressionGateDefinition BossProgressionGate { get; }

        public TownServiceContextDefinition TownServiceContext { get; }

        public BossRewardContentDefinition BossRewardContent { get; }

        public RegionMaterialYieldContentDefinition RegionMaterialYieldContent { get; }

        public bool SupportsRegionMaterialRewards { get; }
    }
}

