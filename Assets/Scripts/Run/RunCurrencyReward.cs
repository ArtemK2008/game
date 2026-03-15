using System;

namespace Survivalon.Runtime
{
    public readonly struct RunCurrencyReward
    {
        public RunCurrencyReward(ResourceCategory resourceCategory, int amount)
        {
            RunRewardResourceCategoryRules.EnsureCurrencyCategory(resourceCategory);

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
