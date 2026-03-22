using System;
using System.Collections.Generic;
using Survivalon.Core;

namespace Survivalon.Data.Towns
{
    public static class TownServiceConversionCatalog
    {
        private static readonly TownServiceConversionDefinition RegionMaterialRefinement =
            new TownServiceConversionDefinition(
                TownServiceConversionId.RegionMaterialRefinement,
                "Region Material Refinement",
                ResourceCategory.RegionMaterial,
                inputAmount: 3,
                ResourceCategory.PersistentProgressionMaterial,
                outputAmount: 1);

        private static readonly TownServiceConversionDefinition[] AllDefinitions =
        {
            RegionMaterialRefinement,
        };

        public static IReadOnlyList<TownServiceConversionDefinition> All => AllDefinitions;

        public static TownServiceConversionDefinition Get(TownServiceConversionId conversionId)
        {
            switch (conversionId)
            {
                case TownServiceConversionId.RegionMaterialRefinement:
                    return RegionMaterialRefinement;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(conversionId),
                        conversionId,
                        "Unknown town service conversion id.");
            }
        }
    }
}
