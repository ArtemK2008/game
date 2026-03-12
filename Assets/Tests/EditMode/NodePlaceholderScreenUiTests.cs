using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Survivalon.Tests.EditMode
{
    public sealed class NodePlaceholderScreenUiTests
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
        public void Show_ShouldCreatePlaceholderNodeUiAndInvokeReturnCallback()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            bool wasInvoked = false;

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();
                NodePlaceholderState placeholderState = new NodePlaceholderState(
                    new NodeId("region_002_node_001"),
                    new RegionId("region_002"),
                    NodeType.ServiceOrProgression,
                    NodeState.Available,
                    new NodeId("region_001_node_002"));

                placeholderScreen.Show(placeholderState, () => wasInvoked = true);

                Canvas canvas = hostObject.GetComponent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));

                Text[] labels = hostObject.GetComponentsInChildren<Text>(true);
                bool containsEnteredFromLabel = false;
                foreach (Text label in labels)
                {
                    if (label.text.Contains("Entered from: region_001_node_002"))
                    {
                        containsEnteredFromLabel = true;
                        break;
                    }
                }

                Assert.That(containsEnteredFromLabel, Is.True);

                Button returnButton = FindButton(hostObject, "ReturnToWorldMapButton");
                returnButton.onClick.Invoke();

                Assert.That(wasInvoked, Is.True);
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
