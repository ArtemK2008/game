using System;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Runtime
{
    public sealed class NodePlaceholderScreen : MonoBehaviour
    {
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private Text statusText;
        private CombatShellView combatShellView;
        private GameObject postRunSummaryPanelObject;
        private Text postRunSummaryText;
        private Button advanceRunLifecycleButton;
        private Text advanceRunLifecycleButtonText;
        private Button replayButton;
        private Text replayButtonText;
        private Button returnToWorldButton;
        private Text returnToWorldButtonText;
        private Button stopSessionButton;
        private Text stopSessionButtonText;
        private Font uiFont;
        private PersistentWorldState persistentWorldState;
        private RunLifecycleController runLifecycleController;
        private PostRunStateController postRunStateController;
        private Action<RunResult> onReturnToWorldRequested;
        private Action<RunResult> onStopSessionRequested;

        public void Show(
            NodePlaceholderState placeholderState,
            Action<RunResult> returnToWorldRequested,
            Action<RunResult> stopSessionRequested = null,
            PersistentWorldState persistentWorldState = null)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            this.persistentWorldState = persistentWorldState;
            runLifecycleController = CreateRunLifecycleController(placeholderState);
            postRunStateController = null;
            onReturnToWorldRequested = returnToWorldRequested ?? throw new ArgumentNullException(nameof(returnToWorldRequested));
            onStopSessionRequested = stopSessionRequested;
            gameObject.name = "NodePlaceholderScreen";

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            runLifecycleController.TryStartAutomaticFlow();
            Refresh();
        }

        private void Refresh()
        {
            SyncPostRunStateController();
            NodePlaceholderState placeholderState = runLifecycleController.NodeContext;
            titleText.text = $"Run Shell: {placeholderState.NodeId.Value}";
            summaryText.text = BuildSummaryText();
            statusText.text = BuildStatusText();
            RefreshCombatShellView();
            RefreshAdvanceButton();
            RefreshPostRunSummaryPanel();
        }

        private void HandleAdvanceRunLifecycleRequested()
        {
            switch (runLifecycleController.CurrentState)
            {
                case RunLifecycleState.RunStart:
                    runLifecycleController.TryEnterActiveState();
                    break;
                case RunLifecycleState.RunActive:
                    if (runLifecycleController.HasCombatEncounterState)
                    {
                        return;
                    }
                    else
                    {
                        runLifecycleController.TryResolveRun(RunResolutionState.Succeeded);
                    }
                    break;
                case RunLifecycleState.RunResolved:
                    runLifecycleController.TryEnterPostRunState();
                    break;
                case RunLifecycleState.PostRun:
                    return;
                default:
                    throw new InvalidOperationException($"Unknown run lifecycle state '{runLifecycleController.CurrentState}'.");
            }

            Refresh();
        }

        private void HandleReplayRequested()
        {
            if (postRunStateController == null || !postRunStateController.CanReplayNode)
            {
                return;
            }

            NodePlaceholderState replayNodeContext = postRunStateController.NodeContext;
            postRunStateController = null;
            runLifecycleController = CreateRunLifecycleController(replayNodeContext);
            runLifecycleController.TryStartAutomaticFlow();
            Refresh();
        }

        private void HandleReturnToWorldRequested()
        {
            if (postRunStateController == null || !postRunStateController.CanReturnToWorld)
            {
                return;
            }

            onReturnToWorldRequested?.Invoke(postRunStateController.RunResult);
        }

        private void HandleStopSessionRequested()
        {
            if (postRunStateController == null || !postRunStateController.CanStopSession)
            {
                return;
            }

            onStopSessionRequested?.Invoke(postRunStateController.RunResult);
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(NodePlaceholderScreen));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 110;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280f, 720f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.anchorMax = Vector2.one;
            rootRectTransform.offsetMin = Vector2.zero;
            rootRectTransform.offsetMax = Vector2.zero;
            rootRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rootRectTransform.localScale = Vector3.one;

            GameObject panelObject = new GameObject(
                "Panel",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup));
            panelObject.transform.SetParent(transform, false);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.09f, 0.09f, 0.12f, 0.96f);

            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.12f, 0.01f);
            panelRectTransform.anchorMax = new Vector2(0.88f, 0.99f);
            panelRectTransform.offsetMin = Vector2.zero;
            panelRectTransform.offsetMax = Vector2.zero;
            panelRectTransform.localScale = Vector3.one;

            VerticalLayoutGroup panelLayout = panelObject.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(24, 24, 24, 24);
            panelLayout.spacing = 12f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            titleText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Title",
                30,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(titleText.gameObject, 44f);

            summaryText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Summary",
                20,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 148f);

            statusText = RuntimeUiSupport.CreateText(
                panelObject.transform,
                uiFont,
                "Status",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.78f, 0.82f, 0.90f, 1f));
            RuntimeUiSupport.AddLayoutElement(statusText.gameObject, 80f);

            GameObject combatShellViewObject = new GameObject("CombatShellView");
            combatShellViewObject.transform.SetParent(panelObject.transform, false);
            RuntimeUiSupport.AddLayoutElement(combatShellViewObject, CombatShellView.PreferredHeight);
            combatShellView = combatShellViewObject.AddComponent<CombatShellView>();
            combatShellView.Hide();

            advanceRunLifecycleButton = CreateActionButton(
                panelObject.transform,
                "AdvanceRunLifecycleButton",
                "Start Placeholder Run",
                out advanceRunLifecycleButtonText);
            RuntimeUiSupport.AddLayoutElement(advanceRunLifecycleButton.gameObject, 56f);
            advanceRunLifecycleButton.onClick.AddListener(HandleAdvanceRunLifecycleRequested);

            postRunSummaryPanelObject = new GameObject(
                "PostRunSummaryPanel",
                typeof(RectTransform),
                typeof(Image),
                typeof(HorizontalLayoutGroup));
            postRunSummaryPanelObject.transform.SetParent(panelObject.transform, false);
            RuntimeUiSupport.AddLayoutElement(postRunSummaryPanelObject, 226f);

            Image postRunPanelImage = postRunSummaryPanelObject.GetComponent<Image>();
            postRunPanelImage.color = new Color(0.12f, 0.13f, 0.18f, 0.96f);

            HorizontalLayoutGroup postRunPanelLayout = postRunSummaryPanelObject.GetComponent<HorizontalLayoutGroup>();
            postRunPanelLayout.padding = new RectOffset(18, 18, 18, 18);
            postRunPanelLayout.spacing = 18f;
            postRunPanelLayout.childAlignment = TextAnchor.UpperLeft;
            postRunPanelLayout.childControlWidth = true;
            postRunPanelLayout.childControlHeight = true;
            postRunPanelLayout.childForceExpandWidth = false;
            postRunPanelLayout.childForceExpandHeight = false;

            postRunSummaryText = RuntimeUiSupport.CreateText(
                postRunSummaryPanelObject.transform,
                uiFont,
                "PostRunSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.92f, 0.97f, 1f));
            RuntimeUiSupport.AddLayoutElement(
                postRunSummaryText.gameObject,
                170f,
                flexibleWidth: 1f);

            GameObject postRunActionsColumnObject = new GameObject(
                "PostRunActionsColumn",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup));
            postRunActionsColumnObject.transform.SetParent(postRunSummaryPanelObject.transform, false);
            RectTransform postRunActionsColumnRect = postRunActionsColumnObject.GetComponent<RectTransform>();
            postRunActionsColumnRect.localScale = Vector3.one;

            VerticalLayoutGroup postRunActionsLayout = postRunActionsColumnObject.GetComponent<VerticalLayoutGroup>();
            postRunActionsLayout.spacing = 10f;
            postRunActionsLayout.childAlignment = TextAnchor.UpperCenter;
            postRunActionsLayout.childControlWidth = true;
            postRunActionsLayout.childControlHeight = true;
            postRunActionsLayout.childForceExpandWidth = true;
            postRunActionsLayout.childForceExpandHeight = false;

            RuntimeUiSupport.AddLayoutElement(
                postRunActionsColumnObject,
                170f,
                preferredWidth: 236f);

            replayButton = CreateActionButton(
                postRunActionsColumnObject.transform,
                "ReplayNodeButton",
                "Replay Node",
                out replayButtonText);
            RuntimeUiSupport.AddLayoutElement(replayButton.gameObject, 48f);
            replayButton.onClick.AddListener(HandleReplayRequested);

            returnToWorldButton = CreateActionButton(
                postRunActionsColumnObject.transform,
                "ReturnToWorldMapButton",
                "Return To World Map",
                out returnToWorldButtonText);
            RuntimeUiSupport.AddLayoutElement(returnToWorldButton.gameObject, 48f);
            returnToWorldButton.onClick.AddListener(HandleReturnToWorldRequested);

            stopSessionButton = CreateActionButton(
                postRunActionsColumnObject.transform,
                "StopSessionButton",
                "Stop Session",
                out stopSessionButtonText);
            RuntimeUiSupport.AddLayoutElement(stopSessionButton.gameObject, 48f);
            stopSessionButton.onClick.AddListener(HandleStopSessionRequested);
        }

        private Button CreateActionButton(Transform parent, string objectName, string label, out Text buttonText)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.20f, 0.46f, 0.26f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = label;

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 8f);
            textRectTransform.offsetMax = new Vector2(-14f, -8f);
            textRectTransform.localScale = Vector3.one;

            return button;
        }

        private string BuildSummaryText()
        {
            NodePlaceholderState placeholderState = runLifecycleController.NodeContext;
            string summary =
                $"Region: {placeholderState.RegionId.Value}\n" +
                $"Type: {placeholderState.NodeType}\n" +
                $"Node state: {placeholderState.NodeState}\n" +
                $"Lifecycle: {runLifecycleController.CurrentState}\n" +
                $"Entered from: {placeholderState.OriginNodeId.Value}";

            if (!runLifecycleController.HasRunResult)
            {
                return summary;
            }

            return runLifecycleController.CurrentState == RunLifecycleState.RunResolved
                ? summary + "\n" + $"Resolution: {runLifecycleController.RunResult.ResolutionState}"
                : summary;
        }

        private string BuildStatusText()
        {
            bool usesCombatShell = runLifecycleController.NodeContext.UsesCombatShell;

            switch (runLifecycleController.CurrentState)
            {
                case RunLifecycleState.RunStart:
                    return usesCombatShell
                        ? "Combat shell initialized. Preparing automatic combat startup."
                        : "Run shell initialized. Start the placeholder run when ready.";
                case RunLifecycleState.RunActive:
                    return runLifecycleController.HasCombatEncounterState
                        ? "Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."
                        : "Run is active. Resolve the placeholder run to produce a run result.";
                case RunLifecycleState.RunResolved:
                    if (usesCombatShell && runLifecycleController.HasCombatEncounterState)
                    {
                        string winnerText = runLifecycleController.CombatEncounterState.WinnerSide?.ToString() ?? "Unknown";
                        return $"Combat shell resolved. Winner: {winnerText}. Preparing post-run summary.";
                    }

                    return "Run resolved. Open the post-run state to review next actions.";
                case RunLifecycleState.PostRun:
                    return "Post-run summary is active. Replay, return to the world map, or stop the session.";
                default:
                    throw new InvalidOperationException($"Unknown run lifecycle state '{runLifecycleController.CurrentState}'.");
            }
        }

        private void RefreshCombatShellView()
        {
            bool shouldShowCombatShell =
                runLifecycleController.HasCombatEncounterState &&
                (runLifecycleController.CurrentState == RunLifecycleState.RunActive ||
                runLifecycleController.CurrentState == RunLifecycleState.RunResolved);

            if (!shouldShowCombatShell)
            {
                combatShellView.Hide();
                return;
            }

            combatShellView.Show(runLifecycleController.CombatEncounterState);
        }

        private void RefreshAdvanceButton()
        {
            bool usesCombatShell = runLifecycleController.NodeContext.UsesCombatShell;

            switch (runLifecycleController.CurrentState)
            {
                case RunLifecycleState.RunStart:
                    advanceRunLifecycleButton.interactable = !usesCombatShell;
                    advanceRunLifecycleButtonText.text = usesCombatShell
                        ? "Combat Auto-Starting"
                        : "Start Placeholder Run";
                    return;
                case RunLifecycleState.RunActive:
                    advanceRunLifecycleButton.interactable = !runLifecycleController.HasCombatEncounterState;
                    advanceRunLifecycleButtonText.text = runLifecycleController.HasCombatEncounterState
                        ? "Combat Auto-Running"
                        : "Resolve Placeholder Run";
                    return;
                case RunLifecycleState.RunResolved:
                    advanceRunLifecycleButton.interactable = !usesCombatShell;
                    advanceRunLifecycleButtonText.text = usesCombatShell
                        ? "Preparing Post-Run"
                        : "Enter Post-Run State";
                    return;
                case RunLifecycleState.PostRun:
                    advanceRunLifecycleButton.interactable = false;
                    advanceRunLifecycleButtonText.text = "Run Lifecycle Complete";
                    return;
                default:
                    throw new InvalidOperationException($"Unknown run lifecycle state '{runLifecycleController.CurrentState}'.");
            }
        }

        private void Update()
        {
            TryAdvanceRuntimeTime(Time.unscaledDeltaTime);
        }

        private void TryAdvanceRuntimeTime(float elapsedSeconds)
        {
            if (runLifecycleController == null)
            {
                return;
            }

            if (runLifecycleController.TryAdvanceAutomaticTime(elapsedSeconds))
            {
                Refresh();
            }
        }

        private void SyncPostRunStateController()
        {
            if (runLifecycleController == null || runLifecycleController.CurrentState != RunLifecycleState.PostRun)
            {
                postRunStateController = null;
                return;
            }

            if (postRunStateController != null)
            {
                return;
            }

            postRunStateController = new PostRunStateController(
                runLifecycleController.NodeContext,
                runLifecycleController.RunResult);
        }

        private void RefreshPostRunSummaryPanel()
        {
            bool isPostRunActive = runLifecycleController.CurrentState == RunLifecycleState.PostRun &&
                postRunStateController != null;

            postRunSummaryPanelObject.SetActive(isPostRunActive);
            if (!isPostRunActive)
            {
                replayButton.interactable = false;
                returnToWorldButton.interactable = false;
                stopSessionButton.interactable = false;
                return;
            }

            postRunSummaryText.text = BuildPostRunSummaryText(postRunStateController, runLifecycleController.RunResult);
            replayButton.interactable = postRunStateController.CanReplayNode;
            replayButtonText.text = "Replay Node";
            returnToWorldButton.interactable = postRunStateController.CanReturnToWorld;
            returnToWorldButtonText.text = "Return To World Map";
            stopSessionButton.interactable =
                postRunStateController.CanStopSession && onStopSessionRequested != null;
            stopSessionButtonText.text = onStopSessionRequested == null
                ? "Stop Session Unavailable"
                : "Stop Session";
        }

        private static string BuildRewardSummary(RunRewardPayload rewardPayload)
        {
            return rewardPayload.HasRewards ? "Placeholder reward payload present" : "None";
        }

        private RunLifecycleController CreateRunLifecycleController(NodePlaceholderState placeholderState)
        {
            return new RunLifecycleController(
                placeholderState,
                persistentWorldState: persistentWorldState);
        }

        private static string BuildPostRunSummaryText(PostRunStateController postRunStateController, RunResult runResult)
        {
            string nodeProgressSummary = runResult.HasTrackedNodeProgress
                ? $"{runResult.NodeProgressValue} / {runResult.NodeProgressThreshold}"
                : "not tracked";

            return
                "Run finished.\n" +
                $"Node: {runResult.NodeId.Value}\n" +
                $"Resolution: {runResult.ResolutionState}\n" +
                $"Node progress total: {nodeProgressSummary}\n" +
                $"Rewards: {BuildRewardSummary(runResult.RewardPayload)}\n" +
                $"Node progress delta: {runResult.NodeProgressDelta}\n" +
                $"Persistent progression delta: {runResult.PersistentProgressionDelta}\n" +
                $"Route unlock changed: {FormatYesNo(runResult.DidUnlockRoute)}\n" +
                "Next actions:\n" +
                $"- Replay: {FormatYesNo(postRunStateController.CanReplayNode)}\n" +
                $"- Return to world: {FormatYesNo(postRunStateController.CanReturnToWorld)}\n" +
                $"- Stop: {FormatYesNo(postRunStateController.CanStopSession)}";
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}
