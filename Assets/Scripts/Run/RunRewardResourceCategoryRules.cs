namespace Survivalon.Runtime
{
    public static class RunRewardResourceCategoryRules
    {
        public static bool IsCurrency(ResourceCategory resourceCategory)
        {
            return resourceCategory == ResourceCategory.SoftCurrency;
        }

        public static bool IsMaterial(ResourceCategory resourceCategory)
        {
            return resourceCategory == ResourceCategory.RegionMaterial ||
                resourceCategory == ResourceCategory.PersistentProgressionMaterial;
        }
    }
}
