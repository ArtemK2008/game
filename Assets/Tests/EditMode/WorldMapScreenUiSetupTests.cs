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
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            SessionContextState sessionContext = new SessionContextState();

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    factory.CreateWorldGraph(),
                    factory.CreateGameState().WorldState,
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
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            GameObject hostObject = new GameObject("WorldMapScreenHost");
            bool wasInvoked = false;
            NodeId enteredNodeId = default;

            try
            {
                WorldMapScreen worldMapScreen = hostObject.AddComponent<WorldMapScreen>();
                worldMapScreen.Show(
                    factory.CreateWorldGraph(),
                    factory.CreateGameState().WorldState,
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
    }
}
