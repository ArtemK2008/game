using System;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Towns;

namespace Survivalon.World
{
    public sealed class WorldNode
    {
        public WorldNode(
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState state,
            CombatEncounterDefinition combatEncounter = null,
            BossProgressionGateDefinition bossProgressionGate = null,
            TownServiceContextDefinition townServiceContext = null,
            BossRewardContentDefinition bossRewardContent = null,
            RegionMaterialYieldContentDefinition regionMaterialYieldContent = null)
        {
            if (combatEncounter != null && nodeType != NodeType.Combat && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Combat encounter data requires a combat-compatible node type.",
                    nameof(combatEncounter));
            }

            if (bossProgressionGate != null && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Boss progression gate data requires a boss-or-gate node type.",
                    nameof(bossProgressionGate));
            }

            if (townServiceContext != null && nodeType != NodeType.ServiceOrProgression)
            {
                throw new ArgumentException(
                    "Town service context data requires a service-or-progression node type.",
                    nameof(townServiceContext));
            }

            if (bossRewardContent != null && nodeType != NodeType.BossOrGate)
            {
                throw new ArgumentException(
                    "Boss reward content requires a boss-or-gate node type.",
                    nameof(bossRewardContent));
            }

            if (regionMaterialYieldContent != null && nodeType != NodeType.Combat)
            {
                throw new ArgumentException(
                    "Region material yield content requires a standard combat node type.",
                    nameof(regionMaterialYieldContent));
            }

            NodeId = nodeId;
            RegionId = regionId;
            NodeType = nodeType;
            State = state;
            CombatEncounter = combatEncounter;
            BossProgressionGate = bossProgressionGate;
            TownServiceContext = townServiceContext;
            BossRewardContent = bossRewardContent;
            RegionMaterialYieldContent = regionMaterialYieldContent;
        }

        public NodeId NodeId { get; }

        public RegionId RegionId { get; }

        public NodeType NodeType { get; }

        public NodeState State { get; }

        public CombatEncounterDefinition CombatEncounter { get; }

        public BossProgressionGateDefinition BossProgressionGate { get; }

        public TownServiceContextDefinition TownServiceContext { get; }

        public BossRewardContentDefinition BossRewardContent { get; }

        public RegionMaterialYieldContentDefinition RegionMaterialYieldContent { get; }
    }
}

