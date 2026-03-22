using System;

namespace Survivalon.World
{
    public sealed class RegionMaterialYieldContentDefinition
    {
        public RegionMaterialYieldContentDefinition(int regionMaterialBonus)
        {
            if (regionMaterialBonus <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(regionMaterialBonus),
                    "Region material bonus must be positive.");
            }

            RegionMaterialBonus = regionMaterialBonus;
        }

        public int RegionMaterialBonus { get; }
    }
}
