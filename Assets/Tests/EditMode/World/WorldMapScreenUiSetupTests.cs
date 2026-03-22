using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
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
                bool containsStateLabel = false;
                bool containsForwardRouteSummary = false;
                bool containsReadableLocationSummary = false;
                bool containsBlockedLinkSummary = false;
                bool containsStateLegend = false;
                bool containsRecentNodeSummary = false;
                bool containsCharacterSelectionSummary = false;
                bool containsAssignedPackageSummary = false;
                bool containsPrimaryGearSummary = false;
                bool containsSupportGearSummary = false;
                bool containsLocationIdentityLabel = false;
                bool containsPathRoleLabel = false;
                foreach (Text label in labels)
                {
                    if (label.text.Contains("State:"))
                    {
                        containsStateLabel = true;
                    }

                    if (label.text.Contains("Forward routes: region_001_node_004, region_002_node_001"))
                    {
                        containsForwardRouteSummary = true;
                    }

                    if (label.text.Contains("Location: Verdant Frontier | Region: region_001"))
                    {
                        containsReadableLocationSummary = true;
                    }

                    if (label.text.Contains("Blocked links: region_001_node_003"))
                    {
                        containsBlockedLinkSummary = true;
                    }

                    if (label.text.Contains("State legend: Available = enterable"))
                    {
                        containsStateLegend = true;
                    }

                    if (label.text.Contains("Recent: region_001_node_002"))
                    {
                        containsRecentNodeSummary = true;
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

                    if (label.text.Contains("Echo Caverns / region_002_node_001"))
                    {
                        containsLocationIdentityLabel = true;
                    }

                    if (label.text.Contains("Path: Forward route"))
                    {
                        containsPathRoleLabel = true;
                    }
                }

                Assert.That(containsStateLabel, Is.True);
                Assert.That(containsForwardRouteSummary, Is.True);
                Assert.That(containsReadableLocationSummary, Is.True);
                Assert.That(containsBlockedLinkSummary, Is.True);
                Assert.That(containsStateLegend, Is.True);
                Assert.That(containsRecentNodeSummary, Is.True);
                Assert.That(containsCharacterSelectionSummary, Is.True);
                Assert.That(containsAssignedPackageSummary, Is.True);
                Assert.That(containsPrimaryGearSummary, Is.True);
                Assert.That(containsSupportGearSummary, Is.True);
                Assert.That(containsLocationIdentityLabel, Is.True);
                Assert.That(containsPathRoleLabel, Is.True);
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
        public void Show_ShouldCreateScrollableNodeListViewport()
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
                Assert.That(scrollRect.vertical, Is.True);
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
                AssertNodeButtonReadableWithinViewport(scrollRect.viewport, nodeButton);

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

