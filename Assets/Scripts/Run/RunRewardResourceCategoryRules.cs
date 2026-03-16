using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;

namespace Survivalon.Runtime.Run
{
    public static class RunRewardResourceCategoryRules
    {
        public static void EnsureCurrencyCategory(ResourceCategory resourceCategory)
        {
            if (!IsCurrencyCategory(resourceCategory))
            {
                throw new ArgumentException(
                    $"Resource category '{resourceCategory}' is not a supported currency reward category.",
                    nameof(resourceCategory));
            }
        }

        public static void EnsureMaterialCategory(ResourceCategory resourceCategory)
        {
            if (!IsMaterialCategory(resourceCategory))
            {
                throw new ArgumentException(
                    $"Resource category '{resourceCategory}' is not a supported material reward category.",
                    nameof(resourceCategory));
            }
        }

        public static bool IsCurrencyCategory(ResourceCategory resourceCategory)
        {
            return resourceCategory == ResourceCategory.SoftCurrency;
        }

        public static bool IsMaterialCategory(ResourceCategory resourceCategory)
        {
            return resourceCategory == ResourceCategory.RegionMaterial ||
                resourceCategory == ResourceCategory.PersistentProgressionMaterial;
        }
    }
}
