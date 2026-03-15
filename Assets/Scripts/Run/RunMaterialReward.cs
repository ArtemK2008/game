using System;

namespace Survivalon.Runtime
{
    public readonly struct RunMaterialReward
    {
        public RunMaterialReward(ResourceCategory resourceCategory, int amount)
        {
            if (!RunRewardResourceCategoryRules.IsMaterial(resourceCategory))
            {
                throw new ArgumentException(
                    $"Resource category '{resourceCategory}' is not a supported material reward category.",
                    nameof(resourceCategory));
            }

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
