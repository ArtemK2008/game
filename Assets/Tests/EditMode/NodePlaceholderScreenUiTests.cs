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
            RunResult completedRunResult = null;

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();
                NodePlaceholderState placeholderState = new NodePlaceholderState(
                    new NodeId("region_002_node_001"),
                    new RegionId("region_002"),
                    NodeType.ServiceOrProgression,
                    NodeState.Available,
                    new NodeId("region_001_node_002"));

                placeholderScreen.Show(placeholderState, runResult => completedRunResult = runResult);

                Canvas canvas = hostObject.GetComponent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));

                Text[] labels = hostObject.GetComponentsInChildren<Text>(true);
                bool containsEnteredFromLabel = false;
                bool containsRunStartState = false;
                foreach (Text label in labels)
                {
                    if (label.text.Contains("Entered from: region_001_node_002"))
                    {
                        containsEnteredFromLabel = true;
                    }

                    if (label.text.Contains("Lifecycle: RunStart"))
                    {
                        containsRunStartState = true;
                    }
                }

                Assert.That(containsEnteredFromLabel, Is.True);
                Assert.That(containsRunStartState, Is.True);

                Button advanceRunLifecycleButton = FindButton(hostObject, "AdvanceRunLifecycleButton");
                Button returnButton = FindButton(hostObject, "ReturnToWorldMapButton");

                Assert.That(returnButton.interactable, Is.False);

                advanceRunLifecycleButton.onClick.Invoke();
                advanceRunLifecycleButton.onClick.Invoke();
                advanceRunLifecycleButton.onClick.Invoke();

                Assert.That(returnButton.interactable, Is.True);

                returnButton.onClick.Invoke();

                Assert.That(completedRunResult, Is.Not.Null);
                Assert.That(completedRunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
                Assert.That(completedRunResult.NextActionContext.CanStopSession, Is.True);
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
