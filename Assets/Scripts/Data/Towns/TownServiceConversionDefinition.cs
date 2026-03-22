using System;
using Survivalon.Core;

namespace Survivalon.Data.Towns
{
    public sealed class TownServiceConversionDefinition
    {
        public TownServiceConversionDefinition(
            TownServiceConversionId conversionId,
            string displayName,
            ResourceCategory inputResourceCategory,
            int inputAmount,
            ResourceCategory outputResourceCategory,
            int outputAmount)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Conversion display name cannot be null or whitespace.", nameof(displayName));
            }

            if (inputAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputAmount), "Conversion input amount must be positive.");
            }

            if (outputAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputAmount), "Conversion output amount must be positive.");
            }

            ConversionId = conversionId;
            DisplayName = displayName;
            InputResourceCategory = inputResourceCategory;
            InputAmount = inputAmount;
            OutputResourceCategory = outputResourceCategory;
            OutputAmount = outputAmount;
        }

        public TownServiceConversionId ConversionId { get; }

        public string DisplayName { get; }

        public ResourceCategory InputResourceCategory { get; }

        public int InputAmount { get; }

        public ResourceCategory OutputResourceCategory { get; }

        public int OutputAmount { get; }
    }
}
