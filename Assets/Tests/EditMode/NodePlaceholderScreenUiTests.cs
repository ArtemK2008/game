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
        public void Show_ShouldDisplayPostRunSummaryAndAllowReplay()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            bool returnRequested = false;
            bool stopRequested = false;

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();
                NodePlaceholderState placeholderState = CreatePlaceholderState();

                placeholderScreen.Show(
                    placeholderState,
                    runResult => returnRequested = runResult != null,
                    runResult => stopRequested = runResult != null);

                Canvas canvas = hostObject.GetComponent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.renderMode, Is.EqualTo(RenderMode.ScreenSpaceOverlay));
                Assert.That(ContainsText(hostObject, "Entered from: region_001_node_002"), Is.True);
                Assert.That(ContainsText(hostObject, "Lifecycle: RunStart"), Is.True);

                Button replayButton = FindButton(hostObject, "ReplayNodeButton");
                Button returnButton = FindButton(hostObject, "ReturnToWorldMapButton");
                Button stopButton = FindButton(hostObject, "StopSessionButton");

                Assert.That(replayButton.interactable, Is.False);
                Assert.That(returnButton.interactable, Is.False);
                Assert.That(stopButton.interactable, Is.False);

                AdvanceToPostRun(hostObject);

                Assert.That(replayButton.interactable, Is.True);
                Assert.That(returnButton.interactable, Is.True);
                Assert.That(stopButton.interactable, Is.True);
                Assert.That(ContainsText(hostObject, "Run finished."), Is.True);

                replayButton.onClick.Invoke();

                Assert.That(ContainsText(hostObject, "Lifecycle: RunStart"), Is.True);
                Assert.That(replayButton.interactable, Is.False);
                Assert.That(returnRequested, Is.False);
                Assert.That(stopRequested, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        [Test]
        public void Show_ShouldInvokeReturnAndStopCallbacksFromPostRunSummary()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");
            RunResult returnedRunResult = null;
            RunResult stoppedRunResult = null;

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreatePlaceholderState(),
                    runResult => returnedRunResult = runResult,
                    runResult => stoppedRunResult = runResult);

                AdvanceToPostRun(hostObject);
                FindButton(hostObject, "ReturnToWorldMapButton").onClick.Invoke();

                Assert.That(returnedRunResult, Is.Not.Null);
                Assert.That(returnedRunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
                Assert.That(returnedRunResult.NextActionContext.CanStopSession, Is.True);
                Assert.That(stoppedRunResult, Is.Null);

                FindButton(hostObject, "StopSessionButton").onClick.Invoke();

                Assert.That(stoppedRunResult, Is.Not.Null);
                Assert.That(stoppedRunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }

        private static NodePlaceholderState CreatePlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        private static void AdvanceToPostRun(GameObject rootObject)
        {
            Button advanceRunLifecycleButton = FindButton(rootObject, "AdvanceRunLifecycleButton");
            advanceRunLifecycleButton.onClick.Invoke();
            advanceRunLifecycleButton.onClick.Invoke();
            advanceRunLifecycleButton.onClick.Invoke();
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
