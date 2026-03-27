using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Survivalon.Run
{
    public sealed class RunRewardPayload
    {
        private static readonly RunRewardPayload EmptyInstance = new RunRewardPayload(
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>(),
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>(),
            Array.Empty<RunCurrencyReward>(),
            Array.Empty<RunMaterialReward>());
        private readonly ReadOnlyCollection<RunCurrencyReward> currencyRewards;
        private readonly ReadOnlyCollection<RunMaterialReward> materialRewards;
        private readonly ReadOnlyCollection<RunCurrencyReward> milestoneCurrencyRewards;
        private readonly ReadOnlyCollection<RunMaterialReward> milestoneMaterialRewards;
        private readonly ReadOnlyCollection<RunCurrencyReward> bossCurrencyRewards;
        private readonly ReadOnlyCollection<RunMaterialReward> bossMaterialRewards;
        private readonly ReadOnlyCollection<RunGearReward> bossGearRewards;

        public RunRewardPayload(
            IEnumerable<RunCurrencyReward> currencyRewards,
            IEnumerable<RunMaterialReward> materialRewards)
            : this(
                currencyRewards,
                materialRewards,
                Array.Empty<RunCurrencyReward>(),
                Array.Empty<RunMaterialReward>(),
                Array.Empty<RunCurrencyReward>(),
                Array.Empty<RunMaterialReward>())
        {
        }

        public RunRewardPayload(
            IEnumerable<RunCurrencyReward> currencyRewards,
            IEnumerable<RunMaterialReward> materialRewards,
            IEnumerable<RunCurrencyReward> milestoneCurrencyRewards,
            IEnumerable<RunMaterialReward> milestoneMaterialRewards)
            : this(
                currencyRewards,
                materialRewards,
                milestoneCurrencyRewards,
                milestoneMaterialRewards,
                Array.Empty<RunCurrencyReward>(),
                Array.Empty<RunMaterialReward>())
        {
        }

        public RunRewardPayload(
            IEnumerable<RunCurrencyReward> currencyRewards,
            IEnumerable<RunMaterialReward> materialRewards,
            IEnumerable<RunCurrencyReward> milestoneCurrencyRewards,
            IEnumerable<RunMaterialReward> milestoneMaterialRewards,
            IEnumerable<RunCurrencyReward> bossCurrencyRewards,
            IEnumerable<RunMaterialReward> bossMaterialRewards,
            IEnumerable<RunGearReward> bossGearRewards = null)
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

            if (bossCurrencyRewards == null)
            {
                throw new ArgumentNullException(nameof(bossCurrencyRewards));
            }

            if (bossMaterialRewards == null)
            {
                throw new ArgumentNullException(nameof(bossMaterialRewards));
            }

            bossGearRewards ??= Array.Empty<RunGearReward>();
            this.currencyRewards = Array.AsReadOnly(CopyRewards(currencyRewards));
            this.materialRewards = Array.AsReadOnly(CopyRewards(materialRewards));
            this.milestoneCurrencyRewards = Array.AsReadOnly(CopyRewards(milestoneCurrencyRewards));
            this.milestoneMaterialRewards = Array.AsReadOnly(CopyRewards(milestoneMaterialRewards));
            this.bossCurrencyRewards = Array.AsReadOnly(CopyRewards(bossCurrencyRewards));
            this.bossMaterialRewards = Array.AsReadOnly(CopyRewards(bossMaterialRewards));
            this.bossGearRewards = Array.AsReadOnly(CopyRewards(bossGearRewards));
        }

        public static RunRewardPayload Empty => EmptyInstance;

        public IReadOnlyList<RunCurrencyReward> CurrencyRewards => currencyRewards;

        public IReadOnlyList<RunMaterialReward> MaterialRewards => materialRewards;

        public IReadOnlyList<RunCurrencyReward> MilestoneCurrencyRewards => milestoneCurrencyRewards;

        public IReadOnlyList<RunMaterialReward> MilestoneMaterialRewards => milestoneMaterialRewards;

        public IReadOnlyList<RunCurrencyReward> BossCurrencyRewards => bossCurrencyRewards;

        public IReadOnlyList<RunMaterialReward> BossMaterialRewards => bossMaterialRewards;

        public IReadOnlyList<RunGearReward> BossGearRewards => bossGearRewards;

        public bool HasCurrencyRewards => currencyRewards.Count > 0;

        public bool HasMaterialRewards => materialRewards.Count > 0;

        public bool HasOrdinaryRewards => HasCurrencyRewards || HasMaterialRewards;

        public bool HasMilestoneCurrencyRewards => milestoneCurrencyRewards.Count > 0;

        public bool HasMilestoneMaterialRewards => milestoneMaterialRewards.Count > 0;

        public bool HasMilestoneRewards => HasMilestoneCurrencyRewards || HasMilestoneMaterialRewards;

        public bool HasBossCurrencyRewards => bossCurrencyRewards.Count > 0;

        public bool HasBossMaterialRewards => bossMaterialRewards.Count > 0;

        public bool HasBossGearRewards => bossGearRewards.Count > 0;

        public bool HasBossRewards => HasBossCurrencyRewards || HasBossMaterialRewards || HasBossGearRewards;

        public bool HasRewards => HasOrdinaryRewards || HasMilestoneRewards || HasBossRewards;

        private static TReward[] CopyRewards<TReward>(IEnumerable<TReward> rewards)
        {
            return new List<TReward>(rewards).ToArray();
        }
    }
}

