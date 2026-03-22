using System;
using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class BossProgressionGateDefinition
    {
        public BossProgressionGateDefinition(NodeId unlockedNodeId)
        {
            if (string.IsNullOrWhiteSpace(unlockedNodeId.Value))
            {
                throw new ArgumentException(
                    "Boss progression gate unlocked node id cannot be null or whitespace.",
                    nameof(unlockedNodeId));
            }

            UnlockedNodeId = unlockedNodeId;
        }

        public NodeId UnlockedNodeId { get; }
    }
}
