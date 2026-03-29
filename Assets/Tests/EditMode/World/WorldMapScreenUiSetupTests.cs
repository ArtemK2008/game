using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Data.Progression;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    /// <summary>
    /// Проверяет runtime UI setup карты мира после переноса character services в Characters.
    /// </summary>
    public sealed class WorldMapScreenUiSetupTests
    {
        [SetUp]
        public void SetUp()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (EventSystem.current != null)
            {
                Object.DestroyImmediate(EventSystem.current.gameObject);
            }
        }

        [Test]
        public void Show_ShouldCreateStableWorldMapSectionSurfacesWithMapAsPrimaryArea()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                Assert.That(FindRectTransform(hostObject, "Panel"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "Title"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "Summary"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "CharacterSelectionSummary"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "CharacterSelectionList"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "BuildAssignmentSummary"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "SkillPackageAssignmentList"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "GearAssignmentList"), Is.Not.Null);
                Assert.That(FindButton(hostObject, "EnterSelectedNodeButton"), Is.Not.Null);
                Assert.That(FindScrollRect(hostObject, "NodeListScrollView"), Is.Not.Null);
                Assert.That(FindRectTransform(hostObject, "MapBackgroundArt"), Is.Not.Null);

                ForceUiLayout(hostObject);

                RectTransform sidebarRect = FindRectTransform(hostObject, "Sidebar");
                RectTransform viewportRect = FindRectTransform(hostObject, "NodeListViewport");
                Assert.That(viewportRect.rect.width, Is.GreaterThan(sidebarRect.rect.width * 1.8f));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRenderAuthoredMapBackgroundAndMeaningBasedNodeIcons()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                Image backgroundImage = FindImage(hostObject, "MapBackgroundArt");
                Image currentNodeIcon = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.ForestPushNodeId.Value + "_Button").gameObject,
                    "StateIcon");
                Image ordinaryNodeIcon = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.CavernPushNodeId.Value + "_Button").gameObject,
                    "StateIcon");
                Image farmNodeIcon = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button").gameObject,
                    "StateIcon");
                Image eliteNodeIcon = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.ForestEliteNodeId.Value + "_Button").gameObject,
                    "StateIcon");
                Image serviceNodeIcon = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.CavernServiceNodeId.Value + "_Button").gameObject,
                    "StateIcon");
                Button availableNodeButton = FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button");
                Button lockedNodeButton = FindButton(hostObject, BootstrapWorldScenario.CavernGateNodeId.Value + "_Button");
                Button currentNodeButton = FindButton(hostObject, BootstrapWorldScenario.ForestPushNodeId.Value + "_Button");
                Image currentNodeMarker = FindChildImage(currentNodeButton.gameObject, "StateMarkerRing");
                Image availableNodeMarker = FindChildImage(availableNodeButton.gameObject, "StateMarkerArc");
                Assert.That(backgroundImage.sprite, Is.Not.Null);
                Assert.That(currentNodeIcon.sprite, Is.Not.Null);
                Assert.That(ordinaryNodeIcon.sprite, Is.Not.Null);
                Assert.That(farmNodeIcon.sprite, Is.Not.Null);
                Assert.That(eliteNodeIcon.sprite, Is.Not.Null);
                Assert.That(serviceNodeIcon.sprite, Is.Not.Null);
                Assert.That(currentNodeIcon.rectTransform.rect.width, Is.InRange(64f, 88f));
                Assert.That(currentNodeIcon.rectTransform.rect.height, Is.InRange(64f, 88f));
                Assert.That(currentNodeIcon.sprite, Is.SameAs(ordinaryNodeIcon.sprite));
                Assert.That(farmNodeIcon.sprite, Is.Not.SameAs(ordinaryNodeIcon.sprite));
                Assert.That(eliteNodeIcon.sprite, Is.Not.SameAs(farmNodeIcon.sprite));
                Assert.That(serviceNodeIcon.sprite, Is.Not.SameAs(ordinaryNodeIcon.sprite));
                Assert.That(CountObjectsNamed(hostObject, "StateBacking"), Is.EqualTo(0));
                Assert.That(CountObjectsNamed(hostObject, "StateMarkerRing"), Is.EqualTo(1));
                Assert.That(CountObjectsNamed(hostObject, "StateMarkerArc"), Is.GreaterThanOrEqualTo(2));
                Assert.That(availableNodeMarker, Is.Not.Null);
                Assert.That(TryFindChildImage(lockedNodeButton.gameObject, "StateMarkerRing"), Is.Null);
                Assert.That(TryFindChildImage(lockedNodeButton.gameObject, "StateMarkerArc"), Is.Null);
                Assert.That(TryFindChildImage(availableNodeButton.gameObject, "StateMarkerRing"), Is.Null);
                Assert.That(currentNodeMarker, Is.Not.Null);
                Assert.That(TryFindChildImage(currentNodeButton.gameObject, "StateMarkerArc"), Is.Null);
                Assert.That(FindChildImage(lockedNodeButton.gameObject, "StateIcon").color.a, Is.LessThan(0.7f));
                Assert.That(currentNodeIcon.color, Is.EqualTo(Color.white));
                Assert.That(currentNodeMarker.color, Is.Not.EqualTo(availableNodeMarker.color));
                Assert.That(currentNodeMarker.rectTransform.rect.width, Is.GreaterThan(availableNodeMarker.rectTransform.rect.width));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldCenterAuthoredMapHitTargetOnVisibleNodeIcon()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                Button currentNodeButton = FindButton(hostObject, BootstrapWorldScenario.ForestPushNodeId.Value + "_Button");
                Button availableNodeButton = FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button");

                AssertCentersAligned(
                    currentNodeButton.GetComponent<RectTransform>(),
                    FindChildImage(currentNodeButton.gameObject, "StateIcon").rectTransform);
                AssertCentersAligned(
                    availableNodeButton.GetComponent<RectTransform>(),
                    FindChildImage(availableNodeButton.gameObject, "StateIcon").rectTransform);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequireReachableSelectionWithoutReturnToWorldReentryOffer()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState,
                    nodeEntryRequested: _ => { });

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Text entryButtonLabel = entryButton.GetComponentInChildren<Text>(true);

                Assert.That(entryButton.interactable, Is.False);
                Assert.That(entryButtonLabel, Is.Not.Null);
                Assert.That(entryButtonLabel.text, Is.EqualTo("Select a reachable node to enter"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldEnableEntryButtonAsQuickRepeatOnlyWhenReturnToWorldReentryOfferExists()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            SessionContextState sessionContext = new SessionContextState();

            sessionContext.OfferReturnToWorldReentry(new NodeId("region_001_node_002"));

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    sessionContext: sessionContext,
                    gameState: gameState,
                    nodeEntryRequested: _ => { });

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Text entryButtonLabel = entryButton.GetComponentInChildren<Text>(true);

                Assert.That(entryButton.interactable, Is.True);
                Assert.That(entryButtonLabel, Is.Not.Null);
                Assert.That(entryButtonLabel.text, Is.EqualTo("Replay Raider Trail"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldLimitAuthoredMapLabelsToSelectedNodesOnly()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                Assert.That(CountObjectsNamed(hostObject, "LabelPlate"), Is.EqualTo(0));
                Assert.That(CountObjectsNamed(hostObject, "StateMarkerRing") + CountObjectsNamed(hostObject, "StateMarkerArc"), Is.GreaterThanOrEqualTo(3));

                FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button").onClick.Invoke();

                Assert.That(
                    ContainsText(hostObject, "Current: Raider Trail (In progress) | Selected: Forest Farm"),
                    Is.True);
                Assert.That(CountObjectsNamed(hostObject, "LabelPlate"), Is.EqualTo(1));
                Assert.That(CountObjectsNamed(hostObject, "StateMarkerRing") + CountObjectsNamed(hostObject, "StateMarkerArc"), Is.GreaterThanOrEqualTo(3));
                Assert.That(
                    FindChildImage(FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button").gameObject, "StateMarkerRing").rectTransform.rect.width,
                    Is.GreaterThan(FindChildImage(FindButton(hostObject, BootstrapWorldScenario.ForestPushNodeId.Value + "_Button").gameObject, "StateMarkerRing").rectTransform.rect.width));
                Text selectedLabel = FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button")
                    .GetComponentInChildren<Text>(true);
                Assert.That(selectedLabel, Is.Not.Null);
                Assert.That(selectedLabel.raycastTarget, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldKeepEliteNodeReadableThroughSelectionWithoutAlwaysVisibleMapCaption()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                FindButton(hostObject, BootstrapWorldScenario.ForestEliteNodeId.Value + "_Button").onClick.Invoke();

                Assert.That(
                    ContainsText(hostObject, "Current: Raider Trail (In progress) | Selected: Raider Holdout"),
                    Is.True);
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
        public void Show_ShouldEnableReplayShortcutForFarmReadyCurrentCombatNodeWhenFarmReplayProjectIsPurchased()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = CreateFarmReadyReplayGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    gameState.WorldState,
                    gameState: gameState,
                    progressionEffects: ResolveProgressionEffects(gameState),
                    nodeEntryRequested: _ => { });

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Text entryButtonLabel = entryButton.GetComponentInChildren<Text>(true);

                Assert.That(entryButton.interactable, Is.True);
                Assert.That(entryButtonLabel, Is.Not.Null);
                Assert.That(entryButtonLabel.text, Is.EqualTo("Replay Forest Farm"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldUseDistinctMarkersForReachableAndReplayableNodes()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = CreateClearedFarmBranchGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    gameState.WorldState,
                    gameState: gameState);

                Image replayableNodeMarker = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button").gameObject,
                    "StateMarkerArc");
                Image reachableNodeMarker = FindChildImage(
                    FindButton(hostObject, BootstrapWorldScenario.ForestEliteNodeId.Value + "_Button").gameObject,
                    "StateMarkerArc");

                Assert.That(replayableNodeMarker, Is.Not.Null);
                Assert.That(reachableNodeMarker, Is.Not.Null);
                Assert.That(TryFindChildImage(FindButton(hostObject, BootstrapWorldScenario.ForestFarmNodeId.Value + "_Button").gameObject, "StateMarkerRing"), Is.Null);
                Assert.That(TryFindChildImage(FindButton(hostObject, BootstrapWorldScenario.ForestEliteNodeId.Value + "_Button").gameObject, "StateMarkerRing"), Is.Null);
                Assert.That(reachableNodeMarker.color, Is.Not.EqualTo(replayableNodeMarker.color));
                Assert.That(reachableNodeMarker.rectTransform.rect.width, Is.GreaterThan(replayableNodeMarker.rectTransform.rect.width));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldPreferSelectedNodeEntryOverFarmReplayShortcutWhenAnotherReachableNodeIsSelected()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = CreateFarmReadyReplayGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    gameState.WorldState,
                    gameState: gameState,
                    progressionEffects: ResolveProgressionEffects(gameState),
                    nodeEntryRequested: _ => { });

                FindButton(hostObject, "region_002_node_001_Button").onClick.Invoke();

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Text entryButtonLabel = entryButton.GetComponentInChildren<Text>(true);

                Assert.That(entryButton.interactable, Is.True);
                Assert.That(entryButtonLabel, Is.Not.Null);
                Assert.That(entryButtonLabel.text, Is.EqualTo("Enter Cavern Service Hub"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldCreateVisibleCanvasAndInputSystemModule()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            SessionContextState sessionContext = new SessionContextState();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    sessionContext: sessionContext,
                    gameState: gameState);

                Canvas canvas = hostObject.GetComponent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
                Assert.That(hostObject.GetComponent<GraphicRaycaster>(), Is.Not.Null);

                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
                Assert.That(eventSystem, Is.Not.Null);
                Assert.That(eventSystem.gameObject.GetComponent<InputSystemUIInputModule>(), Is.Not.Null);
                Assert.That(eventSystem.gameObject.GetComponent<StandaloneInputModule>(), Is.Null);

                Button[] buttons = hostObject.GetComponentsInChildren<Button>(true);
                Assert.That(buttons.Length, Is.GreaterThan(0));

                Text[] labels = hostObject.GetComponentsInChildren<Text>(true);
                bool containsForwardRouteSummary = false;
                bool containsReadableLocationSummary = false;
                bool containsBlockedLinkSummary = false;
                bool containsRouteSummary = false;
                bool containsCharacterSelectionSummary = false;
                bool containsAssignedPackageSummary = false;
                bool containsPrimaryGearSummary = false;
                bool containsSupportGearSummary = false;
                foreach (Text label in labels)
                {
                    if (label.text.Contains("Location: Verdant Frontier"))
                    {
                        containsReadableLocationSummary = true;
                    }

                    if (label.text.Contains("Current: Raider Trail (In progress) | Selected: none"))
                    {
                        containsForwardRouteSummary = true;
                    }

                    if (label.text.Contains("Routes: 4 enterable | 3 forward | 0 replayable"))
                    {
                        containsRouteSummary = true;
                    }

                    if (label.text.Contains("Selected character: Vanguard"))
                    {
                        containsCharacterSelectionSummary = true;
                    }

                    if (label.text.Contains("Assigned package: Standard Guard"))
                    {
                        containsAssignedPackageSummary = true;
                    }

                    if (label.text.Contains("Primary gear: none"))
                    {
                        containsPrimaryGearSummary = true;
                    }

                    if (label.text.Contains("Support gear: none"))
                    {
                        containsSupportGearSummary = true;
                    }
                }

                Assert.That(containsForwardRouteSummary, Is.True);
                Assert.That(containsReadableLocationSummary, Is.True);
                Assert.That(containsRouteSummary, Is.True);
                Assert.That(containsCharacterSelectionSummary, Is.True);
                Assert.That(containsAssignedPackageSummary, Is.True);
                Assert.That(containsPrimaryGearSummary, Is.True);
                Assert.That(containsSupportGearSummary, Is.True);
                Assert.That(FindImage(hostObject, "MapBackgroundArt").sprite, Is.Not.Null);
                Assert.That(
                    FindChildImage(FindButton(hostObject, BootstrapWorldScenario.ForestEntryNodeId.Value + "_Button").gameObject, "StateIcon").sprite,
                    Is.Not.Null);
                Assert.That(
                    FindChildImage(FindButton(hostObject, BootstrapWorldScenario.CavernServiceNodeId.Value + "_Button").gameObject, "StateIcon").sprite,
                    Is.Not.Null);
                Assert.That(FindButton(hostObject, "character_vanguard_CharacterButton"), Is.Not.Null);
                Assert.That(FindButton(hostObject, "character_striker_CharacterButton"), Is.Not.Null);
                Assert.That(
                    FindButton(hostObject, $"{PlayableCharacterSkillPackageIds.VanguardDefault}_SkillPackageButton"),
                    Is.Not.Null);
                Assert.That(
                    FindButton(hostObject, $"{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton"),
                    Is.Not.Null);
                Assert.That(
                    FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton"),
                    Is.Not.Null);
                Assert.That(
                    FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton"),
                    Is.Not.Null);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldInvokeNodeEntryCallbackWhenSelectedNodeIsConfirmed()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            bool wasInvoked = false;
            NodeId enteredNodeId = default;

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    nodeEntryRequested: nodeId =>
                    {
                        wasInvoked = true;
                        enteredNodeId = nodeId;
                    },
                    gameState: gameState);

                Button selectableNodeButton = FindButton(hostObject, "region_002_node_001_Button");
                selectableNodeButton.onClick.Invoke();

                Button enterSelectedNodeButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Assert.That(enterSelectedNodeButton.interactable, Is.True);

                enterSelectedNodeButton.onClick.Invoke();

                Assert.That(wasInvoked, Is.True);
                Assert.That(enteredNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestClickAndConfirmFeedbackForSelectionAndEntry()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    nodeEntryRequested: _ => { },
                    gameState: gameState,
                    feedbackSoundRequested: requestedSounds.Add);

                FindButton(hostObject, "region_002_node_001_Button").onClick.Invoke();
                FindButton(hostObject, "EnterSelectedNodeButton").onClick.Invoke();

                CollectionAssert.AreEqual(
                    new[]
                    {
                        UiSystemFeedbackSoundId.UiClick,
                        UiSystemFeedbackSoundId.UiConfirm,
                    },
                    requestedSounds);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldOpenCompactSystemMenuSettingsAndInvokeSafeExitWhenAvailable()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            bool exitRequested = false;
            UserSettingsState receivedSettingsState = null;

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState,
                    stopSessionRequested: () => exitRequested = true,
                    settingsChanged: settingsState => receivedSettingsState = settingsState);

                FindButton(hostObject, "SystemMenuButton").onClick.Invoke();

                Assert.That(ContainsText(hostObject, "System Menu"), Is.True);
                Assert.That(FindButton(hostObject, "SystemMenuExitButton").interactable, Is.True);

                FindButton(hostObject, "SystemMenuSettingsButton").onClick.Invoke();
                Assert.That(ContainsText(hostObject, "Master volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Music volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "SFX volume: 100%"), Is.True);
                Assert.That(ContainsText(hostObject, "Display mode: Windowed"), Is.True);
                FindButton(hostObject, "MasterVolumeDecreaseButton").onClick.Invoke();

                Assert.That(receivedSettingsState, Is.Not.Null);
                Assert.That(receivedSettingsState.MasterVolume, Is.EqualTo(0.9f).Within(0.001f));

                FindButton(hostObject, "SystemMenuSettingsBackButton").onClick.Invoke();
                FindButton(hostObject, "SystemMenuExitButton").onClick.Invoke();

                Assert.That(exitRequested, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldRequestErrorFeedbackForRejectedWorldMapActions()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            List<UiSystemFeedbackSoundId> requestedSounds = new List<UiSystemFeedbackSoundId>();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    nodeEntryRequested: _ => { },
                    gameState: gameState,
                    feedbackSoundRequested: requestedSounds.Add);

                FindButton(hostObject, "region_001_node_003_Button").onClick.Invoke();
                FindButton(hostObject, "EnterSelectedNodeButton").onClick.Invoke();

                CollectionAssert.AreEqual(
                    new[]
                    {
                        UiSystemFeedbackSoundId.UiError,
                        UiSystemFeedbackSoundId.UiError,
                    },
                    requestedSounds);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldInvokeNodeEntryCallbackForQuickRepeatWhenNothingIsSelected()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            SessionContextState sessionContext = new SessionContextState();
            bool wasInvoked = false;
            NodeId enteredNodeId = default;

            sessionContext.OfferReturnToWorldReentry(new NodeId("region_001_node_002"));

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    nodeEntryRequested: nodeId =>
                    {
                        wasInvoked = true;
                        enteredNodeId = nodeId;
                    },
                    sessionContext: sessionContext,
                    gameState: gameState);

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Assert.That(entryButton.interactable, Is.True);

                entryButton.onClick.Invoke();

                Assert.That(wasInvoked, Is.True);
                Assert.That(enteredNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
                Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldConsumeQuickRepeatOfferAfterItIsUsed()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            SessionContextState sessionContext = new SessionContextState();

            sessionContext.OfferReturnToWorldReentry(new NodeId("region_001_node_002"));

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    nodeEntryRequested: _ => { },
                    sessionContext: sessionContext,
                    gameState: gameState);

                FindButton(hostObject, "EnterSelectedNodeButton").onClick.Invoke();

                Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.False);

                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    nodeEntryRequested: _ => { },
                    sessionContext: sessionContext,
                    gameState: gameState);

                Button entryButton = FindButton(hostObject, "EnterSelectedNodeButton");
                Text entryButtonLabel = entryButton.GetComponentInChildren<Text>(true);

                Assert.That(entryButton.interactable, Is.False);
                Assert.That(entryButtonLabel.text, Is.EqualTo("Select a reachable node to enter"));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldKeepSummaryTextSeparatedFromEnterButton()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            SessionContextState sessionContext = new SessionContextState();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    sessionContext: sessionContext,
                    gameState: gameState);

                ForceUiLayout(hostObject);

                RectTransform summaryRect = FindRectTransform(hostObject, "Summary");
                RectTransform enterButtonRect = FindRectTransform(hostObject, "EnterSelectedNodeButton");
                RectTransform panelRect = FindRectTransform(hostObject, "Panel");

                Assert.That(RectanglesOverlap(summaryRect, enterButtonRect), Is.False);
                Assert.That(RectangleContains(panelRect, summaryRect), Is.True);
                Assert.That(RectangleContains(panelRect, enterButtonRect), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldCreateStaticViewportForAuthoredMapSurface()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                ForceUiLayout(hostObject);

                ScrollRect scrollRect = FindScrollRect(hostObject, "NodeListScrollView");
                RectTransform firstNodeRect = FindRectTransform(hostObject, "region_001_node_002_Button");
                RectTransform panelRect = FindRectTransform(hostObject, "Panel");
                Assert.That(scrollRect.horizontal, Is.False);
                Assert.That(scrollRect.vertical, Is.False);
                Assert.That(scrollRect.viewport, Is.Not.Null);
                Assert.That(scrollRect.viewport.gameObject.name, Is.EqualTo("NodeListViewport"));
                Assert.That(scrollRect.content, Is.Not.Null);
                Assert.That(scrollRect.content.gameObject.name, Is.EqualTo("NodeList"));
                Assert.That(scrollRect.viewport.rect.height, Is.GreaterThanOrEqualTo(firstNodeRect.rect.height - 1f));
                Assert.That(RectanglesOverlap(FindRectTransform(hostObject, "EnterSelectedNodeButton"), scrollRect.viewport), Is.False);
                Assert.That(RectangleContains(panelRect, scrollRect.viewport), Is.True);
                Assert.That(GetWorldRect(scrollRect.content).xMin, Is.EqualTo(GetWorldRect(scrollRect.viewport).xMin).Within(1f));
                Assert.That(GetWorldRect(scrollRect.content).xMax, Is.EqualTo(GetWorldRect(scrollRect.viewport).xMax).Within(1f));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldKeepLowerNodeButtonsReachableWhenNodeListOverflowsViewport()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            const int nodeCount = 18;

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    CreateOverflowWorldGraph(nodeCount),
                    CreateOverflowWorldState(nodeCount),
                    gameState: gameState);

                ForceUiLayout(hostObject);

                ScrollRect scrollRect = FindScrollRect(hostObject, "NodeListScrollView");
                RectTransform lastNodeRect = FindRectTransform(hostObject, "region_scroll_node_018_Button");
                RectTransform lastNodeLabelRect = GetButtonLabelRectTransform(FindButton(hostObject, "region_scroll_node_018_Button"));

                Assert.That(scrollRect.content.rect.height, Is.GreaterThan(scrollRect.viewport.rect.height));
                Assert.That(RectanglesOverlap(lastNodeRect, scrollRect.viewport), Is.False);

                scrollRect.verticalNormalizedPosition = 0f;
                ForceUiLayout(hostObject);

                Assert.That(RectanglesOverlap(lastNodeRect, scrollRect.viewport), Is.True);
                AssertHorizontallyContained(scrollRect.viewport, lastNodeRect);
                AssertHorizontallyContained(scrollRect.viewport, lastNodeLabelRect);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldKeepOverflowingNodeContentReachableAfterCharacterBuildRefresh()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            const int nodeCount = 18;

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    CreateOverflowWorldGraph(nodeCount),
                    CreateOverflowWorldState(nodeCount),
                    gameState: gameState);

                FindButton(hostObject, "character_striker_CharacterButton").onClick.Invoke();
                ForceUiLayout(hostObject);
                FindButton(hostObject, "character_vanguard_CharacterButton").onClick.Invoke();
                ForceUiLayout(hostObject);
                FindButton(hostObject, $"{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton").onClick.Invoke();
                ForceUiLayout(hostObject);
                FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                ForceUiLayout(hostObject);
                FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();
                ForceUiLayout(hostObject);

                ScrollRect scrollRect = FindScrollRect(hostObject, "NodeListScrollView");
                RectTransform lastNodeRect = FindRectTransform(hostObject, "region_scroll_node_018_Button");
                RectTransform lastNodeLabelRect = GetButtonLabelRectTransform(FindButton(hostObject, "region_scroll_node_018_Button"));

                Assert.That(scrollRect.content.rect.height, Is.GreaterThan(scrollRect.viewport.rect.height));
                Assert.That(scrollRect.viewport.rect.height, Is.GreaterThanOrEqualTo(lastNodeRect.rect.height - 1f));

                scrollRect.verticalNormalizedPosition = 0f;
                ForceUiLayout(hostObject);

                Assert.That(RectanglesOverlap(lastNodeRect, scrollRect.viewport), Is.True);
                AssertNodeButtonReadableWithinViewport(scrollRect.viewport, FindButton(hostObject, "region_scroll_node_018_Button"));
                AssertHorizontallyContained(scrollRect.viewport, lastNodeLabelRect);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldKeepNodeButtonsHorizontallyAlignedWithinViewportAfterRefresh()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                ForceUiLayout(hostObject);

                ScrollRect scrollRect = FindScrollRect(hostObject, "NodeListScrollView");
                Button nodeButton = FindButton(hostObject, "region_002_node_001_Button");
                AssertHorizontallyContained(scrollRect.viewport, nodeButton.GetComponent<RectTransform>());

                nodeButton.onClick.Invoke();
                ForceUiLayout(hostObject);

                scrollRect = FindScrollRect(hostObject, "NodeListScrollView");
                nodeButton = FindButton(hostObject, "region_002_node_001_Button");
                Assert.That(GetWorldRect(scrollRect.content).xMin, Is.EqualTo(GetWorldRect(scrollRect.viewport).xMin).Within(1f));
                Assert.That(GetWorldRect(scrollRect.content).xMax, Is.EqualTo(GetWorldRect(scrollRect.viewport).xMax).Within(1f));
                AssertNodeButtonReadableWithinViewport(scrollRect.viewport, nodeButton);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldSwitchSelectedPlayableCharacterWhenCharacterButtonIsPressed()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                FindButton(hostObject, "character_striker_CharacterButton").onClick.Invoke();

                Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
                Assert.That(vanguardState.IsActive, Is.False);
                Assert.That(gameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState), Is.True);
                Assert.That(strikerState.IsActive, Is.True);
                Assert.That(ContainsText(hostObject, "Selected character: Striker"), Is.True);
                Assert.That(ContainsText(hostObject, "Selected: Striker"), Is.True);
                Assert.That(ContainsText(hostObject, "Select: Vanguard"), Is.True);
                Assert.That(ContainsText(hostObject, "Assigned package: Relentless Burst"), Is.True);
                Assert.That(ContainsText(hostObject, "Primary gear: none | Support gear: none"), Is.True);
                Assert.That(
                    FindButton(hostObject, $"{PlayableCharacterSkillPackageIds.StrikerDefault}_SkillPackageButton"),
                    Is.Not.Null);
                Assert.That(FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton"), Is.Not.Null);
                Assert.That(FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton"), Is.Not.Null);
                Assert.That(
                    TryFindButton(hostObject, $"{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton"),
                    Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldAssignSkillPackageForCurrentlySelectedPlayableCharacterWhenPackageButtonIsPressed()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                FindButton(hostObject, $"{PlayableCharacterSkillPackageIds.VanguardBurstDrill}_SkillPackageButton").onClick.Invoke();

                Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
                Assert.That(vanguardState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
                Assert.That(ContainsText(hostObject, "Assigned package: Burst Drill"), Is.True);
                Assert.That(ContainsText(hostObject, "Assigned: Burst Drill"), Is.True);
                Assert.That(ContainsText(hostObject, "Assign: Standard Guard"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldEquipAndUnequipBothGearCategoriesForCurrentlySelectedPlayableCharacterWhenGearButtonIsPressed()
        {
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    gameState: gameState);

                FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();

                Assert.That(gameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState vanguardState), Is.True);
                Assert.That(
                    vanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.PrimaryCombat,
                        out EquippedGearState equippedGearState),
                    Is.True);
                Assert.That(equippedGearState.GearId, Is.EqualTo(GearIds.TrainingBlade));
                Assert.That(
                    vanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.SecondarySupport,
                        out EquippedGearState supportGearState),
                    Is.True);
                Assert.That(supportGearState.GearId, Is.EqualTo(GearIds.GuardCharm));
                Assert.That(
                    ContainsText(hostObject, "Primary gear: Training Blade | Support gear: Guard Charm"),
                    Is.True);
                Assert.That(ContainsText(hostObject, "Unequip: Training Blade"), Is.True);
                Assert.That(ContainsText(hostObject, "Unequip: Guard Charm"), Is.True);

                FindButton(hostObject, $"{GearIds.GuardCharm}_GearButton").onClick.Invoke();
                FindButton(hostObject, $"{GearIds.TrainingBlade}_GearButton").onClick.Invoke();

                Assert.That(
                    vanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.PrimaryCombat,
                    out EquippedGearState _),
                    Is.False);
                Assert.That(
                    vanguardState.LoadoutState.TryGetEquippedGearState(
                        GearCategory.SecondarySupport,
                        out EquippedGearState _),
                    Is.False);
                Assert.That(ContainsText(hostObject, "Primary gear: none | Support gear: none"), Is.True);
                Assert.That(ContainsText(hostObject, "Equip: Training Blade"), Is.True);
                Assert.That(ContainsText(hostObject, "Equip: Guard Charm"), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static ScrollRect FindScrollRect(GameObject rootObject, string objectName)
        {
            ScrollRect[] scrollRects = rootObject.GetComponentsInChildren<ScrollRect>(true);
            foreach (ScrollRect scrollRect in scrollRects)
            {
                if (scrollRect.gameObject.name == objectName)
                {
                    return scrollRect;
                }
            }

            Assert.Fail($"ScrollRect '{objectName}' was not found.");
            return null;
        }

        private static RectTransform GetButtonLabelRectTransform(Button button)
        {
            Text label = button.GetComponentInChildren<Text>(true);
            Assert.That(label, Is.Not.Null);
            return label.rectTransform;
        }

        private static Button FindButton(GameObject rootObject, string buttonObjectName)
        {
            Button button = TryFindButton(rootObject, buttonObjectName);
            if (button != null)
            {
                return button;
            }

            Assert.Fail($"Button '{buttonObjectName}' was not found.");
            return null;
        }

        private static Image FindImage(GameObject rootObject, string objectName)
        {
            Image image = TryFindImage(rootObject, objectName);
            if (image != null)
            {
                return image;
            }

            Assert.Fail($"Image '{objectName}' was not found.");
            return null;
        }

        private static Image FindChildImage(GameObject rootObject, string objectName)
        {
            Image image = TryFindChildImage(rootObject, objectName);
            if (image != null)
            {
                return image;
            }

            Assert.Fail($"Child image '{objectName}' was not found.");
            return null;
        }

        private static Image TryFindChildImage(GameObject rootObject, string objectName)
        {
            Image[] images = rootObject.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.gameObject.name == objectName)
                {
                    return image;
                }
            }

            return null;
        }

        private static Image TryFindImage(GameObject rootObject, string objectName)
        {
            Image[] images = rootObject.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.gameObject.name == objectName)
                {
                    return image;
                }
            }

            return null;
        }

        private static Button TryFindButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName)
                {
                    return button;
                }
            }

            return null;
        }

        private static bool ContainsText(GameObject rootObject, string textFragment)
        {
            Text[] labels = rootObject.GetComponentsInChildren<Text>(true);
            foreach (Text label in labels)
            {
                if (label.text.Contains(textFragment))
                {
                    return true;
                }
            }

            return false;
        }

        private static RectTransform FindRectTransform(GameObject rootObject, string objectName)
        {
            RectTransform[] rectTransforms = rootObject.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rectTransform in rectTransforms)
            {
                if (rectTransform.gameObject.name == objectName)
                {
                    return rectTransform;
                }
            }

            Assert.Fail($"RectTransform '{objectName}' was not found.");
            return null;
        }

        private static int CountObjectsNamed(GameObject rootObject, string objectName)
        {
            Transform[] transforms = rootObject.GetComponentsInChildren<Transform>(true);
            int count = 0;
            foreach (Transform childTransform in transforms)
            {
                if (childTransform.gameObject.name == objectName && childTransform.gameObject.activeInHierarchy)
                {
                    count++;
                }
            }

            return count;
        }

        private static void ForceUiLayout(GameObject rootObject)
        {
            Canvas.ForceUpdateCanvases();

            RectTransform[] rectTransforms = rootObject.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rectTransform in rectTransforms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            Canvas.ForceUpdateCanvases();
        }

        private static bool RectanglesOverlap(RectTransform first, RectTransform second)
        {
            Rect firstRect = GetWorldRect(first);
            Rect secondRect = GetWorldRect(second);
            return firstRect.Overlaps(secondRect, true);
        }

        private static Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            Vector2 min = worldCorners[0];
            Vector2 max = worldCorners[2];
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private static bool RectangleContains(RectTransform container, RectTransform child)
        {
            Rect containerRect = GetWorldRect(container);
            Rect childRect = GetWorldRect(child);
            return containerRect.xMin <= childRect.xMin &&
                containerRect.xMax >= childRect.xMax &&
                containerRect.yMin <= childRect.yMin &&
                containerRect.yMax >= childRect.yMax;
        }

        private static void AssertNodeButtonReadableWithinViewport(RectTransform viewport, Button button)
        {
            AssertHorizontallyContained(viewport, button.GetComponent<RectTransform>());
            AssertHorizontallyContained(viewport, GetButtonLabelRectTransform(button));
        }

        private static void AssertHorizontallyContained(RectTransform viewport, RectTransform child)
        {
            Rect viewportRect = GetWorldRect(viewport);
            Rect childRect = GetWorldRect(child);

            Assert.That(childRect.xMin, Is.GreaterThanOrEqualTo(viewportRect.xMin - 1f));
            Assert.That(childRect.xMax, Is.LessThanOrEqualTo(viewportRect.xMax + 1f));
        }

        private static void AssertCentersAligned(RectTransform first, RectTransform second)
        {
            Vector2 firstCenter = GetWorldRect(first).center;
            Vector2 secondCenter = GetWorldRect(second).center;

            Assert.That(Vector2.Distance(firstCenter, secondCenter), Is.LessThanOrEqualTo(1f));
        }

        private static PersistentGameState CreateFarmReadyReplayGameState()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            AccountWideProgressionBoardService progressionBoardService = new AccountWideProgressionBoardService();
            NodeId farmNodeId = new NodeId("region_001_node_004");
            PersistentNodeState farmNodeState = gameState.WorldState.GetOrAddNodeState(
                farmNodeId,
                unlockThreshold: 3,
                initialState: NodeState.Available);

            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 3);
            Assert.That(
                progressionBoardService.TryPurchase(gameState, AccountWideUpgradeId.FarmReplayProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            farmNodeState.ApplyUnlockProgress(3);
            gameState.WorldState.SetCurrentNode(farmNodeId);
            gameState.WorldState.SetLastSafeNode(farmNodeId);
            gameState.WorldState.ReplaceReachableNodes(new[]
            {
                new NodeId("region_002_node_001"),
            });
            return gameState;
        }

        private static PersistentGameState CreateClearedFarmBranchGameState()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            NodeId farmNodeId = BootstrapWorldScenario.ForestFarmNodeId;
            PersistentNodeState farmNodeState = gameState.WorldState.GetOrAddNodeState(
                farmNodeId,
                unlockThreshold: 3,
                initialState: NodeState.Available);

            farmNodeState.ApplyUnlockProgress(3);
            return gameState;
        }

        private static AccountWideProgressionEffectState ResolveProgressionEffects(PersistentGameState gameState)
        {
            return new AccountWideProgressionEffectResolver().Resolve(gameState.ProgressionState);
        }

        private static WorldGraph CreateOverflowWorldGraph(int nodeCount)
        {
            RegionId regionId = new RegionId("region_scroll");
            List<NodeId> nodeIds = new List<NodeId>(nodeCount);
            List<WorldNode> nodes = new List<WorldNode>(nodeCount);
            List<WorldNodeConnection> connections = new List<WorldNodeConnection>(nodeCount - 1);

            for (int index = 1; index <= nodeCount; index++)
            {
                NodeId nodeId = new NodeId($"region_scroll_node_{index:D3}");
                nodeIds.Add(nodeId);
                nodes.Add(new WorldNode(nodeId, regionId, NodeType.Combat, NodeState.Available));

                if (index > 1)
                {
                    connections.Add(new WorldNodeConnection(nodeIds[index - 2], nodeId));
                }
            }

            WorldRegion region = new WorldRegion(
                regionId,
                progressionOrder: 0,
                entryNodeId: nodeIds[0],
                nodeIds: nodeIds,
                resourceCategory: ResourceCategory.SoftCurrency,
                difficultyBand: "Test");

            return new WorldGraph(new[] { region }, nodes, connections);
        }

        private static PersistentWorldState CreateOverflowWorldState(int nodeCount)
        {
            PersistentWorldState worldState = new PersistentWorldState();
            NodeId currentNodeId = new NodeId("region_scroll_node_001");

            worldState.SetCurrentNode(currentNodeId);
            worldState.SetLastSafeNode(currentNodeId);

            List<NodeId> reachableNodeIds = new List<NodeId>(nodeCount - 1);
            for (int index = 2; index <= nodeCount; index++)
            {
                reachableNodeIds.Add(new NodeId($"region_scroll_node_{index:D3}"));
            }

            worldState.ReplaceReachableNodes(reachableNodeIds);
            return worldState;
        }
    }
}

