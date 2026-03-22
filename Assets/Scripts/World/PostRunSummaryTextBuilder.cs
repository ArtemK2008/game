using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Run;

namespace Survivalon.World
{
    public static class PostRunSummaryTextBuilder
    {
        public static string Build(PostRunStateController postRunStateController, RunResult runResult)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            return
                "Run finished.\n" +
                $"Location: {postRunStateController.NodeContext.LocationIdentity.DisplayName}\n" +
                $"Node: {postRunStateController.NodeContext.NodeDisplayName}\n" +
                $"Resolution: {runResult.ResolutionState}\n" +
                $"Rewards gained: {BuildRewardSummary(runResult.RewardPayload)}\n" +
                BuildRewardSourceLine(postRunStateController, runResult) +
                BuildBossRewardLine(runResult.RewardPayload) +
                BuildMilestoneRewardLine(runResult.RewardPayload) +
                $"Progress changes: {BuildProgressSummary(runResult)}\n" +
                BuildBossProgressionGateLine(runResult);
        }

        private static string BuildRewardSummary(RunRewardPayload rewardPayload)
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

        private static string BuildRewardSourceLine(
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

            if (!runResult.RewardPayload.HasRewards)
            {
                return string.Empty;
            }

            return $"Reward source: {postRunStateController.NodeContext.LocationIdentity.RewardSourceDisplayName}\n";
        }

        private static string BuildMilestoneRewardLine(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasMilestoneRewards)
            {
                return string.Empty;
            }

            return $"Milestone rewards: {BuildRewardListSummary(rewardPayload.MilestoneCurrencyRewards, rewardPayload.MilestoneMaterialRewards)}\n";
        }

        private static string BuildBossRewardLine(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasBossRewards)
            {
                return string.Empty;
            }

            return $"Boss rewards: {BuildRewardListSummary(rewardPayload.BossCurrencyRewards, rewardPayload.BossMaterialRewards)}\n";
        }

        private static string BuildRewardListSummary(
            IReadOnlyList<RunCurrencyReward> currencyRewards,
            IReadOnlyList<RunMaterialReward> materialRewards)
        {
            List<string> rewardSummaries = new List<string>();

            foreach (RunCurrencyReward currencyReward in currencyRewards)
            {
                rewardSummaries.Add($"{FormatResourceCategory(currencyReward.ResourceCategory)} x{currencyReward.Amount}");
            }

            foreach (RunMaterialReward materialReward in materialRewards)
            {
                rewardSummaries.Add($"{FormatResourceCategory(materialReward.ResourceCategory)} x{materialReward.Amount}");
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

            return
                $"{nodeProgressSummary}; " +
                $"persistent +{runResult.PersistentProgressionDelta}; " +
                $"route unlock {FormatYesNo(runResult.DidUnlockRoute)}";
        }

        private static string BuildBossProgressionGateLine(RunResult runResult)
        {
            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            if (!runResult.HasBossProgressionGateUnlock ||
                !runResult.BossProgressionGateUnlock.TryGetUnlockedNodeId(out NodeId unlockedNodeId))
            {
                return string.Empty;
            }

            return $"Boss gate unlock: {BuildBossProgressionGateSummary(unlockedNodeId)}\n";
        }

        private static string BuildBossProgressionGateSummary(NodeId unlockedNodeId)
        {
            if (unlockedNodeId == BootstrapWorldScenario.CavernGateNodeId)
            {
                return "Cavern gate opened";
            }

            return $"{unlockedNodeId.Value} unlocked";
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        private static string FormatResourceCategory(ResourceCategory resourceCategory)
        {
            switch (resourceCategory)
            {
                case ResourceCategory.SoftCurrency:
                    return "Soft currency";
                case ResourceCategory.RegionMaterial:
                    return "Region material";
                case ResourceCategory.PersistentProgressionMaterial:
                    return "Persistent progression material";
                default:
                    return resourceCategory.ToString();
            }
        }
    }
}

