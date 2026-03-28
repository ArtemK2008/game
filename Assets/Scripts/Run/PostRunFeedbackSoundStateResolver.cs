using System;
using Survivalon.Core;
using Survivalon.World;

namespace Survivalon.Run
{
    /// <summary>
    /// Р Р°Р·СЂРµС€Р°РµС‚, РЅСѓР¶РЅРѕ Р»Рё РІРѕСЃРїСЂРѕРёР·РІРµСЃС‚Рё unlock/boss-clear feedback РІ РјРѕРјРµРЅС‚ РїРѕРєР°Р·Р° post-run.
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
