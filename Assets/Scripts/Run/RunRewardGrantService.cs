using System;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.Run
{
    public sealed class RunRewardGrantService
    {
        public void Grant(ResourceBalancesState resourceBalances, RunRewardPayload rewardPayload)
        {
            if (resourceBalances == null)
            {
                throw new ArgumentNullException(nameof(resourceBalances));
            }

            if (rewardPayload == null)
            {
                throw new ArgumentNullException(nameof(rewardPayload));
            }

            GrantCurrencyRewards(resourceBalances, rewardPayload.CurrencyRewards);
            GrantMaterialRewards(resourceBalances, rewardPayload.MaterialRewards);
            GrantCurrencyRewards(resourceBalances, rewardPayload.MilestoneCurrencyRewards);
            GrantMaterialRewards(resourceBalances, rewardPayload.MilestoneMaterialRewards);
        }

        private static void GrantCurrencyRewards(
            ResourceBalancesState resourceBalances,
            System.Collections.Generic.IEnumerable<RunCurrencyReward> currencyRewards)
        {
            foreach (RunCurrencyReward currencyReward in currencyRewards)
            {
                resourceBalances.Add(currencyReward.ResourceCategory, currencyReward.Amount);
            }
        }

        private static void GrantMaterialRewards(
            ResourceBalancesState resourceBalances,
            System.Collections.Generic.IEnumerable<RunMaterialReward> materialRewards)
        {
            foreach (RunMaterialReward materialReward in materialRewards)
            {
                resourceBalances.Add(materialReward.ResourceCategory, materialReward.Amount);
            }
        }
    }
}

