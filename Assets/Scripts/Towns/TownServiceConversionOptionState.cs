using System;
using Survivalon.Core;
using Survivalon.Data.Towns;

namespace Survivalon.Towns
{
    public sealed class TownServiceConversionOptionState
    {
        public TownServiceConversionOptionState(
            TownServiceConversionId conversionId,
            string conversionDisplayName,
            ResourceCategory inputResourceCategory,
            int inputAmount,
            ResourceCategory outputResourceCategory,
            int outputAmount,
            int availableInputAmount,
            bool isAffordable)
        {
            if (string.IsNullOrWhiteSpace(conversionDisplayName))
            {
                throw new ArgumentException(
                    "Conversion display name cannot be null or whitespace.",
                    nameof(conversionDisplayName));
            }

            ConversionId = conversionId;
            ConversionDisplayName = conversionDisplayName;
            InputResourceCategory = inputResourceCategory;
            InputAmount = inputAmount;
            OutputResourceCategory = outputResourceCategory;
            OutputAmount = outputAmount;
            AvailableInputAmount = availableInputAmount;
            IsAffordable = isAffordable;
        }

        public TownServiceConversionId ConversionId { get; }

        public string ConversionDisplayName { get; }

        public ResourceCategory InputResourceCategory { get; }

        public int InputAmount { get; }

        public ResourceCategory OutputResourceCategory { get; }

        public int OutputAmount { get; }

        public int AvailableInputAmount { get; }

        public bool IsAffordable { get; }
    }
}
