using System;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class ResourceBalanceState
    {
        [SerializeField]
        private ResourceCategory resourceCategory = ResourceCategory.SoftCurrency;

        [SerializeField]
        private int amount;

        public ResourceBalanceState()
        {
        }

        public ResourceBalanceState(ResourceCategory resourceCategory, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Resource balance cannot be negative.");
            }

            this.resourceCategory = resourceCategory;
            this.amount = amount;
        }

        public ResourceCategory ResourceCategory => resourceCategory;

        public int Amount => amount;

        public void AddAmount(int delta)
        {
            if (delta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delta), "Resource delta cannot be negative.");
            }

            amount += delta;
        }

        public bool TrySpend(int delta)
        {
            if (delta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delta), "Resource delta cannot be negative.");
            }

            if (amount < delta)
            {
                return false;
            }

            amount -= delta;
            return true;
        }
    }
}

