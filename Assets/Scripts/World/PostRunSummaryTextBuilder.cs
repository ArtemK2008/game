using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
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
                $"Node: {runResult.NodeId.Value}\n" +
                $"Resolution: {runResult.ResolutionState}\n" +
                $"Rewards gained: {BuildRewardSummary(runResult.RewardPayload)}\n" +
                $"Progress changes: {BuildProgressSummary(runResult)}\n" +
                "Next actions:\n" +
                $"- Replay: {FormatYesNo(postRunStateController.CanReplayNode)}\n" +
                $"- Return to world: {FormatYesNo(postRunStateController.CanReturnToWorld)}\n" +
                $"- Stop: {FormatYesNo(postRunStateController.CanStopSession)}";
        }

        private static string BuildRewardSummary(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            if (!rewardPayload.HasRewards)
            {
                return "None";
            }

            List<string> rewardSummaries = new List<string>();

            foreach (RunCurrencyReward currencyReward in rewardPayload.CurrencyRewards)
            {
                rewardSummaries.Add($"{FormatResourceCategory(currencyReward.ResourceCategory)} x{currencyReward.Amount}");
            }

            foreach (RunMaterialReward materialReward in rewardPayload.MaterialRewards)
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
