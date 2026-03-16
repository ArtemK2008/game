using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode
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

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    sessionContext: sessionContext);

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
                bool containsRecentNodeSummary = false;
                foreach (Text label in labels)
                {
                    if (label.text.Contains("State:"))
                    {
                        containsStateLabel = true;
                    }

                    if (label.text.Contains("Forward route options: 2"))
                    {
                        containsForwardRouteSummary = true;
                    }

                    if (label.text.Contains("Recent node: region_001_node_002"))
                    {
                        containsRecentNodeSummary = true;
                    }
                }

                Assert.That(containsStateLabel, Is.True);
                Assert.That(containsForwardRouteSummary, Is.True);
                Assert.That(containsRecentNodeSummary, Is.True);
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
                    });

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

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    BootstrapWorldTestData.CreateWorldState(),
                    sessionContext: sessionContext);

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

        private static Button FindButton(GameObject rootObject, string buttonObjectName)
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name == buttonObjectName)
                {
                    return button;
                }
            }

            Assert.Fail($"Button '{buttonObjectName}' was not found.");
            return null;
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
    }
}
