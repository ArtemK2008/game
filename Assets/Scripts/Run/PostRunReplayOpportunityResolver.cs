using System;
using Survivalon.Core;

namespace Survivalon.Run
{
    public sealed class PostRunReplayOpportunityResolver
    {
        public PostRunReplayReasonKind Resolve(PostRunStateController postRunStateController)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (!postRunStateController.CanReplayNode)
            {
                return PostRunReplayReasonKind.None;
            }

            RunResult runResult = postRunStateController.RunResult;
            if (runResult.ResolutionState != RunResolutionState.Succeeded)
            {
                return PostRunReplayReasonKind.RetryAttempt;
            }

            if (runResult.HasTrackedNodeProgress &&
                runResult.NodeProgressValue < runResult.NodeProgressThreshold)
            {
                return PostRunReplayReasonKind.ContinueNodeProgress;
            }

            if (HasRegionMaterialReward(runResult.RewardPayload))
            {
                return PostRunReplayReasonKind.FarmRegionMaterial;
            }

            if (runResult.RewardPayload.HasRewards)
            {
                return PostRunReplayReasonKind.FarmRewards;
            }

            return PostRunReplayReasonKind.RetryAttempt;
        }

        private static bool HasRegionMaterialReward(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                return false;
            }

            foreach (RunMaterialReward reward in rewardPayload.MaterialRewards)
            {
                if (reward.ResourceCategory == ResourceCategory.RegionMaterial && reward.Amount > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
