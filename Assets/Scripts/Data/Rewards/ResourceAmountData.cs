using System;
using UnityEngine;

namespace Survivalon.Runtime
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
