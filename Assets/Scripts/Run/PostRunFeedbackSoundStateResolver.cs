using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.Run
{
    /// <summary>
    /// Разрешает, нужно ли воспроизводить unlock/boss-clear feedback в момент показа post-run.
    /// </summary>
    public sealed class PostRunFeedbackSoundStateResolver
    {
        public PostRunFeedbackSoundState Resolve(RunResult runResult)
        {
            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            bool shouldPlayRouteUnlockSound = runResult.DidUnlockRoute || runResult.HasBossProgressionGateUnlock;
            bool shouldPlayBossRewardSound = runResult.RewardPayload.HasBossRewards;
            bool shouldPlayNodeClearSound = !shouldPlayBossRewardSound && runResult.RewardPayload.HasMilestoneRewards;

            if (!shouldPlayRouteUnlockSound && !shouldPlayBossRewardSound && !shouldPlayNodeClearSound)
            {
                return PostRunFeedbackSoundState.None;
            }

            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();
            if (shouldPlayBossRewardSound)
            {
                requestedSounds.Add(UiSystemFeedbackSoundId.StateBossReward);
            }
            else if (shouldPlayNodeClearSound)
            {
                requestedSounds.Add(UiSystemFeedbackSoundId.StateNodeClear);
            }

            if (shouldPlayRouteUnlockSound)
            {
                requestedSounds.Add(UiSystemFeedbackSoundId.StateRouteUnlock);
            }

            return requestedSounds.Count == 0
                ? PostRunFeedbackSoundState.None
                : new PostRunFeedbackSoundState(requestedSounds.ToArray());
        }
    }
}
