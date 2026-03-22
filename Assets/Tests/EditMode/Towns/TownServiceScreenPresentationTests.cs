using System;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;
using Survivalon.Towns;

namespace Survivalon.Tests.EditMode.Towns
{
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
                "Service context: Cavern Service Hub\n" +
                "Region: region_002\n" +
                "Node: region_002_node_001\n" +
                "Entered from: region_001_node_002\n" +
                "Functions: Progression hub, Build preparation"));
            Assert.That(progressionText, Does.Contain("Persistent progression material: 1"));
            Assert.That(progressionText, Does.Contain("- Combat Baseline Project | Cost: Persistent progression material x1 | Purchased"));
            Assert.That(progressionText, Does.Contain("- Push Offense Project | Cost: Persistent progression material x2 | Need 1 more"));
            Assert.That(progressionText, Does.Contain("- Farm Yield Project | Cost: Persistent progression material x1 | Affordable"));
            Assert.That(buildText, Is.EqualTo(
                "Build preparation\n" +
                "Selected character: Vanguard\n" +
                "Assigned package: Burst Drill\n" +
                "Primary gear: Training Blade\n" +
                "Support gear: Guard Charm\n" +
                "Current build changes still happen on the world map in this MVP."));
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
                persistentProgressionMaterialAmount: 1,
                progressionOptions: new[]
                {
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.CombatBaselineProject,
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 1,
                        isPurchased: true,
                        isAffordable: false),
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.PushOffenseProject,
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 2,
                        isPurchased: false,
                        isAffordable: false),
                    new TownServiceProgressionOptionState(
                        AccountWideUpgradeId.FarmYieldProject,
                        ResourceCategory.PersistentProgressionMaterial,
                        costAmount: 1,
                        isPurchased: false,
                        isAffordable: true),
                },
                "Vanguard",
                "Burst Drill",
                "Training Blade",
                "Guard Charm");
        }
    }
}
