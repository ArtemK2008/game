using System;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    public sealed class OfflineProgressClaimState
    {
        public OfflineProgressClaimState(
            ResourceCategory resourceCategory,
            int amount,
            int countedWholeHours,
            string sourceNodeDisplayName)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Offline claim amount must be positive.");
            }

            if (countedWholeHours <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(countedWholeHours),
                    "Offline claim must count at least one whole offline hour.");
            }

            if (string.IsNullOrWhiteSpace(sourceNodeDisplayName))
            {
                throw new ArgumentException(
                    "Offline claim source display name cannot be null or whitespace.",
                    nameof(sourceNodeDisplayName));
            }

            ResourceCategory = resourceCategory;
            Amount = amount;
            CountedWholeHours = countedWholeHours;
            SourceNodeDisplayName = sourceNodeDisplayName;
        }

        public ResourceCategory ResourceCategory { get; }

        public int Amount { get; }

        public int CountedWholeHours { get; }

        public string SourceNodeDisplayName { get; }
    }
}
