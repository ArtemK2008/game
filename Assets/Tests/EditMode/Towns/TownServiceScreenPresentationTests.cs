using System;
using NUnit.Framework;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Data.Progression;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;
using Survivalon.Towns;

namespace Survivalon.Tests.EditMode.Towns
{
    /// <summary>
    /// Проверяет town/service presentation после переноса runtime character option models в Characters.
    /// </summary>
    public sealed class TownServiceScreenPresentationTests
    {
        [Test]
        public void ShouldBuildReadableOverviewProgressionAndBuildTexts()
        {
            TownServiceScreenState screenState = CreateScreenState();

            string overviewText = TownServiceScreenTextBuilder.BuildOverviewText(screenState);
            string progressionText = TownServiceScreenTextBuilder.BuildProgressionText(screenState);
            string buildText = TownServiceScreenTextBuilder.BuildBuildPreparationText(screenState);

            Assert.That(overviewText, Is.EqualTo(
                "Hub: Cavern Service Hub\n" +
                "Location: Echo Caverns\n" +
                "Best for: Persistent progression gains\n" +
                "Reward source: Cavern relic caches\n" +
                "Enemy emphasis: Gate guardians\n" +
                "Use this stop for: progression projects and build setup"));
            Assert.That(progressionText, Does.Contain("Progression projects"));
            Assert.That(progressionText, Does.Contain("Progression material on hand: 1"));
            Assert.That(progressionText, Does.Contain("Region material on hand: 2"));
            Assert.That(progressionText, Does.Contain("- Combat Baseline Project | Cost: Persistent progression material x1 | Purchased"));
            Assert.That(progressionText, Does.Contain("- Push Offense Project | Cost: Persistent progression material x2 | Need 1 more"));
            Assert.That(progressionText, Does.Contain("- Farm Yield Project | Cost: Persistent progression material x1 | Affordable"));
            Assert.That(progressionText, Does.Contain("- Refinement Efficiency Project | Cost: Persistent progression material x2 | Need 1 more"));
            Assert.That(progressionText, Does.Contain("- Boss Salvage Project | Cost: Persistent progression material x2 | Need 1 more"));
            Assert.That(
                progressionText,
                Does.Contain(
                    "- Region Material Refinement | Region material x3 -> Persistent progression material x1 | Need 1 more"));
            Assert.That(progressionText, Does.Contain("Refinement options:"));
            Assert.That(progressionText, Does.Contain("Next power path:"));
            Assert.That(progressionText, Does.Contain("Refinement progress: 2 / 3 region material"));
            Assert.That(
                progressionText,
                Does.Contain("Progression material after the next refinement: 2"));
            Assert.That(
                progressionText,
                Does.Contain("Ready to buy now: Combat Baseline Project, Farm Yield Project"));
            Assert.That(
                progressionText,
                Does.Contain(
                    "New projects after the next refinement: Push Offense Project, Refinement Efficiency Project, Boss Salvage Project"));
            Assert.That(buildText, Is.EqualTo(
                "Build setup\n" +
                "Character: Vanguard\n" +
                "Skill package: Burst Drill\n" +
                "Primary gear: Training Blade\n" +
                "Support gear: Guard Charm\n" +
                "Change these here before your next run."));
        }

        [Test]
        public void ShouldRejectMissingScreenState()
        {
            Assert.That(
                () => TownServiceScreenTextBuilder.BuildOverviewText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("screenState"));
            Assert.That(
                () => TownServiceScreenTextBuilder.BuildProgressionText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("screenState"));
            Assert.That(
                () => TownServiceScreenTextBuilder.BuildBuildPreparationText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("screenState"));
        }

        private static TownServiceScreenState CreateScreenState()
        {
            return new TownServiceScreenState(
                TownServiceContextCatalog.CavernServiceHub,
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                new NodeId("region_001_node_002"),
                locationDisplayName: "Echo Caverns",
                locationRewardSourceDisplayName: "Cavern relic caches",
                locationRewardFocusDisplayName: "Persistent progression gains",
                locationEnemyEmphasisDisplayName: "Gate guardians",
                persistentProgressionMaterialAmount: 1,
                regionMaterialAmount: 2,
                materialPowerPathState: new TownServiceMaterialPowerPathState(
                    readyRefinementCount: 0,
                    regionMaterialTowardsNextRefinementAmount: 2,
                    refinementInputRequirement: 3,
                    persistentProgressionMaterialAfterRefinementPath: 2,
                    alreadyAffordableProjectDisplayNames: new[]
                    {
                        "Combat Baseline Project",
                        "Farm Yield Project",
                    },
                    newProjectTargetDisplayNames: new[]
                    {
                        "Push Offense Project",
                        "Refinement Efficiency Project",
                        "Boss Salvage Project",
                    }),
                progressionOptions: new[]
                {
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.CombatBaselineProject,
                        "Combat Baseline Project",
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 1,
                        isPurchased: true,
                        isAffordable: false),
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.PushOffenseProject,
                        "Push Offense Project",
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 2,
                        isPurchased: false,
                        isAffordable: false),
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.FarmYieldProject,
                        "Farm Yield Project",
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 1,
                        isPurchased: false,
                        isAffordable: true),
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.RefinementEfficiencyProject,
                        "Refinement Efficiency Project",
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 2,
                        isPurchased: false,
                        isAffordable: false),
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.BossSalvageProject,
                        "Boss Salvage Project",
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 2,
                        isPurchased: false,
                        isAffordable: false),
                },
                conversionOptions: new[]
                {
                    new TownServiceConversionOptionState(
                        TownServiceConversionId.RegionMaterialRefinement,
                        "Region Material Refinement",
                        ResourceCategory.RegionMaterial,
                        inputAmount: 3,
                        ResourceCategory.PersistentProgressionMaterial,
                        outputAmount: 1,
                        availableInputAmount: 2,
                        isAffordable: false),
                },
                skillPackageOptions: new[]
                {
                    new PlayableCharacterSkillPackageOption(
                        "character_vanguard",
                        PlayableCharacterSkillPackageIds.VanguardBurstDrill,
                        "Burst Drill",
                        "Adds Burst Strike.",
                        isAssigned: true),
                },
                gearAssignmentOptions: new[]
                {
                    new PlayableCharacterGearAssignmentOption(
                        "character_vanguard",
                        GearIds.TrainingBlade,
                        "Training Blade",
                        GearCategory.PrimaryCombat,
                        isEquipped: true),
                    new PlayableCharacterGearAssignmentOption(
                        "character_vanguard",
                        GearIds.GuardCharm,
                        "Guard Charm",
                        GearCategory.SecondarySupport,
                        isEquipped: true),
                },
                selectedCharacterDisplayName: "Vanguard",
                assignedSkillPackageDisplayName: "Burst Drill",
                primaryGearDisplayName: "Training Blade",
                supportGearDisplayName: "Guard Charm");
        }
    }
}
