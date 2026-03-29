using System;
using System.Collections.Generic;
using System.Text;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Gear;
using Survivalon.Data.Towns;

namespace Survivalon.Towns
{
    /// <summary>
    /// Форматирует человекочитаемый текст town/service shell из уже разрешенного screen state.
    /// </summary>
    public static class TownServiceScreenTextBuilder
    {
        public static string BuildOverviewText(TownServiceScreenState screenState)
        {
            if (screenState == null)
            {
                throw new ArgumentNullException(nameof(screenState));
            }

            return
                $"Hub: {screenState.ServiceContext.DisplayName}\n" +
                $"Location: {screenState.LocationDisplayName}\n" +
                $"Best for: {screenState.LocationRewardFocusDisplayName}\n" +
                $"Reward source: {screenState.LocationRewardSourceDisplayName}\n" +
                $"Enemy emphasis: {screenState.LocationEnemyEmphasisDisplayName}\n" +
                $"Use this stop for: {BuildFunctionSummary(screenState)}";
        }

        public static string BuildProgressionText(TownServiceScreenState screenState)
        {
            if (screenState == null)
            {
                throw new ArgumentNullException(nameof(screenState));
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Progression projects");
            builder.AppendLine(
                $"Progression material on hand: {screenState.PersistentProgressionMaterialAmount}");
            builder.AppendLine($"Region material on hand: {screenState.RegionMaterialAmount}");
            builder.Append("Project board:");

            for (int index = 0; index < screenState.ProgressionOptions.Count; index++)
            {
                TownServiceProgressionOptionState progressionOption = screenState.ProgressionOptions[index];
                builder.AppendLine();
                builder.Append("- ");
                builder.Append(progressionOption.UpgradeDisplayName);
                builder.Append(" | Cost: ");
                builder.Append(PlayerFacingCoreLabelFormatter.FormatResourceCategory(progressionOption.CostResourceCategory));
                builder.Append(" x");
                builder.Append(progressionOption.CostAmount);
                builder.Append(" | ");
                builder.Append(BuildProgressionAvailabilityText(
                    progressionOption,
                    screenState.PersistentProgressionMaterialAmount));
            }

            builder.AppendLine();
            builder.Append("Refinement options:");

            for (int index = 0; index < screenState.ConversionOptions.Count; index++)
            {
                TownServiceConversionOptionState conversionOption = screenState.ConversionOptions[index];
                builder.AppendLine();
                builder.Append("- ");
                builder.Append(conversionOption.ConversionDisplayName);
                builder.Append(" | ");
                builder.Append(PlayerFacingCoreLabelFormatter.FormatResourceCategory(conversionOption.InputResourceCategory));
                builder.Append(" x");
                builder.Append(conversionOption.InputAmount);
                builder.Append(" -> ");
                builder.Append(PlayerFacingCoreLabelFormatter.FormatResourceCategory(conversionOption.OutputResourceCategory));
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
                "Build setup\n" +
                $"Character: {screenState.SelectedCharacterDisplayName}\n" +
                $"Skill package: {screenState.AssignedSkillPackageDisplayName}\n" +
                $"Primary gear: {screenState.PrimaryGearDisplayName}\n" +
                $"Support gear: {screenState.SupportGearDisplayName}\n" +
                "Change these here before your next run.";
        }

        public static string BuildSkillPackageActionButtonLabel(PlayableCharacterSkillPackageOption skillPackageOption)
        {
            if (skillPackageOption == null)
            {
                throw new ArgumentNullException(nameof(skillPackageOption));
            }

            return skillPackageOption.IsAssigned
                ? $"Using package: {skillPackageOption.DisplayName}"
                : $"Use package: {skillPackageOption.DisplayName}";
        }

        public static string BuildGearAssignmentActionButtonLabel(PlayableCharacterGearAssignmentOption gearAssignmentOption)
        {
            if (gearAssignmentOption == null)
            {
                throw new ArgumentNullException(nameof(gearAssignmentOption));
            }

            string gearCategoryLabel = ResolveGearCategoryDisplayName(gearAssignmentOption.GearCategory);
            return gearAssignmentOption.IsEquipped
                ? $"Remove {gearCategoryLabel}: {gearAssignmentOption.DisplayName}"
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
                return "progression projects and build setup";
            }

            if (screenState.ServiceContext.HasProgressionHubAccess)
            {
                return "progression projects";
            }

            return "build setup";
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
            builder.AppendLine("Next power path:");
            builder.AppendLine(
                "Farm region material, refine it into persistent progression material, then invest it into projects.");

            if (materialPowerPath.ReadyRefinementCount > 0)
            {
                builder.AppendLine($"Ready refinements now: {materialPowerPath.ReadyRefinementCount}");
                builder.AppendLine(
                    "Progression material after ready refinements: " +
                    materialPowerPath.PersistentProgressionMaterialAmountAfterRefinementPath);
            }
            else
            {
                builder.AppendLine(
                    $"Refinement progress: {materialPowerPath.RegionMaterialTowardsNextRefinementAmount} / " +
                    $"{materialPowerPath.RefinementInputRequirement} region material");
                builder.AppendLine(
                    "Progression material after the next refinement: " +
                    materialPowerPath.PersistentProgressionMaterialAmountAfterRefinementPath);
            }

            builder.AppendLine(
                "Ready to buy now: " +
                BuildProjectDisplayNameSummary(materialPowerPath.AlreadyAffordableProjectDisplayNames));
            builder.Append(
                "New projects after the next refinement: " +
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
