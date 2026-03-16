using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
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
