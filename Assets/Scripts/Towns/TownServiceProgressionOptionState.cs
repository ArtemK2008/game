using System;
using Survivalon.Core;
using Survivalon.Data.Progression;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    /// <summary>
    /// Хранит presentation-ready состояние одной progression purchase option для town/service shell.
    /// </summary>
    public sealed class TownServiceProgressionOptionState
    {
        public TownServiceProgressionOptionState(
            AccountWideUpgradeId upgradeId,
            string upgradeDisplayName,
            ResourceCategory costResourceCategory,
            int costAmount,
            bool isPurchased,
            bool isAffordable)
        {
            if (string.IsNullOrWhiteSpace(upgradeDisplayName))
            {
                throw new ArgumentException("Upgrade display name cannot be null or whitespace.", nameof(upgradeDisplayName));
            }

            UpgradeId = upgradeId;
            UpgradeDisplayName = upgradeDisplayName;
            CostResourceCategory = costResourceCategory;
            CostAmount = costAmount;
            IsPurchased = isPurchased;
            IsAffordable = isAffordable;
        }

        public AccountWideUpgradeId UpgradeId { get; }

        public string UpgradeDisplayName { get; }

        public ResourceCategory CostResourceCategory { get; }

        public int CostAmount { get; }

        public bool IsPurchased { get; }

        public bool IsAffordable { get; }
    }
}
