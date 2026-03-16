using System;
using System.Collections.Generic;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    [Serializable]
    public sealed class ResourceBalancesState
    {
        [SerializeField]
        private List<ResourceBalanceState> balances = new List<ResourceBalanceState>();

        public IReadOnlyList<ResourceBalanceState> Balances => balances;

        public int GetAmount(ResourceCategory resourceCategory)
        {
            ResourceBalanceState balance = FindBalance(resourceCategory);
            return balance == null ? 0 : balance.Amount;
        }

        public void Add(ResourceCategory resourceCategory, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Resource amount cannot be negative.");
            }

            GetOrCreateBalance(resourceCategory).AddAmount(amount);
        }

        public bool TrySpend(ResourceCategory resourceCategory, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Resource amount cannot be negative.");
            }

            ResourceBalanceState balance = FindBalance(resourceCategory);
            return balance != null && balance.TrySpend(amount);
        }

        private ResourceBalanceState FindBalance(ResourceCategory resourceCategory)
        {
            return balances.Find(balance => balance.ResourceCategory == resourceCategory);
        }

        private ResourceBalanceState GetOrCreateBalance(ResourceCategory resourceCategory)
        {
            ResourceBalanceState existingBalance = FindBalance(resourceCategory);
            if (existingBalance != null)
            {
                return existingBalance;
            }

            ResourceBalanceState newBalance = new ResourceBalanceState(resourceCategory, 0);
            balances.Add(newBalance);
            return newBalance;
        }
    }
}

