using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Data.Progression;
using Survivalon.Startup;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;
using Survivalon.World;
using UnityEngine;

namespace Survivalon.Tests.EditMode.Startup
{
    /// <summary>
    /// Проверяет startup -> town/service flow после переноса runtime character services и options в Characters.
    /// </summary>
    public sealed class BootstrapStartupTownServiceFlowTests : BootstrapStartupScreenFlowTestBase
    {
        [Test]
        public void ShouldOpenTownServiceScreenForServiceNodeAndShowProgressionAndBuildSections()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            seededGameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 2);
            storage.Seed(seededGameState);
            TownServiceBackgroundRegistry serviceBackgroundRegistry = TownServiceBackgroundRegistry.LoadOrNull();
            CombatLocationBackgroundRegistry combatBackgroundRegistry = CombatLocationBackgroundRegistry.LoadOrNull();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");

                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Echo Caverns"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward focus: Persistent progression gains"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward source: Cavern relic caches"), Is.True);
                Assert.That(ContainsText(hostObject, "Enemy emphasis: Gate guardians"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression projects"), Is.True);
                Assert.That(ContainsText(hostObject, "Next power path:"), Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "Ready to buy now: Combat Baseline Project, Farm Yield Project"),
                    Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "New projects after the next refinement: Push Offense Project, Refinement Efficiency Project, Boss Salvage Project"),
                    Is.True);
                Assert.That(ContainsText(hostObject, "Build setup"), Is.True);
                Assert.That(ContainsText(hostObject, "Skill package: Standard Guard"), Is.True);
                Assert.That(serviceBackgroundRegistry, Is.Not.Null);
                Assert.That(
                    serviceBackgroundRegistry.TryGetBackground(
                        "town_service_cavern_hub",
                        out Sprite serviceBackground),
                    Is.True);
                Assert.That(combatBackgroundRegistry, Is.Not.Null);
                Assert.That(
                    combatBackgroundRegistry.TryGetBackground(
                        "location_identity_echo_caverns",
                        out Sprite combatBackground),
                    Is.True);
                Assert.That(FindImage(hostObject, "TownServiceBackgroundArt").sprite, Is.EqualTo(serviceBackground));
                Assert.That(FindImage(hostObject, "TownServiceBackgroundArt").sprite, Is.Not.EqualTo(combatBackground));
                Assert.That(
                    FindButton(
                        hostObject,
                        $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton"),
                    Is.Not.Null);
                Assert.That(FindButton(hostObject, "RefinementEfficiencyProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(FindButton(hostObject, "BossSalvageProject_PurchaseUpgradeButton"), Is.Not.Null);
                Assert.That(FindButton(hostObject, "RegionMaterialRefinement_ConversionButton"), Is.Not.Null);
                Assert.That(FindButton(hostObject, $"TownService_{GearIds.TrainingBlade}_GearButton"), Is.Not.Null);
                Assert.That(FindButton(hostObject, $"TownService_{GearIds.GuardCharm}_GearButton"), Is.Not.Null);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "Use the assignment controls below to update the selected character for future runs."),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldFallbackToGenericPlaceholderWhenServiceNodeHasNoTownServiceContext()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                BootstrapStartup bootstrapStartup = CreateAndInitializeBootstrap(hostObject, storage);

                ShowEnteredPlaceholderState(
                    bootstrapStartup,
                    NodePlaceholderTestData.CreateServicePlaceholderState());

                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Reward focus: Persistent progression gains"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldConvertRegionMaterialFromTownServiceScreenAndPersistImmediately()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");

                Assert.That(storage.SaveCallCount, Is.EqualTo(0));
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState, Is.Not.SameAs(seededGameState));
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(3));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(0));

                FindButton(hostObject, "RegionMaterialRefinement_ConversionButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Region material: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression material on hand: 1"), Is.True);
                Assert.That(storage.SaveCallCount, Is.EqualTo(1));
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPurchaseUpgradeFromTownServiceScreenAndPersistImmediately()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "CombatBaselineProject_PurchaseUpgradeButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Progression material on hand: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "- Combat Baseline Project | Cost: Persistent progression material x1 | Purchased"), Is.True);
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
                Assert.That(
                    storage.SavedGameState.ProgressionState.TryGetEntry(
                        "account_wide_combat_baseline_project",
                        out ProgressionEntryState progressionEntry),
                    Is.True);
                Assert.That(progressionEntry.IsUnlocked, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPurchaseRefinementEfficiencyProjectAndIncreaseFutureTownConversionOutput()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 2);
            seededGameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "RefinementEfficiencyProject_PurchaseUpgradeButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Progression material on hand: 0"), Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "- Refinement Efficiency Project | Cost: Persistent progression material x2 | Purchased"),
                    Is.True);
                Assert.That(
                    ContainsText(
                        hostObject,
                        "- Region Material Refinement | Region material x3 -> Persistent progression material x2 | Affordable"),
                    Is.True);

                FindButton(hostObject, "RegionMaterialRefinement_ConversionButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Region material: 0"), Is.True);
                Assert.That(ContainsText(hostObject, "Progression material on hand: 2"), Is.True);
                Assert.That(storage.SaveCallCount, Is.EqualTo(2));
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
                Assert.That(
                    storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                    Is.EqualTo(2));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldAssignBuildPreparationFromTownServiceScreenAndPersistImmediately()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(
                    hostObject,
                    $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton")
                    .onClick.Invoke();
                FindButton(hostObject, $"TownService_{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"TownService_{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Skill package: Burst Drill"), Is.True);
                Assert.That(ContainsText(hostObject, "Primary gear: Training Blade"), Is.True);
                Assert.That(ContainsText(hostObject, "Support gear: Guard Charm"), Is.True);
                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(
                    storage.SavedGameState.TryGetCharacterState(
                        "character_vanguard",
                        out PersistentCharacterState savedVanguardState),
                    Is.True);
                Assert.That(savedVanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
                Assert.That(
                    savedVanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.PrimaryCombat,
                        out EquippedGearState savedPrimaryGearState),
                    Is.True);
                Assert.That(savedPrimaryGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
                Assert.That(
                    savedVanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.SecondarySupport,
                        out EquippedGearState savedSupportGearState),
                    Is.True);
                Assert.That(savedSupportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Character: Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "Skill package: Burst Drill"), Is.True);
                Assert.That(ContainsText(hostObject, "Primary gear: Training Blade"), Is.True);
                Assert.That(ContainsText(hostObject, "Support gear: Guard Charm"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldRestoreTownBuildPreparationAfterRestartLoad()
        {
            GameObject firstHostObject = new GameObject("BootstrapStartupHost_First");
            GameObject secondHostObject = new GameObject("BootstrapStartupHost_Second");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(firstHostObject, storage);

                EnterNodeFromWorldMap(firstHostObject, "region_002_node_001_Button");
                FindButton(
                    firstHostObject,
                    $"TownService_{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton")
                    .onClick.Invoke();
                FindButton(firstHostObject, $"TownService_{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(firstHostObject, $"TownService_{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                Assert.That(storage.SavedGameState, Is.Not.Null);
                Assert.That(
                    storage.SavedGameState.TryGetCharacterState(
                        "character_vanguard",
                        out PersistentCharacterState savedVanguardState),
                    Is.True);
                Assert.That(savedVanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
                Assert.That(
                    savedVanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.PrimaryCombat,
                        out EquippedGearState savedPrimaryGearState),
                    Is.True);
                Assert.That(savedPrimaryGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
                Assert.That(
                    savedVanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.SecondarySupport,
                        out EquippedGearState savedSupportGearState),
                    Is.True);
                Assert.That(savedSupportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));

                CreateAndInitializeBootstrap(secondHostObject, storage);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<WorldMapScreen>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(secondHostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(secondHostObject), Is.EqualTo(1));
                Assert.That(ContainsText(secondHostObject, "Location: Echo Caverns"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Character: Vanguard"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Skill package: Burst Drill"), Is.True);
                Assert.That(
                    ContainsText(secondHostObject, "Primary gear: Training Blade"),
                    Is.True);
                Assert.That(ContainsText(secondHostObject, "Support gear: Guard Charm"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Run finished."), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(firstHostObject);
                Object.DestroyImmediate(secondHostObject);
            }
        }
    }
}
