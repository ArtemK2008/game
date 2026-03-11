using System.Collections.Generic;
using UnityEngine;

namespace Survivalon.Runtime
{
    [CreateAssetMenu(
        fileName = "RewardDefinition",
        menuName = "Survivalon/Data/Rewards/Reward Definition")]
    public sealed class RewardDefinition : ScriptableObject
    {
        [SerializeField]
        private RewardCategory rewardCategory = RewardCategory.Currency;

        [SerializeField]
        private RewardSourceCategory sourceCategory = RewardSourceCategory.RunCompletion;

        [SerializeField]
        private List<ResourceAmountData> resourceAmounts = new List<ResourceAmountData>();

        [SerializeField]
        private List<GearDefinition> gearRewards = new List<GearDefinition>();

        public RewardCategory RewardCategory => rewardCategory;

        public RewardSourceCategory SourceCategory => sourceCategory;

        public IReadOnlyList<ResourceAmountData> ResourceAmounts => resourceAmounts;

        public IReadOnlyList<GearDefinition> GearRewards => gearRewards;
    }
}
