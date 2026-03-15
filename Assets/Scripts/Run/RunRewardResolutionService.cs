using System;

namespace Survivalon.Runtime
{
    public sealed class RunRewardResolutionService
    {
        private static readonly RunRewardPayload SuccessfulCombatRewardPayload = new RunRewardPayload(
            new[]
            {
                new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
            },
            Array.Empty<RunMaterialReward>());
        private static readonly RunRewardPayload SuccessfulRegionMaterialCombatRewardPayload = new RunRewardPayload(
            new[]
            {
                new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
            },
            new[]
            {
                new RunMaterialReward(ResourceCategory.RegionMaterial, 1),
            });
        private static readonly RunMaterialReward[] SuccessfulClearMilestoneMaterialRewards =
        {
            new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
        };

        public RunRewardPayload Resolve(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            WorldGraph worldGraph = null,
            RunProgressResolution? progressResolution = null)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            RunRewardPayload ordinaryRewards = ResolveOrdinaryRewards(nodeContext, resolutionState, worldGraph);

            if (!ShouldGrantMilestoneRewards(progressResolution))
            {
                return ordinaryRewards;
            }

            return new RunRewardPayload(
                ordinaryRewards.CurrencyRewards,
                ordinaryRewards.MaterialRewards,
                Array.Empty<RunCurrencyReward>(),
                SuccessfulClearMilestoneMaterialRewards);
        }

        private static RunRewardPayload ResolveOrdinaryRewards(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            WorldGraph worldGraph)
        {
            if (!ShouldGrantBaselineCombatRewards(nodeContext, resolutionState))
            {
                return RunRewardPayload.Empty;
            }

            return ShouldGrantRegionMaterialReward(nodeContext, worldGraph)
                ? SuccessfulRegionMaterialCombatRewardPayload
                : SuccessfulCombatRewardPayload;
        }

        private static bool ShouldGrantBaselineCombatRewards(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState)
        {
            return resolutionState == RunResolutionState.Succeeded && nodeContext.UsesCombatShell;
        }

        private static bool ShouldGrantRegionMaterialReward(
            NodePlaceholderState nodeContext,
            WorldGraph worldGraph)
        {
            if (worldGraph == null || nodeContext.NodeType != NodeType.Combat)
            {
                return false;
            }

            foreach (WorldRegion region in worldGraph.Regions)
            {
                if (region.RegionId == nodeContext.RegionId)
                {
                    return region.ResourceCategory == ResourceCategory.RegionMaterial;
                }
            }

            return false;
        }

        private static bool ShouldGrantMilestoneRewards(RunProgressResolution? progressResolution)
        {
            return progressResolution.HasValue &&
                progressResolution.Value.NodeProgressUpdate.DidReachClearThreshold;
        }
    }
}
