using System;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Run;

namespace Survivalon.World
{
    public sealed class NodePlaceholderScreen : MonoBehaviour
    {
        private Canvas canvas;
        private Text titleText;
        private Text summaryText;
        private Text statusText;
        private GameObject runTimeSkillUpgradePanelObject;
        private Text runTimeSkillUpgradeText;
        private RectTransform runTimeSkillUpgradeContainer;
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
        private WorldGraph worldGraph;
        private RunPersistentContext persistentContext;
        private readonly RunHudStateResolver runHudStateResolver = new RunHudStateResolver();
        private readonly RunTimeSkillUpgradeChoiceStateResolver runTimeSkillUpgradeChoiceStateResolver =
            new RunTimeSkillUpgradeChoiceStateResolver();
        private RunLifecycleController runLifecycleController;
        private PostRunStateController postRunStateController;
        private Action<RunResult> onReturnToWorldRequested;
        private Action<RunResult> onStopSessionRequested;

        public void Show(
            WorldGraph worldGraph,
            NodePlaceholderState placeholderState,
            Action<RunResult> returnToWorldRequested,
            Action<RunResult> stopSessionRequested = null,
            RunPersistentContext persistentContext = null)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            this.worldGraph = worldGraph;
            this.persistentContext = persistentContext;
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
            summaryText.text = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                placeholderState,
                runLifecycleController.CurrentState,
                runLifecycleController.HasRunResult ? runLifecycleController.RunResult : null);
            statusText.text = NodePlaceholderScreenTextBuilder.BuildStatusText(
                placeholderState,
                runLifecycleController.CurrentState,
                runLifecycleController.HasCombatEncounterState ? runLifecycleController.CombatEncounterState : null,
                runLifecycleController.RequiresRunTimeSkillUpgradeChoice);
            RefreshRunTimeSkillUpgradePanel();
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

        private void HandleRunTimeSkillUpgradeSelection(string runTimeSkillUpgradeId)
        {
            if (!runLifecycleController.TrySelectRunTimeSkillUpgrade(runTimeSkillUpgradeId))
            {
                return;
            }

            runLifecycleController.TryStartAutomaticFlow();
            Refresh();
        }

