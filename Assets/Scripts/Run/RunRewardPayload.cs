using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Survivalon.Runtime
{
    public sealed class RunRewardPayload
    {
        private static readonly RunRewardPayload EmptyInstance = new RunRewardPayload(
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>(),
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>());
        private readonly ReadOnlyCollection<RunCurrencyReward> currencyRewards;
        private readonly ReadOnlyCollection<RunMaterialReward> materialRewards;
        private readonly ReadOnlyCollection<RunCurrencyReward> milestoneCurrencyRewards;
        private readonly ReadOnlyCollection<RunMaterialReward> milestoneMaterialRewards;

        public RunRewardPayload(
            IEnumerable<RunCurrencyReward> currencyRewards,
            IEnumerable<RunMaterialReward> materialRewards)
            : this(
                currencyRewards,
                materialRewards,
                Array.Empty<RunCurrencyReward>(),
                Array.Empty<RunMaterialReward>())
        {
        }

        public RunRewardPayload(
            IEnumerable<RunCurrencyReward> currencyRewards,
            IEnumerable<RunMaterialReward> materialRewards,
            IEnumerable<RunCurrencyReward> milestoneCurrencyRewards,
            IEnumerable<RunMaterialReward> milestoneMaterialRewards)
        {
            if (currencyRewards == null)
            {
                throw new ArgumentNullException(nameof(currencyRewards));
            }

            if (materialRewards == null)
            {
                throw new ArgumentNullException(nameof(materialRewards));
            }

            if (milestoneCurrencyRewards == null)
            {
                throw new ArgumentNullException(nameof(milestoneCurrencyRewards));
            }

            if (milestoneMaterialRewards == null)
            {
                throw new ArgumentNullException(nameof(milestoneMaterialRewards));
            }

            this.currencyRewards = Array.AsReadOnly(CopyRewards(currencyRewards));
            this.materialRewards = Array.AsReadOnly(CopyRewards(materialRewards));
            this.milestoneCurrencyRewards = Array.AsReadOnly(CopyRewards(milestoneCurrencyRewards));
            this.milestoneMaterialRewards = Array.AsReadOnly(CopyRewards(milestoneMaterialRewards));
        }

        public static RunRewardPayload Empty => EmptyInstance;

        public IReadOnlyList<RunCurrencyReward> CurrencyRewards => currencyRewards;

        public IReadOnlyList<RunMaterialReward> MaterialRewards => materialRewards;

        public IReadOnlyList<RunCurrencyReward> MilestoneCurrencyRewards => milestoneCurrencyRewards;

        public IReadOnlyList<RunMaterialReward> MilestoneMaterialRewards => milestoneMaterialRewards;

        public bool HasCurrencyRewards => currencyRewards.Count > 0;

        public bool HasMaterialRewards => materialRewards.Count > 0;

        public bool HasOrdinaryRewards => HasCurrencyRewards || HasMaterialRewards;

        public bool HasMilestoneCurrencyRewards => milestoneCurrencyRewards.Count > 0;

        public bool HasMilestoneMaterialRewards => milestoneMaterialRewards.Count > 0;

        public bool HasMilestoneRewards => HasMilestoneCurrencyRewards || HasMilestoneMaterialRewards;

        public bool HasRewards => HasOrdinaryRewards || HasMilestoneRewards;

        private static TReward[] CopyRewards<TReward>(IEnumerable<TReward> rewards)
        {
            return new List<TReward>(rewards).ToArray();
        }
    }
}
