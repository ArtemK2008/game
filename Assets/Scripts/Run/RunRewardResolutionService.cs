using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
{
    public sealed class RunRewardResolutionService
    {
        private static readonly RunMaterialReward[] SuccessfulClearMilestoneMaterialRewards =
        {
            new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
        };

        public RunRewardPayload Resolve(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            WorldGraph worldGraph = null,
            RunProgressResolution? progressResolution = null,
            AccountWideProgressionEffectState progressionEffects = default)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            RunRewardPayload ordinaryRewards = ResolveOrdinaryRewards(
                nodeContext,
                resolutionState,
                worldGraph,
                progressionEffects);

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
            WorldGraph worldGraph,
            AccountWideProgressionEffectState progressionEffects)
        {
            if (!ShouldGrantBaselineCombatRewards(nodeContext, resolutionState))
            {
                return RunRewardPayload.Empty;
            }

            return CreateOrdinaryCombatRewardPayload(
                ShouldGrantRegionMaterialReward(nodeContext, worldGraph),
                progressionEffects);
        }

        private static RunRewardPayload CreateOrdinaryCombatRewardPayload(
            bool shouldGrantRegionMaterialReward,
            AccountWideProgressionEffectState progressionEffects)
        {
            RunCurrencyReward[] currencyRewards =
            {
                new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
            };

            if (!shouldGrantRegionMaterialReward)
            {
                return new RunRewardPayload(currencyRewards, Array.Empty<RunMaterialReward>());
            }

            RunMaterialReward[] materialRewards =
            {
                new RunMaterialReward(
                    ResourceCategory.RegionMaterial,
                    1 + progressionEffects.OrdinaryRegionMaterialRewardBonus),
            };

            return new RunRewardPayload(currencyRewards, materialRewards);
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
