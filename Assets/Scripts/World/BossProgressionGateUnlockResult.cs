using System;
using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class BossProgressionGateUnlockResult
    {
        private readonly NodeId unlockedNodeId;

        private BossProgressionGateUnlockResult(bool didUnlock, NodeId unlockedNodeId)
        {
            DidUnlock = didUnlock;
            this.unlockedNodeId = unlockedNodeId;
        }

        public static BossProgressionGateUnlockResult None { get; } =
            new BossProgressionGateUnlockResult(false, default);

        public bool DidUnlock { get; }

        public static BossProgressionGateUnlockResult CreateUnlocked(NodeId unlockedNodeId)
        {
            if (string.IsNullOrWhiteSpace(unlockedNodeId.Value))
            {
                throw new ArgumentException(
                    "Unlocked node id cannot be null or whitespace.",
                    nameof(unlockedNodeId));
            }

            return new BossProgressionGateUnlockResult(true, unlockedNodeId);
        }

        public bool TryGetUnlockedNodeId(out NodeId nodeId)
        {
            nodeId = unlockedNodeId;
            return DidUnlock;
        }
    }
}
