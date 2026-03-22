using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    public sealed class TownServiceProgressionOptionState
    {
        public TownServiceProgressionOptionState(
            AccountWideUpgradeId upgradeId,
            ResourceCategory costResourceCategory,
            int costAmount,
            bool isPurchased,
            bool isAffordable)
        {
            UpgradeId = upgradeId;
            CostResourceCategory = costResourceCategory;
            CostAmount = costAmount;
            IsPurchased = isPurchased;
            IsAffordable = isAffordable;
        }

        public AccountWideUpgradeId UpgradeId { get; }

        public ResourceCategory CostResourceCategory { get; }

        public int CostAmount { get; }

        public bool IsPurchased { get; }

        public bool IsAffordable { get; }
    }
}
