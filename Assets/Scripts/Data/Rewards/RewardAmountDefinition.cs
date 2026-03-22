using System;
using Survivalon.Core;

namespace Survivalon.Data.Rewards
{
    /// <summary>
    /// Хранит одно authored значение награды как пару категории и количества.
    /// </summary>
    public sealed class RewardAmountDefinition
    {
        public RewardAmountDefinition(ResourceCategory resourceCategory, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Reward amount cannot be negative.");
            }

            ResourceCategory = resourceCategory;
            Amount = amount;
        }

        public ResourceCategory ResourceCategory { get; }

        public int Amount { get; }
    }
}
