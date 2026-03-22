using System;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Towns
{
    public sealed class TownServiceScreen : MonoBehaviour
    {
        private Canvas canvas;
        private RectTransform panelRectTransform;
        private RectTransform contentViewportRectTransform;
        private RectTransform contentRectTransform;
        private RectTransform progressionActionContainer;
        private ScrollRect contentScrollRect;
        private Text titleText;
        private Text overviewText;
        private Text progressionText;
        private Text buildPreparationText;
        private Button returnToWorldButton;
        private Text returnToWorldButtonText;
        private Button stopSessionButton;
        private Text stopSessionButtonText;
        private Font uiFont;
        private Action onReturnToWorldRequested;
        private Action onStopSessionRequested;
        private NodePlaceholderState currentPlaceholderState;
        private PersistentGameState currentGameState;
        private TownServiceScreenStateResolver stateResolver;
        private TownServiceProgressionInteractionService progressionInteractionService;

        public void Show(
            NodePlaceholderState placeholderState,
            PersistentGameState gameState,
            Action returnToWorldRequested,
            Action stopSessionRequested = null,
            TownServiceScreenStateResolver stateResolver = null,
            TownServiceProgressionInteractionService progressionInteractionService = null)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (placeholderState.NodeType != NodeType.ServiceOrProgression)
            {
                throw new ArgumentException(
                    "Town service screen requires a service-or-progression node context.",
                    nameof(placeholderState));
            }

            if (placeholderState.TownServiceContext == null)
            {
                throw new ArgumentException(
                    "Town service screen requires an explicit town service context definition.",
                    nameof(placeholderState));
            }

            currentPlaceholderState = placeholderState;
            currentGameState = gameState;
            this.stateResolver = stateResolver ?? new TownServiceScreenStateResolver();
            this.progressionInteractionService = progressionInteractionService ?? new TownServiceProgressionInteractionService();
            onReturnToWorldRequested = returnToWorldRequested ?? throw new ArgumentNullException(nameof(returnToWorldRequested));
            onStopSessionRequested = stopSessionRequested;
            gameObject.name = "TownServiceScreen";

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh(this.stateResolver.Resolve(currentPlaceholderState, currentGameState));
        }

        private void Refresh(TownServiceScreenState screenState)
        {
            titleText.text = screenState.ServiceContext.DisplayName;
            overviewText.text = TownServiceScreenTextBuilder.BuildOverviewText(screenState);
            progressionText.text = TownServiceScreenTextBuilder.BuildProgressionText(screenState);
            buildPreparationText.text = TownServiceScreenTextBuilder.BuildBuildPreparationText(screenState);
            progressionText.gameObject.SetActive(screenState.ServiceContext.HasProgressionHubAccess);
            progressionActionContainer.gameObject.SetActive(screenState.ServiceContext.HasProgressionHubAccess);
            buildPreparationText.gameObject.SetActive(screenState.ServiceContext.HasBuildPreparationAccess);
            RefreshProgressionActionButtons(screenState);
            stopSessionButton.interactable = onStopSessionRequested != null;
            stopSessionButtonText.text = onStopSessionRequested == null
                ? "Stop Session Unavailable"
                : "Stop Session";
            RefreshLayout();
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(TownServiceScreen));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 105;

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
            panelImage.color = new Color(0.10f, 0.12f, 0.16f, 0.96f);

            panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.14f, 0.10f);
            panelRectTransform.anchorMax = new Vector2(0.86f, 0.90f);
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

            GameObject contentScrollViewObject = new GameObject(
                "ContentScrollView",
                typeof(RectTransform),
                typeof(Image),
                typeof(ScrollRect),
                typeof(LayoutElement));
            contentScrollViewObject.transform.SetParent(panelObject.transform, false);

            Image contentScrollViewImage = contentScrollViewObject.GetComponent<Image>();
            contentScrollViewImage.color = new Color(1f, 1f, 1f, 0.04f);

            RectTransform contentScrollViewRectTransform = contentScrollViewObject.GetComponent<RectTransform>();
            contentScrollViewRectTransform.localScale = Vector3.one;

            LayoutElement contentScrollViewLayoutElement = contentScrollViewObject.GetComponent<LayoutElement>();
            contentScrollViewLayoutElement.minHeight = 200f;
            contentScrollViewLayoutElement.flexibleHeight = 1f;

            GameObject contentViewportObject = new GameObject(
                "ContentViewport",
                typeof(RectTransform),
                typeof(Image),
                typeof(RectMask2D));
            contentViewportObject.transform.SetParent(contentScrollViewObject.transform, false);

            Image contentViewportImage = contentViewportObject.GetComponent<Image>();
            contentViewportImage.color = new Color(1f, 1f, 1f, 0.01f);

            contentViewportRectTransform = contentViewportObject.GetComponent<RectTransform>();
            contentViewportRectTransform.anchorMin = Vector2.zero;
            contentViewportRectTransform.anchorMax = Vector2.one;
            contentViewportRectTransform.offsetMin = Vector2.zero;
            contentViewportRectTransform.offsetMax = Vector2.zero;
            contentViewportRectTransform.localScale = Vector3.one;

            GameObject contentObject = new GameObject(
                "Content",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            contentObject.transform.SetParent(contentViewportObject.transform, false);

            contentRectTransform = contentObject.GetComponent<RectTransform>();
            contentRectTransform.anchorMin = new Vector2(0f, 1f);
            contentRectTransform.anchorMax = new Vector2(1f, 1f);
            contentRectTransform.pivot = new Vector2(0.5f, 1f);
            contentRectTransform.localScale = Vector3.one;

            VerticalLayoutGroup contentLayout = contentObject.GetComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(12, 12, 12, 12);
            contentLayout.spacing = 12f;
            contentLayout.childAlignment = TextAnchor.UpperLeft;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = true;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            ContentSizeFitter contentFitter = contentObject.GetComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            contentScrollRect = contentScrollViewObject.GetComponent<ScrollRect>();
            contentScrollRect.viewport = contentViewportRectTransform;
            contentScrollRect.content = contentRectTransform;
            contentScrollRect.horizontal = false;
            contentScrollRect.vertical = true;
            contentScrollRect.movementType = ScrollRect.MovementType.Clamped;
            contentScrollRect.scrollSensitivity = 28f;

            overviewText = RuntimeUiSupport.CreateText(
                contentObject.transform,
                uiFont,
                "Overview",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.90f, 0.92f, 0.97f, 1f));
            RuntimeUiSupport.GetOrAddComponent<LayoutElement>(overviewText.gameObject).flexibleWidth = 1f;

            progressionText = RuntimeUiSupport.CreateText(
                contentObject.transform,
                uiFont,
                "ProgressionSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.91f, 0.96f, 1f));
            RuntimeUiSupport.GetOrAddComponent<LayoutElement>(progressionText.gameObject).flexibleWidth = 1f;

            progressionActionContainer = CreateProgressionActionContainer(contentObject.transform);

            buildPreparationText = RuntimeUiSupport.CreateText(
                contentObject.transform,
                uiFont,
                "BuildPreparationSummary",
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.91f, 0.96f, 1f));
            RuntimeUiSupport.GetOrAddComponent<LayoutElement>(buildPreparationText.gameObject).flexibleWidth = 1f;

            GameObject actionRowObject = new GameObject(
                "ActionRow",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            actionRowObject.transform.SetParent(panelObject.transform, false);

            HorizontalLayoutGroup actionRowLayout = actionRowObject.GetComponent<HorizontalLayoutGroup>();
            actionRowLayout.spacing = 12f;
            actionRowLayout.childAlignment = TextAnchor.MiddleCenter;
            actionRowLayout.childControlWidth = true;
            actionRowLayout.childControlHeight = true;
            actionRowLayout.childForceExpandWidth = true;
            actionRowLayout.childForceExpandHeight = false;

            RuntimeUiSupport.AddLayoutElement(actionRowObject, 56f);

            returnToWorldButton = CreateActionButton(
                actionRowObject.transform,
                "ReturnToWorldMapButton",
                "Return To World Map",
                new Color(0.22f, 0.39f, 0.58f, 1f),
                out returnToWorldButtonText);
            returnToWorldButton.onClick.AddListener(HandleReturnToWorldRequested);

            stopSessionButton = CreateActionButton(
                actionRowObject.transform,
                "StopSessionButton",
                "Stop Session",
                new Color(0.36f, 0.28f, 0.22f, 1f),
                out stopSessionButtonText);
            stopSessionButton.onClick.AddListener(HandleStopSessionRequested);
        }

        private Button CreateActionButton(
            Transform parent,
            string objectName,
            string label,
            Color buttonColor,
            out Text buttonText)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 56f;
            layoutElement.preferredHeight = 56f;
            layoutElement.flexibleWidth = 1f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = buttonColor;

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

        private RectTransform CreateProgressionActionContainer(Transform parent)
        {
            GameObject actionContainerObject = new GameObject(
                "ProgressionActionContainer",
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            actionContainerObject.transform.SetParent(parent, false);

            VerticalLayoutGroup actionContainerLayout = actionContainerObject.GetComponent<VerticalLayoutGroup>();
            actionContainerLayout.spacing = 8f;
            actionContainerLayout.childAlignment = TextAnchor.UpperLeft;
            actionContainerLayout.childControlWidth = true;
            actionContainerLayout.childControlHeight = true;
            actionContainerLayout.childForceExpandWidth = true;
            actionContainerLayout.childForceExpandHeight = false;

            ContentSizeFitter actionContainerFitter = actionContainerObject.GetComponent<ContentSizeFitter>();
            actionContainerFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            actionContainerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform actionContainerRectTransform = actionContainerObject.GetComponent<RectTransform>();
            actionContainerRectTransform.anchorMin = new Vector2(0f, 1f);
            actionContainerRectTransform.anchorMax = new Vector2(1f, 1f);
            actionContainerRectTransform.pivot = new Vector2(0.5f, 1f);
            actionContainerRectTransform.localScale = Vector3.one;

            return actionContainerRectTransform;
        }

        private void RefreshProgressionActionButtons(TownServiceScreenState screenState)
        {
            ClearProgressionActionButtons();

            if (!screenState.ServiceContext.HasProgressionHubAccess)
            {
                return;
            }

            for (int index = 0; index < screenState.ProgressionOptions.Count; index++)
            {
                TownServiceProgressionOptionState progressionOption = screenState.ProgressionOptions[index];
                string buttonLabel = BuildProgressionActionButtonLabel(progressionOption);
                Button progressionButton = CreateActionButton(
                    progressionActionContainer,
                    $"{progressionOption.UpgradeId}_PurchaseUpgradeButton",
                    buttonLabel,
                    new Color(0.21f, 0.35f, 0.24f, 1f),
                    out _);
                progressionButton.interactable = progressionOption.IsAffordable && !progressionOption.IsPurchased;

                AccountWideUpgradeId upgradeId = progressionOption.UpgradeId;
                progressionButton.onClick.AddListener(() => HandleProgressionPurchaseRequested(upgradeId));
            }
        }

        private void ClearProgressionActionButtons()
        {
            if (progressionActionContainer == null)
            {
                return;
            }

            for (int index = progressionActionContainer.childCount - 1; index >= 0; index--)
            {
                GameObject childObject = progressionActionContainer.GetChild(index).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(childObject);
                    continue;
                }

                DestroyImmediate(childObject);
            }
        }

        private static string BuildProgressionActionButtonLabel(TownServiceProgressionOptionState progressionOption)
        {
            if (progressionOption.IsPurchased)
            {
                return $"{progressionOption.UpgradeDisplayName} Purchased";
            }

            if (progressionOption.IsAffordable)
            {
                return $"Buy {progressionOption.UpgradeDisplayName}";
            }

            return $"{progressionOption.UpgradeDisplayName} Unavailable";
        }

        private void HandleProgressionPurchaseRequested(AccountWideUpgradeId upgradeId)
        {
            if (progressionInteractionService == null || stateResolver == null)
            {
                return;
            }

            progressionInteractionService.TryPurchase(currentGameState, upgradeId);
            Refresh(stateResolver.Resolve(currentPlaceholderState, currentGameState));
        }

        private void RefreshLayout()
        {
            Canvas.ForceUpdateCanvases();

            if (contentRectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
            }

            if (contentViewportRectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentViewportRectTransform);
            }

            if (panelRectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            }

            if (contentScrollRect != null)
            {
                contentScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private void HandleReturnToWorldRequested()
        {
            onReturnToWorldRequested?.Invoke();
        }

        private void HandleStopSessionRequested()
        {
            onStopSessionRequested?.Invoke();
        }
    }
}
