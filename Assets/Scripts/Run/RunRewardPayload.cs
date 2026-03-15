using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class RunRewardPayload
    {
        private static readonly RunRewardPayload EmptyInstance = new RunRewardPayload(
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>());
        private readonly List<RunCurrencyReward> currencyRewards;
        private readonly List<RunMaterialReward> materialRewards;

        public RunRewardPayload(
            IEnumerable<RunCurrencyReward> currencyRewards,
            IEnumerable<RunMaterialReward> materialRewards)
        {
            if (currencyRewards == null)
            {
                throw new ArgumentNullException(nameof(currencyRewards));
            }

            if (materialRewards == null)
            {
                throw new ArgumentNullException(nameof(materialRewards));
            }

            this.currencyRewards = new List<RunCurrencyReward>(currencyRewards);
            this.materialRewards = new List<RunMaterialReward>(materialRewards);
        }

        public static RunRewardPayload Empty => EmptyInstance;

        public IReadOnlyList<RunCurrencyReward> CurrencyRewards => currencyRewards;

        public IReadOnlyList<RunMaterialReward> MaterialRewards => materialRewards;

        public bool HasCurrencyRewards => currencyRewards.Count > 0;

        public bool HasMaterialRewards => materialRewards.Count > 0;

        public bool HasRewards => HasCurrencyRewards || HasMaterialRewards;
    }
}
