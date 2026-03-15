using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Survivalon.Runtime
{
    public sealed class RunRewardPayload
    {
        private static readonly RunRewardPayload EmptyInstance = new RunRewardPayload(
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>());
        private readonly ReadOnlyCollection<RunCurrencyReward> currencyRewards;
        private readonly ReadOnlyCollection<RunMaterialReward> materialRewards;

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

            this.currencyRewards = Array.AsReadOnly(CopyRewards(currencyRewards));
            this.materialRewards = Array.AsReadOnly(CopyRewards(materialRewards));
        }

        public static RunRewardPayload Empty => EmptyInstance;

        public IReadOnlyList<RunCurrencyReward> CurrencyRewards => currencyRewards;

        public IReadOnlyList<RunMaterialReward> MaterialRewards => materialRewards;

        public bool HasCurrencyRewards => currencyRewards.Count > 0;

        public bool HasMaterialRewards => materialRewards.Count > 0;

        public bool HasRewards => HasCurrencyRewards || HasMaterialRewards;

        private static TReward[] CopyRewards<TReward>(IEnumerable<TReward> rewards)
        {
            return new List<TReward>(rewards).ToArray();
        }
    }
}
