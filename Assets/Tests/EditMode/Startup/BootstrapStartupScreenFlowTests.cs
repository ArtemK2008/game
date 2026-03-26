using NUnit.Framework;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Startup;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.Towns;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapStartupScreenFlowTests : BootstrapStartupScreenFlowTestBase
    {
        [Test]
        public void ShouldReuseSingleWorldMapPlaceholderAndTownServiceScreensAcrossEnterAndReturnFlow()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                AssertScreenCounts(hostObject, 1, 0, 1, 0, 0, 0);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                AssertScreenCounts(hostObject, 0, 0, 1, 0, 1, 1);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                AssertScreenCounts(hostObject, 1, 0, 1, 0, 0, 1);

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                AssertScreenCounts(hostObject, 0, 1, 1, 1, 0, 1);

                ReturnToWorldMap(hostObject);
                AssertScreenCounts(hostObject, 1, 0, 1, 1, 0, 1);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPersistSafeResumeContextWhenReturningToWorldFromTownServiceScreen()
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
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
                Assert.That(storage.SavedGameState.OfflineProgressStableSaveAnchorState.HasStableSaveAnchor, Is.True);
                Assert.That(
                    storage.SavedGameState.OfflineProgressStableSaveAnchorState.LastStableSaveUnixTimeSeconds,
                    Is.GreaterThan(0));
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
        public void ShouldShowRecentNodeContextAfterReturningToWorldMap()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Location: Echo Caverns"), Is.True);
                Assert.That(ContainsText(hostObject, "Current: Cavern Service Hub (Available) | Selected: none"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowStartupPlaceholderWhenTownServiceStopIsRequested()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "StopSessionButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<NodePlaceholderScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));

                StartupPlaceholderView placeholderView = hostObject.GetComponentInChildren<StartupPlaceholderView>(true);
                Assert.That(placeholderView, Is.Not.Null);
                Assert.That(ContainsText(hostObject, "Main Menu Placeholder"), Is.True);
                Assert.That(ContainsText(hostObject, "Session stopped at a safe world or service point."), Is.True);
                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowReadableWorldStateSummaryOnInitialWorldMap()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Current: Raider Trail (In progress) | Selected: none"), Is.True);
                Assert.That(ContainsText(hostObject, "Forward: Cavern Service Hub, Forest Farm"), Is.True);
                Assert.That(ContainsText(hostObject, "Backtrack: Frontier Entry | Replayable: none"), Is.True);
                Assert.That(ContainsText(hostObject, "Blocked: Frontier Gate"), Is.True);
                Assert.That(ContainsText(hostObject, "Node states: Available = enterable"), Is.True);
                Assert.That(FindButton(hostObject, "EnterSelectedNodeButton").interactable, Is.False);
                Assert.That(
                    FindButton(hostObject, "EnterSelectedNodeButton").GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Select a reachable node to enter"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldAllowQuickReturnToTownServiceFromWorldMapWithoutReselectingNode()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Assert.That(entryButton.interactable, Is.True);
                Assert.That(
                    entryButton.GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Return to Cavern Service Hub"));

                entryButton.onClick.Invoke();

                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldNotShowQuickReturnShortcutAfterRestartLoadFromTownServiceWorldSave()
        {
            GameObject firstHostObject = new GameObject("BootstrapStartupHost_First");
            GameObject secondHostObject = new GameObject("BootstrapStartupHost_Second");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(firstHostObject, storage);

                EnterNodeFromWorldMap(firstHostObject, "region_002_node_001_Button");
                FindButton(firstHostObject, "ReturnToWorldMapButton").onClick.Invoke();

                CreateAndInitializeBootstrap(secondHostObject, storage);

                Button entryButton = FindButton(secondHostObject, "EnterSelectedNodeButton");
                Assert.That(entryButton.interactable, Is.False);
                Assert.That(
                    entryButton.GetComponentInChildren<Text>(true).text,
                    Is.EqualTo("Select a reachable node to enter"));
                Assert.That(ContainsText(secondHostObject, "Current: Cavern Service Hub (Available) | Selected: none"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(firstHostObject);
                Object.DestroyImmediate(secondHostObject);
            }
        }

        [Test]
        public void ShouldPersistWorldMapBuildPreparationAcrossRestartAndKeepCharacterBuildsSeparated()
        {
            GameObject firstHostObject = new GameObject("BootstrapStartupHost_First");
            GameObject secondHostObject = new GameObject("BootstrapStartupHost_Second");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(firstHostObject, storage);

                FindButton(firstHostObject, $"{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton")
                    .onClick.Invoke();
                FindButton(firstHostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(firstHostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();
                FindButton(firstHostObject, "character_striker_CharacterButton").onClick.Invoke();

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
                Assert.That(
                    storage.SavedGameState.TryGetCharacterState(
                        "character_striker",
                        out PersistentCharacterState savedStrikerState),
                    Is.True);
                Assert.That(savedStrikerState.IsActive, Is.True);
                Assert.That(savedVanguardState.IsActive, Is.False);

                CreateAndInitializeBootstrap(secondHostObject, storage);

                Assert.That(ContainsText(secondHostObject, "Selected character: Striker"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Assigned package: Relentless Burst"), Is.True);

                FindButton(secondHostObject, "character_vanguard_CharacterButton").onClick.Invoke();

                Assert.That(ContainsText(secondHostObject, "Selected character: Vanguard"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Assigned package: Burst Drill"), Is.True);
                Assert.That(
                    ContainsText(secondHostObject, "Primary gear: Training Blade | Support gear: Guard Charm"),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(firstHostObject);
                Object.DestroyImmediate(secondHostObject);
            }
        }
    }
}

