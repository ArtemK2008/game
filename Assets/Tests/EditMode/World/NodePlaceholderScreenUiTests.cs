using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Run;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class NodePlaceholderScreenUiTests : NodePlaceholderScreenUiTestBase
    {
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
                    CreateWorldGraph(),
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
                Assert.That(ContainsText(hostObject, "Run complete."), Is.True);
                Assert.That(ContainsText(hostObject, "Best next step:"), Is.True);
                Assert.That(ContainsText(hostObject, "End session: Safe to exit after this resolved run."), Is.True);

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
                    CreateWorldGraph(),
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

        [Test]
        public void Show_ShouldKeepPostRunSummaryTextSeparatedFromActionButtons()
        {
            GameObject hostObject = new GameObject("NodePlaceholderHost");

            try
            {
                NodePlaceholderScreen placeholderScreen = hostObject.AddComponent<NodePlaceholderScreen>();

                placeholderScreen.Show(
                    CreateWorldGraph(),
                    CreatePlaceholderState(),
                    runResult => { },
                    runResult => { });

                AdvanceToPostRun(hostObject);
                ForceUiLayout(hostObject);

                RectTransform postRunSummaryTextRect = FindRectTransform(hostObject, "PostRunSummary");
                RectTransform postRunNextActionRect = FindRectTransform(hostObject, "PostRunNextActionSummary");
                RectTransform replayButtonRect = FindRectTransform(hostObject, "ReplayNodeButton");
                RectTransform returnButtonRect = FindRectTransform(hostObject, "ReturnToWorldMapButton");
                RectTransform stopButtonRect = FindRectTransform(hostObject, "StopSessionButton");
                RectTransform advanceButtonRect = FindRectTransform(hostObject, "AdvanceRunLifecycleButton");
                RectTransform postRunPanelRect = FindRectTransform(hostObject, "PostRunSummaryPanel");
                RectTransform mainPanelRect = FindRectTransform(hostObject, "Panel");

                Assert.That(RectanglesOverlap(postRunSummaryTextRect, replayButtonRect), Is.False);
                Assert.That(RectanglesOverlap(postRunSummaryTextRect, returnButtonRect), Is.False);
                Assert.That(RectanglesOverlap(postRunSummaryTextRect, stopButtonRect), Is.False);
                Assert.That(RectanglesOverlap(postRunNextActionRect, replayButtonRect), Is.False);
                Assert.That(RectanglesOverlap(postRunNextActionRect, returnButtonRect), Is.False);
                Assert.That(RectanglesOverlap(postRunNextActionRect, stopButtonRect), Is.False);
                Assert.That(RectanglesOverlap(advanceButtonRect, postRunPanelRect), Is.False);
                Assert.That(RectangleContains(mainPanelRect, advanceButtonRect), Is.True);
                Assert.That(RectangleContains(mainPanelRect, postRunPanelRect), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(hostObject);
            }
        }
    }
}

