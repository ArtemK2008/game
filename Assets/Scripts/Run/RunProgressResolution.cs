using System;
using Survivalon.State.Persistence;

namespace Survivalon.Run
{
    public readonly struct RunProgressResolution
    {
        public RunProgressResolution(
            int nodeProgressDelta,
            NodeProgressUpdateResult nodeProgressUpdate,
            bool didUnlockRoute,
            string bossProgressionGateUnlockSummary = "")
        {
            if (nodeProgressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressDelta), "Node progress delta cannot be negative.");
            }

            NodeProgressDelta = nodeProgressDelta;
            NodeProgressUpdate = nodeProgressUpdate ?? throw new ArgumentNullException(nameof(nodeProgressUpdate));
            DidUnlockRoute = didUnlockRoute;
            BossProgressionGateUnlockSummary = bossProgressionGateUnlockSummary ?? string.Empty;
        }

        public int NodeProgressDelta { get; }

        public NodeProgressUpdateResult NodeProgressUpdate { get; }

        public bool DidUnlockRoute { get; }

        public string BossProgressionGateUnlockSummary { get; }
    }
}

