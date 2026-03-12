using System;

namespace Survivalon.Runtime
{
    public readonly struct RunResourceReward
    {
        public RunResourceReward(ResourceCategory resourceCategory, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Run reward amount cannot be negative.");
            }

            ResourceCategory = resourceCategory;
            Amount = amount;
        }

        public ResourceCategory ResourceCategory { get; }

        public int Amount { get; }
    }
}
