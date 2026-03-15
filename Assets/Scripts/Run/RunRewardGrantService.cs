using System;

namespace Survivalon.Runtime
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

            foreach (RunCurrencyReward currencyReward in rewardPayload.CurrencyRewards)
            {
                resourceBalances.Add(currencyReward.ResourceCategory, currencyReward.Amount);
            }

            foreach (RunMaterialReward materialReward in rewardPayload.MaterialRewards)
            {
                resourceBalances.Add(materialReward.ResourceCategory, materialReward.Amount);
            }
        }
    }
}
