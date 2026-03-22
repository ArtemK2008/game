using System;
using System.Collections.Generic;
using System.Text;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Data.Towns;

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
            builder.AppendLine($"Region material: {screenState.RegionMaterialAmount}");
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

            builder.AppendLine();
            builder.Append("Conversions:");

            for (int index = 0; index < screenState.ConversionOptions.Count; index++)
            {
                TownServiceConversionOptionState conversionOption = screenState.ConversionOptions[index];
                builder.AppendLine();
                builder.Append("- ");
                builder.Append(conversionOption.ConversionDisplayName);
                builder.Append(" | ");
                builder.Append(ResolveResourceDisplayName(conversionOption.InputResourceCategory));
                builder.Append(" x");
                builder.Append(conversionOption.InputAmount);
                builder.Append(" -> ");
                builder.Append(ResolveResourceDisplayName(conversionOption.OutputResourceCategory));
                builder.Append(" x");
                builder.Append(conversionOption.OutputAmount);
                builder.Append(" | ");
                builder.Append(BuildConversionAvailabilityText(conversionOption));
            }

            AppendMaterialPowerPathText(builder, screenState.MaterialPowerPath);
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
                "Use the assignment controls below to update the selected character for future runs.";
        }

        public static string BuildSkillPackageActionButtonLabel(PlayableCharacterSkillPackageOption skillPackageOption)
        {
            if (skillPackageOption == null)
            {
                throw new ArgumentNullException(nameof(skillPackageOption));
            }

            return skillPackageOption.IsAssigned
                ? $"Assigned package: {skillPackageOption.DisplayName}"
                : $"Assign package: {skillPackageOption.DisplayName}";
        }

        public static string BuildGearAssignmentActionButtonLabel(PlayableCharacterGearAssignmentOption gearAssignmentOption)
        {
            if (gearAssignmentOption == null)
            {
                throw new ArgumentNullException(nameof(gearAssignmentOption));
            }

            string gearCategoryLabel = ResolveGearCategoryDisplayName(gearAssignmentOption.GearCategory);
            return gearAssignmentOption.IsEquipped
                ? $"Unequip {gearCategoryLabel}: {gearAssignmentOption.DisplayName}"
                : $"Equip {gearCategoryLabel}: {gearAssignmentOption.DisplayName}";
        }

        public static string BuildConversionActionButtonLabel(TownServiceConversionOptionState conversionOption)
        {
            if (conversionOption == null)
            {
                throw new ArgumentNullException(nameof(conversionOption));
            }

            if (conversionOption.IsAffordable)
            {
                return $"Run {conversionOption.ConversionDisplayName}";
            }

            return $"{conversionOption.ConversionDisplayName} Unavailable";
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

        private static string BuildConversionAvailabilityText(TownServiceConversionOptionState conversionOption)
        {
            if (conversionOption.IsAffordable)
            {
                return "Affordable";
            }

            int missingAmount = conversionOption.InputAmount - conversionOption.AvailableInputAmount;
            if (missingAmount <= 0)
            {
                return "Unavailable";
            }

            return $"Need {missingAmount} more";
        }

        private static void AppendMaterialPowerPathText(
            StringBuilder builder,
            TownServiceMaterialPowerPathState materialPowerPath)
        {
            builder.AppendLine();
            builder.AppendLine("Material power path:");
            builder.AppendLine(
                "Farm region material, refine it into persistent progression material, then invest it into projects.");

            if (materialPowerPath.ReadyRefinementCount > 0)
            {
                builder.AppendLine($"Ready refinements now: {materialPowerPath.ReadyRefinementCount}");
                builder.AppendLine(
                    "Persistent progression material after ready refinements: " +
                    materialPowerPath.PersistentProgressionMaterialAmountAfterRefinementPath);
            }
            else
            {
                builder.AppendLine(
                    $"Refinement progress: {materialPowerPath.RegionMaterialTowardsNextRefinementAmount} / " +
                    $"{materialPowerPath.RefinementInputRequirement} region material");
                builder.AppendLine(
                    "Persistent progression material after next refinement: " +
                    materialPowerPath.PersistentProgressionMaterialAmountAfterRefinementPath);
            }

            builder.AppendLine(
                "Already affordable projects: " +
                BuildProjectDisplayNameSummary(materialPowerPath.AlreadyAffordableProjectDisplayNames));
            builder.Append(
                "New project targets after refinement: " +
                BuildProjectDisplayNameSummary(materialPowerPath.NewProjectTargetDisplayNames));
        }

        private static string BuildProjectDisplayNameSummary(IReadOnlyList<string> projectDisplayNames)
        {
            if (projectDisplayNames == null || projectDisplayNames.Count == 0)
            {
                return "none";
            }

            return string.Join(", ", projectDisplayNames);
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

        private static string ResolveGearCategoryDisplayName(GearCategory gearCategory)
        {
            return gearCategory switch
            {
                GearCategory.PrimaryCombat => "primary",
                GearCategory.SecondarySupport => "support",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(gearCategory),
                    gearCategory,
                    "Unknown gear category."),
            };
        }
    }
}
