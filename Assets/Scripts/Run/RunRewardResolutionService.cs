using System;
using Survivalon.Core;
using Survivalon.Data.Rewards;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    /// <summary>
    /// Собирает runtime reward payload из результата забега и authored reward-тюнинга.
    /// </summary>
    public sealed class RunRewardResolutionService
    {
        private static readonly RunRewardTuningDefinition RewardTuning = RunRewardTuningCatalog.Current;

        public RunRewardPayload Resolve(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            WorldGraph worldGraph = null,
            RunProgressResolution? progressResolution = null,
            AccountWideProgressionEffectState progressionEffects = default,
            System.Collections.Generic.IReadOnlyList<string> ownedGearIds = null)
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
            RunGearReward[] bossGearRewards = ResolveBossGearRewards(
                nodeContext,
                resolutionState,
                ownedGearIds);
            bool shouldGrantMilestoneRewards = ShouldGrantMilestoneRewards(progressResolution);

            if (!shouldGrantMilestoneRewards && bossMaterialRewards.Length == 0 && bossGearRewards.Length == 0)
            {
                return ordinaryRewards;
            }

            return CreateRewardPayload(
                ordinaryRewards,
                Array.Empty<RunCurrencyReward>(),
                ResolveMilestoneMaterialRewards(shouldGrantMilestoneRewards),
                Array.Empty<RunCurrencyReward>(),
                bossMaterialRewards,
                bossGearRewards);
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
                ResolveRegionMaterialYieldBonus(nodeContext),
                progressionEffects);
        }

        private static RunRewardPayload CreateOrdinaryCombatRewardPayload(
            bool shouldGrantRegionMaterialReward,
            int regionMaterialYieldBonus,
            AccountWideProgressionEffectState progressionEffects)
        {
            RunCurrencyReward[] currencyRewards =
            {
                CreateCurrencyReward(RewardTuning.OrdinaryCombatCurrencyReward),
            };

            if (!shouldGrantRegionMaterialReward)
            {
                return new RunRewardPayload(currencyRewards, Array.Empty<RunMaterialReward>());
            }

            RunMaterialReward[] materialRewards =
            {
                CreateMaterialReward(
                    RewardTuning.OrdinaryCombatRegionMaterialReward,
                    progressionEffects.OrdinaryRegionMaterialRewardBonus + regionMaterialYieldBonus),
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
                    return RegionMaterialRewardSupportResolver.Supports(
                        nodeContext.NodeType,
                        region.ResourceCategory);
                }
            }

            return false;
        }

        private static bool ShouldGrantMilestoneRewards(RunProgressResolution? progressResolution)
        {
            return progressResolution.HasValue &&
                progressResolution.Value.NodeProgressUpdate.DidReachClearThreshold;
        }

        private static int ResolveRegionMaterialYieldBonus(NodePlaceholderState nodeContext)
        {
            if (nodeContext.RegionMaterialYieldContent == null)
            {
                return 0;
            }

            return nodeContext.RegionMaterialYieldContent.RegionMaterialBonus;
        }

        private static RunRewardPayload CreateRewardPayload(
            RunRewardPayload ordinaryRewards,
            RunCurrencyReward[] milestoneCurrencyRewards,
            RunMaterialReward[] milestoneMaterialRewards,
            RunCurrencyReward[] bossCurrencyRewards,
            RunMaterialReward[] bossMaterialRewards,
            RunGearReward[] bossGearRewards)
        {
            return new RunRewardPayload(
                ordinaryRewards.CurrencyRewards,
                ordinaryRewards.MaterialRewards,
                milestoneCurrencyRewards,
                milestoneMaterialRewards,
                bossCurrencyRewards,
                bossMaterialRewards,
                bossGearRewards);
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

            RunMaterialReward[] bossMaterialRewards =
            {
                CreateMaterialReward(
                    RewardTuning.SuccessfulBossMaterialReward,
                    progressionEffects.BossProgressionMaterialRewardBonus +
                        ResolveBossRewardContentBonus(nodeContext)),
            };

            return bossMaterialRewards;
        }

        private static RunGearReward[] ResolveBossGearRewards(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState,
            System.Collections.Generic.IReadOnlyList<string> ownedGearIds)
        {
            if (!ShouldGrantBossRewards(nodeContext, resolutionState))
            {
                return Array.Empty<RunGearReward>();
            }

            string rewardedGearId = nodeContext.BossRewardContent?.GearRewardId;
            if (string.IsNullOrWhiteSpace(rewardedGearId) || OwnsGearId(ownedGearIds, rewardedGearId))
            {
                return Array.Empty<RunGearReward>();
            }

            RunGearReward[] bossGearRewards =
            {
                new RunGearReward(rewardedGearId),
            };

            return bossGearRewards;
        }

        private static RunCurrencyReward CreateCurrencyReward(RewardAmountDefinition rewardDefinition)
        {
            return new RunCurrencyReward(rewardDefinition.ResourceCategory, rewardDefinition.Amount);
        }

        private static RunMaterialReward CreateMaterialReward(
            RewardAmountDefinition rewardDefinition,
            int additionalAmount = 0)
        {
            return new RunMaterialReward(
                rewardDefinition.ResourceCategory,
                rewardDefinition.Amount + additionalAmount);
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

        private static bool OwnsGearId(
            System.Collections.Generic.IReadOnlyList<string> ownedGearIds,
            string gearId)
        {
            if (ownedGearIds == null)
            {
                return false;
            }

            for (int index = 0; index < ownedGearIds.Count; index++)
            {
                if (ownedGearIds[index] == gearId)
                {
                    return true;
                }
            }

            return false;
        }

        private static RunMaterialReward[] ResolveMilestoneMaterialRewards(bool shouldGrantMilestoneRewards)
        {
            if (!shouldGrantMilestoneRewards)
            {
                return Array.Empty<RunMaterialReward>();
            }

            RunMaterialReward[] milestoneMaterialRewards =
            {
                CreateMaterialReward(RewardTuning.SuccessfulClearMilestoneMaterialReward),
            };

            return milestoneMaterialRewards;
        }
    }
}

