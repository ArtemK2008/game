using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    public static class TrackedNodeProgressRules
    {
        private const int DefaultTrackedNodeThreshold = 3;

        public static bool ShouldTrack(NodeType nodeType)
        {
            return nodeType == NodeType.Combat || nodeType == NodeType.BossOrGate;
        }

        public static int GetDefaultThreshold(NodeType nodeType)
        {
            return ShouldTrack(nodeType) ? DefaultTrackedNodeThreshold : 0;
        }
    }
}

