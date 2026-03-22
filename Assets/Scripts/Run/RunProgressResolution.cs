using System;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public readonly struct RunProgressResolution
    {
        public RunProgressResolution(
            int nodeProgressDelta,
            NodeProgressUpdateResult nodeProgressUpdate,
            bool didUnlockRoute,
            BossProgressionGateUnlockResult bossProgressionGateUnlock = null)
        {
            if (nodeProgressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressDelta), "Node progress delta cannot be negative.");
            }

            NodeProgressDelta = nodeProgressDelta;
            NodeProgressUpdate = nodeProgressUpdate ?? throw new ArgumentNullException(nameof(nodeProgressUpdate));
            DidUnlockRoute = didUnlockRoute;
            BossProgressionGateUnlock = bossProgressionGateUnlock ?? BossProgressionGateUnlockResult.None;
        }

        public int NodeProgressDelta { get; }

        public NodeProgressUpdateResult NodeProgressUpdate { get; }

        public bool DidUnlockRoute { get; }

        public BossProgressionGateUnlockResult BossProgressionGateUnlock { get; }

        public bool HasBossProgressionGateUnlock => BossProgressionGateUnlock.DidUnlock;
    }
}

