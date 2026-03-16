using System;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.Run
{
    public static class RunRewardResourceCategoryRules
    {
        public static void EnsureCurrencyCategory(ResourceCategory resourceCategory)
        {
            if (resourceCategory != ResourceCategory.SoftCurrency)
            {
                throw new ArgumentException(
                    $"Resource category '{resourceCategory}' is not a supported currency reward category.",
                    nameof(resourceCategory));
            }
        }

        public static void EnsureMaterialCategory(ResourceCategory resourceCategory)
        {
            if (resourceCategory != ResourceCategory.RegionMaterial &&
                resourceCategory != ResourceCategory.PersistentProgressionMaterial)
            {
                throw new ArgumentException(
                    $"Resource category '{resourceCategory}' is not a supported material reward category.",
                    nameof(resourceCategory));
            }
        }
    }
}
