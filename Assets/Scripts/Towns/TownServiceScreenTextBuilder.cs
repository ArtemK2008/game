using System;
using System.Text;
using Survivalon.Core;

namespace Survivalon.Towns
{
    public static class TownServiceScreenTextBuilder
    {
        public static string BuildOverviewText(TownServiceScreenState screenState)
        {
            if (screenState == null)
            {
                throw new ArgumentNullException(nameof(screenState));
            }

            return
                $"Service context: {screenState.ServiceContext.DisplayName}\n" +
                $"Region: {screenState.RegionId.Value}\n" +
                $"Node: {screenState.NodeId.Value}\n" +
                $"Entered from: {screenState.OriginNodeId.Value}\n" +
                $"Functions: {BuildFunctionSummary(screenState)}";
        }

        public static string BuildProgressionText(TownServiceScreenState screenState)
        {
            if (screenState == null)
            {
                throw new ArgumentNullException(nameof(screenState));
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Progression hub");
            builder.AppendLine(
                $"Persistent progression material: {screenState.PersistentProgressionMaterialAmount}");
            builder.Append("Projects:");

            for (int index = 0; index < screenState.ProgressionOptions.Count; index++)
            {
                TownServiceProgressionOptionState progressionOption = screenState.ProgressionOptions[index];
                builder.AppendLine();
                builder.Append("- ");
                builder.Append(progressionOption.UpgradeDisplayName);
                builder.Append(" | Cost: ");
                builder.Append(ResolveResourceDisplayName(progressionOption.CostResourceCategory));
                builder.Append(" x");
                builder.Append(progressionOption.CostAmount);
                builder.Append(" | ");
                builder.Append(BuildProgressionAvailabilityText(
                    progressionOption,
                    screenState.PersistentProgressionMaterialAmount));
            }

            return builder.ToString();
        }

        public static string BuildBuildPreparationText(TownServiceScreenState screenState)
        {
            if (screenState == null)
            {
                throw new ArgumentNullException(nameof(screenState));
            }

            return
                "Build preparation\n" +
                $"Selected character: {screenState.SelectedCharacterDisplayName}\n" +
                $"Assigned package: {screenState.AssignedSkillPackageDisplayName}\n" +
                $"Primary gear: {screenState.PrimaryGearDisplayName}\n" +
                $"Support gear: {screenState.SupportGearDisplayName}\n" +
                "Current build changes still happen on the world map in this MVP.";
        }

        private static string BuildFunctionSummary(TownServiceScreenState screenState)
        {
            if (screenState.ServiceContext.HasProgressionHubAccess &&
                screenState.ServiceContext.HasBuildPreparationAccess)
            {
                return "Progression hub, Build preparation";
            }

            if (screenState.ServiceContext.HasProgressionHubAccess)
            {
                return "Progression hub";
            }

            return "Build preparation";
        }

        private static string BuildProgressionAvailabilityText(
            TownServiceProgressionOptionState progressionOption,
            int persistentProgressionMaterialAmount)
        {
            if (progressionOption.IsPurchased)
            {
                return "Purchased";
            }

            if (progressionOption.IsAffordable)
            {
                return "Affordable";
            }

            int missingAmount = progressionOption.CostAmount - persistentProgressionMaterialAmount;
            if (missingAmount <= 0)
            {
                return "Unavailable";
            }

            return $"Need {missingAmount} more";
        }

        private static string ResolveResourceDisplayName(ResourceCategory resourceCategory)
        {
            return resourceCategory switch
            {
                ResourceCategory.PersistentProgressionMaterial => "Persistent progression material",
                ResourceCategory.RegionMaterial => "Region material",
                ResourceCategory.SoftCurrency => "Soft currency",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(resourceCategory),
                    resourceCategory,
                    "Unknown resource category."),
            };
        }
    }
}
