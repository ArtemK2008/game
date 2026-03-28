using System;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Run
{
    /// <summary>
    /// Разрешает, нужно ли воспроизводить unlock/boss-clear feedback в момент показа post-run.
    /// </summary>
    public sealed class PostRunFeedbackSoundStateResolver
    {
        public PostRunFeedbackSoundState Resolve(NodePlaceholderState nodeContext, RunResult runResult)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            bool shouldPlayUnlockSound = runResult.DidUnlockRoute || runResult.HasBossProgressionGateUnlock;
            bool shouldPlayBossClearSound = nodeContext.NodeType == NodeType.BossOrGate &&
                runResult.ResolutionState == RunResolutionState.Succeeded;

            if (!shouldPlayUnlockSound && !shouldPlayBossClearSound)
            {
                return PostRunFeedbackSoundState.None;
            }

            return new PostRunFeedbackSoundState(
                shouldPlayUnlockSound,
                shouldPlayBossClearSound);
        }
    }
}
