using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Run
{
    /// <summary>
    /// Разделяет обычные награды, reward spikes и unlock outcomes для compact post-run presentation.
    /// </summary>
    public sealed class PostRunResultPresentationStateResolver
    {
        public PostRunResultPresentationState Resolve(
            PostRunStateController postRunStateController,
            RunResult runResult)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            RunRewardPayload rewardPayload = runResult.RewardPayload;

            return new PostRunResultPresentationState(
                BuildOrdinaryRewardSummary(rewardPayload),
                BuildProgressSummary(runResult),
                BuildRewardSourceSummary(postRunStateController, rewardPayload),
                BuildClearSpikeRewardSummary(rewardPayload),
                BuildBossSpikeRewardSummary(rewardPayload),
                BuildBossGearRewardSummary(rewardPayload),
                BuildUnlockOutcomeSummary(postRunStateController, runResult));
        }

        private static string BuildOrdinaryRewardSummary(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasOrdinaryRewards)
            {
                return "None";
            }

            return BuildRewardListSummary(rewardPayload.CurrencyRewards, rewardPayload.MaterialRewards);
        }

        private static string BuildRewardSourceSummary(
            PostRunStateController postRunStateController,
            RunRewardPayload rewardPayload)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasRewards)
            {
                return string.Empty;
            }

            return postRunStateController.NodeContext.LocationIdentity.RewardSourceDisplayName;
        }

        private static string BuildClearSpikeRewardSummary(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasMilestoneRewards)
            {
                return string.Empty;
            }

            return BuildRewardListSummary(
                rewardPayload.MilestoneCurrencyRewards,
                rewardPayload.MilestoneMaterialRewards);
        }

        private static string BuildBossSpikeRewardSummary(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasBossCurrencyRewards && !rewardPayload.HasBossMaterialRewards)
            {
                return string.Empty;
            }

            return BuildRewardListSummary(
                rewardPayload.BossCurrencyRewards,
                rewardPayload.BossMaterialRewards);
        }

        private static string BuildBossGearRewardSummary(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasBossGearRewards)
            {
                return string.Empty;
            }

            List<string> gearRewardSummaries = new List<string>();
            for (int index = 0; index < rewardPayload.BossGearRewards.Count; index++)
            {
                gearRewardSummaries.Add(rewardPayload.BossGearRewards[index].DisplayName);
            }

            return string.Join(", ", gearRewardSummaries);
        }

        private static string BuildUnlockOutcomeSummary(
            PostRunStateController postRunStateController,
            RunResult runResult)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            List<string> unlockOutcomeSummaries = new List<string>();
            if (runResult.DidUnlockRoute)
            {
                unlockOutcomeSummaries.Add("Forward route opened");
            }

            if (runResult.HasBossProgressionGateUnlock &&
                runResult.BossProgressionGateUnlock.TryGetUnlockedNodeId(out NodeId unlockedNodeId))
            {
                unlockOutcomeSummaries.Add(
                    BuildBossProgressionGateSummary(postRunStateController.WorldGraph, unlockedNodeId));
            }

            return string.Join("; ", unlockOutcomeSummaries);
        }

        private static string BuildRewardListSummary(
            IReadOnlyList<RunCurrencyReward> currencyRewards,
            IReadOnlyList<RunMaterialReward> materialRewards)
        {
            List<string> rewardSummaries = new List<string>();

            foreach (RunCurrencyReward currencyReward in currencyRewards)
            {
                rewardSummaries.Add(
                    $"{PlayerFacingCoreLabelFormatter.FormatResourceCategory(currencyReward.ResourceCategory)} x{currencyReward.Amount}");
            }

            foreach (RunMaterialReward materialReward in materialRewards)
            {
                rewardSummaries.Add(
                    $"{PlayerFacingCoreLabelFormatter.FormatResourceCategory(materialReward.ResourceCategory)} x{materialReward.Amount}");
            }

            return string.Join(", ", rewardSummaries);
        }

        private static string BuildProgressSummary(RunResult runResult)
        {
            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            string nodeProgressSummary = runResult.HasTrackedNodeProgress
                ? $"node +{runResult.NodeProgressDelta} this run; tracked total {runResult.NodeProgressValue} / {runResult.NodeProgressThreshold}"
                : "node not tracked";

            return $"{nodeProgressSummary}; persistent +{runResult.PersistentProgressionDelta}";
        }

        private static string BuildBossProgressionGateSummary(
            WorldGraph worldGraph,
            NodeId unlockedNodeId)
        {
            if (worldGraph != null)
            {
                try
                {
                    return $"{FormatOpenedNodeDisplayName(worldGraph.GetNode(unlockedNodeId).DisplayName)} opened";
                }
                catch (KeyNotFoundException)
                {
                }
            }

            return $"{unlockedNodeId.Value} unlocked";
        }

        private static string FormatOpenedNodeDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return string.Empty;
            }

            if (displayName.Length == 1)
            {
                return displayName;
            }

            return char.ToUpperInvariant(displayName[0]) + displayName.Substring(1).ToLowerInvariant();
        }
    }
}