        private void HandleReplayRequested()
        {
            if (postRunStateController == null || !postRunStateController.CanReplayNode)
            {
                return;
            }

            RunLifecycleController replayController = postRunStateController.CreateReplayLifecycleController(
                worldGraph,
                persistentContext);
            postRunStateController = null;
            runLifecycleController = replayController;
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

            runTimeSkillUpgradePanelObject = new GameObject(
                "RunTimeSkillUpgradePanel",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            runTimeSkillUpgradePanelObject.transform.SetParent(panelObject.transform, false);

            RectTransform runTimeSkillUpgradePanelRectTransform =
                runTimeSkillUpgradePanelObject.GetComponent<RectTransform>();
            runTimeSkillUpgradePanelRectTransform.localScale = Vector3.one;

            Image runTimeSkillUpgradePanelImage = runTimeSkillUpgradePanelObject.GetComponent<Image>();
            runTimeSkillUpgradePanelImage.color = new Color(0.13f, 0.14f, 0.19f, 0.96f);

            VerticalLayoutGroup runTimeSkillUpgradePanelLayout =
                runTimeSkillUpgradePanelObject.GetComponent<VerticalLayoutGroup>();
            runTimeSkillUpgradePanelLayout.padding = new RectOffset(14, 14, 12, 12);
            runTimeSkillUpgradePanelLayout.spacing = 8f;
            runTimeSkillUpgradePanelLayout.childAlignment = TextAnchor.UpperLeft;
            runTimeSkillUpgradePanelLayout.childControlWidth = true;
            runTimeSkillUpgradePanelLayout.childControlHeight = true;
            runTimeSkillUpgradePanelLayout.childForceExpandWidth = true;
            runTimeSkillUpgradePanelLayout.childForceExpandHeight = false;

            ContentSizeFitter runTimeSkillUpgradePanelFitter =
                runTimeSkillUpgradePanelObject.GetComponent<ContentSizeFitter>();
            runTimeSkillUpgradePanelFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            runTimeSkillUpgradePanelFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            runTimeSkillUpgradeText = RuntimeUiSupport.CreateText(
                runTimeSkillUpgradePanelObject.transform,
                uiFont,
                "RunTimeSkillUpgradeSummary",
                17,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.92f, 0.97f, 1f));
            RuntimeUiSupport.AddLayoutElement(runTimeSkillUpgradeText.gameObject, 52f);

            GameObject runTimeSkillUpgradeListObject = new GameObject(
                "RunTimeSkillUpgradeList",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            runTimeSkillUpgradeListObject.transform.SetParent(runTimeSkillUpgradePanelObject.transform, false);

            VerticalLayoutGroup runTimeSkillUpgradeListLayout =
                runTimeSkillUpgradeListObject.GetComponent<VerticalLayoutGroup>();
            runTimeSkillUpgradeListLayout.spacing = 8f;
            runTimeSkillUpgradeListLayout.childAlignment = TextAnchor.UpperLeft;
            runTimeSkillUpgradeListLayout.childControlWidth = true;
            runTimeSkillUpgradeListLayout.childControlHeight = true;
            runTimeSkillUpgradeListLayout.childForceExpandWidth = true;
            runTimeSkillUpgradeListLayout.childForceExpandHeight = false;

            ContentSizeFitter runTimeSkillUpgradeListFitter =
                runTimeSkillUpgradeListObject.GetComponent<ContentSizeFitter>();
            runTimeSkillUpgradeListFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            runTimeSkillUpgradeListFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            runTimeSkillUpgradeContainer = runTimeSkillUpgradeListObject.GetComponent<RectTransform>();
            runTimeSkillUpgradeContainer.anchorMin = new Vector2(0f, 1f);
            runTimeSkillUpgradeContainer.anchorMax = new Vector2(1f, 1f);
            runTimeSkillUpgradeContainer.pivot = new Vector2(0.5f, 1f);
            runTimeSkillUpgradeContainer.localScale = Vector3.one;

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

        private void ClearRunTimeSkillUpgradeButtons()
        {
            for (int index = runTimeSkillUpgradeContainer.childCount - 1; index >= 0; index--)
            {
                if (Application.isPlaying)
                {
                    Destroy(runTimeSkillUpgradeContainer.GetChild(index).gameObject);
                    continue;
                }

                DestroyImmediate(runTimeSkillUpgradeContainer.GetChild(index).gameObject);
            }
        }

        private void RefreshRunTimeSkillUpgradePanel()
        {
            bool shouldShow = runLifecycleController.CurrentState == RunLifecycleState.RunStart &&
                runLifecycleController.RequiresRunTimeSkillUpgradeChoice;
            runTimeSkillUpgradePanelObject.SetActive(shouldShow);

            ClearRunTimeSkillUpgradeButtons();
            if (!shouldShow)
            {
                return;
            }

            RunTimeSkillUpgradeChoiceState choiceState = runTimeSkillUpgradeChoiceStateResolver.Resolve(
                runLifecycleController.RunTimeSkillUpgradeOptions);
            runTimeSkillUpgradeText.text = RunTimeSkillUpgradeChoiceTextBuilder.BuildPanelText(choiceState);

            foreach (RunTimeSkillUpgradeChoiceOptionState optionState in choiceState.Options)
            {
                CreateRunTimeSkillUpgradeButton(optionState);
            }
        }

        private void CreateRunTimeSkillUpgradeButton(RunTimeSkillUpgradeChoiceOptionState optionState)
        {
            GameObject buttonObject = new GameObject(
                $"{optionState.UpgradeId}_RunTimeSkillUpgradeButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(runTimeSkillUpgradeContainer, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 86f;
            layoutElement.preferredHeight = 86f;
            layoutElement.flexibleWidth = 1f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.28f, 0.34f, 0.56f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() =>
                HandleRunTimeSkillUpgradeSelection(optionState.UpgradeId));

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            Text buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                15,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                Color.white);
            buttonText.text = RunTimeSkillUpgradeChoiceTextBuilder.BuildOptionButtonText(optionState);

            RectTransform textRectTransform = buttonText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(14f, 10f);
            textRectTransform.offsetMax = new Vector2(-14f, -10f);
            textRectTransform.localScale = Vector3.one;
        }

        private void RefreshCombatShellView()
        {
            if (!NodePlaceholderScreenStateResolver.ShouldShowCombatShell(
                runLifecycleController.CurrentState,
                runLifecycleController.HasCombatEncounterState))
            {
                combatShellView.Hide();
                return;
            }

            RunHudState runHudState = runHudStateResolver.Resolve(
                runLifecycleController.NodeContext,
                runLifecycleController.CurrentState,
                runLifecycleController.CombatEncounterState,
                persistentContext?.PersistentWorldState);
            combatShellView.Show(
                runLifecycleController.CombatEncounterState,
                title: "Run HUD",
                summary: RunHudTextBuilder.BuildSummaryText(runHudState));
        }

        private void RefreshAdvanceButton()
        {
            NodePlaceholderScreenButtonState buttonState = NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                runLifecycleController.NodeContext,
                runLifecycleController.CurrentState,
                runLifecycleController.RequiresRunTimeSkillUpgradeChoice,
                runLifecycleController.HasCombatEncounterState);
            advanceRunLifecycleButton.interactable = buttonState.IsInteractable;
            advanceRunLifecycleButtonText.text = buttonState.Label;
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
            NodePlaceholderScreenPostRunPanelState panelState = NodePlaceholderScreenStateResolver.ResolvePostRunPanelState(
                runLifecycleController.CurrentState,
                postRunStateController,
                onStopSessionRequested != null);

            postRunSummaryPanelObject.SetActive(panelState.IsVisible);
            replayButton.interactable = panelState.ReplayButton.IsInteractable;
            replayButtonText.text = panelState.ReplayButton.Label;
            returnToWorldButton.interactable = panelState.ReturnToWorldButton.IsInteractable;
            returnToWorldButtonText.text = panelState.ReturnToWorldButton.Label;
            stopSessionButton.interactable = panelState.StopSessionButton.IsInteractable;
            stopSessionButtonText.text = panelState.StopSessionButton.Label;

            if (!panelState.IsVisible)
            {
                return;
            }

            postRunSummaryText.text = PostRunSummaryTextBuilder.Build(
                postRunStateController,
                runLifecycleController.RunResult);
        }

        private RunLifecycleController CreateRunLifecycleController(NodePlaceholderState placeholderState)
        {
            return new RunLifecycleController(
                placeholderState,
                worldGraph,
                persistentContext: persistentContext);
        }

    }
}

