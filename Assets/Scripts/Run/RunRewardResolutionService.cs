using System;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class RunRewardResolutionService
    {
        private static readonly RunMaterialReward[] SuccessfulClearMilestoneMaterialRewards =
        {
            new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
        };
        private static readonly RunMaterialReward[] SuccessfulBossMaterialRewards =
        {
            new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 2),
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
            RunMaterialReward[] bossMaterialRewards = ResolveBossMaterialRewards(
                nodeContext,
                resolutionState,
                progressionEffects);
            bool shouldGrantMilestoneRewards = ShouldGrantMilestoneRewards(progressResolution);

            if (!shouldGrantMilestoneRewards && bossMaterialRewards.Length == 0)
            {
                return ordinaryRewards;
            }

            return CreateRewardPayload(
                ordinaryRewards,
                Array.Empty<RunCurrencyReward>(),
                shouldGrantMilestoneRewards
                    ? SuccessfulClearMilestoneMaterialRewards
                    : Array.Empty<RunMaterialReward>(),
                Array.Empty<RunCurrencyReward>(),
                bossMaterialRewards);
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

        private static RunRewardPayload CreateRewardPayload(
            RunRewardPayload ordinaryRewards,
            RunCurrencyReward[] milestoneCurrencyRewards,
            RunMaterialReward[] milestoneMaterialRewards,
            RunCurrencyReward[] bossCurrencyRewards,
            RunMaterialReward[] bossMaterialRewards)
        {
            return new RunRewardPayload(
                ordinaryRewards.CurrencyRewards,
                ordinaryRewards.MaterialRewards,
                milestoneCurrencyRewards,
                milestoneMaterialRewards,
                bossCurrencyRewards,
                bossMaterialRewards);
        }

        private static RunMaterialReward[] ResolveBossMaterialRewards(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            AccountWideProgressionEffectState progressionEffects)
        {
            if (!ShouldGrantBossRewards(nodeContext, resolutionState))
            {
                return Array.Empty<RunMaterialReward>();
            }

            return new[]
            {
                new RunMaterialReward(
                    ResourceCategory.PersistentProgressionMaterial,
                    SuccessfulBossMaterialRewards[0].Amount +
                        progressionEffects.BossProgressionMaterialRewardBonus +
                        ResolveBossRewardContentBonus(nodeContext)),
            };
        }

        private static int ResolveBossRewardContentBonus(NodePlaceholderState nodeContext)
        {
            if (nodeContext.BossRewardContent == null)
            {
                return 0;
            }

            return nodeContext.BossRewardContent.PersistentProgressionMaterialBonus;
        }

        private static bool ShouldGrantBossRewards(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState)
        {
            return resolutionState == RunResolutionState.Succeeded &&
                nodeContext.CombatEncounter != null &&
                nodeContext.CombatEncounter.EncounterType == Data.Combat.CombatEncounterType.Boss;
        }
    }
}

