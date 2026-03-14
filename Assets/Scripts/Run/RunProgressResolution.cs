using System;

namespace Survivalon.Runtime
{
    public readonly struct RunProgressResolution
    {
        public RunProgressResolution(
            int nodeProgressDelta,
            NodeProgressUpdateResult nodeProgressUpdate,
            bool didUnlockRoute)
        {
            if (nodeProgressDelta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeProgressDelta), "Node progress delta cannot be negative.");
            }

            NodeProgressDelta = nodeProgressDelta;
            NodeProgressUpdate = nodeProgressUpdate ?? throw new ArgumentNullException(nameof(nodeProgressUpdate));
            DidUnlockRoute = didUnlockRoute;
        }

        public int NodeProgressDelta { get; }

        public NodeProgressUpdateResult NodeProgressUpdate { get; }

        public bool DidUnlockRoute { get; }
    }
}
