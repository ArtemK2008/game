using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.State.Persistence
{
    public sealed class OfflineProgressEligibilityResolver
    {
        private readonly WorldGraph worldGraph;
        private readonly WorldNodeFarmReadinessResolver worldNodeFarmReadinessResolver;

        public OfflineProgressEligibilityResolver(
            WorldGraph worldGraph,
            WorldNodeFarmReadinessResolver worldNodeFarmReadinessResolver = null)
        {
            this.worldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            this.worldNodeFarmReadinessResolver =
                worldNodeFarmReadinessResolver ?? new WorldNodeFarmReadinessResolver();
        }

        public OfflineProgressEligibilityKind Resolve(
            PersistentWorldState worldState,
            SafeResumeTargetType targetType,
            NodeId resumeNodeId)
        {
            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (targetType != SafeResumeTargetType.WorldMap)
            {
                return OfflineProgressEligibilityKind.None;
            }

            try
            {
                if (!worldNodeFarmReadinessResolver.IsFarmReady(worldGraph, worldState, resumeNodeId))
                {
                    return OfflineProgressEligibilityKind.None;
                }

                WorldNode worldNode = worldGraph.GetNode(resumeNodeId);
                WorldRegion worldRegion = worldGraph.GetRegion(worldNode.RegionId);
                if (!RegionMaterialRewardSupportResolver.Supports(worldNode.NodeType, worldRegion.ResourceCategory) ||
                    worldNode.RegionMaterialYieldContent == null)
                {
                    return OfflineProgressEligibilityKind.None;
                }

                return OfflineProgressEligibilityKind.FarmReadyWorldNode;
            }
            catch (KeyNotFoundException)
            {
                return OfflineProgressEligibilityKind.None;
            }
        }
    }
}
