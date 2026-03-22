using System;
using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class BossProgressionGateDefinition
    {
        public BossProgressionGateDefinition(NodeId unlockedNodeId, string unlockSummaryText)
        {
            if (string.IsNullOrWhiteSpace(unlockSummaryText))
            {
                throw new ArgumentException(
                    "Boss progression gate unlock summary text cannot be null or whitespace.",
                    nameof(unlockSummaryText));
            }

            UnlockedNodeId = unlockedNodeId;
            UnlockSummaryText = unlockSummaryText;
        }

        public NodeId UnlockedNodeId { get; }

        public string UnlockSummaryText { get; }
    }
}
