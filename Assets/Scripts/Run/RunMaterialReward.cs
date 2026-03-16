using System;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.Run
{
    public readonly struct RunMaterialReward
    {
        public RunMaterialReward(ResourceCategory resourceCategory, int amount)
        {
            RunRewardResourceCategoryRules.EnsureMaterialCategory(resourceCategory);

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
