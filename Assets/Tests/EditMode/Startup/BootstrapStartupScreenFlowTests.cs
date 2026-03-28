using NUnit.Framework;
using Survivalon.Combat;
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
        public void ShouldShowCompactMainMenuOnInitialStartupAndDisableContinueWhenNoSavedStateExists()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(ContainsText(hostObject, "Main Menu"), Is.True);
                Assert.That(ContainsText(hostObject, "Start begins a fresh prototype session."), Is.True);
                Assert.That(FindButton(hostObject, "StartButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "ContinueButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.False);
                Assert.That(FindButton(hostObject, "SettingsButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "QuitButton").gameObject.activeInHierarchy, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldOpenRealSettingsSurfaceFromMainMenu()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                OpenSettingsFromMainMenu(hostObject);

                Assert.That(ContainsText(hostObject, "Settings"), Is.True);
                Assert.That(ContainsText(hostObject, "Changes apply immediately and persist automatically."), Is.True);
                Assert.That(ContainsText(hostObject, "Master volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Music volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "SFX volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Display mode: Windowed"), Is.True);
                Assert.That(FindButton(hostObject, "MasterVolumeDecreaseButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "MusicVolumeDecreaseButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "SfxVolumeDecreaseButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "DisplayModeToggleButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "SettingsBackButton").gameObject.activeInHierarchy, Is.True);
                Assert.That(FindButton(hostObject, "StartButton").gameObject.activeInHierarchy, Is.False);

                CloseSettingsFromMainMenu(hostObject);

                Assert.That(ContainsText(hostObject, "Main Menu"), Is.True);
                Assert.That(FindButton(hostObject, "StartButton").gameObject.activeInHierarchy, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldPersistChangedSettingsAcrossRestartAndApplyLoadedValues()
        {
            GameObject firstHostObject = new GameObject("BootstrapStartupHost_First");
            GameObject secondHostObject = new GameObject("BootstrapStartupHost_Second");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            MemoryUserSettingsStorage settingsStorage = new MemoryUserSettingsStorage();
            FakeDisplaySettingsApplier firstDisplaySettingsApplier = new FakeDisplaySettingsApplier();
            FakeDisplaySettingsApplier secondDisplaySettingsApplier = new FakeDisplaySettingsApplier();

            try
            {
                CreateAndInitializeBootstrap(
                    firstHostObject,
                    storage,
                    autoEnterPlayableFlow: false,
                    settingsStorage: settingsStorage,
                    displaySettingsApplier: firstDisplaySettingsApplier);

                OpenSettingsFromMainMenu(firstHostObject);
                FindButton(firstHostObject, "MasterVolumeDecreaseButton").onClick.Invoke();
                FindButton(firstHostObject, "MusicVolumeDecreaseButton").onClick.Invoke();
                FindButton(firstHostObject, "SfxVolumeDecreaseButton").onClick.Invoke();
                FindButton(firstHostObject, "DisplayModeToggleButton").onClick.Invoke();

                Assert.That(settingsStorage.SavedSettingsState, Is.Not.Null);
                Assert.That(settingsStorage.SavedSettingsState.MasterVolume, Is.EqualTo(0.9f).Within(0.001f));
                Assert.That(settingsStorage.SavedSettingsState.MusicVolume, Is.EqualTo(0.9f).Within(0.001f));
                Assert.That(settingsStorage.SavedSettingsState.SfxVolume, Is.EqualTo(0.9f).Within(0.001f));
                Assert.That(settingsStorage.SavedSettingsState.UseFullscreen, Is.True);
                Assert.That(settingsStorage.SaveCallCount, Is.EqualTo(4));
                Assert.That(firstDisplaySettingsApplier.LastAppliedFullscreen, Is.True);

                CreateAndInitializeBootstrap(
                    secondHostObject,
                    storage,
                    autoEnterPlayableFlow: false,
                    settingsStorage: settingsStorage,
                    displaySettingsApplier: secondDisplaySettingsApplier);

                OpenSettingsFromMainMenu(secondHostObject);

                Assert.That(ContainsText(secondHostObject, "Master volume: 90%"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Music volume: 90%"), Is.True);
                Assert.That(ContainsText(secondHostObject, "SFX volume: 90%"), Is.True);
                Assert.That(ContainsText(secondHostObject, "Display mode: Fullscreen"), Is.True);
                Assert.That(secondDisplaySettingsApplier.LastAppliedFullscreen, Is.True);
                Assert.That(secondDisplaySettingsApplier.ApplyCallCount, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(firstHostObject);
                Object.DestroyImmediate(secondHostObject);
            }
        }

        [Test]
        public void ShouldOpenRealSettingsSurfaceFromWorldMapSystemMenuAndResumeCurrentContext()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                OpenSystemMenu(hostObject);

                Assert.That(ContainsText(hostObject, "System Menu"), Is.True);
                Assert.That(FindButton(hostObject, "SystemMenuExitButton").interactable, Is.True);

                OpenSettingsFromSystemMenu(hostObject);

                Assert.That(ContainsText(hostObject, "Settings"), Is.True);
                Assert.That(ContainsText(hostObject, "Changes apply immediately and persist automatically."), Is.True);
                Assert.That(ContainsText(hostObject, "Master volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Display mode: Windowed"), Is.True);
                Assert.That(FindButton(hostObject, "SystemMenuSettingsBackButton").gameObject.activeInHierarchy, Is.True);

                CloseSettingsFromSystemMenu(hostObject);
                ResumeFromSystemMenu(hostObject);

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldDisableContinueWhenPersistedStateHasNoResumableSafeContext()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            storage.Seed(new PersistentGameState());

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldDisableContinueWhenPersistedTownServiceSafeTargetIsInvalid()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.SetLastSafeNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.ReplaceReachableNodes(new[] { new NodeId("region_002_node_001") });
            seededGameState.SafeResumeState.MarkTownService(new NodeId("region_999_node_999"));
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.False);
                Assert.That(
                    ContainsText(hostObject, "Continue becomes available after a safe world or service save."),
                    Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldStartFreshWorldFlowFromMainMenuEvenWhenSavedSafeContextExists()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.SetLastSafeNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.ReplaceReachableNodes(new[] { new NodeId("region_002_node_001") });
            seededGameState.SafeResumeState.MarkTownService(new NodeId("region_002_node_001"));
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.True);

                StartFromMainMenu(hostObject);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Location: Verdant Frontier"), Is.True);
                Assert.That(ContainsText(hostObject, "Current: Raider Trail (In progress) | Selected: none"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowCompactMainMenuWhenWorldMapSystemExitIsRequested()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                OpenSystemMenu(hostObject);
                FindButton(hostObject, "SystemMenuExitButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Main Menu"), Is.True);
                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.True);
                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(BootstrapWorldScenario.ForestPushNodeId));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldContinueSavedWorldFlowFromMainMenuWhenSafeWorldContextExists()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.SetLastSafeNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.ReplaceReachableNodes(new[] { new NodeId("region_002_node_001") });
            seededGameState.SafeResumeState.MarkWorldMap(new NodeId("region_002_node_001"));
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.True);

                ContinueFromMainMenu(hostObject);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Location: Echo Caverns"), Is.True);
                Assert.That(ContainsText(hostObject, "Current: Cavern Service Hub (Available) | Selected: none"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldContinueSavedTownServiceFlowFromMainMenuWhenSafeTownContextExists()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState seededGameState = BootstrapWorldTestData.CreateGameState();
            seededGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.SetLastSafeNode(new NodeId("region_002_node_001"));
            seededGameState.WorldState.ReplaceReachableNodes(new[] { new NodeId("region_002_node_001") });
            seededGameState.SafeResumeState.MarkTownService(new NodeId("region_002_node_001"));
            storage.Seed(seededGameState);

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage, autoEnterPlayableFlow: false);

                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.True);

                ContinueFromMainMenu(hostObject);

                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Cavern Service Hub"), Is.True);
                Assert.That(ContainsText(hostObject, "Location: Echo Caverns"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldRequestQuitFromMainMenuThroughQuitService()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            FakeApplicationQuitService quitService = new FakeApplicationQuitService();

            try
            {
                CreateAndInitializeBootstrap(
                    hostObject,
                    storage,
                    autoEnterPlayableFlow: false,
                    quitService: quitService);

                FindButton(hostObject, "QuitButton").onClick.Invoke();

                Assert.That(quitService.WasQuitRequested, Is.True);
                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

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
                Assert.That(ContainsText(hostObject, "Forward: Echo Approach, Relic Cache"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowCompactMainMenuWhenTownServiceStopIsRequested()
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
                Assert.That(ContainsText(hostObject, "Main Menu"), Is.True);
                Assert.That(ContainsText(hostObject, "Continue resumes the last safe world or service context."), Is.True);
                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.True);
                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.TownService));
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
                Assert.That(ContainsText(hostObject, "Forward: Cavern Service Hub, Forest Farm, Raider Holdout"), Is.True);
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

        [Test]
        public void ShouldShowOptionalEliteChallengePathWithoutReplacingMainRoutes()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(ContainsText(hostObject, "Raider Holdout"), Is.True);
                Assert.That(ContainsText(hostObject, "State: Available | Elite challenge"), Is.True);

                Button eliteButton = FindButton(hostObject, "region_001_node_006_Button");
                Button serviceButton = FindButton(hostObject, "region_002_node_001_Button");
                Button gateButton = FindButton(hostObject, "region_001_node_003_Button");

                Assert.That(eliteButton.interactable, Is.True);
                Assert.That(serviceButton.interactable, Is.True);
                Assert.That(gateButton.interactable, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldShowCompactMainMenuWhenTownServiceSystemExitIsRequested()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                OpenSystemMenu(hostObject);
                FindButton(hostObject, "SystemMenuExitButton").onClick.Invoke();

                Assert.That(CountActiveComponents<WorldMapScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<TownServiceScreen>(hostObject), Is.EqualTo(0));
                Assert.That(CountActiveComponents<StartupPlaceholderView>(hostObject), Is.EqualTo(1));
                Assert.That(ContainsText(hostObject, "Main Menu"), Is.True);
                Assert.That(FindButton(hostObject, "ContinueButton").interactable, Is.True);
                Assert.That(storage.HasSavedState, Is.True);
                Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.TownService));
                Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldKeepSingleUiSystemFeedbackAudioHostAcrossCurrentScreenTransitions()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(hostObject.GetComponentsInChildren<UiSystemFeedbackAudioHost>(true).Length, Is.EqualTo(1));

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                Assert.That(hostObject.GetComponentsInChildren<UiSystemFeedbackAudioHost>(true).Length, Is.EqualTo(1));

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                Assert.That(hostObject.GetComponentsInChildren<UiSystemFeedbackAudioHost>(true).Length, Is.EqualTo(1));

                EnterNodeFromWorldMap(hostObject, "region_001_node_002_Button");
                Assert.That(hostObject.GetComponentsInChildren<UiSystemFeedbackAudioHost>(true).Length, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldKeepSingleCombatFeedbackAudioHostAcrossCurrentScreenTransitions()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(hostObject.GetComponentsInChildren<CombatFeedbackAudioHost>(true).Length, Is.EqualTo(1));

                EnterNodeFromWorldMap(hostObject, "region_001_node_001_Button");
                Assert.That(hostObject.GetComponentsInChildren<CombatFeedbackAudioHost>(true).Length, Is.EqualTo(1));

                ReturnToWorldMap(hostObject);
                Assert.That(hostObject.GetComponentsInChildren<CombatFeedbackAudioHost>(true).Length, Is.EqualTo(1));

                EnterNodeFromWorldMap(hostObject, "region_001_node_003_Button");
                Assert.That(hostObject.GetComponentsInChildren<CombatFeedbackAudioHost>(true).Length, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void ShouldKeepSingleMusicAudioHostAndUseCalmMusicAcrossSafeContexts()
        {
            GameObject hostObject = new GameObject("BootstrapStartupHost");
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            MusicAudioClipRegistry clipRegistry = MusicAudioClipRegistry.LoadOrNull();

            try
            {
                CreateAndInitializeBootstrap(hostObject, storage);

                Assert.That(hostObject.GetComponentsInChildren<MusicAudioHost>(true).Length, Is.EqualTo(1));
                AssertCurrentMusicClip(hostObject, clipRegistry, MusicContextId.Calm);

                EnterNodeFromWorldMap(hostObject, "region_002_node_001_Button");
                Assert.That(hostObject.GetComponentsInChildren<MusicAudioHost>(true).Length, Is.EqualTo(1));
                AssertCurrentMusicClip(hostObject, clipRegistry, MusicContextId.Calm);

                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();
                Assert.That(hostObject.GetComponentsInChildren<MusicAudioHost>(true).Length, Is.EqualTo(1));
                AssertCurrentMusicClip(hostObject, clipRegistry, MusicContextId.Calm);

                FindButton(hostObject, "StopSessionButton").onClick.Invoke();
                Assert.That(hostObject.GetComponentsInChildren<MusicAudioHost>(true).Length, Is.EqualTo(1));
                AssertCurrentMusicClip(hostObject, clipRegistry, MusicContextId.Calm);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static void AssertCurrentMusicClip(
            GameObject hostObject,
            MusicAudioClipRegistry clipRegistry,
            MusicContextId contextId)
        {
            Assert.That(clipRegistry, Is.Not.Null);
            Assert.That(clipRegistry.TryGetClip(contextId, out AudioClip expectedClip), Is.True);
            Assert.That(expectedClip, Is.Not.Null);

            MusicAudioHost musicAudioHost = hostObject.GetComponentInChildren<MusicAudioHost>(true);
            Assert.That(musicAudioHost, Is.Not.Null);

            AudioSource audioSource = musicAudioHost.GetComponent<AudioSource>();
            Assert.That(audioSource, Is.Not.Null);
            Assert.That(audioSource.clip, Is.EqualTo(expectedClip));
        }

        private sealed class FakeApplicationQuitService : IApplicationQuitService
        {
            public bool WasQuitRequested { get; private set; }

            public void RequestQuit()
            {
                WasQuitRequested = true;
            }
        }
    }
}

