using System;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    /// <summary>
    /// Resolves the effective persistent output for town/service conversions from authored conversion data and purchased account-wide effects.
    /// </summary>
    public sealed class TownServiceConversionEffectResolver
    {
        public int ResolveOutputAmount(
            TownServiceConversionDefinition conversionDefinition,
            AccountWideProgressionEffectState progressionEffects = default)
        {
            if (conversionDefinition == null)
            {
                throw new ArgumentNullException(nameof(conversionDefinition));
            }

            return conversionDefinition.ConversionId switch
            {
                TownServiceConversionId.RegionMaterialRefinement =>
                    conversionDefinition.OutputAmount + progressionEffects.RegionMaterialRefinementOutputBonus,
                _ => conversionDefinition.OutputAmount,
            };
        }
    }
}
