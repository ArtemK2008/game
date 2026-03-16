using System;
using UnityEngine;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Gear;

namespace Survivalon.Runtime.Data.Rewards
{
    [Serializable]
    public sealed class ResourceAmountData
    {
        [SerializeField]
        private ResourceCategory resourceCategory = ResourceCategory.SoftCurrency;

        [SerializeField]
        private int amount;

        public ResourceCategory ResourceCategory => resourceCategory;

        public int Amount => amount;
    }
}
